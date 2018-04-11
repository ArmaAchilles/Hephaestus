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
            var attr = GetAssemblyAttribute<AssemblyTitleAttribute>();
            if (attr != null)
                return attr.Title;
            return string.Empty;
        }

        public static string GetCompany()
        {
            var attr = GetAssemblyAttribute<AssemblyCompanyAttribute>();
            if (attr != null)
                return attr.Company;
            return string.Empty;
        }

        public static string GetCopyright()
        {
            var attr = GetAssemblyAttribute<AssemblyCopyrightAttribute>();
            if (attr != null)
                return attr.Copyright;
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
