using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Product.Common
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

        public static T ParseEnum<T>(string value, T defaultValue) where T : struct
        {
            T result;
            if (Enum.TryParse(value, out result))
            {
                return result;
            }
            return defaultValue;
        }

        public static string ToDisplayString<T>(this T enumValue) where T : struct
        {
            return enumValue.GetType()
                .GetField(enumValue.ToString())
                .GetCustomAttributes(typeof(DisplayAttribute), false)
                .OfType<DisplayAttribute>().FirstOrDefault().GetValueOrDefault(x => x.Name);
        }

        public static int ParseInt(object value, int defaultValue)
        {
            return (int) ParseLong(value, defaultValue);
        }

        public static int? ParseInt(object value, int? defaultValue = null)
        {
            return (int?) ParseLong(value, defaultValue);
        }

        public static uint ParseUInt(object value, uint defaultValue)
        {
            return (uint) ParseLong(value, defaultValue);
        }

        public static uint? ParseUInt(object value, uint? defaultValue = null)
        {
            return (uint?) ParseLong(value, defaultValue);
        }

        public static long ParseLong(object value, long defaultValue)
        {
            return ParseLong(value, (long?) defaultValue).Value;
        }

        public static long? ParseLong(object value, long? defaultValue = null)
        {
            if (value != null)
            {
                if (value is string)
                {
                    long result;
                    if (long.TryParse((string) value, out result))
                        return result;
                }
                else
                {
                    try
                    {
                        return Convert.ToInt64(value);
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

        public static IList<T> Combine<T>(IList<T> list1, IList<T> list2)
        {
            var result =  new List<T>(list1);
            result.AddRange(list2);
            return result;
        }
    }
}
