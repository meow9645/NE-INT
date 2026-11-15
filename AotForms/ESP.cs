using ClickableTransparentOverlay;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Reflection;
using static AotForms.WinAPI;

namespace AotForms
{
    internal class ESP : Overlay
    {
        public static ESP Instance { get; private set; }

        private IntPtr _lastHandle = IntPtr.Zero;
        private bool _affinitySetForCurrentHandle = false;

        // ---- P/Invoke for display affinity ----
        [DllImport("user32.dll")]
        private static extern bool SetWindowDisplayAffinity(IntPtr hWnd, uint dwAffinity);

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        private const uint WDA_EXCLUDEFROMCAPTURE = 0x00000011;
        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_LAYERED = 0x80000;
        private const int WS_EX_TOOLWINDOW = 0x80;

        // ---- Constructor ----
        public ESP()
        {
            Instance = this;
            // Disable ImGui .ini file creation
            // Disable ImGui .ini file handling (no load, no save)
            //unsafe
            //{
            //    // Directly set the native ImGui IO pointer's IniFilename to null
            //    ImGui.GetIO().NativePtr->IniFilename = null;
            //}

            //// Force ImGui to save settings to memory instead of a file.
            //// This still triggers the save process, but it never writes to disk.
            //ImGui.GetIO().IniSavingRate = 0; // This disables automatic saving.

        }

        // ===== VISIBILITY CONTROL =====
        public void SetVisible(bool visible)
        {
            try
            {
                var methodName = visible ? "Show" : "Hide";
                var method = this.GetType().GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance);
                if (method != null)
                {
                    method.Invoke(this, null);
                    return;
                }
            }
            catch { /* fallback */ }

            IntPtr hWnd = GetWindowHandle();
            if (hWnd != IntPtr.Zero)
                ShowWindow(hWnd, visible ? 5 : 0);
        }

