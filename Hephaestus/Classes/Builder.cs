using System;
using System.Diagnostics;
using System.IO;
using Hephaestus.Common.Classes;
using Hephaestus.Common.Utilities;

namespace Hephaestus.Classes
{
    public class Builder
    {
        public Process Process { get; set; }
        private Project Project { get; set; }
        private Driver Driver { get; set; }

        public Builder(string sourceCodeDirectory, Project project)
        {
            Project = project;
            Driver = DriverUtility.GetSelectedDriver(project);
            
            Build(sourceCodeDirectory);
        }
        
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
                
                /*
                 * "$SOURCE_DIR_FULL$" "$TARGET_DIR_FULL$"
                 * -prefix="$PROJECT_PREFIX$\$SOURCE_DIR_NAME$"
                 * -temp="$TEMP_DIR_FULL$\$PROJECT_PREFIX$"
                 * -include="$HEPHAESTUS_DIR_FULL$\Hephaestus.AddonBuilderIncludes.txt"
                 * -binarizeFullLogs
                 */
                
                /*
                 * build -f -k "$PRIVATE_KEY_FULL$" -w unquoted-string "$SOURCE_DIR_FULL$" "$TARGET_DIR_FULL$\$SOURCE_DIR_NAME$.pbo"
                 */

                process.Start();
                
                process.BeginErrorReadLine();
                
                process.ErrorDataReceived += (sender, args) => OnErrorDataReceived(args, sourceCodeDirectory);

                Process = process;
            }
            catch (Exception e)
            {
                Console.Error.WriteLine($"Builder failed to launch for {Path.GetFileName(sourceCodeDirectory)} because {e.Message}");
            }
        }

        private static void OnErrorDataReceived(DataReceivedEventArgs e, string sourceCodeDirectory)
        {
            if (! string.IsNullOrEmpty(e.Data))
            {
                Console.Error.WriteLine($"{Path.GetFileName(sourceCodeDirectory)} | {e.Data}");
            }
        }

        private string ConvertArgumentVariables(string sourceCodeDirectory, string arguments)
        {
            /*
             * Available variables:
             *     $SOURCE_DIR_FULL$
             *     $SOURCE_DIR_PATH$
             *     $SOURCE_DIR_NAME$
             * 
             *     $TARGET_DIR_FULL$
             *     $TEMP_DIR_FULL$
             *     $HEPHAESTUS_DIR_FULL$
             *
             *     $PRIVATE_KEY_FULL$
             *     $PRIVATE_KEY_NAME$
             *
             *     $PROJECT_PREFIX$
             */

            string sourceCodeDirectoryFull = Project.SourceDirectory;
            string sourceCodeDirectoryPath = sourceCodeDirectory;
            string sourceCodeDirectoryName = Path.GetFileName(sourceCodeDirectory);

            string targetDirectoryFull = Project.TargetDirectory;
            string tempDirectoryFull = Path.GetTempPath();
            string hephaestusDirectory = AppDomain.CurrentDomain.BaseDirectory;

            string privateKeyFull = Project.PrivateKeyFile;
            string privateKeyName = Path.GetFileName(privateKeyFull);

            string projectPrefix = Project.ProjectPrefix;

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