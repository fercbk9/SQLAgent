using Dapper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace SQLAgent
{
    /// <summary>
    /// Clase de conexion generica a SQL
    /// </summary>
    public class SQLConnection
    {
        #region Properties

        public SqlConnection SqlConnection { get; set; }
        public SQLSetting SQLSetting { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// 
        /// </summary>
        /// <param name="setting"></param>
        public SQLConnection(SQLSetting setting)
        {
            this.SQLSetting = setting;

            SqlConnection = new SqlConnection()
            {
                ConnectionString = $@"Data source = { SQLSetting.Instance }; Initial Catalog = {SQLSetting.DBName }; User = {SQLSetting.User }; Password = {SQLSetting.Pass} "
            };

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        public SQLConnection(string connectionString)
        {
            SqlConnection = new SqlConnection(connectionString);
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
                this.SqlConnection.Open();
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
                this.SqlConnection.Close();
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
                if(parameters != null)
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
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
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

        public int ExecuteProcedure(string sql,SqlParameterCollection parameters,string msgcolumns ,ref string msg)
        {
            try
            {
                SqlCommand cm = new SqlCommand(sql, this.SqlConnection);
                cm.CommandType = CommandType.StoredProcedure;
                if (this.SqlConnection.State != ConnectionState.Open)
                {
                    this.OpenConnection();
                }
                foreach (SqlParameter item in parameters)
                {
                    cm.Parameters.AddWithValue(item.ParameterName, item.Value);
                }
                var i = cm.ExecuteNonQuery();
                if (msgcolumns != "")
                {
                    msg = cm.Parameters["@" + msgcolumns].Value.ToString();
                }
                return i;
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


        public int Execute(string sql, object param = null)
        {
            try
            {
                if (SqlConnection.State != ConnectionState.Open)
                {
                    OpenConnection();
                }
                return SqlConnection.Execute(sql, param);
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

        public IEnumerable<EntityT> Select<EntityT>(string sql, object param = null)
        {
            try
            {
                if (SqlConnection.State != ConnectionState.Open)
                {
                    OpenConnection();
                }
                return SqlConnection.Query<EntityT>(sql, param);
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
        #endregion
    }

}
