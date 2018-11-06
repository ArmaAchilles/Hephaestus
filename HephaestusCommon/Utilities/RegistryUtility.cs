using Microsoft.Win32;
using System;

namespace HephaestusCommon.Utilities
{
    public static class RegistryUtility
    {
        public static string GetKey(string keyPath, string subKeyName)
        {
            string value = null;

            try
            {
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(keyPath))
                {
                    object subKeyObject = key?.GetValue(subKeyName);

                    if (subKeyObject != null)
                    {
                        value = subKeyObject as string;
                    }
                }
            }
            catch (Exception)
            {
                // ignored
            }

            return value;
        }
    }
}
