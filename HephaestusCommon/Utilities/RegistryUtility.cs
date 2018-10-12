using Microsoft.Win32;
using System;

namespace HephaestusCommon.Utilities
{
    public class RegistryUtility
    {
        public static string GetKey(string keyPath, string subKeyName)
        {
            string value = null;

            try
            {
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(keyPath))
                {
                    if (key != null)
                    {
                        Object subKeyObject = key.GetValue(subKeyName);

                        if (subKeyObject != null)
                        {
                            value = subKeyObject as String;
                        }
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }

            return value;
        }
    }
}
