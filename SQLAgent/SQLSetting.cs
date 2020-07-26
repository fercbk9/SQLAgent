using System.Data.SqlClient;

namespace SQLAgent
{
    public class SQLSetting
    {
        #region Properties

        public string Instance { get; set; }
        public string DBName { get; set; }
        public string User { get; set; }
        public string Pass { get; set; }

        #endregion

        #region Constructor
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="dBName"></param>
        /// <param name="user"></param>
        /// <param name="pass"></param>
        public SQLSetting(string instance, string dBName, string user, string pass)
        {
            Instance = instance;
            DBName = dBName;
            User = user;
            Pass = pass;
        }

        public SQLSetting(string connectionString)
        {
            var settings = new SqlConnectionStringBuilder(connectionString);
            Instance = settings.DataSource;
            DBName = settings.InitialCatalog;
            User = settings.UserID;
            Pass = settings.Password;
        }

        public SQLSetting() { }

        #endregion

    }
}
