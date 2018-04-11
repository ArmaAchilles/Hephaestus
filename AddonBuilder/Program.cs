using IniParser;
using IniParser.Model;
using System;
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.Reflection;
using System.Collections.Generic;
using AddonBuilder.Models;
using AddonBuilder.Utility;

namespace AddonBuilder
{
    public class Program
    {
        private static readonly string ConfigFileName = "Hephaestus.ini";

        private static bool hasPrivateKey = false;

        public static void Main(string[] args)
        {
            // Get version number
            string appVersion = Assembly.GetCallingAssembly().GetName().Version.ToString();

            // Show our info
            Console.WriteLine("Launching Hephaestus v" + appVersion);
            Console.WriteLine("Made by CreepPork_LV");
            Console.WriteLine("========");

            // If config.ini was not found, display an error
            if (!File.Exists(ConfigFileName))
            {
                ConsoleUtil.ShowConsoleErrorMsg($"Failed to find {ConfigFileName}, please ensure it is in the same directory as the application.");
                Environment.Exit(1);
            }

            //TODO: Finish the updater

            //Console.WriteLine("Checking for updates...");
            //// Check for updates
            //if (NetworkInterface.GetIsNetworkAvailable())
            //{
            //    //new Task(() => {  }).Start();
            //    Classes.Updates.Updater.UpdateManager(appVersion);
            //}
            //else
            //{
            //    ShowConsoleErrorMsg("Failed to check for updates! No internet connection available!");
            //}

            // Read our .ini info
            var iniParser = new FileIniDataParser();
            IniData iniData = iniParser.ReadFile(ConfigFileName);

            string sourceDir = iniData["AddonFolders"]["sourceDir"];
            string targetDir = iniData["AddonFolders"]["targetDir"];

            string projectPrefix = iniData["AddonInformation"]["projectPrefix"];

            string privateKeyDir = iniData["AddonSigning"]["privateKeyDir"];
            string privateKeyPrefix = iniData["AddonSigning"]["privateKeyPrefix"];
            string privateKeyVersion = iniData["AddonSigning"]["privateKeyVersionDefault"];

            string armaFolder = iniData["ArmaInformation"]["ArmaFolder"];
            string addonBuilderDir = iniData["ArmaInformation"]["AddonBuilderDir"];
            bool shutdownArma = bool.Parse(iniData["ArmaInformation"]["ShutdownArma"]);
            bool openArma = bool.Parse(iniData["ArmaInformation"]["OpenArma"]);
            string armaExecutable = iniData["ArmaInformation"]["OpenArmaExecutable"];
            string armaLaunchArguments = iniData["ArmaInformation"]["OpenArmaArguments"];

            // Check if any arguments were passed (version number)
            if (args.Length > 0)
            {
                privateKeyVersion = args[0];
                Console.WriteLine("Detected build version: {0}!", privateKeyVersion);
            }

            // Handle files and folders (show errors and/or create)
            HandleFolders(sourceDir, targetDir, privateKeyDir, armaFolder, addonBuilderDir, projectPrefix);
            HandleFiles(armaFolder, addonBuilderDir, privateKeyDir, privateKeyPrefix, privateKeyVersion);

            // Get the Addon Builder exe
            string addonBuilderExe = addonBuilderDir + "\\" + "AddonBuilder.exe";

            // Close Arma if open
            if (shutdownArma)
            {
                HandleArmaClose();
            }
            else
            {
                Console.WriteLine("Found Arma 3 open, not shutting down!");
            }

            // Get the private key full path
            string privateKey = privateKeyDir + "\\" + privateKeyPrefix + "_" + privateKeyVersion + ".biprivatekey";

            // Handle the building of the addons
            HandleBuild(addonBuilderExe, sourceDir, targetDir, privateKey, projectPrefix, openArma, armaLaunchArguments,
                armaFolder, armaExecutable);

            Console.WriteLine("Finished all tasks. Exiting!");
        }

