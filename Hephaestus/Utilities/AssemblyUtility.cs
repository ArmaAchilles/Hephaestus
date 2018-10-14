using System.Reflection;

namespace Hephaestus.Utilities
{
    internal static class AssemblyUtility
    {
        internal static string GetVersion()
        {
            return Assembly.GetCallingAssembly().GetName().Version.ToString();
        }
    }
}