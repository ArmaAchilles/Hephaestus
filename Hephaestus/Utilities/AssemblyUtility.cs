using System.Reflection;

namespace Hephaestus.Utilities
{
    internal static class AssemblyUtility
    {
        /// <summary>
        /// Gets the version number from the assembly data.
        /// </summary>
        /// <returns>Version number, e.g. 1.0.0.0</returns>
        internal static string GetVersion()
        {
            return Assembly.GetCallingAssembly().GetName().Version.ToString();
        }
    }
}