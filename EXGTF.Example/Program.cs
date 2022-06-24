using Newtonsoft.Json.Linq;

namespace ExGTF.Example
{
    using ExGTF.Reader;
    using Newtonsoft.Json;
    using ExGTF_Params = ExGTF.Reader.ParamsHelper;

    internal class Program
    {
        static void Main(string[] args)
        {
            using var sr = new StreamReader("../../../config.json");
            var config = JsonConvert.DeserializeObject<Config>(sr.ReadToEnd());
            using var sr2 = new StreamReader("../../../DictValue.json");
            var @params = ExGTF_Params.GetParams(sr2.ReadToEnd());
            var rd = new ExGTFReader(config.Provider.Template, @params);
            rd.Create(config.Provider.Path, true, "Form8_12SvodyReport");
            var rdMap = new ExGTFReader(config.Map.Template, @params);
            rdMap.Create(config.Provider.Path, true, "Form8_12ShowCaseMap");
            var rdEntity = new ExGTFReader(config.Entity.Template, @params);
            rdEntity.Create(config.Provider.Path, true, "Form8_12ShowCase");
        }
    }
}