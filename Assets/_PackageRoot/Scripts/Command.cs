using UnityEditor;
using System.Linq;
using System;

namespace UBuilder
{
    public static partial class Command
    {
        public static string[] GetScenePaths() => EditorBuildSettings.scenes.Select(x => x.path).ToArray();

        [MenuItem("File/UBuilder/Build Current Platform")]
        public static void Build()
        {
            Console.WriteLine($"{EditorUserBuildSettings.activeBuildTarget} build started");

            if (Utils.GetVariableBool(Command.Variables.Development) != null) EditorUserBuildSettings.development = Utils.GetVariableBool(Command.Variables.Development).Value;
            if (Utils.BuildNumberInt() >= 0)                                  PlayerSettings.Android.bundleVersionCode = Utils.BuildNumberInt();
            if (Utils.GetVariable(Variables.BuildVersion) != null)            PlayerSettings.bundleVersion = Utils.GetVariable(Variables.BuildVersion);

            var destination = Utils.GetVariable(Command.Variables.OutputDestination) ?? $"Builds/{EditorUserBuildSettings.activeBuildTarget}/buildDefault";
            if (destination == null)
            {
                Environment.ExitCode = -1;
                Console.Error.WriteLine("Build canceled");
                Console.Error.WriteLine("output destination is null");
                return;
            }
            Console.WriteLine($"Build destination: {destination}");

            var buildReport = BuildPipeline.BuildPlayer(Command.GetScenePaths(), destination, EditorUserBuildSettings.activeBuildTarget, BuildOptions.None);
            BuildProcessor.ProcessBuildReport(buildReport);
        }

        public static class Variables
        {
            public const string OutputDestination   = "output";
            public const string BuildNumber         = "buildNumber";
            public const string BuildVersion        = "buildVersion";
            public const string Development         = "developmentBuild";
        }
    }
}