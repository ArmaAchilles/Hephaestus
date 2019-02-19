using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using Newtonsoft.Json;

namespace Hephaestus.Configurator.Classes
{
    public class Master
    {
        public List<string> ProjectDirectories { get; set; }
        
        public Master()
        {
        }

        public Master(List<string> projectDirectories)
        {
            ProjectDirectories = projectDirectories;
        }

        public Master Get()
        {
            string jsonPath = GetJsonPath();

            if (File.Exists(jsonPath))
            {
                ProjectDirectories = JsonConvert.DeserializeObject<Master>(File.ReadAllText(GetJsonPath())).ProjectDirectories;
            }

            return this;
        }

        public void Save()
        {
            string jsonPath = GetJsonPath();

            string json = JsonConvert.SerializeObject(this, Formatting.Indented);

            try
            {
                File.WriteAllText(jsonPath, json);
            }
            catch (Exception)
            {
                MessageBox.Show("Failed to load Project Master list!", "Hephaestus Configurator", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private static string GetJsonPath()
        {
            string path = AppDomain.CurrentDomain.BaseDirectory;
            
            return $@"{path}.hephaestus.master.json";
        }
    }
}