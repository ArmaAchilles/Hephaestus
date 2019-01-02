using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Hephaestus.Classes.Builders;
using Hephaestus.Common.Classes;
using Hephaestus.Utilities;

namespace Hephaestus.Classes
{
    public static class Builder
    {
        private static readonly List<int> ExitCodes = new List<int>();
        private static int SourceCodeDirectoryCount { get; set; }
        private static int LaunchedAddonBuilders { get; set; }
        private static int ExitedAddonBuilders { get; set; }
        private static int NotBuiltDirectories { get; set; }

        public static int Build(Project project, bool forceBuild)
        {
            while (true)
            {
                // Reset counts (in case of a rebuild)
                SourceCodeDirectoryCount = 0;
                LaunchedAddonBuilders = 0;
                ExitedAddonBuilders = 0;
                NotBuiltDirectories = 0;

                if (forceBuild)
                {
                    Console.WriteLine("Rebuilding...");
                }

                if (project.ShutdownGameBeforeBuilding)
                {
                    Game.Shutdown(new List<string> {"arma3", "arma3_x64"});
                }

                string[] sourceCodeDirectories = GetSourceCodeDirectories(project.SourceDirectory);
                SourceCodeDirectoryCount = sourceCodeDirectories.Length;

                Console.WriteLine($"info: Found {SourceCodeDirectoryCount} source code directories");

                if (project.Hashes == null)
                {
                    project.Hashes = new Dictionary<string, Hash>();
                }

                foreach (string sourceCodeDirectory in sourceCodeDirectories)
                {
                    Hash hash = new Hash(sourceCodeDirectory);

                    if (forceBuild)
                    {
                        BuildDirectory(sourceCodeDirectory, project);
                    }
                    else
                    {
                        if (project.Hashes.ContainsKey(sourceCodeDirectory))
                        {
                            Hash selectedHash = project.Hashes[sourceCodeDirectory];

                            if (selectedHash.Sha1 == hash.Sha1)
                            {
                                NotBuiltDirectories++;
                                Console.WriteLine($"info: Not building {Path.GetFileName(sourceCodeDirectory)} because it hasn't changed");
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
                while (LaunchedAddonBuilders + NotBuiltDirectories != SourceCodeDirectoryCount ||
                       ExitedAddonBuilders + NotBuiltDirectories != SourceCodeDirectoryCount)
                {
                    Thread.Sleep(100);
                }

                if (project.StartGameAfterBuilding)
                {
                    Game.Launch(project.Game.GameExecutable, project.Game.GameExecutableArguments);
                }

                // If doesn't only have successful exit codes
                lock (OnBuilderExitLock)
                {
                    // Check if any non-zero exit codes were present
                    int exitCode = ExitCodes.All(v => v == 0) ? 0 : 1;
                    
                    if (NotBuiltDirectories == SourceCodeDirectoryCount)
                    {
                        if (! ConsoleUtility.AskYesNoQuestion("Rebuild?"))
                        {
                            return exitCode;
                        }

                        forceBuild = true;

                        continue;
                    }

                    project.Save();

                    return exitCode;
                }
            }
        }

        private static void BuildDirectory(string sourceCodeDirectory, Project project)
        {
            LaunchedAddonBuilders++;

            Console.WriteLine(
                $"info: Building {Path.GetFileName(sourceCodeDirectory)} ({LaunchedAddonBuilders}/{SourceCodeDirectoryCount - NotBuiltDirectories})");
            if (project.UseArmake)
            {
                Armake armake = new Armake(sourceCodeDirectory, project);

                armake.Process.Exited += (sender, eventArgs) =>
                    OnBuilderExit(sourceCodeDirectory, armake.Process.ExitCode,
                        armake.Process.StartTime, armake.Process.ExitTime, project);
            }
            else
            {
                AddonBuilder addonBuilder = new AddonBuilder(sourceCodeDirectory, project);

                addonBuilder.Process.Exited += (sender, eventArgs) =>
                    OnBuilderExit(sourceCodeDirectory, addonBuilder.Process.ExitCode,
                        addonBuilder.Process.StartTime, addonBuilder.Process.ExitTime, project);
            }
        }

        private static readonly object OnBuilderExitLock = new object();
        private static void OnBuilderExit(string sourceCodeDirectory, int exitCode, DateTime startTime, DateTime exitTime, Project project)
        {
            // Needs a lock due to a race condition if multiple AddonBuilders exit at the same time
            lock (OnBuilderExitLock)
            {
                ExitedAddonBuilders++;
                
                ExitCodes.Add(exitCode);

                if (exitCode == 0)
                {
                    double timeToBuild = Math.Round((exitTime - startTime).TotalSeconds, 3);
                    
                    Console.WriteLine(
                        $"info: Completed building {Path.GetFileName(sourceCodeDirectory)} in {timeToBuild}s" 
                            + $" ({ExitedAddonBuilders}/{LaunchedAddonBuilders})");
                    
                    project.Hashes[sourceCodeDirectory].Sha1 = new Hash(sourceCodeDirectory).Sha1;
                }
                else
                {
                    Console.Error.WriteLine(
                        $"error: Failed to build {Path.GetFileName(sourceCodeDirectory)}. Exit code {exitCode}");
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
