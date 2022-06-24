using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExGTF.Example
{
    internal class DictValue
    {
        public string Name { get; set; }
        public bool IsArray { get; set; } = false;
        public bool IsArrayObjects { get; set; } = false;
        public object Value { get; set; }
    }
}
