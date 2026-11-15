namespace AotForms
{
    internal enum Bones : uint
    {
        // Core
        Head = 0x4A4,       // UPDATED
        Neck = 0x4A4,
        Spine = 0x4AC,      // UPDATED
        Hip = 0x4A8,        // UPDATED
        Root = 0x4B8,       // UPDATED
        Chest = 0x4A0,     // NOT FOUND - unchanged

        // Arms
        LeftShoulder = 0x4D8, // UPDATED
        LeftElbow = 0x4EC,    // UPDATED
        LeftWrist = 0x4E4,    // UPDATED

        RightShoulder = 0x4DC, // UPDATED
        RightElbow = 0x4E8,    // UPDATED
        RightWrist = 0x4E0,    // UPDATED

        // Legs
        LeftCalf = 0x4C0,     // UPDATED
        RightCalf = 0x4C4,    // UPDATED

        LeftFoot = 0x4C8,     // UPDATED
        RightFoot = 0x4CC,   // UPDATED

        // Extra / Transform
        // Bone_Extra1 = 0x454,
        // Bone_Extra2 = 0x468,
        // Bone_Extra4 = 0x484,
        // Bone_Transform = 0x488
    }
}