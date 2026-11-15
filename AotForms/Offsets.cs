using System;

namespace AotForms
{
    internal static class Offsets
    {
        internal static ulong Il2Cpp = 0x0;

        // STATIC / BASE
        internal static uint InitBase = 0xA4632F0; // UPDATED
        internal static uint StaticClass = 0x5C;

        // ARMS / WEAPON
        internal static uint Weapon = 0x43C;       // UPDATED
        internal static uint WeaponData = 0x58;
        internal static uint IntWeaponType = 0x64;
        internal static uint WeaponRecoil = 0xC;
        internal static uint NoReload = 0x99;      // UPDATED

        // CAMERA
        internal static uint FollowCamera = 0x49C; // UPDATED
        internal static uint AimRotation = 0x408;  // UPDATED

        // GENERAL
        internal static uint CurrentMatch = 0x50;
        internal static uint MatchStatus = 0x8C;
        internal static uint LocalPlayer = 0x94;
        internal static uint Camera = 0x18;
        internal static uint Player_Data = 0x48;
        internal static uint AvatarManager = 0x50C; // UPDATED
        internal static uint Avatar = 0xA8;
        internal static uint Player_ShadowBase = 0x18C8; // UPDATED
        internal static uint UnlimitedAmmo = 0xE0;
        internal static uint Avatar_Data = 0x14;
        internal static uint Avatar_Data_IsTeam = 0x59;
        internal static uint Avatar_IsVisible = 0x95;
        internal static uint IS_FIRING = 0x548; // UPDATED

        // PLAYER
        internal static uint Player_IsDead = 0x50;
        internal static uint Player_Name = 0x2E4; // UPDATED
        internal static uint XPose = 0x78;
        internal static uint AimbotVisible = 0x4F0;    // was 0x4E8;
        internal static uint PlayerAttributes = 0x508; // UPDATED
        internal static uint MainCameraTransform = 0x290; // UPDATED
        internal static uint DictionaryEntities = 0x68;
        internal static uint ViewMatrix = 0xE8;

        // SILENT AIM
        internal static uint sAim1 = 0x548; // UPDATED
        internal static uint sAim2 = 0x980; // UPDATED
        internal static uint sAim3 = 0x38;
        internal static uint sAim4 = 0x2C;
        internal static uint SientAim = 0x980; // NOT FOUND - unchanged earlier - 0x548;

        // EMOTE CHANGER

        // SPEED INTERNAL
        internal static uint GameTimer = 0x10;
        internal static uint FixedDeltaTime = 0x24;

        internal static uint adarshcheat = 0x54; // NOT FOUND - unchanged then try  0x4F0;
    }
}