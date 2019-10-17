using Newtonsoft.Json;

namespace Oracle.Copy.Test.Helper
{
    public static class JsonSerialization
    {
        public static T ParseJson<T>(this string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        public static string ToJsonString<T>(this T obj)
        {
            return JsonConvert.SerializeObject(obj);
        }
    }
}