        /// <summary>
        /// Handles folders information, shows error messages and/or creates folders if required.
        /// </summary>
        /// <param name="source">Source directory (to get the folders with code)</param>
        /// <param name="target">Target directory (to put the PBOs)</param>
        /// <param name="privateKey">Private key directory</param>
        /// <param name="armaFolder">Arma 3 folder</param>
        /// <param name="addonBuilder">Addon Builder folder</param>
        private static void HandleFolders(string source, string target, string privateKey, string armaFolder,
            string addonBuilder, string projectPrefix)
        {
            if (!Directory.Exists(source))
            {
                ConsoleUtil.ShowConsoleErrorMsg(
                    $"The given source path does not exist or does not represent a valid path:\n {source}");
                Environment.Exit(1);
            }

            if (Directory.GetDirectories(source).Length == 0)
            {
                ConsoleUtil.ShowConsoleErrorMsg($"The given source path does not contain any buildable folders:\n {source}");
                Environment.Exit(1);
            }

            if (!Directory.Exists(target))
            {
                Console.WriteLine("Target directory does not exist! Creating folder!");
                Directory.CreateDirectory(target);
            }

            if (!Directory.Exists(privateKey))
            {
                Console.WriteLine("Failed to find the private key directory!");
            }

            if (!Directory.Exists(armaFolder))
            {
                ConsoleUtil.ShowConsoleErrorMsg(
                    $"The given path to the Arma 3 game directory does not exist or does not represent a valid path:\n {armaFolder}");
                Environment.Exit(1);
            }

            if (!Directory.Exists(addonBuilder))
            {
                ConsoleUtil.ShowConsoleErrorMsg(
                    $"The given path to the Arma 3 Addon Builder does not exist or does not represent a valid path:\n {addonBuilder}");
                Environment.Exit(1);
            }

            if (!Directory.Exists(Path.GetTempPath() + "\\" + projectPrefix))
            {
                Console.WriteLine("Temporary directory not found, creating!");
                Directory.CreateDirectory(Path.GetTempPath() + "\\" + projectPrefix);
            }
        }

        /// <summary>
        /// Handles errors if some of the required files do not exist.
        /// </summary>
        /// <param name="armaFolder">Arma 3 folder</param>
        /// <param name="addonBuilder">Addon Builder folder</param>
        /// <param name="privateKey">Private key folder</param>
        /// <param name="privateKeyPrefix">Private key prefix</param>
        /// <param name="privateKeyVersion">Private key version (default or one from the argument)</param>
        private static void HandleFiles(string armaFolder, string addonBuilder, string privateKey,
            string privateKeyPrefix, string privateKeyVersion)
        {
            string privateKeyName = privateKeyPrefix + "_" + privateKeyVersion + ".biprivatekey";
            if (!File.Exists(privateKey + "\\" + privateKeyName))
            {
                Console.WriteLine("No private key has been found! Skipping the signing of the PBOs!");
            }
            else
            {
                hasPrivateKey = true;
            }

            if (!File.Exists(armaFolder + "\\" + "arma3_x64.exe") | !File.Exists(armaFolder + "\\" + "arma3.exe"))
            {
                ConsoleUtil.ShowConsoleErrorMsg(
                    $"The given Arma 3 game path does not contain any valid Arma 3 executables:\n {armaFolder}");
                Environment.Exit(1);
            }

            if (!File.Exists(addonBuilder + "\\" + "AddonBuilder.exe"))
            {
                ConsoleUtil.ShowConsoleErrorMsg(
                    $"The given Addon Builder path does not contain a valid Addon Builder executable:\n {addonBuilder}");
                Environment.Exit(1);
            }
        }

        /// <summary>
        /// Handles the closing of Arma 3 if open
        /// </summary>
        private static void HandleArmaClose()
        {
            Process[] processes = Process.GetProcesses();
            string processName32 = "arma3";
            string processName64 = "arma3_x64";

            foreach (Process proc in processes)
            {
                string processName = proc.ProcessName.ToLower();
                if (processName == processName32 | processName == processName64)
                {
                    Console.WriteLine("Found Arma 3 open, closing!");

                    if (!proc.HasExited)
                    {
                        proc.CloseMainWindow();

                        proc.WaitForExit();
                    }

                    proc.Close();
                }
            }
        }

        static bool haveAllBuildersClosed = false;
        static int buildersNotLaunched = 0;
        static int closedBuilders = 0;

