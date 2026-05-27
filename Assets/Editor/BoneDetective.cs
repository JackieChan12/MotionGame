using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace MotionGame.Animation
{
    public static class BoneDetective
    {
        private static Dictionary<string, string[]> synonyms = new Dictionary<string, string[]>
        {
            { StandardBones.Hips, new[] { "hips", "pelvis", "pelvis_jnt", "root" } },
            { StandardBones.Spine, new[] { "spine", "spine_01", "spine_jnt", "waist" } },
            { StandardBones.Chest, new[] { "chest", "spine_02", "spine_03", "upper_spine" } },
            { StandardBones.Neck, new[] { "neck", "neck_jnt", "neck_01" } },
            { StandardBones.Head, new[] { "head", "head_jnt", "skull" } },
            
            { StandardBones.LeftUpperArm, new[] { "leftupperarm", "left_arm", "l_arm", "l_upperarm", "l_shoulder" } },
            { StandardBones.LeftLowerArm, new[] { "leftlowerarm", "left_forearm", "l_forearm", "l_elbow" } },
            { StandardBones.LeftHand, new[] { "lefthand", "left_hand", "l_hand", "l_wrist" } },
            
            { StandardBones.RightUpperArm, new[] { "rightupperarm", "right_arm", "r_arm", "r_upperarm", "r_shoulder" } },
            { StandardBones.RightLowerArm, new[] { "rightlowerarm", "right_forearm", "r_forearm", "r_elbow" } },
            { StandardBones.RightHand, new[] { "righthand", "right_hand", "r_hand", "r_wrist" } },

            { StandardBones.LeftUpperLeg, new[] { "leftupperleg", "left_upleg", "l_upleg", "l_thigh" } },
            { StandardBones.LeftLowerLeg, new[] { "leftlowerleg", "left_leg", "l_leg", "l_knee" } },
            { StandardBones.LeftFoot, new[] { "leftfoot", "left_foot", "l_foot", "l_ankle" } },

            { StandardBones.RightUpperLeg, new[] { "rightupperleg", "right_upleg", "r_upleg", "r_thigh" } },
            { StandardBones.RightLowerLeg, new[] { "rightlowerleg", "right_leg", "r_leg", "r_knee" } },
            { StandardBones.RightFoot, new[] { "rightfoot", "right_foot", "r_foot", "r_ankle" } }
        };

        /// <summary>
        /// Finds the best matching transform for each standard bone.
        /// </summary>
        public static Dictionary<string, Transform> DetectBones(GameObject root)
        {
            Dictionary<string, Transform> map = new Dictionary<string, Transform>();
            Animator animator = root.GetComponent<Animator>();
            Transform[] allTransforms = root.GetComponentsInChildren<Transform>(true);

            // 1. Try Humanoid Mapping (Highest Priority)
            if (animator != null && animator.isHuman)
            {
                map[StandardBones.Hips] = animator.GetBoneTransform(HumanBodyBones.Hips);
                map[StandardBones.Spine] = animator.GetBoneTransform(HumanBodyBones.Spine);
                map[StandardBones.Chest] = animator.GetBoneTransform(HumanBodyBones.Chest);
                map[StandardBones.Neck] = animator.GetBoneTransform(HumanBodyBones.Neck);
                map[StandardBones.Head] = animator.GetBoneTransform(HumanBodyBones.Head);
                
                map[StandardBones.LeftUpperArm] = animator.GetBoneTransform(HumanBodyBones.LeftUpperArm);
                map[StandardBones.LeftLowerArm] = animator.GetBoneTransform(HumanBodyBones.LeftLowerArm);
                map[StandardBones.LeftHand] = animator.GetBoneTransform(HumanBodyBones.LeftHand);
                
                map[StandardBones.RightUpperArm] = animator.GetBoneTransform(HumanBodyBones.RightUpperArm);
                map[StandardBones.RightLowerArm] = animator.GetBoneTransform(HumanBodyBones.RightLowerArm);
                map[StandardBones.RightHand] = animator.GetBoneTransform(HumanBodyBones.RightHand);

                map[StandardBones.LeftUpperLeg] = animator.GetBoneTransform(HumanBodyBones.LeftUpperLeg);
                map[StandardBones.LeftLowerLeg] = animator.GetBoneTransform(HumanBodyBones.LeftLowerLeg);
                map[StandardBones.LeftFoot] = animator.GetBoneTransform(HumanBodyBones.LeftFoot);

                map[StandardBones.RightUpperLeg] = animator.GetBoneTransform(HumanBodyBones.RightUpperLeg);
                map[StandardBones.RightLowerLeg] = animator.GetBoneTransform(HumanBodyBones.RightLowerLeg);
                map[StandardBones.RightFoot] = animator.GetBoneTransform(HumanBodyBones.RightFoot);

                // Filter out nulls
                var keys = map.Keys.ToList();
                foreach (var k in keys) { if (map[k] == null) map.Remove(k); }
                
                if (map.Count > 5) return map; // If we found most core bones, we're good
            }

            // 2. Try Name-based Mapping (Heuristic)
            foreach (var standard in StandardBones.All)
            {
                if (map.ContainsKey(standard)) continue;

                string[] patterns = synonyms[standard];
                foreach (var t in allTransforms)
                {
                    string lowName = t.name.ToLower();
                    if (patterns.Any(p => lowName.Contains(p)))
                    {
                        // Minor optimization: check side for limbs
                        if (standard.Contains("Left") && (lowName.Contains("right") || lowName.StartsWith("r_"))) continue;
                        if (standard.Contains("Right") && (lowName.Contains("left") || lowName.StartsWith("l_"))) continue;

                        map[standard] = t;
                        break;
                    }
                }
            }

            return map;
        }

        /// <summary>
        /// Estimates the height of the character for normalization.
        /// </summary>
        public static float EstimateHeight(GameObject root)
        {
            SkinnedMeshRenderer[] renderers = root.GetComponentsInChildren<SkinnedMeshRenderer>();
            if (renderers.Length > 0)
            {
                Bounds b = renderers[0].bounds;
                for (int i = 1; i < renderers.Length; i++) b.Encapsulate(renderers[i].bounds);
                return b.size.y;
            }
            return 1.8f; // Fallback
        }
    }
}
