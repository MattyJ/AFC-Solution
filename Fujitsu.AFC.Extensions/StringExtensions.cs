using Fujitsu.AFC.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Security;
using System.Xml.Linq;

namespace Fujitsu.AFC.Extensions
{
    public static class StringExtensions
    {
        public static bool SafeEquals(this string value, string compare, StringComparison comparison = StringComparison.InvariantCultureIgnoreCase)
        {
            if (value == null)
            {
                return compare == null;
            }

            return value.Equals(compare, comparison);
        }

        public static bool SafeEquals(this string value, string[] compares, StringComparison comparison = StringComparison.InvariantCultureIgnoreCase)
        {
            if (value == null)
            {
                return false;
            }

            return compares.Any(str => value.Equals(str, comparison));
        }

        public static bool SafeContains(this string value, string compare)
        {
            if (value == null)
            {
                return compare == null;
            }

            return value.ToUpper().Contains(compare.ToUpper());
        }


        public static string SafeTrim(this string value)
        {
            if (value == null)
            {
                return String.Empty;
            }

            return value.Trim();
        }


        public static string SafeTrim(this string value, int maxLength)
        {
            if (value == null)
            {
                return String.Empty;
            }

            value = value.Trim();

            if (value.Length > maxLength)
            {
                return value.Substring(0, maxLength).Trim();
            }
            return value;
        }

        public static int? ToInt(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            int tmpInt;
            if (int.TryParse(value, out tmpInt))
            {
                return tmpInt;
            }
            throw new ArgumentException("Invalid Integer value : " + value);
        }

        public static decimal? ToDecimal(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            decimal tmpDecimal;
            if (decimal.TryParse(value, out tmpDecimal))
            {
                return tmpDecimal;
            }
            throw new ArgumentException("Invalid Decimal value : " + value);
        }

        public static bool ToBool(this string value)
        {
            if (value.SafeEquals("yes") || value.SafeEquals("y") || value.SafeEquals("true") || value.SafeEquals("t") || (value.SafeEquals("1")))
            {
                return true;
            }
            if (value.SafeEquals("no") || value.SafeEquals("n") || value.SafeEquals("false") || value.SafeEquals("f") || (value.SafeEquals("0")))
            {
                return false;
            }

            throw new ArgumentException("Invalid Boolean value : " + value);
        }

        public static DateTime? ToDateTime(this string value)
        {
            DateTime d;

            if (value == null)
            {
                return null;
            }

            var s = value.Trim().ToUpper();

            if (s.Length == 0)
            {
                return null;
            }
            if (DateTime.TryParse(s, out d))
            {
                return d;
            }

            throw new ArgumentException("Invalid Date value : " + value);
        }

        public static T ConvertTo<T>(this string value)
        {
            if (typeof(T).IsEnum)
            {
                return (T)Enum.Parse(typeof(T), value);
            }

            var convertor = TypeDescriptor.GetConverter(typeof(T));
            return (T)convertor.ConvertFromString(value);
        }

        public static T ConvertStringToGenericValue<T>(this string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return default(T);
            }

            T result;

            if (typeof(T) == typeof(DateTime))
            {
                result = (T)(object)DateTime.ParseExact(value, Database.DateTimeFormat, CultureInfo.InvariantCulture);
            }
            else
            {
                result = value.ConvertTo<T>();
            }

            return result;
        }

        public static bool IsNullOrEmpty(this string value)
        {
            return string.IsNullOrEmpty(value);
        }

        private static readonly HashSet<char> DefaultNonWordCharacters
          = new HashSet<char> { ',', '.', ':', ';' };

        public static string CropWholeWords(
          this string value,
          int length,
          HashSet<char> nonWordCharacters = null)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (length < 0)
            {
                throw new ArgumentException("Negative values not allowed.", nameof(length));
            }

            if (nonWordCharacters == null)
            {
                nonWordCharacters = DefaultNonWordCharacters;
            }

