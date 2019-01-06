using System;
using System.Diagnostics;
using System.IO;
using Hephaestus.Common.Classes;
using Hephaestus.Common.Utilities;

namespace Hephaestus.Classes
{
    public class Builder
    {
        public Process Process { get; private set; }
        private Project Project { get; }
        private Driver Driver { get; }

        /// <summary>
        /// Upon creation it automatically starts building a single folder with the currently selected driver.
        /// </summary>
        /// <param name="sourceCodeDirectory">The directory that will be built.</param>
        /// <param name="project">The current selected project's data.</param>
        public Builder(string sourceCodeDirectory, Project project)
        {
            Project = project;
            Driver = DriverUtility.GetSelectedDriver(project);
            
            Build(sourceCodeDirectory);
        }
        
        /// <summary>
        /// Build a single source code directory.
        /// </summary>
        /// <param name="sourceCodeDirectory">The directory that will be built.</param>
        private void Build(string sourceCodeDirectory)
        {
            try
            {
                Process process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        RedirectStandardError = true,
                        RedirectStandardOutput = false,

                        UseShellExecute = false,

                        CreateNoWindow = true,

                        FileName = Driver.Path,
                        
                        WorkingDirectory = Path.GetTempPath(),

                        Arguments = ConvertArgumentVariables(sourceCodeDirectory, Driver.Arguments)
                    },

                    EnableRaisingEvents = true
                };

                process.Start();
                
                // Start listening for Console.Error outputs
                process.BeginErrorReadLine();
                
                // If any Console.Error data has been received then call OnErrorDataReceived
                process.ErrorDataReceived += (sender, args) => OnErrorDataReceived(args, sourceCodeDirectory);

                Process = process;
            }
            catch (Exception e)
            {
                ConsoleUtility.Error($"Builder failed to launch for {Path.GetFileName(sourceCodeDirectory)} because {e.Message}");
            }
        }

        /// <summary>
        /// If any Console.Error stream data will be heard then log the received data to the console window.
        /// </summary>
        /// <param name="e">Event data from Console.Error stream.</param>
        /// <param name="sourceCodeDirectory">The directory that is being currently built.</param>
        private static void OnErrorDataReceived(DataReceivedEventArgs e, string sourceCodeDirectory)
        {
            // Sometimes empty strings and not "empty" (closer to null) strings are being sent
            //     and we don't want to log that into our console.
            if (! string.IsNullOrEmpty(e.Data))
            {
                ConsoleUtility.Error($"{Path.GetFileName(sourceCodeDirectory)} | {e.Data}");
            }
        }

        /// <summary>
        /// Convert a given arguments string with proper data for usage in other methods.
        /// </summary>
        /// <param name="sourceCodeDirectory">The directory that will be built.</param>
        /// <param name="arguments">A string of arguments to be converted with proper data.</param>
        /// <returns>Converted arguments string with proper data.</returns>
        private string ConvertArgumentVariables(string sourceCodeDirectory, string arguments)
        {
            /*
             *     Available variables      |    Examples
             *     --------------------------------------------------------------------------------------------------------
             *     $SOURCE_DIR_FULL$        |    H:\Steam\steamapps\common\Arma 3\Achilles\@Achilles\addons
             *     $SOURCE_DIR_PATH$        |    H:\Steam\steamapps\common\Arma 3\Achilles\@Achilles\addons\data_f_achilles
             *     $SOURCE_DIR_NAME$        |    data_f_achilles
             *                              |
             *     $TARGET_DIR_FULL$        |    H:\Steam\steamapps\common\Arma 3\Achilles\@Achilles\addons\output
             *     $TEMP_DIR_FULL$          |    C:\Users\User\AppData\Local\Temp
             *     $HEPHAESTUS_DIR_FULL$    |    C:\Users\User\Source\Repos\Hephaestus\Hephaestus\bin\Debug\netcoreapp2.2
             *                              |
             *     $PRIVATE_KEY_FULL$       |    H:\Steam\steamapps\common\Arma 3\myKey.biprivatekey
             *     $PRIVATE_KEY_NAME$       |    myKey
             *                              |
             *     $PROJECT_PREFIX$         |    achilles
             */

            // Get our data for the variables.
            string sourceCodeDirectoryFull = Project.SourceDirectory;
            string sourceCodeDirectoryPath = sourceCodeDirectory;
            string sourceCodeDirectoryName = Path.GetFileName(sourceCodeDirectory);

            string targetDirectoryFull = Project.TargetDirectory;
            string tempDirectoryFull = Path.GetTempPath();
            string hephaestusDirectory = AppDomain.CurrentDomain.BaseDirectory;

            string privateKeyFull = Project.PrivateKeyFile;
            string privateKeyName = Path.GetFileName(privateKeyFull);

            string projectPrefix = Project.ProjectPrefix;

            // Replace the variables in the arguments string with our data.
            arguments = arguments.Replace("$SOURCE_DIR_FULL$", sourceCodeDirectoryFull);
            arguments = arguments.Replace("$SOURCE_DIR_PATH$", sourceCodeDirectoryPath);
            arguments = arguments.Replace("$SOURCE_DIR_NAME$", sourceCodeDirectoryName);
            
            arguments = arguments.Replace("$TARGET_DIR_FULL$", targetDirectoryFull);
            arguments = arguments.Replace("$TEMP_DIR_FULL$", tempDirectoryFull);
            arguments = arguments.Replace("$HEPHAESTUS_DIR_FULL$", hephaestusDirectory);
            
            arguments = arguments.Replace("$PRIVATE_KEY_FULL$", privateKeyFull);
            arguments = arguments.Replace("$PRIVATE_KEY_NAME$", privateKeyName);
            
            arguments = arguments.Replace("$PROJECT_PREFIX$", projectPrefix);

            return arguments;
        }
    }
}
