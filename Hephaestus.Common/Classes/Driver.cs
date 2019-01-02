namespace Hephaestus.Common.Classes
{
    public class Driver
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public string Arguments { get; set; }

        public Driver(string name, string path, string arguments)
        {
            Name = name;
            Path = path;
            Arguments = arguments;
        }
    }
}