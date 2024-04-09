using System;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace UBuilder
{
    internal static class BuildProcessor
    {
        public static void ProcessBuildReport(BuildReport report)
        {
            Console.WriteLine($"Build result: {report.summary.result}");

            PrintSteps(report);

            if (report.summary.result == BuildResult.Succeeded)
            {
                Console.WriteLine($"Build size: {report.summary.totalSize} kb");
                Console.WriteLine("Build succeeded");
            }
            else if (report.summary.result == BuildResult.Failed)
            {
                Console.Error.WriteLine("Build failed");
                Environment.ExitCode = -1;
            }
        }

        private static void PrintSteps(BuildReport report)
        {
            if (report.steps == null)
                return;

            Console.WriteLine($"Steps: {report.steps.Length}");
            foreach (var step in report.steps)
            {
                var depth = new string(' ', step.depth);
                var duration = Mathf.RoundToInt((float)step.duration.TotalSeconds).ToString().PadLeft(4);
                Console.WriteLine($"{depth}[{duration}s] {step.name}");
                Console.WriteLine($"{depth}Messages: {step.messages}\n");
            }
        }
    }
}