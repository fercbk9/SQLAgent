using Dapper;
using SQLAgent.Attributes;
using SQLAgent.Interfaces.Relations;
using SQLAgent.Models;
using SQLAgent.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SQLAgent
{
    public class SQLManager
    {
        #region Properties
        private SqlConnection SqlConnection { get; set; }
        public SqlConnection QueryManager { get; set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor with connection string
        /// </summary>
        /// <param name="connectionString"></param>
        public SQLManager(string connectionString)
        {
            try
            {
                SqlConnection = new SqlConnection(connectionString);
                QueryManager = SqlConnection;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        /// <summary>
        /// Constructor with SQLSetting Object
        /// </summary>
        /// <param name="sqlSetting"></param>
        public SQLManager(SQLSetting sqlSetting)
        {
            try
            {
                SqlConnection = new SqlConnection()
                {
                    ConnectionString = $@"Data source = { sqlSetting.Instance }; Initial Catalog = {sqlSetting.DBName }; User = {sqlSetting.User }; Password = {sqlSetting.Pass} "
                };
                QueryManager = SqlConnection;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        /// <summary>
        /// Constructor with SQLSetting Object in SQLContext
        /// </summary>
        /// <param name="sqlSetting"></param>
        public SQLManager()
        {
            try
            {
                SqlConnection = new SqlConnection()
                {
                    ConnectionString = $@"Data source = { Context.SQLContext.sqlSetting.Instance }; Initial Catalog = {Context.SQLContext.sqlSetting.DBName }; User = {Context.SQLContext.sqlSetting.User }; Password = {Context.SQLContext.sqlSetting.Pass} "
                };
                QueryManager = SqlConnection;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        #endregion

        #region Standar Methods

        /// <summary>
        /// Abre la conexion a SQL
        /// </summary>
        public void OpenConnection()
        {
            try
            {
                SqlConnection.Open();
            }
            catch (Exception ex) { throw ex; }

        }

        /// <summary>
        /// Cierra conexión SQL.
        /// </summary>
        public void CloseConnection()
        {
            try
            {
                SqlConnection.Close();
            }
            catch (Exception ex) { throw ex; }

        }

        #endregion

        #region Custom Methods
        /// <summary>
        /// Ejecuta de manera generica un select de sql con parametros o sin ellos si los pasamos a null.
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="consql"></param>
        /// <param name="parameters"></param>
        /// <returns>DataTable</returns>
        public DataTable Select(string sql, SqlParameterCollection parameters)
        {
            try
            {
                SqlDataAdapter da = new SqlDataAdapter(sql, this.SqlConnection);
                if (this.SqlConnection.State != ConnectionState.Open)
                {
                    this.OpenConnection();
                }
                DataTable dt = new DataTable();
                if (parameters != null)
                {
                    foreach (SqlParameter item in parameters)
                    {
                        da.SelectCommand.Parameters.AddWithValue(item.ParameterName, item.Value);
                    }
                }
                da.Fill(dt);
                this.CloseConnection();
                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Metodo que inserta,updatea y borra los registros en bbdd.
        /// </summary>
        /// <param name="sql">Cadena de texto con la sentencia SQL.</param>
        /// <param name="parameters">Parametros de la sentencia SQL.</param>
        /// <returns></returns>
        public int DataManipulation(string sql, SqlParameterCollection parameters)
        {
            try
            {
                SqlCommand cm = new SqlCommand(sql, this.SqlConnection);
                if (this.SqlConnection.State != ConnectionState.Open)
                {
                    this.OpenConnection();
                }
                foreach (SqlParameter item in parameters)
                {
                    cm.Parameters.AddWithValue(item.ParameterName, item.Value);
                }
                return cm.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.CloseConnection();
            }
        }

        /// <summary>
        /// Metodo para ejecturar insert,update, delete con Dapper.
        /// </summary>
        /// <param name="sql">Sentencia sql con paramatros si los tiene.</param>
        /// <param name="param">Parametros para ejecutar la consulta. Si los tiene.</param>
        /// <returns></returns>
        public int Execute(string sql, object param = null, CommandType? typeCommand = null)
        {
            try
            {
                if (SqlConnection.State != ConnectionState.Open)
                {
                    OpenConnection();
                }
                return SqlConnection.Execute(sql, param,commandType: typeCommand);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                CloseConnection();
            }
        }

        /// <summary>
        /// Metedo para buscar segun modelo. Con Dapper
        /// </summary>
        /// <typeparam name="EntityT">Clase para mapear el IEnumerable.</typeparam>
        /// <param name="sql">Sentencia sql con paramatros si los tiene.</param>
        /// <param name="param">Parametros para ejecutar la consulta. Si los tiene.</param>
        /// <returns></returns>
        public IEnumerable<EntityT> Select<EntityT>(string sql, object param = null,CommandType? typeCommand = null)
        {
            try
            {
                if (SqlConnection.State != ConnectionState.Open)
                {
                    OpenConnection();
                }
                return SqlConnection.Query<EntityT>(sql, param,commandType: typeCommand);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                CloseConnection();
            }
        }
        /// <summary>
        /// Método que busca según la clase que se pase por reflection. Si tiene atributos complex property los busca tambien y asi sucesivamente.
        /// </summary>
        /// <typeparam name="EntityT"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="typeCommand"></param>
        /// <returns></returns>
        public IEnumerable<EntityT> SelectDeep<EntityT>(string sql,object param = null,CommandType? typeCommand = null) where EntityT : BaseModel, new()
        {
            try
            {
                if (SqlConnection.State != ConnectionState.Open)
                {
                    OpenConnection();
                }
                EntityT t = new EntityT();
                var list = GetComplexProperties<EntityT>();
                var result = SqlConnection.Query<EntityT>(sql, param, commandType: typeCommand);
                    if (t.Relations.Count > 0 && list.Count > 0)
                    {
                        foreach (var item in result)
                        {
                            foreach (var item2 in list)
                            {
                                IRelation relation = t.Relations[item2.Key];
                                PropertyInfo property = t.GetType().GetProperty(item2.Key);
                                Type type = relation.ForeignType;
                                var aux = Activator.CreateInstance(type) as BaseModel;
                                string sql2 = $"select * from {aux.tableName} {GetWhereWithRelations(relation)}";
                                Dictionary<string, object> parameters = GetParameters(relation, item);
                                var ie = SelectDeepV2(sql2,type,parameters);
                                if (typeof(IEnumerable).IsAssignableFrom(property.PropertyType))
                                {
                                    var Converted = Converters.ConvertCustom(ie, type);
                                    property.SetValue(item, Converted);
                                }
                                else
                                {
                                    property.SetValue(item, ie.FirstOrDefault());
                                }
                        }
                        }
                    }
                    return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// Método que devuelve una colleccion de objetos sin tipar para poder usar una busqueda progresiva dentro de si mismo. Que se pueda llamar n veces a si mismo.
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="type"></param>
        /// <param name="param"></param>
        /// <param name="typeCommand"></param>
        /// <returns></returns>
        public IEnumerable<object> SelectDeepV2(string sql,Type type, object param = null, CommandType? typeCommand = null)
        {
            try
            {
                if (SqlConnection.State != ConnectionState.Open)
                {
                    OpenConnection();
                }
                
                var list = GetComplexProperties(type);
                var aux = Activator.CreateInstance(type) as BaseModel;
                var result = SqlConnection.QueryMultiple(sql, param, commandType: typeCommand).Read(type);
                if (aux.Relations.Count > 0 && list.Count > 0)
                {
                    foreach (var item in result)
                    {
                        foreach (var item2 in list)
                        {
                            IRelation relation = aux.Relations[item2.Key];
                            PropertyInfo property = aux.GetType().GetProperty(item2.Key);
                            Type type2 = relation.ForeignType;
                            var aux2 = Activator.CreateInstance(type2) as BaseModel;
                            string sql2 = $"select * from {aux2.tableName} {GetWhereWithRelations(relation)}";
                            Dictionary<string, object> parameters = GetParameters(relation, item);
                            var ie = SelectDeepV2(sql2,type2,parameters);
                            if (typeof(IEnumerable).IsAssignableFrom(property.PropertyType))
                            {
                                var Converted = Converters.ConvertCustom(ie, type2);
                                property.SetValue(item, Converted);
                            }
                            else
                            {
                                property.SetValue(item, ie.FirstOrDefault());
                            }
                        }
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
        #region Private Methods
        /// <summary>
        /// Get the properties with IsComplexProperty = true in the model class.
        /// </summary>
        /// <typeparam name="EntityT">Class for get their properties.</typeparam>
        /// <returns>Dictionary<string, Type></returns>
        private Dictionary<string, Type> GetComplexProperties<EntityT>()
        {
            var list = new Dictionary<string, Type>();
            typeof(EntityT).GetProperties().ToList().ForEach(x =>
            {
                if (x.GetCustomAttributes<BaseAttribute>().Count() > 0)
                    if (x.GetCustomAttribute<BaseAttribute>().IsComplexProperty)
                    {
                        list.Add(x.Name, x.GetType());
                    }
            }
            );
            return list;
        }

        private Dictionary<string, Type> GetComplexProperties(Type type)
        {
            var list = new Dictionary<string, Type>();
            type.GetProperties().ToList().ForEach(x =>
            {
                if (x.GetCustomAttributes<BaseAttribute>().Count() > 0)
                    if (x.GetCustomAttribute<BaseAttribute>().IsComplexProperty)
                    {
                        list.Add(x.Name, x.GetType());
                    }
            }
            );
            return list;
        }

        private string GetWhereWithRelations(IRelation relation)
        {
            string where = "";
            if (relation.Details.Count() > 0)
            {
                where = " WHERE ";
                foreach (var item in relation.Details)
                {
                    where += $" {item.ForeignFieldName} = @{item.ForeignFieldName}";
                    where += $" AND";
                }
                where = where.Substring(0, where.Length - 3);
            }
            return where;
        }

        private Dictionary<string, object> GetParameters<EntityT>(IRelation relation, EntityT entity) where EntityT : BaseModel, new()
        {
            Dictionary<string, object> parameters = null;
            if (relation.Details.Count() > 0)
            {
                parameters = new Dictionary<string, object>();
                foreach (var relationDetail in relation.Details)
                {
                    var property = entity.GetType().GetProperty(relationDetail.ForeignFieldName);
                    object value = property.GetValue(entity);
                    parameters.Add(relationDetail.ForeignFieldName, value);
                }
            }
            return parameters;
        }

        private Dictionary<string, object> GetParameters(IRelation relation, object entity)
        {
            Dictionary<string, object> parameters = null;
            if (relation.Details.Count() > 0)
            {
                parameters = new Dictionary<string, object>();
                foreach (var relationDetail in relation.Details)
                {
                    var property = entity.GetType().GetProperty(relationDetail.ForeignFieldName);
                    object value = property.GetValue(entity);
                    parameters.Add(relationDetail.ForeignFieldName, value);
                }
            }
            return parameters;
        }
        #endregion
    }
}
