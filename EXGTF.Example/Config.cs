using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExGTF.Example
{
    internal class Config
    {
        public ConfigValue Provider { get; set; }
        public ConfigValue Map { get; set; }
        public ConfigValue Entity { get; set; }
    }

    internal class ConfigValue
    {
        public string Path { get; set; }
        public string Template { get; set; }
    }
}
