using System;
using System.Collections.Generic;
using System.Text;

namespace SQLAgent.Attributes
{
    public class BaseAttribute : Attribute
    {
        public BaseAttribute() { }
        public bool IsPrimaryKey { get; set; }
        public bool IsUpdateable { get; set; }
        public bool IsComplexProperty { get; set; }
    }
}
