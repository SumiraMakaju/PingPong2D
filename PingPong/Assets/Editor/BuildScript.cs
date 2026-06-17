using UnityEditor;
using UnityEngine;

public class BuildScript
{
    [MenuItem("Build/Build Android APK")]
    public static void PerformAndroidBuild()
    {
        string[] scenes = { "Assets/Scenes/Main.unity" };
        string outputPath = "Builds/Android/PingPong.apk";

        // Create the directory if it doesn't exist
        System.IO.Directory.CreateDirectory("Builds/Android");

        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = scenes;
        buildPlayerOptions.locationPathName = outputPath;
        buildPlayerOptions.target = BuildTarget.Android;
        buildPlayerOptions.options = BuildOptions.None;

        Debug.Log("Starting Android build...");
        var report = BuildPipeline.BuildPlayer(buildPlayerOptions);
        var summary = report.summary;

        if (summary.result == UnityEditor.Build.Reporting.BuildResult.Succeeded)
        {
            Debug.Log("Android build completed successfully: " + outputPath);
        }
        else
        {
            Debug.LogError("Android build failed with result: " + summary.result);
        }
    }
}
