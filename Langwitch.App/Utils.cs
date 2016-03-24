using System;

namespace Langwitch
{
    public static class Utils
    {
        public static string CapitalizeFirst(this string str)
        {
            if (str == null)
                throw new ArgumentNullException("str");
            var first = str.Substring(0, 1).ToUpper();
            var rest = str.Substring(1);
            return first + rest;
        }

        public static int ParseInt(object value, int defaultValue)
        {
            return ParseInt(value, (int?) defaultValue).Value;
        }

        public static int? ParseInt(object value, int? defaultValue = null)
        {
            if (value != null)
            {
                if (value is string)
                {
                    int result;
                    if (int.TryParse((string) value, out result))
                        return result;
                }
                else
                {
                    try
                    {
                        return Convert.ToInt32(value);
                    }
                    catch { }
                }
            }
            return defaultValue;
        }

        public static bool ParseBool(object value, bool defaultValue)
        {
            return ParseBool(value, (bool?) defaultValue).Value;
        }

        public static bool? ParseBool(object value, bool? defaultValue = null)
        {
            if (value != null)
            {
                if (value is string)
                {
                    bool result;
                    if (bool.TryParse((string) value, out result))
                        return result;
                }
                else
                {
                    try
                    {
                        return Convert.ToBoolean(value);
                    }
                    catch { }
                }
            }
            return defaultValue;
        }

        public static T2 GetValueOrDefault<T1, T2>(this T1 value, Func<T1, T2> f, T2 defaultValue = default(T2))
        {
            if (value != null)
                return f(value);
            return defaultValue;
        }

        public static T2 GetValueOrDefault<T2>(this string value, Func<string, T2> f, T2 defaultValue = default(T2))
        {
            if (!String.IsNullOrEmpty(value))
                return f(value);
            return defaultValue;
        }
    }
}
