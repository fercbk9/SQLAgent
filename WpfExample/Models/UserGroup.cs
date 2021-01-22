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
        [Base(IsPrimaryKey = true)]
        public string IDUserGroup { get; set; }
        public string CodUserGroup { get; set; }
        public string Description { get; set; }

        [Base(IsComplexProperty = true,IsUpdateable = false)]
        public List<User> Users { get; set; }


        public UserGroup()
            :base()
        {
            Users = new List<User>();
            ImportRelations();
        }

        public override void ImportRelations()
        {
            Relation<UserGroup, User> relation_User = new Relation<UserGroup, User>("UserGroup_User");
            relation_User.Add(new RelationDetail<UserGroup, User>()
            {
                PrimaryField = x => x.IDUserGroup,
                ForeignField = x => x.IDUserGroup
            });
            Relations.Add("Users", relation_User);
        }
    }


}
