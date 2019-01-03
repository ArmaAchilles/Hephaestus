using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Hephaestus.Common.Classes;
using Hephaestus.Common.Utilities;

namespace Hephaestus.Classes
{
    public static class Compiler
    {
        // List of exit codes from all builders that will be compared if all builders have successful exits.
        private static readonly List<int> ExitCodes = new List<int>();
        private static int SourceCodeDirectoryCount { get; set; }
        // Number of builders that were only started.
        private static int LaunchedAddonBuilders { get; set; }
        // Number of builders that have exited (all exit codes) and are no longer building.
        private static int ExitedAddonBuilders { get; set; }
        // Number of directories that were not built because their contents didn't change.
        private static int NotBuiltDirectories { get; set; }

        /// <summary>
        /// Prepares the building of the source directories.
        /// 
        /// Method is looped in case user wants to rebuild if there were no changes to the source code directories.
        /// 
        /// Method handles the closing and opening of the specified game, comparing stored hashes with current hashes,
        /// launching builders for each source code directory and exit code comparision from all the builders.
        /// </summary>
        /// <param name="project">Project data.</param>
        /// <param name="forceBuild">Should the method ignore all checksums and build without taking them into consideration?</param>
        /// <returns>
        /// Exit code. If all builders returned 0, then 0 is returned.
        /// If at least one has a code different from 0, then method returns exit code 1.
        /// </returns>
        public static int Build(Project project, bool forceBuild)
        {
            // Loop is used in case the user wants to force build.
            while (true)
            {
                // Reset counts (in case of a rebuild)
                SourceCodeDirectoryCount = 0;
                LaunchedAddonBuilders = 0;
                ExitedAddonBuilders = 0;
                NotBuiltDirectories = 0;

                if (forceBuild)
                {
                    ConsoleUtility.Info("Rebuilding...");
                }

                if (project.ShutdownGameBeforeBuilding)
                {
                    // Get the file name (with extension) of the entered game executable path (e.g. arma3_x64.exe)
                    //     which will be closed if open.
                    Game.Shutdown(Path.GetFileName(project.Game.GameExecutable));
                }

                // Get all source directories and store them in an array (e.g. data_f_achilles, functions_f_achilles, etc.)
                string[] sourceCodeDirectories = GetSourceCodeDirectories(project.SourceDirectory);
                SourceCodeDirectoryCount = sourceCodeDirectories.Length;

                ConsoleUtility.Info($"Found {SourceCodeDirectoryCount} source code directories");

                // If no hashes have been stored in the JSON then create a new dictionary so we don't get a NullReferenceException when adding data.
                if (project.Hashes == null)
                {
                    // <string, Hash> --> <absolute path of the source code directory, SHA1>
                    project.Hashes = new Dictionary<string, Hash>();
                }

                // Loop though each source code directory.
                foreach (string sourceCodeDirectory in sourceCodeDirectories)
                {
                    // Generate a SHA1 checksum for the source code directory.
                    Hash hash = new Hash(sourceCodeDirectory);

                    // If force build was selected then we bypass checksums.
                    if (forceBuild)
                    {
                        BuildDirectory(sourceCodeDirectory, project);
                    }
                    else
                    {
                        // If the JSON already has the checksum then we are going to compare them
                        //     if they are equal to the current status of the source code directories.
                        if (project.Hashes.ContainsKey(sourceCodeDirectory))
                        {
                            Hash selectedHash = project.Hashes[sourceCodeDirectory];

                            // If the newly generated hash is equal to the stored hash, then increment NotBuiltDirectories property and we don't build it.
                            if (selectedHash.Sha1 == hash.Sha1)
                            {
                                NotBuiltDirectories++;
                                ConsoleUtility.Info($"Not building {Path.GetFileName(sourceCodeDirectory)} because it hasn't changed");
                            }
                            else
                            {
                                // If the checksums are now different then we build the directory.
                                BuildDirectory(sourceCodeDirectory, project);
                            }
                        }
                        else
                        {
                            // If the JSON doesn't contain the current source code directories hash then we add it to the list
                            //    but leave it as an empty hash for now.
                            project.Hashes.Add(sourceCodeDirectory, new Hash());
                            BuildDirectory(sourceCodeDirectory, project);
                        }
                    }
                }

                // Wait until all builders have closed.
                while (LaunchedAddonBuilders + NotBuiltDirectories != SourceCodeDirectoryCount ||
                       ExitedAddonBuilders + NotBuiltDirectories != SourceCodeDirectoryCount)
                {
                    Thread.Sleep(100);
                }

                // As it has finished building then we can safely launch the game (if configured).
                if (project.StartGameAfterBuilding)
                {
                    // If none of the source code directories have changed, then ask the user if they want to start the game.
                    // This prevents accidental launching of the game.
                    if (NotBuiltDirectories == SourceCodeDirectoryCount)
                    {
                        if (ConsoleUtility.AskYesNoQuestion("Do you want to launch the game?"))
                        {
                            Game.Launch(project.Game);
                        }
                    }
                    else
                    {
                        // If at least one source code directory did change then we launch anyways.
                        Game.Launch(project.Game);
                    }
                }

                // Wait until all builders have exited.
                lock (OnBuilderExitLock)
                {
                    // Check if any non-zero exit codes are present in the ExitCodes list.
                    // If it contains a non-zero exit code, then return an exit code of 1.
                    int exitCode = ExitCodes.All(v => v == 0) ? 0 : 1;
                    
                    // If nothing has changed.
                    if (NotBuiltDirectories == SourceCodeDirectoryCount)
                    {
                        // If the user does not want to rebuild then return the exit code (0, because nothing was built.)
                        if (! ConsoleUtility.AskYesNoQuestion("Rebuild?"))
                        {
                            return 0;
                        }

                        // If the user wants to rebuild then we set forceBuild to true and mak
                        //     the while(true) loop loop again to build but ignoring all checksums.
                        forceBuild = true;

                        continue;
                    }

                    // Save the current project data (this is used to save SHA1 checksums).
                    project.Save();

                    // Return the combined exit code back to the Program class.
                    return exitCode;
                }
            }
        }

