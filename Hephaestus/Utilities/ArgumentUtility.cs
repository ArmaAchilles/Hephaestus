using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using Hephaestus.Common.Classes;
using Hephaestus.Common.Utilities;

namespace Hephaestus.Utilities
{
    public static class ArgumentUtility
    {
        internal static void Handle(string[] arguments)
        {
            if (arguments.Length != 1)
            {
                return;
            }
            
            switch (arguments[0])
            {
                case "help":
                case "-h":
                case "--help":
                    HelpCommand();
                    break;

                case "init":
                    InitCommand();
                    break;
                
                case "force":
                    Program.ForceBuild = true;
                    break;
                
                case "version":
                case "-v":
                case "--version":
                    VersionCommand();
                    break;

                default:
                    Console.Error.WriteLine(
                        $"'{arguments[0]}' is not a Hephaestus command. See 'hephaestus help'"
                    );
                    
                    Environment.Exit(1);
                    break;
            }
        }

        private static void HelpCommand()
        {
            Console.WriteLine("Available Hephaestus commands are:");
            Console.WriteLine("    Launching with no commands will initiate building of your current project (if present).");
            Console.WriteLine("    init        Initiate a new Hephaestus project at your current CD location.");
            Console.WriteLine("    force       Force build a project ignoring all checksums.");
            Console.WriteLine("    version     Show Hephaestus version.");
            Console.WriteLine("    help        Display this again.");

            Environment.Exit(0);
        }

        private static void InitCommand()
        {
            // Check if project file already exists
            string path = Environment.CurrentDirectory;

            if (ProjectUtility.ProjectExists(path))
            {
                Console.Error.WriteLine("A Hephaestus project is already initialized. Use 'hephaestus' to launch Hephaestus.");
                
                Environment.Exit(1);
            }

            if (! RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                InitFromCommandLineCommand();
                return;
            }
            
            if (ConsoleUtility.AskYesNoQuestion("Use the command line?"))
            {
                InitFromCommandLineCommand();
                return;
            }

            // Launch Hephaestus Configurator
            ProcessStartInfo processStartInfo = new ProcessStartInfo
            {
                Arguments = path,
                FileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Hephaestus.Configurator.exe")
            };

            Process.Start(processStartInfo);
            
            Environment.Exit(0);
        }

        private static void VersionCommand()
        {
            Console.WriteLine(AssemblyUtility.GetVersion());
            
            Environment.Exit(0);
        }

        private static void InitFromCommandLineCommand()
        {
            string projectDirectory = Environment.CurrentDirectory;
            
            string sourceDirectory = ConsoleUtility.AskToEnterPath("Source directory", PathType.Directory);
            string targetDirectory = ConsoleUtility.AskToEnterPath("Target directory", PathType.Directory);

            string projectPrefix = ConsoleUtility.AskToEnterString("Project prefix");

            string privateKeyFile = ConsoleUtility.AskToEnterPath("Private key file", PathType.File);

            string gameExecutable = ConsoleUtility.AskToEnterPath("Game executable", PathType.File);
            string gameExecutableArguments = ConsoleUtility.AskToEnterString("Game executable arguments");
            Game game = new Game(gameExecutable, gameExecutableArguments);

            bool shutdownGameBeforeBuilding = ConsoleUtility.AskYesNoQuestion("Shutdown game before building?");
            bool startGameAfterBuilding = ConsoleUtility.AskYesNoQuestion("Start game after building?");

            List<Driver> drivers = new List<Driver>();

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                if (ConsoleUtility.AskYesNoQuestion("Do you want to use AddonBuilder?"))
                {
                    string addonBuilderPath = ConsoleUtility.AskToEnterPath("Addon Builder executable", PathType.File);
                    
                    Driver addonBuilderDriver = new Driver(
                        "AddonBuilder",
                        addonBuilderPath,
                        "\"$SOURCE_DIR_FULL$\" \"$TARGET_DIR_FULL$\" -prefix=\"$PROJECT_PREFIX$\\$SOURCE_DIR_NAME$\" -temp=\"$TEMP_DIR_FULL$\\$PROJECT_PREFIX$\" -include=\"$HEPHAESTUS_DIR_FULL$\\Hephaestus.AddonBuilderIncludes.txt\" -binarizeFullLogs"
                    );
                    
                    drivers.Add(addonBuilderDriver);
                }
            }

            string armakeExecutable;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                armakeExecutable = Environment.Is64BitOperatingSystem ? "armake_w64.exe" : "armake_w32.exe";
            }
            else
            {
                armakeExecutable = "armake";
            }
            
            Driver armakeDriver = new Driver(
                "Armake",
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Libraries", armakeExecutable),
                "build -f -k \"$PRIVATE_KEY_FULL$\" -w unquoted-string \"$SOURCE_DIR_PATH$\" \"$TARGET_DIR_FULL$\\$SOURCE_DIR_NAME$.pbo\""
            );
            
            drivers.Add(armakeDriver);

            if (ConsoleUtility.AskYesNoQuestion("Do you want to add a custom driver?"))
            {
                while (true)
                {
                    string driverName = ConsoleUtility.AskToEnterString("Driver name");
                    string driverPath = ConsoleUtility.AskToEnterPath("Driver executable path", PathType.File);
                    string driverArguments = ConsoleUtility.AskToEnterString("Driver arguments");
                    
                    Driver customDriver = new Driver(driverName, driverPath, driverArguments);
                    
                    drivers.Add(customDriver);

                    if (! ConsoleUtility.AskYesNoQuestion("Do you want to add another custom driver?"))
                        break;
                }
            }
            
            string selectedDriver = ConsoleUtility.AskToEnterString("Driver to use");
            
            Project project = new Project(projectDirectory, sourceDirectory, targetDirectory, projectPrefix, privateKeyFile, game,
                shutdownGameBeforeBuilding, startGameAfterBuilding, selectedDriver, drivers);
            
            project.Save();
            
            Console.WriteLine("Hephaestus project initialized. Run 'hephaestus' to launch Hephaestus.");
            Environment.Exit(0);
        }
    }
}
