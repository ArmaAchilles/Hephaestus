using IniParser;
using IniParser.Model;
using System;
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.Reflection;
using System.Collections.Generic;

namespace AddonBuilder
{
    class Program
    {
        static bool HasPrivateKey { get; set; } = false;

        static void Main(string[] args)
        {
            // Get version number
            string version = Assembly.GetCallingAssembly().GetName().Version.ToString();

            // Show our info
            Console.WriteLine("Launching Addon Builder v" + version);
            Console.WriteLine("Made by CreepPork_LV");
            Console.WriteLine("========");

            // If config.ini was not found, display error
            if (!File.Exists("config.ini"))
            {
                throw new FileNotFoundException("Failed to find the config.ini file!");
            }

            // Read our .ini info
            var parser = new FileIniDataParser();
            IniData data = parser.ReadFile("config.ini");

            string sourceDir = data["AddonFolders"]["sourceDir"];
            string targetDir = data["AddonFolders"]["targetDir"];

            string projectPrefix = data["AddonInformation"]["projectPrefix"];

            string privateKeyDir = data["AddonSigning"]["privateKeyDir"];
            string privateKeyPrefix = data["AddonSigning"]["privateKeyPrefix"];
            string privateKeyVersion = data["AddonSigning"]["privateKeyVersionDefault"];

            string ArmaFolder = data["ArmaInformation"]["ArmaFolder"];
            string AddonBuilderDir = data["ArmaInformation"]["AddonBuilderDir"];
            bool shutdownArma = bool.Parse(data["ArmaInformation"]["ShutdownArma"]);
            bool openArma = bool.Parse(data["ArmaInformation"]["OpenArma"]);
            string ArmaExecutable = data["ArmaInformation"]["OpenArmaExecutable"];
            string openArmaArguments = data["ArmaInformation"]["OpenArmaArguments"];

            // Check if any arguments were passed (version number)
            if (args.Length > 0)
            {
                privateKeyVersion = args[0];
                Console.WriteLine("Detected build version: {0}!", privateKeyVersion);
            }

            // Handle files and folders (show errors and/or create)
            HandleFolders(sourceDir, targetDir, privateKeyDir, ArmaFolder, AddonBuilderDir, projectPrefix);
            HandleFiles(ArmaFolder, AddonBuilderDir, privateKeyDir, privateKeyPrefix, privateKeyVersion);

            // Get the Addon Builder exe
            string AddonBuilderExe = AddonBuilderDir + "\\" + "AddonBuilder.exe";

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
            HandleBuild(AddonBuilderExe, sourceDir, targetDir, privateKey, projectPrefix, openArma, openArmaArguments, ArmaFolder, ArmaExecutable);

            Console.WriteLine("Finished all tasks. Exiting!");
        }