            if (length >= value.Length)
            {
                return value;
            }
            var end = length;

            for (var i = end; i > 0; i--)
            {
                if (value[i].IsWhitespace())
                {
                    break;
                }

                if (nonWordCharacters.Contains(value[i])
                    && (value.Length == i + 1 || value[i + 1] == ' '))
                {
                    //Removing a character that isn't whitespace but not part
                    //of the word either (ie ".") given that the character is
                    //followed by whitespace or the end of the string makes it
                    //possible to include the word, so we do that.
                    break;
                }
                end--;
            }

            if (end == 0)
            {
                //Return nothing at all if the first word is longer than the length, could
                // also favour returing what we have (e.g. end = length)
                end = 0;
            }

            return value.Substring(0, end);
        }

        public static Dictionary<int, string> CropWholeWordsIntoChunks(
          this string value,
          int chunks,
          int length,
          HashSet<char> nonWordCharacters = null)
        {
            var result = new Dictionary<int, string>();

            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (chunks < 1)
            {
                throw new ArgumentException("Value less than one is not allowed.", nameof(chunks));
            }

            if (length < 0)
            {
                throw new ArgumentException("Negative values not allowed.", nameof(length));
            }

            if (nonWordCharacters == null)
            {
                nonWordCharacters = DefaultNonWordCharacters;
            }

            var completed = false;
            for (var i = 1; i <= chunks; i++)
            {
                if (!completed)
                {
                    var stringValue = value.CropWholeWords(length, nonWordCharacters);
                    result.Add(i, stringValue);
                    if (stringValue.Length + 1 < value.Length)
                    {
                        value = value.Substring(stringValue.Length + 1);
                    }
                    else
                    {
                        completed = true;
                    }
                }
                else
                {
                    // Fill remaining dictionary values with empty strings
                    result.Add(i, string.Empty);
                }

            }

            return result;
        }

        public static SecureString PasswordToSecureString(this string value)
        {
            var result = new SecureString();
            foreach (var c in value.ToCharArray()) result.AppendChar(c);
            return result;
        }

        public static bool IsValidXml(this string value)
        {
            if (value.IsNullOrEmpty())
            {
                return true;
            }

            try
            {
                value.ToKeyValueDictionary();
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }

        public static Dictionary<string, string> ToKeyValueDictionary(this string value)
        {
            return new XDocument(XDocument.Parse(value)).Descendants("item").ToDictionary(i => i.Element("key").Value.Trim(), i => i.Element("value").Value.Trim(), StringComparer.InvariantCultureIgnoreCase);
        }

        public static Dictionary<string, string> ToKeyValueDictionaryIgnoreEmptyValues(this string value)
        {
            return new XDocument(XDocument.Parse(value)).Descendants("item").Where(i => i.Element("value").Value.Trim() != "").ToDictionary(i => i.Element("key").Value.Trim(), i => i.Element("value").Value.Trim(), StringComparer.InvariantCultureIgnoreCase);
        }

        public static string UpdateTitle(this string value, string title)
        {
            const string open = "[Open]";
            const string closed = "[Closed]";
            var openIndex = value.IndexOf(open, StringComparison.InvariantCultureIgnoreCase);
            var closedIndex = value.IndexOf(closed, StringComparison.InvariantCultureIgnoreCase);
            if (openIndex != -1 || closedIndex != -1)
            {
                return openIndex != -1 ? $"{title} {value.Substring(openIndex)}" : $"{title} {value.Substring(closedIndex)}";
            }
            return value;
        }

        public static string GetMergeFilename(this string value)
        {
            const string text = " (merge)";

            if (value.Contains('.'))
            {
                var extension = value.LastIndexOf(".");
                var oldExtension = value.Substring(extension);
                var newExtension = string.Concat(text, oldExtension);
                return value.Replace(oldExtension, newExtension);
            }

            return string.Concat(value, text);
        }

        private static bool IsWhitespace(this char character)
        {
            return character == ' ';
        }
    }
}
