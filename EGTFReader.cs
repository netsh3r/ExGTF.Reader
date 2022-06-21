using System;
using System.Collections.Generic;
using System.Text;

namespace EGTF.Reader
{
    public class EGTFReader
    {
        private readonly string url;
        private readonly Dictionary<string, string> dictValue;
        public EGTFReader(string url, Dictionary<string, string> dictValue)
        {
            this.url = url;
            this.dictValue = dictValue;
        }

        public void Create(string createUrl)
        {

        }
    }
}
