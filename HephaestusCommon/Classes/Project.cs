using System.Collections.Generic;

namespace HephaestusCommon.Classes
{
    public class Project
    {
        public Project(string projectDirectory, string sourceDirectory, string targetDirectory,
            string addonBuilderFile, string projectPrefix, string privateKeyFile,
            string gameExecutable, string gameExecutableArguments, bool shutdownGameBeforeBuilding,
            bool startGameAfterBuilding, Dictionary<string, string> checksums)
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
            Checksums = checksums;
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

        // Folder name, checksum
        public Dictionary<string, string> Checksums { get; set; }
    }
}
