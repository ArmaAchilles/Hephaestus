using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Hephaestus.Classes;
using Hephaestus.Classes.Exceptions.BuilderExceptions;
using HephaestusCommon.Classes;

namespace Hephaestus.Utilities
{
    public class BuilderUtility
    {
        static List<int> exitCodes = new List<int>();
        static int SourceCodeDirectoryCount { get; set; }
        static int LaunchedAddonBuilders { get; set; }
        static int ExitedAddonBuilders { get; set; }

        public static int Build(Project project)
        {
            int exitCode = 0;

            string[] sourceCodeDirectories = GetSourceCodeDirectories(project.SourceDirectory);
            SourceCodeDirectoryCount = sourceCodeDirectories.Length;

            foreach (string sourceCodeDirectory in sourceCodeDirectories)
            {
                LaunchedAddonBuilders++;

                AddonBuilder addonBuilder = new AddonBuilder(sourceCodeDirectory, project);

                Console.WriteLine(String.Format("Building {0}", Path.GetFileName(sourceCodeDirectory)));

                addonBuilder.Process.Exited += (sender, eventArgs) => OnAddonBuilderExit(sourceCodeDirectory, addonBuilder.Process.ExitCode);
            }

            // Wait until all builds are complete
            while (LaunchedAddonBuilders != SourceCodeDirectoryCount || ExitedAddonBuilders != SourceCodeDirectoryCount)
            {
                Thread.Sleep(100);
            }

            if (LaunchedAddonBuilders == SourceCodeDirectoryCount && ExitedAddonBuilders == SourceCodeDirectoryCount)
            {
                // If doesn't only have successful exit codes
                if (exitCodes.Except(new[] { 0 }).Any())
                {
                    exitCode = 1;
                }
            }

            return exitCode;
        }

        private static void OnAddonBuilderExit(string sourceCodeDirectory, int exitCode)
        {
            exitCodes.Add(exitCode);

            Console.WriteLine(String.Format("Completed building {0}", Path.GetFileName(sourceCodeDirectory)));

            ExitedAddonBuilders++;
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
                throw new SourceDirectoryNotFoundException();
            }

            return sourceCodeDirectories;
        }
    }
}
