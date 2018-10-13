using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using Hephaestus.Classes.Exceptions;
using HephaestusCommon.Classes;

namespace Hephaestus.Classes
{
    public static class Builder
    {
        private static readonly List<int> ExitCodes = new List<int>();
        private static int SourceCodeDirectoryCount { get; set; }
        private static int LaunchedAddonBuilders { get; set; }
        private static int ExitedAddonBuilders { get; set; }

        public static int Build(Project project)
        {
            int exitCode = 0;

            if (project.ShutdownGameBeforeBuilding)
            {
                Game.Shutdown(
                    new List<string>{"arma3", "arma3_x64"}
                );
            }

            string[] sourceCodeDirectories = GetSourceCodeDirectories(project.SourceDirectory);
            SourceCodeDirectoryCount = sourceCodeDirectories.Length;

            Console.WriteLine($"Found {SourceCodeDirectoryCount} source code directories");

            foreach (string sourceCodeDirectory in sourceCodeDirectories)
            {
                LaunchedAddonBuilders++;

                AddonBuilder addonBuilder = new AddonBuilder(sourceCodeDirectory, project);

                Console.WriteLine($"Building {Path.GetFileName(sourceCodeDirectory)} ({LaunchedAddonBuilders}/{SourceCodeDirectoryCount})");

                addonBuilder.Process.Exited += (sender, eventArgs) =>
                    OnAddonBuilderExit(sourceCodeDirectory, addonBuilder.Process.ExitCode, addonBuilder.Process.StartTime, addonBuilder.Process.ExitTime);
            }

            // Wait until all builds are complete
            while (LaunchedAddonBuilders != SourceCodeDirectoryCount || ExitedAddonBuilders != SourceCodeDirectoryCount)
            {
                Thread.Sleep(100);
            }

            if (project.StartGameAfterBuilding)
            {
                Console.WriteLine($"Starting {Path.GetFileName(project.GameExecutable)}");

                Game.Launch(project.GameExecutable, project.GameExecutableArguments);
            }
            
            // If doesn't only have successful exit codes
            if (ExitCodes.Except(new[] { 0 }).Any())
            {
                exitCode = 1;
            }

            return exitCode;
        }

        private static readonly object OnAddonBuilderExitLock = new object();
        private static void OnAddonBuilderExit(string sourceCodeDirectory, int exitCode, DateTime startTime, DateTime exitTime)
        {
            // Needs a lock due to a race condition if multiple AddonBuilders exit at the same time
            lock (OnAddonBuilderExitLock)
            {
                ExitedAddonBuilders++;
                
                ExitCodes.Add(exitCode);

                if (exitCode == 0)
                {
                    double timeToBuild = Math.Round((exitTime - startTime).TotalSeconds, 3);
                    
                    Console.WriteLine(
                        $"Completed building {Path.GetFileName(sourceCodeDirectory)} in {timeToBuild}" 
                            + $" ({ExitedAddonBuilders}/{LaunchedAddonBuilders})");
                }
                else
                {
                    throw new AddonBuilderFailedToBuildException(
                        $"Failed to build {Path.GetFileName(sourceCodeDirectory)}");
                }
            }
        }

        private static string[] GetSourceCodeDirectories(string sourceDirectory)
        {
            string[] sourceCodeDirectories;

            if (Directory.Exists(sourceDirectory))
            {
                sourceCodeDirectories = Directory.GetDirectories(sourceDirectory);
            }
            else
            {
                throw new SourceDirectoryNotFoundException($"{sourceDirectory} does not exist.");
            }

            return sourceCodeDirectories;
        }
    }
}
