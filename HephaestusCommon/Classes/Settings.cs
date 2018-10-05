using System.Collections.Generic;

namespace HephaestusCommon.Classes
{
    public class Settings
    {
        public class Project
        {
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
            Dictionary<string, string> Checksums { get; set; }
        }
    }
}
