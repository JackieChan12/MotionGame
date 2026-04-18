using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class ExtractFBXToJSON : EditorWindow
{
    private AnimationClip sourceClip;
    private GameObject sourcePrefab;
    private string outputPath = "Assets/_For_SS2/Hurdle_Race/RunningAnimTimeline.json";
    private int sampleFrameRate = 30;

    [MenuItem("Window/Animation/FBX to JSON Extractor")]
    public static void ShowWindow()
    {
        GetWindow<ExtractFBXToJSON>("FBX to JSON");
    }

    private void OnGUI()
    {
        GUILayout.Label("FBX Animation to JSON Extractor", EditorStyles.boldLabel);
        
        sourceClip = (AnimationClip)EditorGUILayout.ObjectField("Source Animation Clip", sourceClip, typeof(AnimationClip), false);
        sourcePrefab = (GameObject)EditorGUILayout.ObjectField("Model Prefab (To Sample)", sourcePrefab, typeof(GameObject), false);
        
        sampleFrameRate = EditorGUILayout.IntSlider("Sample Frame Rate", sampleFrameRate, 1, 60);
        outputPath = EditorGUILayout.TextField("Output JSON Path", outputPath);

        GUILayout.Space(20);

        if (GUILayout.Button("Extract & Save JSON", GUILayout.Height(40)))
        {
            if (sourceClip == null || sourcePrefab == null)
            {
                Debug.LogError("Error: Source Clip and Prefab must be assigned.");
                return;
            }
            ExtractData();
        }
    }

    private void ExtractData()
    {
        GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(sourcePrefab);
        
        // Cần thu thập những xương cơ bản
        string[] targetBones = { "Hips", "Spine", "Spine1", "Spine2", "Neck", "Head",
                                 "RightUpLeg", "RightLeg", "RightFoot", 
                                 "LeftUpLeg", "LeftLeg", "LeftFoot", 
                                 "LeftShoulder", "LeftArm", "LeftForeArm", "LeftHand", 
                                 "RightShoulder", "RightArm", "RightForeArm", "RightHand" };
                                 
        Dictionary<string, Transform> bones = new Dictionary<string, Transform>();
        Transform[] allBones = instance.GetComponentsInChildren<Transform>(true);
        foreach(var b in allBones) {
            if (!bones.ContainsKey(b.name)) {
                bones.Add(b.name, b);
            }
        }

        JSONAnimClip jsonClip = new JSONAnimClip();
        jsonClip.frameRate = sampleFrameRate;
        
        float length = sourceClip.length;
        int frameCount = Mathf.CeilToInt(length * sampleFrameRate);
        if (frameCount == 0) frameCount = 1;
        
        List<JSONKeyframe> keyframes = new List<JSONKeyframe>();

        for (int i = 0; i <= frameCount; i++)
        {
            float time = Mathf.Clamp((float)i / sampleFrameRate, 0f, length);
            
            // Ép tư thế của model instance theo đúng thời điểm trong Animation FBX
            sourceClip.SampleAnimation(instance, time);
            
            JSONKeyframe kf = new JSONKeyframe();
            kf.time = time;
            
            List<JSONBoneInfo> boneInfos = new List<JSONBoneInfo>();
            foreach (string bName in targetBones)
            {
                if (bones.ContainsKey(bName))
                {
                    Transform t = bones[bName];
                    JSONBoneInfo info = new JSONBoneInfo();
                    info.name = bName;
                    info.localPosition = t.localPosition;
                    info.localRotation = t.localRotation;
                    boneInfos.Add(info);
                }
            }
            kf.bones = boneInfos.ToArray();
            keyframes.Add(kf);
        }
        
        jsonClip.keyframes = keyframes.ToArray();
        
        // Auto create directories if not exists
        FileInfo fileInfo = new FileInfo(outputPath);
        fileInfo.Directory.Create();
        
        File.WriteAllText(outputPath, JsonUtility.ToJson(jsonClip, true));
        Debug.Log("Successfully extracted FBX animation to JSON: " + outputPath);
        
        DestroyImmediate(instance);
        AssetDatabase.Refresh();
    }
}
