using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLAgent.Criteria
{
    public class Criteria
    {
        #region Properties
        public string ComparisonEntityName { get; set; }
        public string Property { get; set; }
        public string Property2 { get; set;}
        public object Value { get; set; }
        public ComparisonOperators ComparisonOperator { get; set; }
        public LogicalOperators logicalOperator { get; set; }
        public CriteriaTypes CriteriaType { get; set; }

        //Join Properties
        public string OriginalEntityTypeName { get; set; }
        public string JoinedEntityTypeName { get; set; }
        public string OriginalEntityProperty { get; set; }
        public string JoinEntityProperty { get; set; }
        public JoinTypes JoinType { get; set; }
        #endregion

        #region Constructors

        #endregion
        public Criteria()
        {

        }
        public Criteria(LogicalOperators logicalOperator)
        {
            this.logicalOperator = logicalOperator;
            this.CriteriaType = CriteriaTypes.Logical;
        }
        public Criteria(string property, ComparisonOperators comparisonOperators, object value)
        {
            this.Property = property;
            this.ComparisonOperator = comparisonOperators;
            this.Value = value;
            this.CriteriaType = CriteriaTypes.Comparison;
        }
        public Criteria(Type EntityComparison,string property, ComparisonOperators comparisonOperators, object value)
        {
            this.ComparisonEntityName = EntityComparison.GetField("TableName").GetRawConstantValue().ToString();
            this.Property = property;
            this.ComparisonOperator = comparisonOperators;
            this.Value = value;
            this.CriteriaType = CriteriaTypes.Comparison;
        }
        public Criteria(Type originalEntityType, string originalEntityProperty, Type joinedEntityType, string joinEntityProperty,JoinTypes joinType,ComparisonOperators comparisonOperators)
        {
            this.OriginalEntityTypeName = originalEntityType.GetField("TableName").GetRawConstantValue().ToString();
            this.OriginalEntityProperty = originalEntityProperty;
            this.JoinedEntityTypeName = joinedEntityType.GetField("TableName").GetRawConstantValue().ToString();
            this.JoinEntityProperty = joinEntityProperty;
            this.CriteriaType = CriteriaTypes.Join;
            this.JoinType = joinType;
            this.ComparisonOperator = comparisonOperators;
        }
    }
    public enum CriteriaTypes
    {
       Unknown,
       Comparison,
       Logical,
       Sort,
       Join,
       Include,
       Distinct,
       PropertyComparison,
       RelatedCriteriaSet,
       PagingCriteriaSet,
       Subquery,
       EnableMultiService,
       VoidCriteria,
    }
    public enum JoinTypes
    {
        Inner,
        Left,
        Right
    }
}
