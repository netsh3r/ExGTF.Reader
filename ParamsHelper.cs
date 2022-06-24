using System.Collections.Generic;
using System.Linq;
using ExGTF.Reader.Props;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ExGTF.Reader
{
    public static class ParamsHelper
    {
        private static object[] GetArrayObjectParams(string value)
        {
            var arV = JsonConvert.DeserializeObject<object[]>(value.ToString());
            var arVresult = new List<Dictionary<string, string>>();
            foreach (JObject val in arV)
            {
                var arD = new Dictionary<string, string>();
                foreach (var jVal in val)
                {
                    arD.Add(jVal.Key, jVal.Value.ToString());
                }
                arVresult.Add(arD);
            }

            return arVresult.ToArray();
        }

        public static Dictionary<string, object> GetParams(string json)
        {
            var dict = JsonConvert.DeserializeObject<DictValue[]>(json);
            var @params = new Dictionary<string, object>();
            foreach (var d in dict)
            {
                if (d.IsArray)
                {
                    var arV = JsonConvert.DeserializeObject<string[]>(d.Value.ToString());
                    @params.Add(d.Name, arV);
                }
                else if (d.IsArrayObjects)
                {
                    var arVresult = GetArrayObjectParams(d.Value.ToString());
                    @params.Add(d.Name, arVresult.ToArray());
                }
                else
                {
                    @params.Add(d.Name, d.Value);
                }
            }

            return @params;
        }
    }
}