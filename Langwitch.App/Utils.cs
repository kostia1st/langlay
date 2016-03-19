using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
    }
}
