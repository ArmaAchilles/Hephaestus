using HephaestusCommon.Utilities;
using System;
using System.Diagnostics;
using System.IO;
using HephaestusCommon.Classes;

namespace Hephaestus.Utilities
{
    public static class ArgumentUtility
    {
        internal static void Handle(string[] arguments)
        {
            if (arguments.Length != 1) return;
            
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
            Console.WriteLine("    init    Initiate a new Hephaestus project at your current CD location.");
            Console.WriteLine("    help    Display this again.");

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
            
            if (ConsoleUtility.AskYesNoQuestion("Use the command line?"))
            {
                InitFromCommandLineCommand();
                return;
            }

            // Launch Hephaestus Configurator
            ProcessStartInfo processStartInfo = new ProcessStartInfo
            {
                Arguments = path,
                // TODO: Create key in HephaestusInstaller
                FileName = RegistryUtility.GetKey(@"SOFTWARE\WOW6432Node\ArmaAchilles\Hephaestus\HephaestusConfigurator", "Path")
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
            string projectDirectory = ConsoleUtility.AskToEnterString("Project directory");
            if (! Directory.Exists(projectDirectory))
                throw new DirectoryNotFoundException($"{projectDirectory} does not exist.");

            string sourceDirectory = ConsoleUtility.AskToEnterString("Source directory");
            if (! Directory.Exists(sourceDirectory))
                throw new DirectoryNotFoundException($"{sourceDirectory} does not exist.");

            string targetDirectory = ConsoleUtility.AskToEnterString("Target directory");
            if (! Directory.Exists(targetDirectory))
                throw new DirectoryNotFoundException($"{targetDirectory} does not exist.");
            
            string addonBuilderFile = ConsoleUtility.AskToEnterString("Addon Builder file path");
            if (! File.Exists(addonBuilderFile))
                throw new FileNotFoundException($"{addonBuilderFile} file does not exist.");

            string projectPrefix = ConsoleUtility.AskToEnterString("Project prefix");

            string privateKeyFile = ConsoleUtility.AskToEnterString("Private key file path");
            if (! File.Exists(privateKeyFile))
                throw new FileNotFoundException($"{privateKeyFile} file does not exist.");

            string gameExecutable = ConsoleUtility.AskToEnterString("Game executable file path");
            if (! File.Exists(gameExecutable))
                throw new FileNotFoundException($"{gameExecutable} file does not exist.");

            string gameExecutableArguments = ConsoleUtility.AskToEnterString("Game executable arguments");

            bool shutdownGameBeforeBuilding = ConsoleUtility.AskYesNoQuestion("Shutdown game before building?");
            bool startGameAfterBuilding = ConsoleUtility.AskYesNoQuestion("Start game after building?");
            
            Project project = new Project(projectDirectory, sourceDirectory, targetDirectory, addonBuilderFile, projectPrefix,
                privateKeyFile, gameExecutable, gameExecutableArguments, shutdownGameBeforeBuilding, startGameAfterBuilding);
            
            project.Save();
            
            Console.WriteLine("Hephaestus project initialized. Run 'hephaestus' to launch Hephaestus.");
            Environment.Exit(0);
        }
    }
}
