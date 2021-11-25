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

            if (Command.GetVariableBool(Command.Variables.Development) != null)         EditorUserBuildSettings.development = Command.GetVariableBool(Command.Variables.Development).Value;
            if (Command.GetVariable(VariablesiOS.SigningTeamID) != null)                PlayerSettings.iOS.appleDeveloperTeamID = Command.GetVariable(VariablesiOS.SigningTeamID);
            if (Command.BuildNumber() != null)                                          PlayerSettings.iOS.buildNumber = Command.BuildNumber();
            if (Command.GetVariableBool(VariablesiOS.EnableAutomaticSigning) != null)   PlayerSettings.iOS.appleEnableAutomaticSigning = Command.GetVariableBool(VariablesiOS.EnableAutomaticSigning).Value;

            var destination = Command.GetVariable(Command.Variables.OutputDestination) ?? "Builds/iOS/buildDefault";
            Console.WriteLine($"Build destination: {destination}");
            BuildPipeline.BuildPlayer(Command.GetScenePaths(), destination, BuildTarget.iOS, BuildOptions.None);
        }
    }
}