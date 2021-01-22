using SQLAgent.Interfaces.Relations;
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

        #region Properties
        public SQLSetting sQLSetting;
        public string tableName;
        public IDictionary<string, IRelation> Relations = new Dictionary<string, IRelation>();
        protected virtual void ImportRelations() { }
        public string ID { get; set; }
        public string IDName { get; }
        public int State { get; set; }
        #endregion

        #region Constructor
        public BaseModel() { ImportRelations(); }

        public BaseModel(string tableName, string idName)
        {
            this.sQLSetting = Context.SQLContext.sqlSetting;
            this.tableName = tableName;
            this.IDName = idName;
            ImportRelations();
        }

        public BaseModel(string tableName)
        {
            this.sQLSetting = Context.SQLContext.sqlSetting;
            this.tableName = tableName;
            ImportRelations();
        }
        #endregion

        #region Methods
        /// <summary>
        /// Devuelve las propiedades del objeto.
        /// </summary>
        /// <returns></returns>
        public virtual string[] GetProperties()
        {
            return this.GetType().GetProperties(BindingFlags.Public).Select(x => x.Name).ToArray();
        }
        #endregion

    }
}
