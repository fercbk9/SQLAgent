using Dapper;
using SQLAgent.Attributes;
using SQLAgent.DataAccessObject;
using SQLAgent.Interfaces;
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
        public int DataManipulation(string sql, Dictionary<string,object> parameters)
        {
            try
            {
                SqlCommand cm = new SqlCommand(sql, this.SqlConnection);
                if (this.SqlConnection.State != ConnectionState.Open)
                {
                    this.OpenConnection();
                }
                foreach (var item in parameters)
                {
                    cm.Parameters.AddWithValue(item.Key, item.Value);
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
        /// <param name="typeCommand">Tipo de comando, si es procedure o query.</param>
        /// <param name="transaction">objeto de transaccion</param>
        /// <param name="openConnection">Si se requiere usar el open connection de dentro o no.</param>
        /// <returns></returns>
        public int Execute(string sql, object param = null, CommandType? typeCommand = null, IDbTransaction transaction = null, bool openConnection = true)
        {
            try
            {
                if (openConnection)
                {
                    if (SqlConnection.State != ConnectionState.Open)
                    {
                        OpenConnection();
                    }
                }
                return SqlConnection.Execute(sql, param, commandType: typeCommand, transaction: transaction);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (openConnection)
                {
                    CloseConnection();
                }
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
                            var foreingEntity = Activator.CreateInstance(type) as BaseModel;
                            string sql2 = $"select * from {foreingEntity.tableName} {GetWhereWithRelations(relation, foreingEntity.tableName)}";
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
            finally
            {
                CloseConnection();
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
                            var foreingEntity = Activator.CreateInstance(type2) as BaseModel;
                            string sql2 = $"select {foreingEntity.tableName}.* from {foreingEntity.tableName} {GetWhereWithRelations(relation, foreingEntity.tableName)}";
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
            finally
            {
                CloseConnection();
            }
        }
        /// <summary>
        /// Metodo para Insert simple, solo de entidad.
        /// </summary>
        /// <typeparam name="EntityT">Objeto que hereda de BaseModel.</typeparam>
        /// <param name="entity">objeto a insertar que hereda de BaseModel.</param>
        /// <returns></returns>
        public int Insert<EntityT>(EntityT entity) where EntityT : BaseModel, new()
        {
            int result;
            try
            {
                var auxDAO = new BaseDAO<EntityT>(entity.tableName);
                result = auxDAO.Insert(entity);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }
        /// <summary>
        /// Metodo para Insert con entidades hijas complejas que se pueden modificar en BBDD.
        /// </summary>
        /// <typeparam name="EntityT"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int InsertDeep<EntityT>(EntityT entity) where EntityT : BaseModel, new()
        {
            int result = 0;
            try
            {
                if (SqlConnection.State != ConnectionState.Open)
                {
                    OpenConnection();
                }
                using var transaction = QueryManager.BeginTransaction();
                try
                {
                    var complexPropsList = GetComplexPropertiesToUpdate(typeof(EntityT));
                    var auxDAO = new BaseDAO<EntityT>(entity.tableName,this);
                    result = auxDAO.Insert(entity,null,transaction: transaction,false);
                    if (entity.Relations.Count > 0 && complexPropsList.Count > 0)
                    {
                        foreach (var complexProp in complexPropsList)
                        {
                            PropertyInfo property = entity.GetType().GetProperty(complexProp.Key);
                            var instance = property.GetValue(entity);
                            if (typeof(IEnumerable).IsAssignableFrom(property.PropertyType))
                            {
                                //Insertamos lista de objetos hijos para el type que sea.
                                var convertedObject = instance as IEnumerable<IBaseModel>;
                                foreach (var item in convertedObject)
                                {
                                    var aux2DAO = new BaseDAO<BaseModel>(item.tableName, this);
                                    result += aux2DAO.Insert(item,null,transaction,false);
                                }
                            }
                            else
                            {
                                //Insertamos objeto simple.
                                var aux2DAO = new BaseDAO<BaseModel>((instance as IBaseModel)?.tableName, this);
                                result += aux2DAO.Insert(instance,null,transaction: transaction,false);
                            }
                        }
                    }
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                CloseConnection();
            }
            return result;
        }
        /// <summary>
        /// Metodo para Update simple, solo de entidad.
        /// </summary>
        /// <typeparam name="EntityT">Objeto que hereda de BaseModel.</typeparam>
        /// <param name="entity">objeto a actualizar que hereda de BaseModel.</param>
        /// <returns></returns>
        public int Update<EntityT>(EntityT entity) where EntityT : BaseModel, new()
        {
            int result;
            try
            {
                var auxDAO = new BaseDAO<EntityT>(entity.tableName);
                result = auxDAO.Update(entity);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }
        /// <summary>
        /// Metodo para Update con entidades hijas complejas que se pueden modificar en BBDD.
        /// </summary>
        /// <typeparam name="EntityT"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int UpdateDeep<EntityT>(EntityT entity) where EntityT : BaseModel, new()
        {
            int result;
            try
            {
                if (SqlConnection.State != ConnectionState.Open)
                {
                    OpenConnection();
                }
                using var transaction = QueryManager.BeginTransaction();
                try
                {
                    var complexPropsList = GetComplexPropertiesToUpdate(typeof(EntityT));
                    var auxDAO = new BaseDAO<EntityT>(entity.tableName, this);
                    result = auxDAO.Update(entity, null,transaction, false);
                    if (entity.Relations.Count > 0 && complexPropsList.Count > 0)
                    {
                        foreach (var complexProp in complexPropsList)
                        {
                            PropertyInfo property = entity.GetType().GetProperty(complexProp.Key);
                            var instance = property.GetValue(entity);
                            if (typeof(IEnumerable).IsAssignableFrom(property.PropertyType))
                            {
                                //Actualizamos lista de objetos hijos para el type que sea.
                                var convertedObject = instance as IEnumerable<IBaseModel>;
                                foreach (var item in convertedObject)
                                {
                                    var aux2DAO = new BaseDAO<BaseModel>(item.tableName, this);
                                    result += aux2DAO.Update(item, null, transaction, false);
                                }
                            }
                            else
                            {
                                //Updateamos objeto simple.
                                var iBaseModel = (instance as IBaseModel);
                                if (iBaseModel != null)
                                {
                                    var aux2DAO = new BaseDAO<BaseModel>(iBaseModel.tableName, this);
                                    result += aux2DAO.Update(iBaseModel, null, transaction, false);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
                transaction.Commit();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                CloseConnection();
            }
            return result;      
        }
        /// <summary>
        /// Metodo para Update simple, solo de entidad.
        /// </summary>
        /// <typeparam name="EntityT">Objeto que hereda de BaseModel.</typeparam>
        /// <param name="entity">objeto a actualizar que hereda de BaseModel.</param>
        /// <returns></returns>
        public int Delete<EntityT>(EntityT entity) where EntityT : BaseModel, new()
        {
            int result;
            try
            {
                var auxDAO = new BaseDAO<EntityT>(entity.tableName);
                result = auxDAO.Delete(entity);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }
        /// <summary>
        /// Metodo para Delete con entidades hijas complejas que se pueden modificar en BBDD.
        /// </summary>
        /// <typeparam name="EntityT"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int DeleteDeep<EntityT>(EntityT entity) where EntityT : BaseModel, new()
        {
            int result;
            try
            {
                if (SqlConnection.State != ConnectionState.Open)
                {
                    OpenConnection();
                }
                using var transaction = QueryManager.BeginTransaction();
                try
                {
                    var complexPropsList = GetComplexPropertiesToUpdate(typeof(EntityT));
                    var auxDAO = new BaseDAO<EntityT>(entity.tableName, this);
                    result = auxDAO.Delete(entity, null, transaction, false);
                    if (entity.Relations.Count > 0 && complexPropsList.Count > 0)
                    {
                        foreach (var complexProp in complexPropsList)
                        {
                            PropertyInfo property = entity.GetType().GetProperty(complexProp.Key);
                            var instance = property.GetValue(entity);
                            if (typeof(IEnumerable).IsAssignableFrom(property.PropertyType))
                            {
                                //Borranos lista de objetos hijos para el type que sea.
                                var convertedObject = instance as IEnumerable<IBaseModel>;
                                foreach (var item in convertedObject)
                                {
                                    var aux2DAO = new BaseDAO<BaseModel>(item.tableName, this);
                                    result += aux2DAO.Delete(item, null, transaction, false);
                                }
                            }
                            else
                            {
                                //Borramos objeto simple.
                                var iBaseModel = (instance as IBaseModel);
                                if (iBaseModel != null)
                                {
                                    var aux2DAO = new BaseDAO<BaseModel>(iBaseModel.tableName, this);
                                    result += aux2DAO.Delete(iBaseModel, null, transaction, false);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
                transaction.Commit();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                CloseConnection();
            }
            return result;
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
        /// <summary>
        /// Devolvemos todas las propiedades complejas. Se usara para los selects.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
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
        /// <summary>
        /// Devuelve las propiedas complejas que heredan de BaseModel y que además se pueden tocar en base de datos.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private Dictionary<string, Type> GetComplexPropertiesToUpdate(Type type)
        {
            var list = new Dictionary<string, Type>();
            type.GetProperties().ToList().ForEach(x =>
            {
                if (x.GetCustomAttributes<BaseAttribute>().Count() > 0)
                    if (x.GetCustomAttribute<BaseAttribute>().IsComplexProperty && x.GetCustomAttribute<BaseAttribute>().IsUpdateable)
                    {
                        list.Add(x.Name, x.GetType());
                    }
            }
            );
            return list;
        }
        /// <summary>
        /// Método que devuelve una cadena de texto con la sintaxis de Where para añadir al select.
        /// </summary>
        /// <param name="relation"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        private string GetWhereWithRelations(IRelation relation,string tableName)
        {
            string where = "";
            if (relation.Details.Count() > 0)
            {
                where = " WHERE ";
                foreach (var item in relation.Details)
                {
                    where += $" {tableName}.{item.ForeignFieldName} = @{item.ForeignFieldName}";
                    where += $" AND";
                }
                //where.Substring(0, where.Length - 3);
                where = where[0..^3];
            }
            return where;
        }
        /// <summary>
        /// Metodo que devuelve los parametros introducir segun la relacion que tienen. Se usa para el select deep.
        /// </summary>
        /// <typeparam name="EntityT"></typeparam>
        /// <param name="relation"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
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
        /// <summary>
        /// Metodo que devuelve los parametros introducir segun la relacion que tienen. Se usa para el select deep V2.
        /// </summary>
        /// <param name="relation"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
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
