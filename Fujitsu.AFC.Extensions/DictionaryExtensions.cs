using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Fujitsu.AFC.Extensions
{
    public static class DictionaryExtensions
    {
        public static void MergeDictionary(this Dictionary<string, string> value, Dictionary<string, string> deltaKeyValuePairDictionary)
        {
            foreach (var d in deltaKeyValuePairDictionary)
            {
                if (value.ContainsKey(d.Key))
                {
                    if (string.IsNullOrEmpty(d.Value))
                    {
                        value.Remove(d.Key);
                    }
                    else if (!value[d.Key].SafeEquals(d.Value))
                    {
                        value[d.Key] = d.Value;
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(d.Value))
                    {
                        value.Add(d.Key, d.Value);
                    }
                }
            }
        }

        public static string ToXmlString(this Dictionary<string, string> value)
        {
            var xElem = new XElement("items", value.Select(x => new XElement("item", new XElement("key", x.Key), new XElement("value", x.Value))));
            return xElem.ToString(SaveOptions.DisableFormatting);
        }
    }
}