using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ExGTF.Reader
{
    public static class ParamsHelper
    {
        public static object[] GetArrayObjectParams(string value)
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
    }
}