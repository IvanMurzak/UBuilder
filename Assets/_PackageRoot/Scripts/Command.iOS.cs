using UnityEditor;
using System;

namespace UBuilder
{
    public static partial class CommandiOS
    {
        [MenuItem("File/UBuilder/Build iOS")]
        public static void Build()
        {
            Console.WriteLine($"iOS build started");
            var destination = Setup();
            if (destination == null)
            {
                Environment.ExitCode = -1;
                Console.Error.WriteLine("Build canceled");
                Console.Error.WriteLine("output destination is null");
                return;
            }
            Console.WriteLine($"Build destination: {destination}");

            var buildReport = BuildPipeline.BuildPlayer(Command.GetScenePaths(), destination, BuildTarget.iOS, BuildOptions.None);
            BuildProcessor.ProcessBuildReport(buildReport);
        }
        public static string Setup()
        {
            if (!EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.iOS, BuildTarget.iOS))
            {
                Console.Error.WriteLine("Target build target hadn't been switched");
                Environment.ExitCode = -1;
                return null; // note, we can not use Environment.Exit(-1) - the build process will just hang afterwards
            }

            if (Utils.GetVariableBool(Command.Variables.Development) != null)       EditorUserBuildSettings.development = Utils.GetVariableBool(Command.Variables.Development).Value;
            if (Utils.GetVariable(Command.Variables.BuildVersion) != null)          PlayerSettings.bundleVersion = Utils.GetVariable(Command.Variables.BuildVersion);
            if (Utils.GetVariable(Variables.SigningTeamID) != null)                 PlayerSettings.iOS.appleDeveloperTeamID = Utils.GetVariable(Variables.SigningTeamID);
            if (Utils.BuildNumber() != null)                                        PlayerSettings.iOS.buildNumber = Utils.BuildNumber();
            if (Utils.GetVariableBool(Variables.EnableAutomaticSigning) != null)    PlayerSettings.iOS.appleEnableAutomaticSigning = Utils.GetVariableBool(Variables.EnableAutomaticSigning).Value;

            var destination = Utils.GetVariable(Command.Variables.OutputDestination) ?? "Builds/iOS/buildDefault";
            return destination;
        }
        public static class Variables
        {
            public const string SigningTeamID           = "ios_SigningTeamId";
            public const string EnableAutomaticSigning  = "ios_EnableAutomaticSigning";
        }
    }
}