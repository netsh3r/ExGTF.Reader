namespace ExGTF.Example
{
    using ExGTF.Reader;
    using Newtonsoft.Json;

    internal class Program
    {
        static void Main(string[] args)
        {
            using var sr = new StreamReader("../../../config.json");
            var config = JsonConvert.DeserializeObject<Config>(sr.ReadToEnd());
            using var sr2 = new StreamReader("../../../DictValue.json");
            var dict = JsonConvert.DeserializeObject<DictValue[]>(sr2.ReadToEnd());
            var @params = new Dictionary<string, object>();
            foreach (var d in dict)
            {
                if (d.ObjectType != null && d.ObjectType == "Array")
                {
                    var arV = JsonConvert.DeserializeObject<string[]>(d.Value.ToString());
                    @params.Add(d.Name, arV);
                }
                else
                {
                    @params.Add(d.Name, d.Value);
                }
            }

            var rd = new ExGTFReader(config.Provider.Template, @params);
            rd.Create(config.Provider.Path);
            //var testUrl = "C:/Users/dmitr/source/repos/RTN_Report";
            //var test2Url = "C:/Users/dmitr/Documents/Tests/EGTF.Reader/";
            //var rd = new ExGTFReader($"{testUrl}/EGTF.Reader/test.exgtf", 
            //    new Dictionary<string, object>
            //    {
            //        { "userName", "dima" }, 
            //        { "password", "123" }, 
            //        { "cw", "Console.WriteLine(\"123\")" },
            //        { "users", new[]{"1", "2"} }
            //    });
            //rd.Create(testUrl);
        }
    }
}