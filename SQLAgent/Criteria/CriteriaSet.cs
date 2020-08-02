using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLAgent.Criteria
{
    public class CriteriaSet : List<Criteria>
    {
    }

    public class CriteriaSet<EntityT> where EntityT : Models.BaseModel, new()
    {
        private CriteriaSet cs;
        public SQLConnection SQLConnection { get; }
        EntityT model = new EntityT();
        private CriteriaSet JoinCriterias;
        private CriteriaSet WhereCriterias;
        public CriteriaSet<EntityT> Compare(string property, ComparisonOperators comparisonOperators, object value)
        {
            this.WhereCriterias.Add(new Criteria(property,comparisonOperators,value));
            this.cs.Add(new Criteria(typeof(EntityT),property,comparisonOperators,value));
            return this;
        }

        public CriteriaSet<EntityT> Compare<EntityCompare>(string property, ComparisonOperators comparisonOperators, object value)
        {
            this.WhereCriterias.Add(new Criteria(typeof(EntityCompare),property, comparisonOperators, value));
            this.cs.Add(new Criteria(typeof(EntityCompare), property, comparisonOperators, value));
            return this;
        }
        public CriteriaSet<EntityT> And()
        {
            this.cs.Add(new Criteria(LogicalOperators.AND));
            return this;
        }

        public CriteriaSet<EntityT> Or()
        {
            this.cs.Add(new Criteria(LogicalOperators.OR));
            return this;
        }


        /// <summary>
        /// Metodo para hacer un inner join a otra entidad. **Es Obligatorio que esta entidad tenga una propiedad constante con TableName para que se pueda mapear a la tabla**
        /// </summary>
        /// <typeparam name="EntityJoinT"></typeparam>
        /// <param name="entityProperty"></param>
        /// <param name="comparisonOperators"></param>
        /// <param name="joinedEntityProperty"></param>
        /// <returns></returns>
        public CriteriaSet<EntityT> Join<EntityJoinT>(string entityProperty,ComparisonOperators comparisonOperators,string joinedEntityProperty)
        {

            this.cs.Add(new Criteria(typeof(EntityT),entityProperty,typeof(EntityJoinT),joinedEntityProperty,JoinTypes.Inner,comparisonOperators));
            this.JoinCriterias.Add(new Criteria(typeof(EntityT),entityProperty,typeof(EntityJoinT),joinedEntityProperty,JoinTypes.Inner,comparisonOperators));
            return this;
        }

        public CriteriaSet()
        {
            this.cs = new CriteriaSet();
            this.JoinCriterias = new CriteriaSet();
            this.WhereCriterias = new CriteriaSet();
            this.SQLConnection = new SQLAgent.SQLConnection(model.sQLSetting);
        }


        /*public IEnumerable<EntityT> GetEntities()
        {
            return Utilities.Converters.DataTableToList<EntityT>(SQLConnection.Select(CriteriaSetToSql(), GetParametersFromCriteriaSet()));
        }

        /// <summary>
        /// Metodo que devuelve los parametros del criteria para las comparaciones.
        /// </summary>
        /// <returns></returns>
        public SqlParameterCollection GetParametersFromCriteriaSet()
        {
            SqlParameterCollection collection = new SqlCommand().Parameters;
            int countWhere = 0;
            foreach (var item in this.cs.Where(x => x.CriteriaType == CriteriaTypes.Comparison))
            {
                if (item.CriteriaType == CriteriaTypes.Comparison)
                {
                    collection.AddWithValue($"@{item.Property}{countWhere}", item.Value);
                    countWhere++;
                }
                
            }
            return collection;
        }*/

        public IEnumerable<EntityT> GetEntities()
        {
            return SQLConnection.Select<EntityT>(CriteriaSetToSql(), GetParametersFromCriteriaSet());
        }

        /// <summary>
        /// Metodo que devuelve los parametros del criteria para las comparaciones.
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, object> GetParametersFromCriteriaSet()
        {
            var collection = new Dictionary<string, object>();
            int countWhere = 0;
            foreach (var item in this.cs.Where(x => x.CriteriaType == CriteriaTypes.Comparison))
            {
                if (item.CriteriaType == CriteriaTypes.Comparison)
                {
                    collection.Add($"{item.Property}{countWhere}", item.Value);
                    countWhere++;
                }

            }
            return collection;
        }


        /// <summary>
        /// Metodo que devuelve el criteria como consulta de sql.
        /// </summary>
        /// <returns></returns>
        public string CriteriaSetToSql()
        {
            string predicado = $" Select {model.tableName}.* from {model.tableName} ";
            string where = "";
            string join = "";
            string order = "";
            int countWhere = 0;
            foreach (var item in this.cs)
            {
                switch (item.CriteriaType)
                {
                    case CriteriaTypes.Unknown:
                        break;
                    case CriteriaTypes.Comparison:
                        if (countWhere == 0)
                        {
                            predicado += " WHERE ";
                        }
                        predicado += $" {item.ComparisonEntityName}.{item.Property} {GetComparisonCharacter(item.ComparisonOperator)} @{item.Property}{countWhere} ";
                        countWhere++;
                        break;
                    case CriteriaTypes.Logical:
                        switch (item.logicalOperator)
                        {
                            case LogicalOperators.AND:
                                predicado += " AND ";
                                break;
                            case LogicalOperators.OR:
                                predicado += " OR ";
                                break;
                            case LogicalOperators.NOT:
                                predicado += " NOT ";
                                break;
                            case LogicalOperators.OpenParenthesis:
                                predicado += " (";
                                break;
                            case LogicalOperators.CloseParenthesis:
                                predicado += ") ";
                                break;
                            case LogicalOperators.Unknown:
                                break;
                            default:
                                break;
                        }
                        break;
                    case CriteriaTypes.Sort:
                        break;
                    case CriteriaTypes.Join:
                        predicado += $" inner join {item.JoinedEntityTypeName} on {item.OriginalEntityTypeName}.{item.OriginalEntityProperty} {GetComparisonCharacter(item.ComparisonOperator)} {item.JoinedEntityTypeName}.{item.JoinEntityProperty} "; 
                        break;
                    case CriteriaTypes.Include:
                        break;
                    case CriteriaTypes.Distinct:
                        break;
                    case CriteriaTypes.PropertyComparison:
                        break;
                    case CriteriaTypes.RelatedCriteriaSet:
                        break;
                    case CriteriaTypes.PagingCriteriaSet:
                        break;
                    case CriteriaTypes.Subquery:
                        break;
                    case CriteriaTypes.EnableMultiService:
                        break;
                    case CriteriaTypes.VoidCriteria:
                        break;
                    default:
                        break;
                }
            }
            return predicado;
        }

        public string GetComparisonCharacter(ComparisonOperators comparison)
        {
            switch (comparison)
            {
                case ComparisonOperators.Equals:
                    return " = ";
                case ComparisonOperators.NotEquals:
                    return " <> ";
                case ComparisonOperators.BiggerThan:
                    return " > ";
                case ComparisonOperators.LowerThan:
                    return " < ";
                default:
                    return "";
            }
        }
    }
}
