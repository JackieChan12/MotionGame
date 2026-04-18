using UnityEngine;
using UnityEditor;
using System.IO;

public static class TestJsonParse {
    [InitializeOnLoadMethod]
    public static void Test() {
        string path = "Assets/_For_SS2/Hurdle_Race/RunningAnimTimeline.json";
        if (!File.Exists(path)) return;
        string json = File.ReadAllText(path);
        
        try {
            JSONAnimClip clip = JsonUtility.FromJson<JSONAnimClip>(json);
            if (clip == null) {
                Debug.LogError("Parsed clip is null.");
            } else if (clip.keyframes == null) {
                Debug.LogError("keyframes array is null.");
            } else {
                Debug.Log("Parsed successfully! keyframes count: " + clip.keyframes.Length);
                foreach(var k in clip.keyframes) {
                    if (k.bones == null) {
                       Debug.LogError("Bones array is null for time: " + k.time);
                    } else {
                       Debug.Log("Time: " + k.time + " Bones: " + k.bones.Length);
                    }
                }
            }
        } catch (System.Exception e) {
            Debug.LogError("Exception parsing JSON test: " + e.Message);
        }
    }
}
