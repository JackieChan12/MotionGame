using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System;

[Serializable]
public class JSONAnimClip {
    public float frameRate = 30f;
    public JSONKeyframe[] keyframes;
}

[Serializable]
public class JSONKeyframe {
    public float time;
    public JSONBoneInfo[] bones;
}

[Serializable]
public class JSONBoneInfo {
    public string name;
    public Vector3 localPosition;
    public Quaternion localRotation;
}

public class AnimationFromJsonCreator : EditorWindow
{
    private GameObject targetPrefab;
    private TextAsset jsonFile;
    private string animationName = "NewAnimation";
    private string outputFolder = "Assets";

    private JSONAnimClip currentAnimData;
    private float previewTime = 0f;
    private GameObject previewInstance;
    private AnimationClip previewClip;

    private RenderTexture previewTexture;
    private Camera previewCamera;
    private GameObject previewCameraGO;
    private int previewLayer = 31;

    private Vector2 cameraOrbit = new Vector2(0, 10f);
    private bool isPlaying = false;
    private double lastTime = 0;

    private bool importPositions = false;
    private bool importRotations = true;

    [MenuItem("Window/Animation/JSON to Animation Creator")]
    public static void ShowWindow()
    {
        GetWindow<AnimationFromJsonCreator>("JSON to Anim");
    }

    private void OnGUI()
    {
        GUILayout.Label("Input Settings", EditorStyles.boldLabel);
        
        targetPrefab = (GameObject)EditorGUILayout.ObjectField("Target Prefab", targetPrefab, typeof(GameObject), false);
        jsonFile = (TextAsset)EditorGUILayout.ObjectField("JSON File", jsonFile, typeof(TextAsset), false);
        animationName = EditorGUILayout.TextField("Animation Name", animationName);
        
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.TextField("Output Folder", outputFolder);
        if (GUILayout.Button("Browse", GUILayout.Width(70)))
        {
            string path = EditorUtility.OpenFolderPanel("Select Output Folder", "Assets", "");
            if (!string.IsNullOrEmpty(path))
            {
                if (path.StartsWith(Application.dataPath))
                    outputFolder = "Assets" + path.Substring(Application.dataPath.Length);
                else
                    Debug.LogError("Error: Please select a folder inside the Assets directory.");
            }
        }
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(5);
        EditorGUILayout.LabelField("Import Settings", EditorStyles.boldLabel);
        importPositions = EditorGUILayout.Toggle("Import Positions", importPositions);
        importRotations = EditorGUILayout.Toggle("Import Rotations", importRotations);

        GUILayout.Space(10);
        
        GUI.enabled = targetPrefab != null && jsonFile != null;
        if (GUILayout.Button("Load Data for Preview"))
        {
            LoadJSONData();
            if (currentAnimData != null)
            {
                previewClip = GenerateClip();
                InstantiatePreview();
            }
        }
        GUI.enabled = true;

        if (currentAnimData != null && previewClip != null && previewInstance != null)
        {
            GUILayout.Space(10);
            GUILayout.Label("Preview Playback", EditorStyles.boldLabel);
            
            if (previewTexture != null)
            {
                Rect texRect = GUILayoutUtility.GetRect(256, 256, GUILayout.ExpandWidth(true));
                GUI.DrawTexture(texRect, previewTexture, ScaleMode.ScaleToFit);

                Event e = Event.current;
                if (texRect.Contains(e.mousePosition))
                {
                    if (e.type == EventType.MouseDrag && e.button == 0) // Left click drag
                    {
                        cameraOrbit.x -= e.delta.x * 0.8f;
                        cameraOrbit.y += e.delta.y * 0.8f;
                        cameraOrbit.y = Mathf.Clamp(cameraOrbit.y, -45f, 85f);
                        e.Use();
                        
                        UpdateCamera();
                        if (previewCamera != null) previewCamera.Render();
                        Repaint();
                    }
                }
            }
            
            float maxTime = currentAnimData.keyframes[currentAnimData.keyframes.Length - 1].time;
            
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(isPlaying ? "Pause" : "Play ", GUILayout.Width(60)))
            {
                isPlaying = !isPlaying;
                lastTime = EditorApplication.timeSinceStartup;
            }
            
            EditorGUI.BeginChangeCheck();
            previewTime = EditorGUILayout.Slider(previewTime, 0f, maxTime);
            if (EditorGUI.EndChangeCheck())
            {
                isPlaying = false;
                SamplePreview();
            }
            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("Close Preview"))
            {
                CleanupPreview();
            }
            
