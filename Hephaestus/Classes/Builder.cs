using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using Hephaestus.Classes.Exceptions;
using Hephaestus.Utilities;
using HephaestusCommon.Classes;

namespace Hephaestus.Classes
{
    public static class Builder
    {
        private static readonly List<int> ExitCodes = new List<int>();
        private static int SourceCodeDirectoryCount { get; set; }
        private static int LaunchedAddonBuilders { get; set; }
        private static int ExitedAddonBuilders { get; set; }
        private static int NotBuiltDirectories { get; set; }

        public static int Build(Project project, bool? forceBuild)
        {
            while (true)
            {
                int exitCode = 0;
                
                // Reset counts (in case of a rebuild)
                SourceCodeDirectoryCount = 0;
                LaunchedAddonBuilders = 0;
                ExitedAddonBuilders = 0;
                NotBuiltDirectories = 0;

                if (project.ShutdownGameBeforeBuilding)
                {
                    Game.Shutdown(new List<string> {"arma3", "arma3_x64"});
                }

                string[] sourceCodeDirectories = GetSourceCodeDirectories(project.SourceDirectory);
                SourceCodeDirectoryCount = sourceCodeDirectories.Length;

                Console.WriteLine($"Found {SourceCodeDirectoryCount} source code directories");

                if (project.Hashes == null)
                {
                    project.Hashes = new Dictionary<string, Hash>();
                }

                foreach (string sourceCodeDirectory in sourceCodeDirectories)
                {
                    Hash hash = new Hash(sourceCodeDirectory);

                    if (forceBuild == true)
                    {
                        BuildDirectory(sourceCodeDirectory, project);
                    }
                    else
                    {
                        if (project.Hashes.ContainsKey(sourceCodeDirectory))
                        {
                            Hash selectedHash = project.Hashes[sourceCodeDirectory];

                            if (selectedHash.SHA1 == hash.SHA1)
                            {
                                NotBuiltDirectories++;
                                Console.WriteLine($"Not building {Path.GetFileName(sourceCodeDirectory)} because it hasn't changed");
                            }
                            else
                            {
                                BuildDirectory(sourceCodeDirectory, project);
                            }
                        }
                        else
                        {
                            project.Hashes.Add(sourceCodeDirectory, new Hash());
                            BuildDirectory(sourceCodeDirectory, project);
                        }
                    }
                }

                // Wait until all builds are complete
                while (LaunchedAddonBuilders + NotBuiltDirectories != SourceCodeDirectoryCount || ExitedAddonBuilders + NotBuiltDirectories != SourceCodeDirectoryCount)
                {
                    Thread.Sleep(100);
                }

                if (project.StartGameAfterBuilding)
                {
                    Console.WriteLine($"Starting {Path.GetFileName(project.GameExecutable)}");

                    Game.Launch(project.GameExecutable, project.GameExecutableArguments);
                }

                // If doesn't only have successful exit codes
                lock (OnAddonBuilderExitLock)
                {
                    if (ExitCodes.Except(new[] {0}).Any())
                    {
                        exitCode = 1;
                    }
                }

                if (NotBuiltDirectories == SourceCodeDirectoryCount)
                {
                    if (!ConsoleUtility.AskYesNoQuestion("Rebuild?")) return exitCode;
                    
                    forceBuild = true;
                    continue;
                }

                project.Save();

                return exitCode;
            }
        }

        private static void BuildDirectory(string sourceCodeDirectory, Project project)
        {
            LaunchedAddonBuilders++;

            AddonBuilder addonBuilder = new AddonBuilder(sourceCodeDirectory, project);

            Console.WriteLine($"Building {Path.GetFileName(sourceCodeDirectory)} ({LaunchedAddonBuilders}/{SourceCodeDirectoryCount - NotBuiltDirectories})");

            addonBuilder.Process.Exited += (sender, eventArgs) =>
                OnAddonBuilderExit(sourceCodeDirectory, addonBuilder.Process.ExitCode, addonBuilder.Process.StartTime, addonBuilder.Process.ExitTime, project);
        }

        private static readonly object OnAddonBuilderExitLock = new object();
        private static void OnAddonBuilderExit(string sourceCodeDirectory, int exitCode, DateTime startTime, DateTime exitTime, Project project)
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
                        $"Completed building {Path.GetFileName(sourceCodeDirectory)} in {timeToBuild}s" 
                            + $" ({ExitedAddonBuilders}/{LaunchedAddonBuilders})");
                    
                    project.Hashes[sourceCodeDirectory].SHA1 = new Hash(sourceCodeDirectory).SHA1;
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
                throw new DirectoryNotFoundException($"{sourceDirectory} does not exist.");
            }

            return sourceCodeDirectories;
        }
    }
}
