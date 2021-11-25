using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System;

namespace UBuilder
{
    public static partial class Command
    {
        public static string[] GetScenePaths() => EditorBuildSettings.scenes.Select(x => x.path).ToArray();

        public static bool? GetVariableBool(string name)
        {
            var value = GetVariable(name);
            if (value == null) return null;

            bool result;
            if (bool.TryParse(value, out result))
                return result;

            return null;
        }
        public static string GetVariable(string name)
        {
            var commandLineVariable = Environment.GetCommandLineArgs().FirstOrDefault(x => x.StartsWith($"{name}="));
            if (commandLineVariable != null)
                return commandLineVariable.Substring(name.Length + 1, commandLineVariable.Length - (name.Length + 1)).Replace("'", "").Replace("\"", "");

            try
            {
                return Environment.GetEnvironmentVariable(name);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
        }
        public static bool EnvironmentVariablesMissing(string[] requiredVariables)
        {
            var missing = false;
            foreach (var requiredVariable in requiredVariables)
            {
                var value = GetVariable(requiredVariable);
                if (value == null)
                {
                    Console.Write("BUILD ERROR: Required Environment Variable is not set: ");
                    Console.WriteLine(requiredVariable);
                    missing = true;
                }
            }

            return missing;
        }

        public static int BuildNumberInt()
        {
            var buildNumber = BuildNumber();
            if (buildNumber == null) return -1;
            return int.Parse(buildNumber);
        }
        public static string BuildNumber()
        {
            var numberVariable = GetVariable(Variables.BuildNumber);
            if (numberVariable == "timestamp") 
                return "" + (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

            int numberInt = 0;
            if (int.TryParse(numberVariable, out numberInt)) return numberVariable;
            return null;
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