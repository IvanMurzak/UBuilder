using System;
using UnityEditor;

namespace UBuilder
{
    public static partial class CommandAndroid
    {
        [MenuItem("File/UBuilder/Build Android")]
        public static void Build()
        {
            Console.WriteLine($"Android build started");
            var destination = Setup();
            if (destination == null)
            {
                Environment.ExitCode = -1;
                Console.Error.WriteLine("Build canceled");
                Console.Error.WriteLine("output destination is null");
                return;
            }
            Console.WriteLine($"Build destination: {destination}");
            var buildReport = BuildPipeline.BuildPlayer(Command.GetScenePaths(), destination, BuildTarget.Android, BuildOptions.None);
            BuildProcessor.ProcessBuildReport(buildReport);
        }
        public static string Setup()
        {
            var variables = new string[]
            {
                Variables.KeystorePath,
                Variables.KeystorePassword,
                Variables.KeyAliasName,
                Variables.KeyAliasPassword
            };
            Console.WriteLine($"Validating variables");
            if (Utils.EnvironmentVariablesMissing(variables))
            {
                UnityEngine.Debug.LogError("Build canceled, missed variable");
                Console.WriteLine("Build canceled, missed variable");
                Environment.ExitCode = -1;
                return null; // note, we can not use Environment.Exit(-1) - the build process will just hang afterwards
            }
            Console.WriteLine($"Variables are valid");

            if (!EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android))
            {
                UnityEngine.Debug.LogError("Target build target hadn't been switched");
                Console.WriteLine("Target build target hadn't been switched");
            }

            if (Utils.GetVariableBool(Command.Variables.Development) != null)   EditorUserBuildSettings.development = Utils.GetVariableBool(Command.Variables.Development).Value;
            if (Utils.GetVariable(Command.Variables.BuildVersion) != null)      PlayerSettings.bundleVersion = Utils.GetVariable(Command.Variables.BuildVersion);
            if (Utils.BuildNumberInt() >= 0)                                    PlayerSettings.Android.bundleVersionCode = Utils.BuildNumberInt();

            EditorUserBuildSettings.exportAsGoogleAndroidProject = false;
            EditorUserBuildSettings.buildAppBundle = Utils.GetVariableBool(Variables.BuildAppBundle) ?? false;

            PlayerSettings.Android.keystoreName = Utils.GetVariable(Variables.KeystorePath);
            PlayerSettings.Android.keystorePass = Utils.GetVariable(Variables.KeystorePassword);
            PlayerSettings.Android.keyaliasName = Utils.GetVariable(Variables.KeyAliasName);
            PlayerSettings.Android.keyaliasPass = Utils.GetVariable(Variables.KeyAliasPassword);

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
            return destination;
        }

        [MenuItem("File/UBuilder/Build Android (Export)")]
        public static void Export()
        {
            Console.WriteLine($"Android export started");

            EditorUserBuildSettings.exportAsGoogleAndroidProject = true;

            var destination = Utils.GetVariable(Command.Variables.OutputDestination) ?? $"Builds/Android/projectDefault";
            if (destination == null)
            {
                Environment.ExitCode = -1;
                Console.Error.WriteLine("Build canceled");
                Console.Error.WriteLine("output destination is null");
                return;
            }
            if (destination.ToLower().EndsWith(".apk") || destination.ToLower().EndsWith(".aab"))
                destination = destination.Substring(0, destination.Length - 4);

            Console.WriteLine($"Build destination: {destination}");

            var buildReport = BuildPipeline.BuildPlayer(Command.GetScenePaths(), destination, BuildTarget.Android, BuildOptions.None);
            BuildProcessor.ProcessBuildReport(buildReport);
        }
        public static class Variables
        {
            public const string BuildAppBundle      = "android_BUILD_APP_BUNDLE";
            public const string KeystorePath        = "android_KEYSTORE_PATH";
            public const string KeystorePassword    = "android_KEYSTORE_PASSWORD";
            public const string KeyAliasName        = "android_KEYALIAS_NAME";
            public const string KeyAliasPassword    = "android_KEYALIAS_PASSWORD";
        }
    }
}