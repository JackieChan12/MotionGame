using UnityEngine;
using UnityEditor;

public static class TestCurveBinding {
    [MenuItem("Tools/Test Curve Binding")]
    public static void Test() {
        AnimationClip clip = new AnimationClip();
        
        AnimationCurve curve = new AnimationCurve();
        curve.AddKey(0, 0);
        
        clip.SetCurve("", typeof(Transform), "localPosition.x", curve);
        clip.SetCurve("", typeof(Transform), "localRotation.x", curve);
        
        EditorCurveBinding[] bindings = AnimationUtility.GetCurveBindings(clip);
        foreach(var b in bindings) {
            Debug.Log(b.path + " - " + b.type + " - " + b.propertyName);
        }
    }
}
