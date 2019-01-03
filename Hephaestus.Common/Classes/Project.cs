using System.Collections.Generic;
using Hephaestus.Common.Utilities;

namespace Hephaestus.Common.Classes
{
    public class Project
    {
        // ReSharper disable once UnusedMember.Global | Required for JSON to work
        public Project()
        {
        }

        public Project(string projectDirectory, string sourceDirectory, string targetDirectory, string projectPrefix, string privateKeyFile, Game game, bool shutdownGameBeforeBuilding, bool startGameAfterBuilding, string selectedDriver, List<Driver> drivers)
        {
            ProjectDirectory = projectDirectory;
            SourceDirectory = sourceDirectory;
            TargetDirectory = targetDirectory;
            ProjectPrefix = projectPrefix;
            PrivateKeyFile = privateKeyFile;
            Game = game;
            ShutdownGameBeforeBuilding = shutdownGameBeforeBuilding;
            StartGameAfterBuilding = startGameAfterBuilding;
            SelectedDriver = selectedDriver;
            Drivers = drivers;
        }

        public string ProjectDirectory { get; set; }

        public string SourceDirectory { get; set; }
        public string TargetDirectory { get; set; }

        public string ProjectPrefix { get; set; }

        public string PrivateKeyFile { get; set; }
        
        public Game Game { get; set; }

        public bool ShutdownGameBeforeBuilding { get; set; }
        public bool StartGameAfterBuilding { get; set; }
        
        // Directory name, SHA1
        public Dictionary<string, Hash> Hashes { get; set; }
        
        public string SelectedDriver { get; set; }

        public List<Driver> Drivers { get; set; }

        public void Save()
        {
            ProjectUtility.SetProject(ProjectDirectory, this);
        }
    }
}