        /// <summary>
        /// Build a single source code directory via the Builder class but this also handles the exiting of builders.
        /// </summary>
        /// <param name="sourceCodeDirectory">The directory to be built.</param>
        /// <param name="project">Project data.</param>
        private static void BuildDirectory(string sourceCodeDirectory, Project project)
        {
            LaunchedAddonBuilders++;

            // e.g. Building data_f_achilles (1/9)
            ConsoleUtility.Info(
                $"Building {Path.GetFileName(sourceCodeDirectory)} ({LaunchedAddonBuilders}/{SourceCodeDirectoryCount - NotBuiltDirectories})");

            // Launch the builder with the specified driver.
            Builder builder = new Builder(sourceCodeDirectory, project);
            
            // Hook into the builder's process's exit event and call OnBuilderExit once the builder exits.
            builder.Process.Exited += (sender, eventArgs) =>
                OnBuilderExit(sourceCodeDirectory, builder.Process.ExitCode,
                    builder.Process.StartTime, builder.Process.ExitTime, project);
        }

        // The object used for our locks to prevent a race condition (as the builders are launched multi-threaded).
        private static readonly object OnBuilderExitLock = new object();
        /// <summary>
        /// Handles individual exits of a builder. This is used to output data to the user and for checksum updating.
        /// </summary>
        /// <param name="sourceCodeDirectory">The source code directory that was built.</param>
        /// <param name="exitCode">Exit code of the builder. BI's AddonBuilder always returns 0.</param>
        /// <param name="startTime">Time when the builder was launched.</param>
        /// <param name="exitTime">Time when the builder had exited.</param>
        /// <param name="project">Project data.</param>
        private static void OnBuilderExit(string sourceCodeDirectory, int exitCode, DateTime startTime, DateTime exitTime, Project project)
        {
            // Add a lock in case multiple builders exit at the same time (race condition), so we don't override each other.
            lock (OnBuilderExitLock)
            {
                ExitedAddonBuilders++;
                
                // Add the builder's exit code to the list for later comparision.
                ExitCodes.Add(exitCode);

                // If it was a successful build (BI's AddonBuilder always returns an exit code of 0).
                if (exitCode == 0)
                {
                    // As the exit time and the start times are doubles with a lot of numbers behind the decimal,
                    //     we round them to 3 numbers after the decimal point to help readability.
                    double timeToBuild = Math.Round((exitTime - startTime).TotalSeconds, 3);
                    
                    // e.g. Completed building data_f_achilles in 2.343s (1/9)
                    ConsoleUtility.Info(
                        $"Completed building {Path.GetFileName(sourceCodeDirectory)} in {timeToBuild}s" 
                            + $" ({ExitedAddonBuilders}/{LaunchedAddonBuilders})");
                    
                    // Update the hashes for this source code directory.
                    project.Hashes[sourceCodeDirectory].Sha1 = new Hash(sourceCodeDirectory).Sha1;
                }
                else
                {
                    // If a non-zero exit code was returned then we log an error.
                    ConsoleUtility.Error(
                        $"Failed to build {Path.GetFileName(sourceCodeDirectory)}. Exit code {exitCode}");
                }
            }
        }

        /// <summary>
        /// Get all the source code directories present in the project's specified source directory.
        /// </summary>
        /// <param name="sourceDirectory">Path to all the source code directories.</param>
        /// <returns>Returns an array full of directory paths for building.</returns>
        /// <exception cref="DirectoryNotFoundException">If no directories are present in the given source directory.</exception>
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
