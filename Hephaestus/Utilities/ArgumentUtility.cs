using HephaestusCommon.Utilities;
using System;
using System.Diagnostics;
using Hephaestus.Classes.Exceptions;

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
                    HelpCommand();;
                    break;

                case "init":
                    InitCommand();
                    break;
                
                case "version":
                case "-v":
                case "--version":
                    VersionCommand();
                    break;

                default:
                    throw new InvalidCommandException(
                        $"'{arguments[0]}' is not a Hephaestus command. See 'hephaestus help'"
                    );
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
                throw new InvalidCommandException("A Hephaestus project is already initialized. Use 'hephaestus' to launch Hephaestus.");
            }

            // Launch Hephaestus Configurator
            ProcessStartInfo processStartInfo = new ProcessStartInfo
            {
                Arguments = Environment.CurrentDirectory,
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
    }
}
