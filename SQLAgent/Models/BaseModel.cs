using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


namespace SQLAgent.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class BaseModel
    {
        public SQLSetting sQLSetting;
        public string tableName;

        public string ID { get; set; }
        public string IDName { get; }

        public BaseModel() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idName"></param>
        public BaseModel(string idName)
        {
            this.IDName = idName;
        }

        public BaseModel(SQLSetting sQLSetting, string tableName,string idName)
        {
            this.sQLSetting = sQLSetting;
            this.tableName = tableName;
            this.IDName = idName;
        }

        /// <summary>
        /// Devuelve las propiedades del objeto.
        /// </summary>
        /// <returns></returns>
        public virtual string[] GetProperties()
        {
            return this.GetType().GetProperties(BindingFlags.Public).Select(x => x.Name).ToArray();
        } 
    }
}
