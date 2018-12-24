using System;
using System.Collections.Generic;
using System.Linq;

namespace Product.Common {
    public static class EnumUtils {
        public static string GetDisplayName<T>(this T enumValue) where T : struct, IConvertible {
            if (!typeof(T).IsEnum)
                throw new ArgumentException("T must be a valid enum");
            var enumValues = (EnumItem[]) (new EnumValueConverter().Convert(enumValue, typeof(EnumItem[]), null, null));
            return enumValues
                .FirstOrDefault(x => EqualityComparer<T>.Default.Equals((T) x.Key, enumValue))
                .GetValueOrDefault(x => x.Text);
        }
    }
}