        private IntPtr GetWindowHandle()
        {
            // Try reflection first (common for Overlay library)
            var prop = this.GetType().GetProperty("Handle", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (prop != null)
                return (IntPtr)prop.GetValue(this);

            var field = this.GetType().GetField("Handle", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (field != null)
                return (IntPtr)field.GetValue(this);

            field = this.GetType().GetField("hWnd", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (field != null)
                return (IntPtr)field.GetValue(this);

            // Fallback: search by class name (typical overlay window class)
            return FindWindow("Overlay", null);
        }

        // ============================================================
        //  RENDER LOOP – Main drawing + affinity enforcement
        // ============================================================
        protected override unsafe void Render()
        {
            // ---- 1. ENSURE CAPTURE EXCLUSION IS APPLIED ----
            IntPtr currentHandle = GetWindowHandle();
            if (currentHandle != IntPtr.Zero && currentHandle != _lastHandle)
            {
                // Apply affinity
                bool success = SetWindowDisplayAffinity(currentHandle, WDA_EXCLUDEFROMCAPTURE);
                if (success)
                {
                    _lastHandle = currentHandle;
                    _affinitySetForCurrentHandle = true;

                    // Optionally, set extended styles to make it a layered tool window
                    // (helps some capture methods)
                    int exStyle = GetWindowLong(currentHandle, GWL_EXSTYLE);
                    SetWindowLong(currentHandle, GWL_EXSTYLE, exStyle | WS_EX_LAYERED | WS_EX_TOOLWINDOW);
                }
                // If failed, we'll try again next frame (handle may not be fully ready)
            }

            // ---- 2. UPDATE WINDOW SIZE & POSITION ----
            RECT rect;
            GetWindowRect(Core.Handle, out rect);
            int x = rect.Left;
            int y = rect.Top;
            int width = rect.Right - rect.Left;
            int height = rect.Bottom - rect.Top;
            ImGui.SetWindowSize(new Vector2((float)width, (float)height));
            Size = new Size(width, height);
            Position = new Point(x, y);
            Core.Width = width;
            Core.Height = height;

            // ---- 3. EARLY EXIT IF NO MATRIX ----
            if (!Core.HaveMatrix) return;

            // ---- 4. DRAWING ----
            string text = " ";
            var windowWidth = Core.Width;
            var windowHeight = Core.Height;
            var textSize = ImGui.CalcTextSize(text);
            var textPosX = (windowWidth - textSize.X) / 2;
            var textPosY = 80;
            uint textColor = ImGui.ColorConvertFloat4ToU32(new Vector4(1.0f, 1.0f, 1.0f, 1.0f));
            uint shadowColor = ImGui.ColorConvertFloat4ToU32(new Vector4(0.0f, 0.0f, 0.0f, 0.5f));
            var drawList = ImGui.GetForegroundDrawList();

            ImGui.GetForegroundDrawList().AddText(new Vector2(textPosX + 1, textPosY + 1), shadowColor, text);
            ImGui.GetForegroundDrawList().AddText(new Vector2(textPosX, textPosY), textColor, text);

            int playerCounter = 1;
            int enemyCount = 0;
            var tmp = Core.Entities;

            if (Config.FOVEnabled)
                DrawFOVCircle(Config.AimFov);

            foreach (var entity in tmp.Values)
            {
                if (entity.IsDead || !entity.IsKnown) continue;

                var dist = Vector3.Distance(Core.LocalMainCamera, entity.Head);
                if (dist > Config.espran) continue;
                enemyCount++;

                var headScreenPos = W2S.WorldToScreen(Core.CameraMatrix, entity.Head, Core.Width, Core.Height);
                var bottomScreenPos = W2S.WorldToScreen(Core.CameraMatrix, entity.Root, Core.Width, Core.Height);

                if (headScreenPos.X < 1 || headScreenPos.Y < 1) continue;
                if (bottomScreenPos.X < 1 || bottomScreenPos.Y < 1) continue;

                float CornerHeight = Math.Abs(headScreenPos.Y - bottomScreenPos.Y);
                float CornerWidth = (float)(CornerHeight * 0.65);

                if (Config.ESPLine)
                {
                    uint lineColor = entity.IsKnocked ? ColorToUint32(Color.Red) : ColorToUint32(Config.ESPLineColor);
                    ImGui.GetBackgroundDrawList().AddLine(
                        new Vector2(Core.Width / 2f, 0f),
                        headScreenPos,
                        lineColor,
                        1f
                    );
                }

                if (Config.ESPFillBox)
                {
                    Color topColor = Color.FromArgb((int)(0.1f * 255), Config.ESPFillBoxColor);
                    Color bottomColor = Color.FromArgb((int)(0.75f * 255), Config.ESPFillBoxColor);
                    DrawGradientBox(
                        headScreenPos.X - (CornerWidth / 2),
                        headScreenPos.Y,
                        CornerWidth,
                        CornerHeight,
                        topColor,
                        bottomColor
                    );
                }

                if (Config.ESPBox)
                {
                    uint boxColor = ColorToUint32(Config.ESPBoxColor);
                    DrawBox(headScreenPos.X - (CornerWidth / 2), headScreenPos.Y, CornerWidth, CornerHeight, boxColor, Config.ESPBoxThickness);
                }

                if (Config.ESPSkeleton)
                    DrawSkeleton(entity);

                if (Config.ESPBox2)
                {
                    uint boxColor = ColorToUint32(Config.ESPBoxColor);
                    DrawCorneredBox(headScreenPos.X - (CornerWidth / 2), headScreenPos.Y, CornerWidth, CornerHeight, boxColor, 1f);
                }

                var namePosition = new Vector2(headScreenPos.X - (CornerWidth / 2), headScreenPos.Y - 20);
                var nameText = string.IsNullOrWhiteSpace(entity.Name) ? "BOT" : entity.Name;

                if (Config.ESPName)
                {
                    string cleanName = RemoveUnsupportedCharacters(nameText);
                    drawList.AddText(namePosition, ColorToUint32(Color.White), cleanName);
                    Vector2 nameTextSize = ImGui.CalcTextSize(cleanName);
                    Vector2 distancePosition = new Vector2(namePosition.X + nameTextSize.X, namePosition.Y);
                    drawList.AddText(distancePosition, ColorToUint32(Color.Yellow), $"     {MathF.Round(dist)}M");
                }

                if (Config.ESPDistance)
                {
                    string distanceText = $"{dist.ToString("F1")}m";
                    float estimatedTextWidth = distanceText.Length * 6f;
                    Vector2 distancePosition = new Vector2(bottomScreenPos.X + 1 - (estimatedTextWidth / 2), bottomScreenPos.Y + 5f);
                    ImGui.GetForegroundDrawList().AddText(distancePosition, ColorToUint32(Config.ESPDistanceColor), distanceText);
                }

                if (Config.ESPHealth)
                    DrawHealthBar(entity.Health, 200, headScreenPos.X - (CornerWidth / 2) - 5, headScreenPos.Y, CornerHeight);
            }
        }

        // ============================================================
        //  ALL HELPER METHODS (unchanged from your original)
        // ============================================================

        public void DrawGlowingBall(Vector2 position, Color color, float radius)
        {
            var drawList = ImGui.GetBackgroundDrawList();
            uint ballColor = ColorToUint32(color);

            for (int i = 0; i < 5; i++)
            {
                float glowRadius = radius + (i * 2);
                float alpha = 1.0f - (i * 0.2f);

                drawList.AddCircleFilled(
                    position,
                    glowRadius,
                    ImGui.ColorConvertFloat4ToU32(new Vector4(color.R / 255f, color.G / 255f, color.B / 255f, alpha)),
                    50
                );
            }

            drawList.AddCircleFilled(position, radius, ballColor, 50);
        }

        private string RemoveUnsupportedCharacters(string input)
        {
            return new string(input.Where(c => c >= 32 && c <= 126).ToArray());
        }

        public void DrawGradientBox(float X, float Y, float W, float H, Color topColor, Color bottomColor)
        {
            var vList = ImGui.GetForegroundDrawList();

            int slices = 50;
            float sliceHeight = H / slices;

            for (int i = 0; i < slices; i++)
            {
                float t = (float)i / slices;
                Color sliceColor = Color.FromArgb(
                    (int)(topColor.A * (1 - t) + bottomColor.A * t),
                    (int)(topColor.R * (1 - t) + bottomColor.R * t),
                    (int)(topColor.G * (1 - t) + bottomColor.G * t),
                    (int)(topColor.B * (1 - t) + bottomColor.B * t)
                );

                uint sliceColorUint = ColorToUint32(sliceColor);

                vList.AddRectFilled(
                    new Vector2(X, Y + i * sliceHeight),
                    new Vector2(X + W, Y + (i + 1) * sliceHeight),
                    sliceColorUint
                );
            }
        }

        private void DrawBox(float X, float Y, float W, float H, uint color, float thickness)
        {
            var drawList = ImGui.GetForegroundDrawList();
            drawList.AddLine(new Vector2(X, Y), new Vector2(X + W, Y), color, thickness);
            drawList.AddLine(new Vector2(X, Y + H), new Vector2(X + W, Y + H), color, thickness);
            drawList.AddLine(new Vector2(X, Y), new Vector2(X, Y + H), color, thickness);
            drawList.AddLine(new Vector2(X + W, Y), new Vector2(X + W, Y + H), color, thickness);
        }

        void DrawESPHeadLine(Vector2 head, Vector2 aim)
        {
            float maxDistance = 200f;
            Vector2 direction = aim - head;
            float distance = direction.Length();

            if (distance > maxDistance)
            {
                direction = Vector2.Normalize(direction) * maxDistance;
                aim = head + direction;
            }

            var drawList = ImGui.GetForegroundDrawList();
            drawList.AddLine(head, aim, ColorToUint32(Config.ESPLineColor), 1f);
        }

        public void DrawCorneredBox(float X, float Y, float W, float H, uint color, float thickness)
        {
            var drawList = ImGui.GetForegroundDrawList();

            float lineW = W / 3;
            float lineH = H / 3;

            drawList.AddLine(new Vector2(X, Y - thickness / 2), new Vector2(X, Y + lineH), color, thickness);
            drawList.AddLine(new Vector2(X - thickness / 2, Y), new Vector2(X + lineW, Y), color, thickness);
            drawList.AddLine(new Vector2(X + W - lineW, Y), new Vector2(X + W + thickness / 2, Y), color, thickness);
            drawList.AddLine(new Vector2(X + W, Y - thickness / 2), new Vector2(X + W, Y + lineH), color, thickness);
            drawList.AddLine(new Vector2(X, Y + H - lineH), new Vector2(X, Y + H + thickness / 2), color, thickness);
            drawList.AddLine(new Vector2(X - thickness / 2, Y + H), new Vector2(X + lineW, Y + H), color, thickness);
            drawList.AddLine(new Vector2(X + W - lineW, Y + H), new Vector2(X + W + thickness / 2, Y + H), color, thickness);
            drawList.AddLine(new Vector2(X + W, Y + H - lineH), new Vector2(X + W, Y + H + thickness / 2), color, thickness);
        }

        private void DrawLine(ImDrawListPtr drawList, Vector2 startPos, Vector2 endPos, uint color)
        {
            if (startPos.X > 0 && startPos.Y > 0 && endPos.X > 0 && endPos.Y > 0)
            {
                drawList.AddLine(startPos, endPos, color, 1f);
            }
        }

        private void DrawSkeleton(Entity entity)
        {
            var drawList = ImGui.GetForegroundDrawList();
            uint lineColor = ColorToUint32(Config.ESPSkeletonColor);
            uint circleColor = ColorToUint32(Color.Red);

            var headScreenPos = W2S.WorldToScreen(Core.CameraMatrix, entity.Head, Core.Width, Core.Height);
            var leftWristScreenPos = W2S.WorldToScreen(Core.CameraMatrix, entity.RightWrist, Core.Width, Core.Height);
            var spineScreenPos = W2S.WorldToScreen(Core.CameraMatrix, entity.Spine, Core.Width, Core.Height);
            var hipScreenPos = W2S.WorldToScreen(Core.CameraMatrix, entity.Hip, Core.Width, Core.Height);
            var rootScreenPos = W2S.WorldToScreen(Core.CameraMatrix, entity.Root, Core.Width, Core.Height);
            var rightCalfScreenPos = W2S.WorldToScreen(Core.CameraMatrix, entity.RightCalf, Core.Width, Core.Height);
            var leftCalfScreenPos = W2S.WorldToScreen(Core.CameraMatrix, entity.LeftCalf, Core.Width, Core.Height);
            var rightFootScreenPos = W2S.WorldToScreen(Core.CameraMatrix, entity.RightFoot, Core.Width, Core.Height);
            var leftFootScreenPos = W2S.WorldToScreen(Core.CameraMatrix, entity.LeftFoot, Core.Width, Core.Height);
            var rightWristScreenPos = W2S.WorldToScreen(Core.CameraMatrix, entity.RightWrist, Core.Width, Core.Height);
            var leftHandScreenPos = W2S.WorldToScreen(Core.CameraMatrix, entity.LeftHand, Core.Width, Core.Height);
            var leftShoulderScreenPos = W2S.WorldToScreen(Core.CameraMatrix, entity.LeftShoulder, Core.Width, Core.Height);
            var rightShoulderScreenPos = W2S.WorldToScreen(Core.CameraMatrix, entity.RightShoulder, Core.Width, Core.Height);
            var rightWristJointScreenPos = W2S.WorldToScreen(Core.CameraMatrix, entity.RightWristJoint, Core.Width, Core.Height);
            var leftWristJointScreenPos = W2S.WorldToScreen(Core.CameraMatrix, entity.LeftWristJoint, Core.Width, Core.Height);
            var leftElbowScreenPos = W2S.WorldToScreen(Core.CameraMatrix, entity.LeftElbow, Core.Width, Core.Height);
            var rightElbowScreenPos = W2S.WorldToScreen(Core.CameraMatrix, entity.RightElbow, Core.Width, Core.Height);

            DrawLine(drawList, spineScreenPos, rightShoulderScreenPos, lineColor);
            DrawLine(drawList, spineScreenPos, hipScreenPos, lineColor);
            DrawLine(drawList, spineScreenPos, leftShoulderScreenPos, lineColor);
            DrawLine(drawList, leftShoulderScreenPos, rightElbowScreenPos, lineColor);
            DrawLine(drawList, leftElbowScreenPos, rightWristJointScreenPos, lineColor);
            DrawLine(drawList, rightShoulderScreenPos, leftElbowScreenPos, lineColor);
            DrawLine(drawList, hipScreenPos, rightFootScreenPos, lineColor);
            DrawLine(drawList, hipScreenPos, leftFootScreenPos, lineColor);

            float distance = entity.Distance;
            float baseRadius = 50.0f;
            float circleRadius = baseRadius / distance;

            if (headScreenPos.X > 0 && headScreenPos.Y > 0)
            {
                drawList.AddCircle(headScreenPos, circleRadius, circleColor, 30);
            }
        }

        public void DrawHealthBar(short health, short maxHealth, float X, float Y, float height)
        {
            var vList = ImGui.GetForegroundDrawList();
            float healthPercentage = (float)health / maxHealth;
            float barHeight = height * healthPercentage;

            vList.AddRectFilled(new Vector2(X, Y), new Vector2(X + 3, Y + height), ColorToUint32(Color.Black));
            vList.AddRectFilled(new Vector2(X, Y + (height - barHeight)), new Vector2(X + 3, Y + height), ColorToUint32(Config.ESPHealthColor));
        }

        public void DrawFOVCircle(float radius)
        {
            var drawList = ImGui.GetBackgroundDrawList();
            var center = new Vector2(Core.Width / 2f, Core.Height / 2f);
            uint color = ColorToUint32(Config.FOVColor);
            drawList.AddCircle(center, radius, color, 0, 1f);
        }

        static uint ColorToUint32(Color color)
        {
            return ImGui.ColorConvertFloat4ToU32(new Vector4(
                (float)(color.R / 255.0),
                (float)(color.G / 255.0),
                (float)(color.B / 255.0),
                (float)(color.A / 255.0)));
        }
    }
}