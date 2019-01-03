namespace Hephaestus.Common.Classes
{
    public class Driver
    {
        public string Name { get; }
        public string Path { get; }
        public string Arguments { get; }

        public Driver(string name, string path, string arguments)
        {
            Name = name;
            Path = path;
            Arguments = arguments;
        }
    }
}