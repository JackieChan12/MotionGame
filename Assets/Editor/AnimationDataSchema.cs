using UnityEngine;
using System;
using System.Collections.Generic;

namespace MotionGame.Animation
{
    [Serializable]
    public class JSONMetadata
    {
        public string generator = "MotionGame Animation Tool";
        public string version = "2.0";
        public float frameRate = 30f;
        public float sourceHeight = 1.8f; // Standardized height for normalization
        public int frameCount;
        public List<string> boneNames; // List of bones included in this clip
    }

    [Serializable]
    public class JSONBoneInfo
    {
        public string name; // Standard name (e.g., Hips, LeftArm)
        public Vector3 localPos; // Only used for Hips (Root)
        public Quaternion localRot;
    }

    [Serializable]
    public class JSONKeyframe
    {
        public float time;
        public List<JSONBoneInfo> bones;
    }

    [Serializable]
    public class JSONAnimClip
    {
        public JSONMetadata metadata;
        public List<JSONKeyframe> keyframes;
    }

    /// <summary>
    /// Standard Bone Names used for cross-character retargeting.
    /// Based on Unity Humanoid standard.
    /// </summary>
    public static class StandardBones
    {
        public const string Hips = "Hips";
        public const string Spine = "Spine";
        public const string Chest = "Chest";
        public const string Neck = "Neck";
        public const string Head = "Head";
        
        public const string LeftUpperArm = "LeftUpperArm";
        public const string LeftLowerArm = "LeftLowerArm";
        public const string LeftHand = "LeftHand";
        
        public const string RightUpperArm = "RightUpperArm";
        public const string RightLowerArm = "RightLowerArm";
        public const string RightHand = "RightHand";
        
        public const string LeftUpperLeg = "LeftUpperLeg";
        public const string LeftLowerLeg = "LeftLowerLeg";
        public const string LeftFoot = "LeftFoot";
        
        public const string RightUpperLeg = "RightUpperLeg";
        public const string RightLowerLeg = "RightLowerLeg";
        public const string RightFoot = "RightFoot";

        public static readonly string[] All = {
            Hips, Spine, Chest, Neck, Head,
            LeftUpperArm, LeftLowerArm, LeftHand,
            RightUpperArm, RightLowerArm, RightHand,
            LeftUpperLeg, LeftLowerLeg, LeftFoot,
            RightUpperLeg, RightLowerLeg, RightFoot
        };
    }
}
