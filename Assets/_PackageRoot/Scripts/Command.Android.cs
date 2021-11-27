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
            if (Utils.EnvironmentVariablesMissing(variables))
            {
                Console.WriteLine("Build canceled, missed variable");
                Environment.ExitCode = -1;
                return; // note, we can not use Environment.Exit(-1) - the buildprocess will just hang afterwards
            }
            Console.WriteLine($"Variables are valid");

            if (!EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android))
                Console.WriteLine("Target build target hadn't been switched");

            if (Utils.GetVariableBool(Command.Variables.Development) != null)   EditorUserBuildSettings.development = Utils.GetVariableBool(Command.Variables.Development).Value;
            if (Utils.GetVariable(Command.Variables.BuildVersion) != null)      PlayerSettings.bundleVersion = Utils.GetVariable(Command.Variables.BuildVersion);
            if (Utils.BuildNumberInt() >= 0)                                    PlayerSettings.Android.bundleVersionCode = Utils.BuildNumberInt();

            EditorUserBuildSettings.exportAsGoogleAndroidProject = false;
            EditorUserBuildSettings.buildAppBundle = Utils.GetVariableBool(VariablesAndroid.BuildAppBundle) ?? false;

            PlayerSettings.Android.keystoreName = Utils.GetVariable(VariablesAndroid.KeystorePath);
            PlayerSettings.Android.keystorePass = Utils.GetVariable(VariablesAndroid.KeystorePassword);
            PlayerSettings.Android.keyaliasName = Utils.GetVariable(VariablesAndroid.KeyAliasName);
            PlayerSettings.Android.keyaliasPass = Utils.GetVariable(VariablesAndroid.KeyAliasPassword);

            Console.WriteLine($"development: {EditorUserBuildSettings.development}");
            Console.WriteLine($"buildAppBundle: {EditorUserBuildSettings.buildAppBundle}");
            Console.WriteLine($"bundleVersionCode: {PlayerSettings.Android.bundleVersionCode}");
            Console.WriteLine($"keystoreName: {PlayerSettings.Android.keystoreName}");
            Console.WriteLine($"keystorePass: {PlayerSettings.Android.keystorePass}");
            Console.WriteLine($"keyaliasName: {PlayerSettings.Android.keyaliasName}");
            Console.WriteLine($"keyaliasPass: {PlayerSettings.Android.keyaliasPass}");

            var destination = Utils.GetVariable(Command.Variables.OutputDestination) ?? $"Builds/Android/buildDefault";
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

            var destination = Utils.GetVariable(Command.Variables.OutputDestination) ?? $"Builds/Android/projectDefault";
            if (destination.ToLower().EndsWith(".apk") || destination.ToLower().EndsWith(".aab"))
                destination = destination.Substring(0, destination.Length - 4);

            Console.WriteLine($"Build destination: {destination}");
            BuildPipeline.BuildPlayer(Command.GetScenePaths(), destination, BuildTarget.Android, BuildOptions.None);
        }
    }
}