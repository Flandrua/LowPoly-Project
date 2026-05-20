using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;

public static class OfficerBuildCli
{
    public static void BuildOfficerForResearch()
    {
        var projectRoot = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "."));
        var outputRoot = Path.GetFullPath(Path.Combine(projectRoot, "..", "OfficerForResearch"));
        Directory.CreateDirectory(outputRoot);

        UnityEngine.Debug.Log("[OfficerBuildCli] Generating Puerts IL2CPP files...");
        Puerts.Editor.Generator.UnityMenu.GenV2WithoutWrapper();

        var enabledScenes = EditorBuildSettings.scenes
            .Where(scene => scene.enabled)
            .Select(scene => scene.path)
            .ToArray();

        if (enabledScenes.Length == 0)
        {
            throw new InvalidOperationException("No enabled scenes in Build Settings.");
        }

        var target = EditorUserBuildSettings.activeBuildTarget;
        var outputPath = GetOutputPath(outputRoot, target);

        UnityEngine.Debug.Log($"[OfficerBuildCli] Build target: {target}");
        UnityEngine.Debug.Log($"[OfficerBuildCli] Output path: {outputPath}");

        var options = new BuildPlayerOptions
        {
            scenes = enabledScenes,
            target = target,
            targetGroup = BuildPipeline.GetBuildTargetGroup(target),
            locationPathName = outputPath,
            options = BuildOptions.None
        };

        var report = BuildPipeline.BuildPlayer(options);
        var summary = report.summary;

        UnityEngine.Debug.Log($"[OfficerBuildCli] Build result: {summary.result}, errors: {summary.totalErrors}, warnings: {summary.totalWarnings}, duration: {summary.totalTime}");

        if (summary.result != BuildResult.Succeeded)
        {
            throw new Exception($"Build failed with result: {summary.result}. See Editor.log for details.");
        }
    }

    private static string GetOutputPath(string outputRoot, BuildTarget target)
    {
        switch (target)
        {
            case BuildTarget.StandaloneWindows:
            case BuildTarget.StandaloneWindows64:
                return Path.Combine(outputRoot, "OfficerForResearch.exe");
            case BuildTarget.Android:
                return Path.Combine(outputRoot, "OfficerForResearch.apk");
            default:
                return Path.Combine(outputRoot, $"OfficerForResearch_{target}");
        }
    }
}
