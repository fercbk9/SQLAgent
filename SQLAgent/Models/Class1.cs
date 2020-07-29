using System;
using System.Collections.Generic;
using System.Text;

namespace SQLAgent.Models
{
    class Class1 : BaseModel
    {
        [AttributeModel(IsPrimaryKey = true)]
        public string IDUser
        {
            get;
            set;
        }
    }
}
