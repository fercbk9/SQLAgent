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
        public string IDUserGroup
        {
            get { return base.ID; }
            set
            {
                base.ID = value;
            }
        }
        public string CodUserGroup { get; set; }
        public string Description { get; set; }
        [Base(IsComplexProperty = true,IsUpdateable = true)]
        public User User { get; set; }


        public UserGroup()
            :base(TableName,Properties.IDUser)
        {
        }

        protected override void ImportRelations()
        {
            Relation<UserGroup, User> relation_User = new Relation<UserGroup, User>("User_UserGroup");
            relation_User.Add(new RelationDetail<UserGroup, User>()
            {
                PrimaryField = x => x.IDUserGroup,
                ForeignField = x => x.IDUserGroup
            });
            Relations.Add("User", relation_User);
        }
        public static class Properties
        {
            public static string IDUser = "IDUserGroup";
            public static string CodUser = "CodUserGroup";
            public static string Description = "Description";
        }
    }


}
