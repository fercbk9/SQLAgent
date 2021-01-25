using System;
using System.Collections.Generic;
using System.Text;

namespace SQLAgent.Interfaces
{
    public interface IBaseModel
    {
        string tableName { get; set; }
        string ID { get; set; }
        string IDName { get; }
    }
}
