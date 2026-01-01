
using System.IO;
using UnityEditor;
using UnityEngine;

public class SceneBuildTools
{
    [MenuItem("Tools/Build Fruit Game")]
    public static void BuildScene1()
    {
        BuildScene("Assets/FruitSlash/Scene/MainFruitSlash.unity", "BuildFruitGame/BuildFruitGame.exe");
    }

    [MenuItem("Tools/Build Shape Game")]
    public static void BuildScene2()
    {
        BuildScene("Assets/GatheringTheGivenShapes/Scene/MainShapes.unity", "BuildShapeGame/BuildShapeGame.exe");
    }

    [MenuItem("Tools/Build Hamburger House")]
    public static void BuildScene3()
    {
        BuildScene("Assets/HamburgerHouse/Scene/MainGame.unity", "BuildHamburger/BuildHamburgerHouse.exe");
    }

    private static void BuildScene(string scenePath, string buildPath)
    {
        string path = EditorUtility.SaveFilePanel("Chọn đường dẫn lưu file build", "", "Build app", "");
        if (!string.IsNullOrEmpty(buildPath))
        {
            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
            buildPlayerOptions.scenes = new[] { scenePath };
            buildPlayerOptions.locationPathName = Path.Combine(path, buildPath);
            buildPlayerOptions.target = BuildTarget.StandaloneWindows64;
            buildPlayerOptions.options = BuildOptions.None;
            BuildPipeline.BuildPlayer(buildPlayerOptions);

            Debug.Log("Build hoàn thành. File đầu ra: " + buildPath);
        }
        else
        {
            Debug.LogWarning("Build đã bị hủy bởi người dùng.");
        }
    }
}
