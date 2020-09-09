using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace SQLAgent.DataAccessObject
{
    /// <summary>
    /// Clase Base de acceso a datos con conexión SQL
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BaseDAO<T> : IBaseDAO<T> where T : Models.BaseModel, new ()
    {
        public SQLAgent.SQLConnection SQLConnection { get; }
        public SQLAgent.SQLManager Manager { get; }
        private readonly string TableName;

        /// <summary>
        /// Enumerador de tipos de accesso a sql
        /// </summary>
        private enum SQLTypes
        {
            Insert,
            Update,
            Delete,
            SelectAll,
            GetById
        }

        /// <summary>
        /// 
        /// </summary>
        public BaseDAO( SQLAgent.SQLSetting setting, string tableName)
        {
            SQLConnection = new SQLAgent.SQLConnection(setting);
            this.TableName = tableName;
        }

        public BaseDAO(string tableName)
        {
            TableName = tableName;
            Manager = new SQLManager();
        }

        #region IBaseDAO implementation

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public virtual int Delete(T model)
        {
            return Manager.Execute(this.GetSqlCommandText(SQLTypes.Delete, this.GetPropertiesCustom(), model), this.GetParameterCollection(model));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<T> GetAll()
        {
            return Manager.Select<T>(this.GetSqlCommandText(SQLTypes.SelectAll),null);
        }

        /// <summary>
        /// Devolvemos todos los elementos de una entidad de manera asincrona.
        /// </summary>
        /// <returns></returns>
        public virtual Task<IEnumerable<T>> GetAllAsync()
        {
            return Task.Run(() => {
                return Manager.Select<T>(this.GetSqlCommandText(SQLTypes.SelectAll), null);
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual T GetByID(string id)
        {
            T model = new T();
            SqlParameterCollection sqlParameterCollection = new SqlCommand().Parameters;
            sqlParameterCollection.AddWithValue(model.IDName, id);
            return Utilities.Converters.RowToObject<T>(this.SQLConnection.Select(this.GetSqlCommandText(SQLTypes.GetById,null, model), sqlParameterCollection).Rows[0]);
        }

        /// <summary>
        /// Devuelve Lista de Objetos segun la columna que queremos comprobar. 
        /// </summary>
        /// <param name="column"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public virtual IEnumerable<T> GetByColumn(string column,string result)
        {
            SqlParameterCollection sqlParameterCollection = new SqlCommand().Parameters;
            sqlParameterCollection.AddWithValue(column, result);
            return Utilities.Converters.DataTableToList<T>(this.SQLConnection.Select(GetPropertyStringForGetByID(column), sqlParameterCollection));
        }

        /// <summary>
        /// Devuelve los datos del modelo pasandole un diccionario de parametros para el where.
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public virtual IEnumerable<T> GetByColumns(Dictionary<string, object> parameters)
        {
            SqlParameterCollection sqlParameterCollection = new SqlCommand().Parameters;
            foreach (var item in parameters)
            {
                sqlParameterCollection.AddWithValue(item.Key, item.Value);
            }
            return Utilities.Converters.DataTableToList<T>(this.SQLConnection.Select(GetPropertyStringForGetByParameters(parameters.Select(x => x.Key).ToArray()), sqlParameterCollection));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public virtual int Insert(T model)
        {
            return Manager.Execute(this.GetSqlCommandText(SQLTypes.Insert, this.GetPropertiesCustom(), model), this.GetParameterCollection(model));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public virtual int Update(T model)
        {
            return Manager.Execute(this.GetSqlCommandText(SQLTypes.Update, this.GetPropertiesCustom(), model), this.GetParameterCollection(model));
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Recoge una lista de propiedades del tipo pasado por parametro
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private ICollection<PropertyInfo> GetPropertiesCustom()
        {
            Type objType = typeof(T);
            return objType.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(property => property.CanWrite && property.DeclaringType.Name != "BaseModel").ToList();

        }

        /// <summary>
        /// Recupera una lista de todas las propiedades
        /// </summary>
        /// <returns></returns>
        private IEnumerable<string> GetPropertyNames()
        {
            return this.GetPropertiesCustom().Select(prop => prop.Name);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sqlType"></param>
        /// <param name="properties"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        private string GetSqlCommandText(SQLTypes sqlType, ICollection<PropertyInfo> properties = null, T model = null)
        {
            string sqlCommandText = String.Empty;
            switch (sqlType)
            {
                case SQLTypes.Insert:
                    sqlCommandText = GetPropertyStringForInsert(properties);
                    break;
                case SQLTypes.Update:
                    sqlCommandText = this.GetPropertyStringForUpdate(properties, model.IDName);
                    break;
                case SQLTypes.Delete:
                    sqlCommandText = this.GetPropertyStringForDelete(GetPrimaryKeyProperties(model));
                    break;
                case SQLTypes.SelectAll:
                    sqlCommandText = this.GetPropertyStringForGetAll();
                    break;
                case SQLTypes.GetById:
                    sqlCommandText = this.GetPropertyStringForGetByID(model.IDName);
                    break;
            }

            return sqlCommandText;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="properties"></param>
        /// <returns></returns>
        private string GetPropertyStringForInsert(ICollection<PropertyInfo> properties)
        {
            return "INSERT INTO " + TableName + " (" + String.Join(",", properties.Select(prop => prop.Name)) + ") VALUES (" + String.Join(",", properties.Select(prop => "@" +  prop.Name)) + ") ";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="properties"></param>
        /// <returns></returns>
        private string GetPropertyStringForUpdate(ICollection<PropertyInfo> properties, string columnNameID )
        {
            return  "UPDATE " + TableName + " SET " 
                + String.Join(",", properties.Where(prop => prop.Name != columnNameID).Select(prop => prop.Name + "= @" + prop.Name))
                + " WHERE " + columnNameID + " =@"+ columnNameID;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="properties"></param>
        /// <returns></returns>
        private string GetPropertyStringForDelete( string columnNameID)
        {
            return "DELETE FROM " + TableName + " WHERE " + columnNameID + " = @" + columnNameID; 
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="properties"></param>
        /// <returns></returns>
        private string GetPropertyStringForDelete(List<string> primaryKeyProperties)
        {
            return "DELETE FROM " + TableName + " WHERE " + primaryKeyProperties.Select(x => x + "=@" + x);

        }


        /// <summary>
        /// Devolver Sql para select todo del modelo.
        /// </summary>
        /// <returns></returns>
        public string GetPropertyStringForGetAll()
        {
            return "SELECT * FROM " + TableName;
        }

        /// <summary>
        /// Devolver sql para seleccion por ID de un registro.
        /// </summary>
        /// <param name="columnNameID"></param>
        /// <returns></returns>
        public string GetPropertyStringForGetByID(string columnNameID)
        {
            return "SELECT * FROM " + TableName + " WHERE " + columnNameID + " = @" + columnNameID;
        }

        public string GetPropertyStringForGetByParameters(string[] parameters)
        {
            return "SELECT * FROM " + TableName + " WHERE " + String.Join(" AND ",parameters.Select( x=> x + " = @" + x));
        }

        public string AND()
        {
            return " AND ";
        }

        public string OR()
        {
            return " OR ";
        }

        public string Compare(string column)
        {
            return $" {this.TableName}.{column} = @{column} ";
        }

        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private SqlParameterCollection GetParameterCollection(T model)
        {
            SqlParameterCollection sqlParameterCollection = new SqlCommand().Parameters;
            this.GetPropertiesCustom().ToList().ForEach(prop => sqlParameterCollection.AddWithValue(prop.Name, prop.GetValue(model) ?? DBNull.Value));
            return sqlParameterCollection;
        }


        private List<string> GetPrimaryKeyProperties(T model)
        {
            var list = new List<string>();
            model.GetType().GetProperties().ToList().ForEach(x => 
                {
                    if(x.GetCustomAttributes<Attributes.BaseAttribute>().Count() > 0)
                    if (x.GetCustomAttribute<Attributes.BaseAttribute>().IsPrimaryKey)
                    {
                        list.Add(x.Name);
                    }
                }
            );
            return list;
        }
        #endregion

    }
}
