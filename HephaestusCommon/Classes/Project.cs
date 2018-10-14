using System.Collections.Generic;
using HephaestusCommon.Utilities;

namespace HephaestusCommon.Classes
{
    public class Project
    {
        // ReSharper disable once UnusedMember.Global | Required for JSON to work
        public Project()
        {
        }

        public Project(string projectDirectory, string sourceDirectory, string targetDirectory,
            string addonBuilderFile, string projectPrefix, string privateKeyFile,
            string gameExecutable, string gameExecutableArguments, bool shutdownGameBeforeBuilding,
            bool startGameAfterBuilding)
        {
            ProjectDirectory = projectDirectory;
            SourceDirectory = sourceDirectory;
            TargetDirectory = targetDirectory;
            AddonBuilderFile = addonBuilderFile;
            ProjectPrefix = projectPrefix;
            PrivateKeyFile = privateKeyFile;
            GameExecutable = gameExecutable;
            GameExecutableArguments = gameExecutableArguments;
            ShutdownGameBeforeBuilding = shutdownGameBeforeBuilding;
            StartGameAfterBuilding = startGameAfterBuilding;
        }

        public string ProjectDirectory { get; set; }

        public string SourceDirectory { get; set; }
        public string TargetDirectory { get; set; }

        public string AddonBuilderFile { get; set; }

        public string ProjectPrefix { get; set; }

        public string PrivateKeyFile { get; set; }

        public string GameExecutable { get; set; }
        public string GameExecutableArguments { get; set; }

        public bool ShutdownGameBeforeBuilding { get; set; }
        public bool StartGameAfterBuilding { get; set; }
        
        public bool UseArmake { get; set; }

        // Directory name, SHA1
        public Dictionary<string, Hash> Hashes { get; set; }

        public void Save()
        {
            ProjectUtility.SetProject(ProjectDirectory, this);
        }
    }
}
