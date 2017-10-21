using System;
using System.Globalization;
using Fujitsu.AFC.Constants;

namespace Fujitsu.AFC.Extensions
{
    public static class ObjectExtensions
    {
        public static string ConvertGenericValueToString<T>(this T value)
        {
            if (value == null)
            {
                return null;
            }
            string valueString;
            if (value is DateTime)
            {
                valueString = ((DateTime)(object)value).ToString(Database.DateTimeFormat, CultureInfo.InvariantCulture);
            }
            else
            {
                valueString = value.ToString();
            }
            return valueString;
        }
    }

}
