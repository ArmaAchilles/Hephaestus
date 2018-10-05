using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace HephaestusCommon.Utilities
{
    public static class JsonUtility
    {
        public static string GetJsonPath(EJsonType jsonType = EJsonType.Master, string path = "")
        {
            string appPath = AppDomain.CurrentDomain.BaseDirectory;
            string jsonPath = "";

            switch (jsonType)
            {
                case EJsonType.Master:
                    jsonPath = String.Format("{0}\\.hephaestus.master.json", appPath);

                    break;

                case EJsonType.Project:
                    jsonPath = String.Format("{0}\\.hephaestus.json", path);

                    break;
            }

            if (! File.Exists(jsonPath))
            {
                jsonPath = null;
            }

            return jsonPath;
        }

        public static void WriteJsonFile(EJsonType jsonType, string json, string path = "")
        {
            string jsonPath = GetJsonPath(jsonType, path);

            File.WriteAllText(jsonPath, json);
        }

        public enum EJsonType
        {
            Master = 0,
            Project = 1
        }
    }
}
