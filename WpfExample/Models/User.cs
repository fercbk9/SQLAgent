using SQLAgent.Attributes;
using SQLAgent.Models;
using SQLAgent.Models.Relations;

namespace WpfExample.Models
{
    public class User : BaseModel
    {

        public const string TableName = "[User]";
        [Base(IsPrimaryKey = true)]
        public string IDUser { get; set; }
        public string CodUser { get; set; }
        public string Description { get; set; }
        public string IDUserGroup { get; set; }
        [Base(IsComplexProperty = true,IsUpdateable = false)]
        public UserGroup UserGroup { get; set; }

        public User()
            :base(TableName)
        {
            
        }

        public static class Properties
        {
            public static string IDUser = "IDUser";
            public static string CodUser = "CodUser";
            public static string Description = "Description";
            public static string IDUserGroup = "IDUserGroup";
        }

        protected override void ImportRelations()
        {
            Relation<User, UserGroup> relation_User = new Relation<User,UserGroup>("User_UserGroup");
            relation_User.Add(new RelationDetail<User, UserGroup>()
            {
                PrimaryField = x => x.IDUserGroup,
                ForeignField = x => x.IDUserGroup
            });
            Relations.Add("UserGroup", relation_User);
            base.ImportRelations();
        }
    }
}
