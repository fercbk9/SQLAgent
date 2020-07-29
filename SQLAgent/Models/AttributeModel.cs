using System;
using System.Collections.Generic;
using System.Text;

namespace SQLAgent.Models
{
    public class AttributeModel : Attribute
    {
        public AttributeModel() { }
        public bool IsPrimaryKey { get; set; }
        public bool IsUpdateable { get; set; }
        public bool IsComplexProperty { get; set; }
    }
}
