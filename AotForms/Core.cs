// ============================================================
// 🎯 KRISHU X CHEATS - CORE
// ============================================================
// GAME: Free Fire (Garena)
// VERSION: Latest (2026)
// AUTHOR: Krishu X Cheats
// ============================================================

using System.Collections.Concurrent;
using System.Numerics;

namespace AotForms
{
    internal static class Core
    {
        // ============================================================
        // 🎮 GAME STATE
        // ============================================================
        public static uint GameInstance;
        public static Matrix4x4 ViewMatrix;
        internal static IntPtr Handle;
        internal static int Width = -1;
        internal static int Height = -1;
        internal static bool HaveMatrix = false;
        internal static Matrix4x4 CameraMatrix;
        internal static ulong LocalPlayer;
        internal static Vector3 LocalMainCamera;
        public static ConcurrentDictionary<long, Entity> Entities = new();

        // ============================================================
        // 🎯 TARGETING HELPERS
        // ============================================================
        internal static IEnumerable<Entity> GetEntitySnapshot()
        {
            return Entities.Values.Where(e => e != null).ToList();
        }

        internal static Entity GetClosestEnemy()
        {
            if (LocalPlayer == 0 || !HaveMatrix) return null;

            Entity closest = null;
            float closestDist = float.MaxValue;
            var screenCenter = new Vector2(Width / 2f, Height / 2f);

            foreach (var entity in GetEntitySnapshot())
            {
                if (entity.IsDead || !entity.IsKnown || entity.IsTeam == Bool3.True) continue;

                var head2D = W2S.WorldToScreen(CameraMatrix, entity.Head, Width, Height);
                if (head2D.X < 1 || head2D.Y < 1) continue;

                float dist = Vector2.Distance(screenCenter, head2D);
                if (dist < closestDist)
                {
                    closestDist = dist;
                    closest = entity;
                }
            }

            return closest;
        }

        internal static Entity GetClosestEnemyByDistance()
        {
            if (LocalPlayer == 0 || !HaveMatrix) return null;

            Entity closest = null;
            float closestDist = float.MaxValue;

            foreach (var entity in GetEntitySnapshot())
            {
                if (entity.IsDead || !entity.IsKnown || entity.IsTeam == Bool3.True) continue;

                float dist = Vector3.Distance(LocalMainCamera, entity.Head);
                if (dist < closestDist)
                {
                    closestDist = dist;
                    closest = entity;
                }
            }

            return closest;
        }

        internal static Entity GetLowestHealthEnemy()
        {
            if (LocalPlayer == 0 || !HaveMatrix) return null;

            Entity lowest = null;
            float lowestHealth = float.MaxValue;

            foreach (var entity in GetEntitySnapshot())
            {
                if (entity.IsDead || !entity.IsKnown || entity.IsTeam == Bool3.True) continue;

                if (entity.Health < lowestHealth)
                {
                    lowestHealth = entity.Health;
                    lowest = entity;
                }
            }

            return lowest;
        }

        // ============================================================
        // 🔄 ENTITY MANAGEMENT
        // ============================================================
        internal static void AddOrUpdateEntity(uint address, Entity entity)
        {
            Entities.AddOrUpdate(address, entity, (key, old) => entity);
        }

        internal static void RemoveEntity(uint address)
        {
            Entities.TryRemove(address, out _);
        }

        internal static void ClearEntities()
        {
            Entities.Clear();
        }

        // ============================================================
        // 🧹 RESET STATE
        // ============================================================
        internal static void ResetState()
        {
            HaveMatrix = false;
            LocalPlayer = 0;
            LocalMainCamera = Vector3.Zero;
            Entities.Clear();
            InternalMemory.ClearCache();
        }
    }
}