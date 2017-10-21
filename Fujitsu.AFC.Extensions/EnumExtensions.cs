using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Fujitsu.AFC.Extensions
{
    public static class EnumExtensions
    {

        public static List<string> ConvertEnumToList<T>()
        {
            // set response item

            // get the type of the enum and types
            var enumType = typeof(T);
            var values = Enum.GetValues(enumType);

            // iterate the types

            return (from object value in values select value.ToString()).ToList();
        }

        public static string GetEnumDescription(Enum value)
        {
            var fi = value.GetType().GetField(value.ToString());

            var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            return attributes.Length > 0 ? attributes[0].Description : value.ToString();
        }

        public static T ToEnum<T>(this string value, bool ignoreCase = true)
        {
            return (T)Enum.Parse(typeof(T), value, ignoreCase);
        }

        public static string ToEnumText<T>(this int value)
        {
            var e = (T)(object)value;
            return e.ToString();
        }

    }
}
