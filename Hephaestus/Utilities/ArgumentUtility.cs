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
        /// <summary>
        /// Handles all the available arguments that can be passed in the command line.
        /// e.g. 'hephaestus init', 'hephaestus help', etc.
        /// </summary>
        /// <param name="arguments">Arguments that should be processed.</param>
        internal static void Handle(string[] arguments)
        {
            // In case no arguments were given, then return back to Program class.
            if (arguments.Length != 1)
            {
                return;
            }
            
            // Compare the given string in the array for any commands that are available.
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

        /// <summary>
        /// Displays the usage information.
        /// </summary>
        private static void HelpCommand()
        {
            Console.WriteLine("Usage:");
            Console.WriteLine("    Launching with no commands will initiate building of your current project (if present).");
            Console.WriteLine("    init        Initiate a new Hephaestus project at your current CD location.");
            Console.WriteLine("    force       Force build a project ignoring all checksums.");
            Console.WriteLine("    version     Show Hephaestus version.");
            Console.WriteLine("    help        Display this again.");

            Environment.Exit(0);
        }

        /// <summary>
        /// Creates .hephaestus.json file at the current CD location of the terminal.
        /// </summary>
        private static void InitCommand()
        {
            // Environment.CurrentDirectory returns from where Hephaestus is getting executed (not where Hephaestus.dll is located).
            string path = Environment.CurrentDirectory;

            // If .hephaestus.json already exists then we exit.
            if (ProjectUtility.ProjectExists(path))
            {
                Console.Error.WriteLine("A Hephaestus project is already initialized. Use 'hephaestus' to launch Hephaestus.");
                
                Environment.Exit(1);
            }

            // If the user is not on a Windows machine, then we don't even ask if they want to use Hephaestus.Configurator
            //    as it's only supported on Windows.
            if (! RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                InitFromCommandLineCommand();
                return;
            }
            
            // If the user is on a Windows machine then we ask if they want to use the command line.
            if (ConsoleUtility.AskYesNoQuestion("Use the command line?"))
            {
                InitFromCommandLineCommand();
                return;
            }

            // Launch Hephaestus.Configurator.
            ProcessStartInfo processStartInfo = new ProcessStartInfo
            {
                // We give Hephaestus.Configurator the current path (CD location of the terminal).
                Arguments = path,
                
                // We open Hephaestus.Configurator from AppDomain.CurrentDomain.BaseDirectory which is the location where Hephaestus.dll is located at.
                FileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Hephaestus.Configurator.exe")
            };

            Process.Start(processStartInfo);
            
            Environment.Exit(0);
        }

        /// <summary>
        /// Display version information.
        /// </summary>
        private static void VersionCommand()
        {
            Console.WriteLine(AssemblyUtility.GetVersion());
            
            Environment.Exit(0);
        }

        /// <summary>
        /// Create .hephaestus.json from the command line.
        /// </summary>
        private static void InitFromCommandLineCommand()
        {
            // CD location of the terminal.
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

            // Create our list of drivers that we will fill with custom drivers (if selected) and base ones.
            List<Driver> drivers = new List<Driver>();

            // If the user is on a Windows machine then we ask if they want to use BI's AddonBuilder.
            // As BI's AddonBuilder only works on Windows machines, we don't display the prompt on other machines.
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                if (ConsoleUtility.AskYesNoQuestion("Do you want to use AddonBuilder?"))
                {
                    string addonBuilderPath = ConsoleUtility.AskToEnterPath("Addon Builder executable", PathType.File);
                    
                    Driver addonBuilderDriver = new Driver(
                        "AddonBuilder",
                        addonBuilderPath,
                        /*
                         * Arguments for AddonBuilder:
                         * 
                         * "$SOURCE_DIR_FULL$" "$TARGET_DIR_FULL$"
                         * -temp="$TEMP_DIR_FULL$\$PROJECT_PREFIX$"	
                         * -include="$HEPHAESTUS_DIR_FULL$\Hephaestus.AddonBuilderIncludes.txt"
                         * -binarizeFullLogs
                         */
                        "\"$SOURCE_DIR_FULL$\" \"$TARGET_DIR_FULL$\" -prefix=\"$PROJECT_PREFIX$\\$SOURCE_DIR_NAME$\" -temp=\"$TEMP_DIR_FULL$\\$PROJECT_PREFIX$\" -include=\"$HEPHAESTUS_DIR_FULL$\\Hephaestus.AddonBuilderIncludes.txt\" -binarizeFullLogs"
                    );
                    
                    drivers.Add(addonBuilderDriver);
                }
            }

            string armakeExecutable;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                // As Armake provides two different versions for 64-bit and 32-bit machines, we select the one needed without user interaction.
                armakeExecutable = Environment.Is64BitOperatingSystem ? "armake_w64.exe" : "armake_w32.exe";
            }
            else
            {
                // For Debian machines it's only 'armake'.
                armakeExecutable = "armake";
            }
            
            Driver armakeDriver = new Driver(
                "Armake",
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Libraries", armakeExecutable),
                "build -f -k \"$PRIVATE_KEY_FULL$\" -w unquoted-string \"$SOURCE_DIR_PATH$\" \"$TARGET_DIR_FULL$\\$SOURCE_DIR_NAME$.pbo\""
            );
            
            drivers.Add(armakeDriver);

            // Ask if the user wants to add any custom drivers and loop until they say no.
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
            
            // Ask the user to select one driver from the given list.
            Console.WriteLine("Select one driver by entering the displayed number.");
            for (int i = 0; i < drivers.Count; i++)
            {
                Driver driver = drivers[i];

                // As i is zero-based we increment it by for presentation.
                Console.WriteLine($"{i + 1}. {driver.Name}");
            }

            string selectedDriver;
            // Loop until the user selects a proper answer.
            while (true)
            {
                string selectedDriverString = ConsoleUtility.AskToEnterString("Driver to use");

                try
                {
                    // May return an exception if not an Int32.
                    int selectedDriverNumber = Convert.ToInt32(selectedDriverString);
                    
                    // Decrement by one from selectedDriverNumber as we incremented it by one.
                    selectedDriverNumber--;

                    // Get the selected driver number.
                    // May return an exception if out of bounds.
                    selectedDriver = drivers[selectedDriverNumber].Name;

                    break;
                }
                catch (Exception)
                {
                    Console.WriteLine("Incorrect selection. Try again.");
                }
            }

            // Create our project from the given data.
            Project project = new Project(projectDirectory, sourceDirectory, targetDirectory, projectPrefix, privateKeyFile, game,
                shutdownGameBeforeBuilding, startGameAfterBuilding, selectedDriver, drivers);
            
            // Save our data to JSON.
            project.Save();
            
            Console.WriteLine("Hephaestus project initialized. Run 'hephaestus' to launch Hephaestus.");
            Environment.Exit(0);
        }
    }
}
