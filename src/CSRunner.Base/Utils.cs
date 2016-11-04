using CSRunner.Base;
using CSRunner.Base.Properties;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace CSRunner.Base
{
    public static class Utils
    {
        public static void SetConsoleUncode()
        {
            if (Config.Unicode)
                Console.OutputEncoding = Encoding.Unicode;
        }

        //public static void SetLoggingContext(this CommandLine cmdLine)
        //{
        //    SetLoggingContext(Path.GetFileNameWithoutExtension(cmdLine.Assembly), cmdLine.Task);
        //}

        //public static void SetLoggingContext(string assembly = "csrunner", string task = "csrunner")
        //{
        //    MappedDiagnosticsContext.Set("assembly", string.IsNullOrWhiteSpace(assembly) ? "csrunner" : assembly);
        //    MappedDiagnosticsContext.Set("task", string.IsNullOrWhiteSpace(task) ? "csrunner" : task);
        //}


        public static T Get<T>(this DbDataReader reader, int index, T defaultValue = default(T))
        {
            var val = reader.GetValue(index);
            if (null == val || DBNull.Value.Equals(val))
            {
                return defaultValue;
            }
            else
            {
                return (T)Convert.ChangeType(val, typeof(T));
            }
        }

        public static T? GetNullable<T>(this DbDataReader reader, int index, T? defaultValue = null)
            where T : struct
        {
            var val = reader.GetValue(index);
            if (null == val || DBNull.Value.Equals(val))
            {
                return defaultValue;
            }
            else
            {
                return (T)Convert.ChangeType(val, typeof(T));
            }
        }

        public static object Get(this DbDataReader reader, int index)
        {
            var val = reader.GetValue(index);
            if (null == val || DBNull.Value.Equals(val))
            {
                return null;
            }
            else
            {
                return val;
            }
        }


        internal static string GetNodeValue(this XmlNode node)
        {
            if (null == node)
                return null;
            else
                return node.InnerText;
        }

        internal static void ShowError(this Action<string, object[]> errorHandler, string message, params object[] parameters)
        {
            if (null != errorHandler)
                errorHandler(message, parameters);
        }

        public static bool HasAttribute<T>(this object element, bool inherit = false) where T : Attribute
        {
            return null != element && Attribute.IsDefined(element.GetType(), typeof(T), inherit);
        }

        public static string Replace(this string str, string oldValue, string newValue, StringComparison comparison)
        {
            if (null == str || null == oldValue || str.Length < oldValue.Length)
            {
                return str;
            }

            StringBuilder sb = new StringBuilder();

            int previousIndex = 0;
            int index = str.IndexOf(oldValue, comparison);
            while (index != -1)
            {
                sb.Append(str.Substring(previousIndex, index - previousIndex));
                if (null != newValue)
                {
                    sb.Append(newValue);
                }

                index += oldValue.Length;

                previousIndex = index;
                index = str.IndexOf(oldValue, index, comparison);
            }
            sb.Append(str.Substring(previousIndex));

            return sb.ToString();
        }

        internal static void CopyTo(this IList<string> list, StringCollection coll)
        {
            foreach(var s in list)
            {
                coll.Add(s);
            }
        }
    }
}
