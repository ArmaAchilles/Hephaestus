using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hephaestus.Classes
{
    public class Settings
    {
        public class Directories
        {
            public string SourceDirectory { get; set; }
            public string TargetDirectory { get; set; }

            public string AddonBuilderDirectory { get; set; }
        }

        public string ProjectPrefix { get; set; }

        public class Signing
        {
            public string PrivateKeyDirectory { get; set; }
            public string PrivateKeyPrefix { get; set; }
            public string PrivateKeyDefaultVersion { get; set; }
        }

        public class GameInformation
        {
            public string GameFolder { get; set; }
            public string GameExecutable { get; set; }
            public string GameExecutableArguments { get; set; }

            public bool ShutdownGameBeforeBuilding { get; set; }
            public bool StartGameAfterBuilding { get; set; }
        }

        public class Checksums
        {
            //
        }
    }
}
