using SQLAgent.Attributes;
using SQLAgent.Models;

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
    }
}