            GUILayout.Space(10);
            GUI.backgroundColor = Color.green;
            if (GUILayout.Button("Generate & Save AnimationClip", GUILayout.Height(40)))
            {
                SaveAnimationClip();
            }
            GUI.backgroundColor = Color.white;
        }
    }

    private void OnEnable()
    {
        EditorApplication.update += EditorUpdate;
    }

    private void OnDisable()
    {
        EditorApplication.update -= EditorUpdate;
        CleanupPreview();
    }

    private void EditorUpdate()
    {
        if (isPlaying && currentAnimData != null && previewClip != null && previewInstance != null)
        {
            double currentTime = EditorApplication.timeSinceStartup;
            float deltaTime = (float)(currentTime - lastTime);
            lastTime = currentTime;

            float maxTime = currentAnimData.keyframes[currentAnimData.keyframes.Length - 1].time;
            previewTime += deltaTime;
            if (previewTime > maxTime) previewTime -= maxTime;
            
            SamplePreview();
            Repaint();
        }
    }

    private void LoadJSONData()
    {
        if (jsonFile == null) return;
        try {
            currentAnimData = JsonUtility.FromJson<JSONAnimClip>(jsonFile.text);
            if (currentAnimData == null || currentAnimData.keyframes == null || currentAnimData.keyframes.Length == 0)
            {
                Debug.LogError("Failed to parse JSON. Please check if the structure matches the required format.");
                currentAnimData = null;
            }
        } catch (Exception e) {
            Debug.LogError("JSON Parse Error: " + e.Message);
        }
    }

    private void InstantiatePreview()
    {
        if (targetPrefab == null) return;
        
        CleanupPreview(keepClip: true);

        previewInstance = (GameObject)PrefabUtility.InstantiatePrefab(targetPrefab);
        if (previewInstance == null) return;
        
        previewInstance.SetActive(true);
        previewInstance.hideFlags = HideFlags.HideAndDontSave;
        
        SetLayerRecursively(previewInstance, previewLayer);
        previewInstance.transform.position = new Vector3(0, -9000f, 0);

        // Very Important: Disable Animator to prevent it from locking/overriding the manual SampleAnimation
        foreach (var animator in previewInstance.GetComponentsInChildren<Animator>())
        {
            animator.enabled = false;
        }

        // Very Important: Force meshes to render even if far away from main scene cameras
        foreach (var smr in previewInstance.GetComponentsInChildren<SkinnedMeshRenderer>())
        {
            smr.updateWhenOffscreen = true;
        }

        previewCameraGO = new GameObject("PreviewCamera_Hidden");
        previewCameraGO.hideFlags = HideFlags.HideAndDontSave;
        previewCamera = previewCameraGO.AddComponent<Camera>();
        previewCamera.enabled = false; // Only render manually when SamplePreview is called!
        previewCamera.cameraType = CameraType.Game; 
        previewCamera.cullingMask = 1 << previewLayer;
        previewCamera.clearFlags = CameraClearFlags.SolidColor;
        previewCamera.backgroundColor = new Color(0.2f, 0.2f, 0.25f, 1f);

        previewTexture = new RenderTexture(512, 512, 16);
        previewTexture.Create();
        previewCamera.targetTexture = previewTexture;

        Light light = previewCameraGO.AddComponent<Light>();
        light.type = LightType.Directional;
        light.cullingMask = 1 << previewLayer;
        light.intensity = 1.2f;
        
        previewTime = 0f;
        cameraOrbit = new Vector2(0, 10f); // Set default orbit angle
        UpdateCamera();
        SamplePreview();
    }

    private void UpdateCamera()
    {
        if (previewCamera == null || previewInstance == null) return;
        
        float height = 2.0f;
        Vector3 center = previewInstance.transform.position + new Vector3(0, height * 0.5f, 0);

        Quaternion rotation = Quaternion.Euler(cameraOrbit.y, cameraOrbit.x, 0);
        previewCamera.transform.position = center + rotation * new Vector3(0, 0, 3.0f);
        previewCamera.transform.LookAt(center);

        // Always keep the light facing the character from the camera's perspective
        if (previewCameraGO != null) 
        {
            Light light = previewCameraGO.GetComponent<Light>();
            if (light != null) light.transform.rotation = previewCamera.transform.rotation;
        }
    }

    private void SetLayerRecursively(GameObject obj, int newLayer)
    {
        if (null == obj) return;
        obj.layer = newLayer;
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }

    private void CleanupPreview(bool keepClip = false)
    {
        if (previewInstance != null)
        {
            DestroyImmediate(previewInstance);
            previewInstance = null;
        }
        if (previewCameraGO != null)
        {
            DestroyImmediate(previewCameraGO);
            previewCameraGO = null;
        }
        if (previewTexture != null)
        {
            previewTexture.Release();
            DestroyImmediate(previewTexture);
            previewTexture = null;
        }
        
        if (!keepClip)
        {
            currentAnimData = null;
            if (previewClip != null)
            {
                DestroyImmediate(previewClip);
                previewClip = null;
            }
        }
    }

    private void SamplePreview()
    {
        if (previewClip != null && previewInstance != null)
        {
            previewClip.SampleAnimation(previewInstance, previewTime);
            if (previewCamera != null)
            {
                previewCamera.Render();
            }
        }
    }

    private AnimationClip GenerateClip()
    {
        if (currentAnimData == null || targetPrefab == null) return null;

        AnimationClip clip = new AnimationClip();
        clip.frameRate = currentAnimData.frameRate;

        // Build bone paths from prefab. This makes the script support any "similar" skeleton.
        Dictionary<string, string> bonePaths = GetBonePaths(targetPrefab.transform, "");

        Dictionary<string, AnimationCurve> curves = new Dictionary<string, AnimationCurve>();

        List<string> propsList = new List<string>();
        if (importPositions) {
            propsList.AddRange(new string[] { "m_LocalPosition.x", "m_LocalPosition.y", "m_LocalPosition.z" });
        }
        if (importRotations) {
            propsList.AddRange(new string[] { "m_LocalRotation.x", "m_LocalRotation.y", "m_LocalRotation.z", "m_LocalRotation.w" });
        }
        string[] props = propsList.ToArray();

        foreach (var frame in currentAnimData.keyframes)
        {
            foreach (var bone in frame.bones)
            {
                if (!bonePaths.ContainsKey(bone.name)) continue;
                string path = bonePaths[bone.name];
                
                foreach (var prop in props)
                {
                    string key = path + "|" + prop;
                    if (!curves.ContainsKey(key))
                    {
                        curves[key] = new AnimationCurve();
                    }
                }
            }
        }

        foreach (var frame in currentAnimData.keyframes)
        {
            foreach (var bone in frame.bones)
            {
                if (!bonePaths.ContainsKey(bone.name)) continue;
                string path = bonePaths[bone.name];

                if (importPositions) {
                    curves[path + "|m_LocalPosition.x"].AddKey(frame.time, bone.localPosition.x);
                    curves[path + "|m_LocalPosition.y"].AddKey(frame.time, bone.localPosition.y);
                    curves[path + "|m_LocalPosition.z"].AddKey(frame.time, bone.localPosition.z);
                }
                
                if (importRotations) {
                    curves[path + "|m_LocalRotation.x"].AddKey(frame.time, bone.localRotation.x);
                    curves[path + "|m_LocalRotation.y"].AddKey(frame.time, bone.localRotation.y);
                    curves[path + "|m_LocalRotation.z"].AddKey(frame.time, bone.localRotation.z);
                    curves[path + "|m_LocalRotation.w"].AddKey(frame.time, bone.localRotation.w);
                }
            }
        }

        foreach (var kvp in curves)
        {
            string[] parts = kvp.Key.Split('|');
            string path = parts[0];
            string propertyName = parts[1];
            clip.SetCurve(path, typeof(Transform), propertyName, kvp.Value);
        }
        
        clip.EnsureQuaternionContinuity();
        return clip;
    }

    private void SaveAnimationClip()
    {
        AnimationClip finalClip = GenerateClip();
        if (finalClip == null) return;

        string fullPath = outputFolder + "/" + animationName + ".anim";
        AssetDatabase.CreateAsset(finalClip, fullPath);
        AssetDatabase.SaveAssets();
        
        Debug.Log("Animation Clip successfully saved to: " + fullPath);
        
        CleanupPreview();
    }

    private Dictionary<string, string> GetBonePaths(Transform current, string currentPath)
    {
        Dictionary<string, string> paths = new Dictionary<string, string>();
        
        if (!string.IsNullOrEmpty(currentPath))
        {
            paths[current.name] = currentPath;
        }

        for (int i = 0; i < current.childCount; i++)
        {
            Transform child = current.GetChild(i);
            string childPath = string.IsNullOrEmpty(currentPath) ? child.name : currentPath + "/" + child.name;
            Dictionary<string, string> childPaths = GetBonePaths(child, childPath);
            foreach (var kvp in childPaths)
            {
                if (!paths.ContainsKey(kvp.Key))
                {
                    paths.Add(kvp.Key, kvp.Value);
                }
            }
        }
        return paths;
    }
}
