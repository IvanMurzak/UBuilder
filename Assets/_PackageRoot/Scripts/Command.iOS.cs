using UnityEditor;
using System;

namespace UBuilder
{
    public static partial class CommandiOS
    {
        static class VariablesiOS
        {
            public const string SigningTeamID           = "ios_SigningTeamId";
            public const string EnableAutomaticSigning  = "ios_EnableAutomaticSigning";
        }
        [MenuItem("File/UBuilder/Build iOS")]
        public static void Build()
        {
            Console.WriteLine($"iOS build started");
            if (!EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.iOS, BuildTarget.iOS))
            {
                Console.Error.WriteLine("Target build target hadn't been switched");
                Environment.ExitCode = -1;
                return; // note, we can not use Environment.Exit(-1) - the buildprocess will just hang afterwards
            }

            if (Utils.GetVariableBool(Command.Variables.Development) != null)       EditorUserBuildSettings.development = Utils.GetVariableBool(Command.Variables.Development).Value;
            if (Utils.GetVariable(Command.Variables.BuildVersion) != null)          PlayerSettings.bundleVersion = Utils.GetVariable(Command.Variables.BuildVersion);
            if (Utils.GetVariable(VariablesiOS.SigningTeamID) != null)              PlayerSettings.iOS.appleDeveloperTeamID = Utils.GetVariable(VariablesiOS.SigningTeamID);
            if (Utils.BuildNumber() != null)                                        PlayerSettings.iOS.buildNumber = Utils.BuildNumber();
            if (Utils.GetVariableBool(VariablesiOS.EnableAutomaticSigning) != null) PlayerSettings.iOS.appleEnableAutomaticSigning = Utils.GetVariableBool(VariablesiOS.EnableAutomaticSigning).Value;

            var destination = Utils.GetVariable(Command.Variables.OutputDestination) ?? "Builds/iOS/buildDefault";
            Console.WriteLine($"Build destination: {destination}");
            BuildPipeline.BuildPlayer(Command.GetScenePaths(), destination, BuildTarget.iOS, BuildOptions.None);
        }
    }
}