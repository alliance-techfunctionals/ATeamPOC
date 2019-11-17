using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Exate.Rules.WebApi.DataAccess.Helper
{
    public static class JsonParser
    {
        public static void FillFromJToken(HashSet<string> leaves, JToken token, string prefix,
            bool includeArrayIndex = false)
        {
            switch (token.Type)
            {
                case JTokenType.Object:
                    foreach (JProperty prop in token.Children<JProperty>())
                    {
                        FillFromJToken(leaves, prop.Value, prefix + "." + prop.Name, includeArrayIndex);
                    }

                    break;

                case JTokenType.Array:
                    int index = 0;
                    foreach (JToken value in token.Children())
                    {
                        var arrayPrefix = includeArrayIndex ? string.Format("{0}[{1}]", prefix, index) : prefix;
                        FillFromJToken(leaves, value, arrayPrefix, includeArrayIndex);
                        index++;
                    }

                    break;

                default:
                    leaves.Add(prefix);
                    break;
            }
        }
    }
}
