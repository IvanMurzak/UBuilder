using System;
using UnityEditor;

namespace UBuilder
{
    public static partial class CommandAndroid
    {
        static class VariablesAndroid
        {
            public const string BuildAppBundle      = "android_BUILD_APP_BUNDLE";
            public const string KeystorePath        = "android_KEYSTORE_PATH";
            public const string KeystorePassword    = "android_KEYSTORE_PASSWORD";
            public const string KeyAliasName        = "android_KEYALIAS_NAME";
            public const string KeyAliasPassword    = "android_KEYALIAS_PASSWORD";
        }

        [MenuItem("File/UBuilder/Build Android")]
        public static void Build()
        {
            Console.WriteLine($"Android build started");
            var variables = new string[]
            {
                VariablesAndroid.KeystorePath,
                VariablesAndroid.KeystorePassword,
                VariablesAndroid.KeyAliasName,
                VariablesAndroid.KeyAliasPassword
            };
            Console.WriteLine($"Validating variables");
            if (Command.EnvironmentVariablesMissing(variables))
            {
                Console.WriteLine("Build canceled, missed variable");
                Environment.ExitCode = -1;
                return; // note, we can not use Environment.Exit(-1) - the buildprocess will just hang afterwards
            }
            Console.WriteLine($"Variables are valid");

            if (!EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android))
                Console.WriteLine("Target build target hadn't been switched");

            if (Command.GetVariableBool(Command.Variables.Development) != null) EditorUserBuildSettings.development = Command.GetVariableBool(Command.Variables.Development).Value;
            if (Command.BuildNumberInt() >= 0)                                  PlayerSettings.Android.bundleVersionCode = Command.BuildNumberInt();

            EditorUserBuildSettings.exportAsGoogleAndroidProject = false;
            EditorUserBuildSettings.buildAppBundle = Command.GetVariableBool(VariablesAndroid.BuildAppBundle) ?? false;

            PlayerSettings.Android.keystoreName = Command.GetVariable(VariablesAndroid.KeystorePath);
            PlayerSettings.Android.keystorePass = Command.GetVariable(VariablesAndroid.KeystorePassword);
            PlayerSettings.Android.keyaliasName = Command.GetVariable(VariablesAndroid.KeyAliasName);
            PlayerSettings.Android.keyaliasPass = Command.GetVariable(VariablesAndroid.KeyAliasPassword);

            Console.WriteLine($"development: {EditorUserBuildSettings.development}");
            Console.WriteLine($"buildAppBundle: {EditorUserBuildSettings.buildAppBundle}");
            Console.WriteLine($"bundleVersionCode: {PlayerSettings.Android.bundleVersionCode}");
            Console.WriteLine($"keystoreName: {PlayerSettings.Android.keystoreName}");
            Console.WriteLine($"keystorePass: {PlayerSettings.Android.keystorePass}");
            Console.WriteLine($"keyaliasName: {PlayerSettings.Android.keyaliasName}");
            Console.WriteLine($"keyaliasPass: {PlayerSettings.Android.keyaliasPass}");

            var destination = Command.GetVariable(Command.Variables.OutputDestination) ?? $"Builds/Android/buildDefault";
            var validDest = destination.ToLower().EndsWith(".apk") || destination.ToLower().EndsWith(".aar");
            if (!validDest) destination = $"{destination}.{(EditorUserBuildSettings.buildAppBundle ? "aab" : "apk")}";

            Console.WriteLine($"Build destination: {destination}");
            BuildPipeline.BuildPlayer(Command.GetScenePaths(), destination, BuildTarget.Android, BuildOptions.None);
        }

        [MenuItem("File/UBuilder/Build Android (Export)")]
        public static void Export()
        {
            Console.WriteLine($"Android export started");

            EditorUserBuildSettings.exportAsGoogleAndroidProject = true;

            var destination = Command.GetVariable(Command.Variables.OutputDestination) ?? $"Builds/Android/projectDefault";
            if (destination.ToLower().EndsWith(".apk") || destination.ToLower().EndsWith(".aab"))
                destination = destination.Substring(0, destination.Length - 4);

            Console.WriteLine($"Build destination: {destination}");
            BuildPipeline.BuildPlayer(Command.GetScenePaths(), destination, BuildTarget.Android, BuildOptions.None);
        }
    }
}