        /// <summary>
        /// Handle the building of the folders to get the magical PBOs
        /// </summary>
        /// <param name="addonBuilder">Full path including .exe to Addon Builder</param>
        /// <param name="source">Source directory of the code</param>
        /// <param name="target">Target directory where the built PBOs will be placed</param>
        /// <param name="privateKey">The private key full path</param>
        /// <param name="projectPrefix">Project prefix</param>
        /// <param name="openArma">Should we open Arma after build is complete?</param>
        /// <param name="openArmaArguments">Arguments to launch Arma with</param>
        /// <param name="armaFolder">Arma 3 Folder</param>
        /// <param name="armaExecutable">Executable of Arma to launch with</param>
        private static void HandleBuild(string addonBuilder, string source, string target, string privateKey,
            string projectPrefix, bool openArma, string openArmaArguments, string armaFolder, string armaExecutable)
        {
            string[] folders = Directory.GetDirectories(source);

            int folderCount = folders.Length;

            var parser = new FileIniDataParser();
            IniData iniData = parser.ReadFile(ConfigFileName);

            int addedHashes = 0;
            List<Hash> hashesToChange = new List<Hash>();

            foreach (var folder in folders)
            {
                try
                {
                    Process builder = new Process();

                    string folderName = Path.GetFileName(folder);

                    if (hasPrivateKey)
                    {
                        builder.StartInfo.Arguments = "\"" + folder + "\"" + " " + "\"" + target + "\"" +
                                                      " -packonly -sign=" + "\"" + privateKey + "\"" + " -prefix=" +
                                                      "\"" + projectPrefix + "\"" + "\\" + folderName + " -temp=" +
                                                      "\"" + Path.GetTempPath() + "\\" + projectPrefix + "\"" +
                                                      " -binarizeFullLogs";
                    }
                    else
                    {
                        builder.StartInfo.Arguments = "\"" + folder + "\"" + " " + "\"" + target + "\"" +
                                                      " -packonly -prefix=" + "\"" + projectPrefix + "\"" + "\\" +
                                                      folderName + " -temp=" + "\"" + Path.GetTempPath() + "\\" +
                                                      projectPrefix + "\"" + " -binarizeFullLogs";
                    }

                    builder.StartInfo.RedirectStandardOutput = true;
                    builder.StartInfo.UseShellExecute = false;
                    builder.StartInfo.CreateNoWindow = true;
                    builder.StartInfo.FileName = addonBuilder;

                    builder.EnableRaisingEvents = true;
                    builder.Exited += (sender, e) => BuilderExit(sender, e, folderCount, openArmaArguments, armaFolder,
                        armaExecutable, openArma);

                    string checksumName = projectPrefix.ToLower() + folderName.ToUpper();
                    string hash = HashUtil.HashDirectory("SHA1", new DirectoryInfo(folder));

                    if (iniData["Checksums"][checksumName] == null)
                    {
                        Console.WriteLine("No checksum found for {0}! Generating hash and building!", folderName);

                        iniData["Checksums"].AddKey(checksumName, hash);

                        addedHashes++;

                        builder.Start();
                    }
                    else if (iniData["Checksums"][checksumName] != hash)
                    {
                        Console.WriteLine("Hash mismatch for {0}! Building!", folderName);

                        builder.Start();

                        hashesToChange.Add(new Hash(checksumName, hash));
                    }
                    else
                    {
                        Console.WriteLine("Not building {0} because the file hasn't changed!", folderName);
                        buildersNotLaunched++;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Failed to start Addon Builder!");
                    Console.WriteLine("Error is: " + ex);
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                    Environment.Exit(1);
                }
            }

            while (!haveAllBuildersClosed | buildersNotLaunched == folderCount)
            {
                Thread.Sleep(100);
                if (hashesToChange.Count > 0 | addedHashes > 0)
                {
                    if (hashesToChange.Count > 0)
                    {
                        Hash[] hashes = hashesToChange.ToArray();
                        foreach (var hash in hashes)
                        {
                            iniData["Checksums"].RemoveKey(hash.ChecksumName);
                            iniData["Checksums"].AddKey(hash.ChecksumName, hash.FileHash);
                        }
                    }

                    parser.WriteFile(ConfigFileName, iniData);
                }

                if (openArma)
                {
                    if (buildersNotLaunched == folderCount)
                    {
                        try
                        {
                            Console.WriteLine("Nothing built, opening Arma 3!");
                            Process arma = new Process();
                            arma.StartInfo.Arguments = openArmaArguments;
                            arma.StartInfo.FileName = armaFolder + "\\" + armaExecutable;
                            arma.Start();
                            break;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Failed to start Arma 3!");
                            Console.WriteLine("Error is: " + ex);
                            Console.WriteLine("Press any key to continue...");
                            Console.ReadKey();
                            Environment.Exit(1);
                        }
                    }
                }
                else
                {
                    if (buildersNotLaunched + closedBuilders == folderCount)
                    {
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Handles the exit of each builder and when all of the builders have exited, then start up Arma 3.
        /// </summary>
        /// <param name="sender">Builder object</param>
        /// <param name="e">Event arguments</param>
        /// <param name="folderCount">Number of folders (number of builders launched)</param>
        /// <param name="openArmaArguments">Arguments to open Arma with</param>
        /// <param name="armaFolder">Arma 3 Folder</param>
        /// <param name="armaExecutable">Arma 3 Executable to launch the game with</param>
        /// <param name="openArma">Should we open Arma?</param>
        private static void BuilderExit(object sender, EventArgs e, int folderCount, string openArmaArguments,
            string armaFolder, string armaExecutable, bool openArma)
        {
            closedBuilders++;
            if (closedBuilders + buildersNotLaunched == folderCount)
            {
                haveAllBuildersClosed = true;
                if (openArma)
                {
                    try
                    {
                        Console.WriteLine("All builders finished, opening Arma 3!");
                        Process arma = new Process();
                        arma.StartInfo.Arguments = openArmaArguments;
                        arma.StartInfo.FileName = armaFolder + "\\" + armaExecutable;
                        arma.Start();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Failed to start Arma 3!");
                        Console.WriteLine("Error is: " + ex);
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey();
                        Environment.Exit(1);
                    }
                }
            }
        }
    }
}
