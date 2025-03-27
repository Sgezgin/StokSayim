using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StokSayim.Helpers
{
    public static class JsonHelper
    {
        public static string SerializeObject(object obj)
        {
            return JsonConvert.SerializeObject(obj, Formatting.None);
        }

        public static T DeserializeObject<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        public static Dictionary<string, object> DeserializeCustomFields(string json)
        {
            if (string.IsNullOrEmpty(json))
            {
                return new Dictionary<string, object>();
            }

            try
            {
                return JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
            }
            catch
            {
                return new Dictionary<string, object>();
            }
        }

        public static string ExtractFieldFromJson(string json, string fieldName)
        {
            if (string.IsNullOrEmpty(json))
            {
                return string.Empty;
            }

            try
            {
                var dict = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
                if (dict != null && dict.ContainsKey(fieldName))
                {
                    return dict[fieldName]?.ToString();
                }

                return string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
