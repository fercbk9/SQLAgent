using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace SQLAgent.Utilities
{
    public static class Converters
    {
        ///METODOS ADICIONALES DE CONVERSION DE DATATABLES

        private static readonly IDictionary<Type, ICollection<PropertyInfo>> _Properties = new Dictionary<Type, ICollection<PropertyInfo>>();
        /// <summary>
        /// Converts a DataTable to a list with generic objects
        /// </summary>
        /// <typeparam name="T">Generic object</typeparam>
        /// <param name="table">DataTable</param>
        /// <returns>List with generic objects</returns>
        public static IEnumerable<T> DataTableToList<T>(this DataTable table) where T : class, new()
        {
            try
            {
                var objType = typeof(T);
                ICollection<PropertyInfo> properties;

                lock (_Properties)
                {
                    _Properties.Clear();
                    if (!_Properties.TryGetValue(objType, out properties))
                    {
                        properties = objType.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(property => property.CanWrite).ToList();
                        _Properties.Add(objType, properties);
                    }
                }

                var list = new List<T>(table.Rows.Count);

                foreach (var row in table.AsEnumerable())
                {
                    var obj = new T();

                    foreach (var prop in properties)
                    {
                        try
                        {
                            var propType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                            var safeValue = row.IsNull(prop.Name) ? null : Convert.ChangeType(row[prop.Name], propType);

                            prop.SetValue(obj, safeValue, null);
                        }
                        catch
                        {
                            // ignored
                        }
                    }

                    list.Add(obj);
                }

                return list;
            }
            catch
            {
                return Enumerable.Empty<T>();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <returns></returns>
        public static T RowToObject<T>(this DataRow row) where T : class, new()
        {
            try
            {
                var objType = typeof(T);
                ICollection<PropertyInfo> properties;

                //Bloqueo la libreria de propiedades y las obtengo del objeto T
                lock (_Properties)
                {
                    _Properties.Clear();
                    if (!_Properties.TryGetValue(objType, out properties))
                    {
                        properties = objType.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(property => property.CanWrite).ToList();
                        _Properties.Add(objType, properties);
                    }
                }

                var obj = new T();
                foreach (var prop in properties)
                {
                    try
                    {
                        var propType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                        var safeValue = row.IsNull(prop.Name) ? null : Convert.ChangeType(row[prop.Name], propType);

                        prop.SetValue(obj, safeValue, null);

                    }
                    catch (Exception)
                    {
                        //ignore
                       // throw new Exception($"Error al insertar la propiedad {prop.Name} con valor en la fila de { row[prop.Name] } || Exception -> {ex.Message}");
                    }
                }
                return obj;

            }
            catch (Exception ex)
            {
                throw new Exception($"Error al covertir la fila a el objeto { typeof(T).ToString() } || Exception -> {ex.Message}");

            }


        }

        public static int DateTimeToSeconds(DateTime dateTime)
        {
            return (dateTime.Hour * 3600) + (dateTime.Minute * 60) + dateTime.Second;
        }

        /// <summary>
        /// Metodo para castear IEnumerable de object al tipo sin usar reflection.
        /// </summary>
        /// <param name="list">Lista a convertir.</param>
        /// <param name="type">Tipo al que se desea convertir la lista.</param>
        /// <returns></returns>
        public static object ConvertCustom(IEnumerable<object> list, Type type)
        {
            var GenericCastMethod = typeof(Enumerable)
                                .GetMethod("Cast", BindingFlags.Public | BindingFlags.Static);
            var SpecificCastMethod = GenericCastMethod.MakeGenericMethod(type);
            var IEnumerableAux = SpecificCastMethod.Invoke(null, new object[] { list });

            var GenericToListMethod = typeof(Enumerable)
              .GetMethod("ToList", BindingFlags.Public | BindingFlags.Static);
            var SpecificToListMethod = GenericToListMethod.MakeGenericMethod(type);
            return SpecificToListMethod.Invoke(null, new object[] { IEnumerableAux });
        }
    }
}
