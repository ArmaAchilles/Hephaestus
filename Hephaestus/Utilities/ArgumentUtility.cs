﻿using HephaestusCommon.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Hephaestus.Utilities
{
    public class ArgumentUtility
    {
        internal static void Handle(string[] arguments)
        {
            if (arguments.Length == 1)
            {
                switch (arguments[0])
                {
                    case "help":
                    case "-h":
                    case "--help":
                        Console.WriteLine("Available Hephaestus commands are:");
                        Console.WriteLine("    Launching with no commands will initiate building of your current project (if present).");
                        Console.WriteLine("    init    Initiate a new Hephaestus project at your current CD location.");
                        Console.WriteLine("    help    Display this again.");

                        Environment.Exit(0);
                        break;

                    case "init":
                        // Check if project file already exists
                        string path = Environment.CurrentDirectory;

                        if (ProjectUtility.ProjectExists(path))
                        {
                            Console.Error.WriteLine("A Hephaestus project is already initialized. Use 'hephaestus' to launch Hephaestus.");

                            Environment.Exit(4);
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
                        break;

                    default:
                        Console.Error.WriteLine(
                            String.Format("'{0}' is not a Hephaestus command. See 'hephaestus help'", arguments[0]
                        ));

                        Environment.Exit(3);
                        break;
                }
            }
        }
    }
}