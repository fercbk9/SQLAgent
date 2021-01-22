using SQLAgent.Interfaces;
using SQLAgent.Attributes;
using SQLAgent.Models;
using SQLAgent.Models.Relations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlTypes;
using System.Text;
using SQLAgent.Interfaces.Relations;

namespace WpfExample.Models
{
    public class UserGroup : BaseModel
    {
        public const string TableName = "[UserGroup]";
        [Base(IsPrimaryKey = true)]
        public string IDUserGroup { get; set; }
        public string CodUserGroup { get; set; }
        public string Description { get; set; }


        public UserGroup()
            :base(TableName)
        {
        }
    }


}
