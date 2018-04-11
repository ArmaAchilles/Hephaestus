using System;
using System.Reflection;

namespace AddonBuilder.Utility
{
    /// <summary>
    /// Utility class that provides various methods to access specific assembly attributes.
    /// </summary>
    public static class AssemblyUtil
    {
        public static string GetTitle()
        {
            var attribute = GetAssemblyAttribute<AssemblyTitleAttribute>();
            if (attribute != null)
                return attribute.Title;
            return string.Empty;
        }

        public static string GetCompany()
        {
            var attribute = GetAssemblyAttribute<AssemblyCompanyAttribute>();
            if (attribute != null)
                return attribute.Company;
            return string.Empty;
        }

        public static string GetCopyright()
        {
            var attribute = GetAssemblyAttribute<AssemblyCopyrightAttribute>();
            if (attribute != null)
                return attribute.Copyright;
            return string.Empty;
        }

        public static string GetVersion()
        {
            return Assembly.GetCallingAssembly().GetName().Version.ToString();
        }

        private static T GetAssemblyAttribute<T>() where T : Attribute
        {
            object[] attributes = Assembly.GetExecutingAssembly()
                .GetCustomAttributes(typeof(T), true);
            if (attributes == null || attributes.Length == 0) return null;
            return (T)attributes[0];
        }
    }
}