        /// <summary>
        /// Handles folders information, shows error messages and/or creates folders if required.
        /// </summary>
        /// <param name="source">Source directory (to get the folders with code)</param>
        /// <param name="target">Target directory (to put the PBOs)</param>
        /// <param name="privateKey">Private key directory</param>
        /// <param name="ArmaFolder">Arma 3 folder</param>
        /// <param name="AddonBuilder">Addon Builder folder</param>
        public static void HandleFolders(string source, string target, string privateKey, string ArmaFolder, string AddonBuilder, string projectPrefix)
        {
            if (!Directory.Exists(source))
            {
                throw new DirectoryNotFoundException("Failed to find the source folder!");
            }

            if (Directory.GetDirectories(source).Length == 0)
            {
                throw new DirectoryNotFoundException("Failed to find PBO folders in the source directory!");
            }

            if (!Directory.Exists(target))
            {
                Console.WriteLine("Target directory does not exist! Creating new folder!");
                Directory.CreateDirectory(target);
            }

            if (!Directory.Exists(privateKey))
            {
                Console.WriteLine("Failed to find the private key directory!");
            }

            if (!Directory.Exists(ArmaFolder))
            {
                throw new DirectoryNotFoundException("Failed to find the Arma 3 directory!");
            }

            if (!Directory.Exists(AddonBuilder))
            {
                throw new DirectoryNotFoundException("Failed to find the Addon Builder directory!");
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
        /// <param name="ArmaFolder">Arma 3 folder</param>
        /// <param name="AddonBuilder">Addon Builder folder</param>
        /// <param name="privateKey">Private key folder</param>
        /// <param name="privateKeyPrefix">Private key prefix</param>
        /// <param name="privateKeyVersion">Private key version (default or one from the argument)</param>
        public static void HandleFiles(string ArmaFolder, string AddonBuilder, string privateKey, string privateKeyPrefix, string privateKeyVersion)
        {
            string privateKeyName = privateKeyPrefix + "_" + privateKeyVersion + ".biprivatekey";
            if (!File.Exists(privateKey + "\\" + privateKeyName))
            {
                Console.WriteLine("No private key has been found! Skiping signing of the PBOs!");
            }
            else
            {
                HasPrivateKey = true;
            }

            if (!File.Exists(ArmaFolder + "\\" + "arma3_x64.exe") | !File.Exists(ArmaFolder + "\\" + "arma3.exe"))
            {
                throw new FileNotFoundException("Failed to find the Arma 3 executable file!");
            }

            if (!File.Exists(AddonBuilder + "\\" + "AddonBuilder.exe"))
            {
                throw new FileNotFoundException("Failed to find the Addon Builder executable file!");
            }
        }

        /// <summary>
        /// Handles the closing of Arma 3 if open
        /// </summary>
        public static void HandleArmaClose()
        {
            Process[] processes = Process.GetProcesses();
            string processName32 = "arma3";
            string processName64 = "arma3_x64";

            foreach (Process proc in processes)
            {
                string ProcessName = proc.ProcessName.ToLower();
                if (ProcessName == processName32 | ProcessName == processName64)
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
        /// <param name="AddonBuilder">Full path including .exe to Addon Builder</param>
        /// <param name="source">Source directory of the code</param>
        /// <param name="target">Target directory where the built PBOs will be placed</param>
        /// <param name="privateKey">The private key full path</param>
        /// <param name="projectPrefix">Project prefix</param>
        /// <param name="openArma">Should we open Arma after build is complete?</param>
        /// <param name="openArmaArguments">Arguments to launch Arma with</param>
        /// <param name="ArmaFolder">Arma 3 Folder</param>
        /// <param name="ArmaExecutable">Executable of Arma to launch with</param>
        public static void HandleBuild(string AddonBuilder, string source, string target, string privateKey, string projectPrefix, bool openArma, string openArmaArguments, string ArmaFolder, string ArmaExecutable)
        {
            string[] folders = Directory.GetDirectories(source);

            int folderCount = folders.Length;

            var parser = new FileIniDataParser();
            IniData data = parser.ReadFile("config.ini");

            int addedHashes = 0;
            List<Hash> hashesToChange = new List<Hash>();

            foreach (var folder in folders)
            {
                try
                {
                    Process builder = new Process();

                    string folderName = Path.GetFileName(folder);

                    if (HasPrivateKey)
                    {
                        builder.StartInfo.Arguments = "\"" + folder + "\"" + " " + "\"" + target + "\"" + " -packonly -sign=" + "\"" + privateKey + "\"" + " -prefix=" + "\"" + projectPrefix + "\"" + "\\" + folderName + " -temp=" + "\"" + Path.GetTempPath() + "\\" + projectPrefix + "\"" + " -binarizeFullLogs";
                    }
                    else
                    {
                        builder.StartInfo.Arguments = "\"" + folder + "\"" + " " + "\"" + target + "\"" + " -packonly -prefix=" + "\"" + projectPrefix + "\"" + "\\" + folderName + " -temp=" + "\"" + Path.GetTempPath() + "\\" + projectPrefix + "\"" + " -binarizeFullLogs";
                    }
                    builder.StartInfo.RedirectStandardOutput = true;
                    builder.StartInfo.UseShellExecute = false;
                    builder.StartInfo.CreateNoWindow = true;
                    builder.StartInfo.FileName = AddonBuilder;

                    builder.EnableRaisingEvents = true;
                    builder.Exited += (sender, e) => BuilderExit(sender, e, folderCount, openArmaArguments, ArmaFolder, ArmaExecutable, openArma);

                    string checksumName = projectPrefix.ToLower() + folderName.ToUpper();
                    string hash = HashHelper.HashDirectory("SHA1", new DirectoryInfo(folder));

                    if (data["Checksums"][checksumName] == null)
                    {
                        Console.WriteLine("No checksum found for {0}! Generating hash and building!", folderName);

                        data["Checksums"].AddKey(checksumName, hash);

                        addedHashes++;

                        builder.Start();
                    }
                    else if (data["Checksums"][checksumName] != hash)
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
                            data["Checksums"].RemoveKey(hash.ChecksumName);
                            data["Checksums"].AddKey(hash.ChecksumName, hash.FileHash);
                        }
                    }
                    parser.WriteFile("config.ini", data);
                }

                if (openArma)
                {
                    if (buildersNotLaunched == folderCount)
                    {
                        try
                        {
                            Console.WriteLine("Nothing built, opening Arma 3!");
                            Process Arma = new Process();
                            Arma.StartInfo.Arguments = openArmaArguments;
                            Arma.StartInfo.FileName = ArmaFolder + "\\" + ArmaExecutable;
                            Arma.Start();
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
        /// <param name="ArmaFolder">Arma 3 Folder</param>
        /// <param name="ArmaExecutable">Arma 3 Executable to launch the game with </param>
        /// <param name="openArma">Should we open Arma?</param>
        private static void BuilderExit(object sender, EventArgs e, int folderCount, string openArmaArguments, string ArmaFolder, string ArmaExecutable, bool openArma)
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
                        Process Arma = new Process();
                        Arma.StartInfo.Arguments = openArmaArguments;
                        Arma.StartInfo.FileName = ArmaFolder + "\\" + ArmaExecutable;
                        Arma.Start();
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

    public class Hash
    {
        public string ChecksumName { get; set; }
        public string FileHash { get; set; }

        public Hash(string checksumName, string hash)
        {
            ChecksumName = checksumName;
            FileHash = hash;
        }
    }
}
