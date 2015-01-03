using System;
using System.Diagnostics;
using System.Reflection;

namespace AIR.Common.Diagnostics
{
    public static class DebugHelper
    {
        private static bool enabled = true;

        /// <summary>
        /// Writes out debug information about an objects public properties
        /// </summary>
        /// <param name="message">The beginning of the debug string</param>
        /// <param name="o">The object you want to write out the properties</param>
        /// <example>
        /// Debug("My Object: ", object);
        /// </example>
        [Conditional("DEBUG")]
        public static void WriteProperties(string message, object o)
        {
            if (!enabled) return;

            string debugInfo = String.Empty;
            
            try
            {
                Type typeData = o.GetType();
                PropertyInfo[] properties = typeData.GetProperties(BindingFlags.Instance | BindingFlags.Public);

                foreach (PropertyInfo property in properties)
                {
                    debugInfo += property.Name + "='" + property.GetValue(o, null).ToString() + "' ";
                }
            }
            catch (Exception e)
            {
                debugInfo = "ERROR DEBUGGING PROPERTIES: " + e.Message;
            }

            Debug.WriteLine(message + " " + debugInfo);
        }

        /// <summary>
        /// Writes out debug information about an object
        /// </summary>
        /// <param name="message">The beginning of the debug string</param>
        /// <param name="o">The object you want to write out the properties</param>
        /// <example>
        /// Debug("My Object: ", object);
        /// </example>
        [Conditional("DEBUG")]
        public static void WriteFields(string message, object o)
        {
            if (!enabled) return;

            string debugInfo = String.Empty;
            
            try
            {
                Type typeData = o.GetType();
                FieldInfo[] fields = typeData.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);

                foreach (FieldInfo field in fields)
                {
                    debugInfo += field.Name + "='" + field.GetValue(o).ToString() + "' ";
                }
            }
            catch(Exception e)
            {
                debugInfo = "ERROR DEBUGGING FIELDS: " + e.Message;
            }

            Debug.WriteLine(message + " " + debugInfo);
        }

        [Conditional("DEBUG")]
        public static void WriteParams(string message, params object[] parameters)
        {
            if (!enabled) return;

            string debugInfo = String.Empty;

            try
            {
                for (int i = 0; i < parameters.Length; i++)
                {
                    if (parameters[i] != null)
                    {
                        if ((i % 2) == 0)
                        {
                            debugInfo += parameters[i].ToString() + "='";
                        }
                        else
                        {
                            debugInfo += parameters[i].ToString() + "' ";
                        }
                    }
                }
            }
            catch (Exception e)
            {
                debugInfo = "ERROR DEBUGGING PARAMS: " + e.Message;
            }

            Debug.WriteLine(message + " " + debugInfo);
        }
    }
}
