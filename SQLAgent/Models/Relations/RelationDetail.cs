using SQLAgent.Interfaces.Relations;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace SQLAgent.Models.Relations
{
    public class RelationDetail<TPrimary, TForeign> : IRelationDetail
        where TPrimary : BaseModel
        where TForeign : BaseModel
    {
        public string PrimaryFieldName { get; set; }
        public string ForeignFieldName { get; set; }

        private Expression<Func<TPrimary, IComparable>> _primaryField;
        private Expression<Func<TForeign, IComparable>> _foreignField;

        public Func<TPrimary, IComparable> PrimaryFunction { get; set; }
        public Func<TForeign, IComparable> ForeignFunction { get; set; }

        public Expression<Func<TPrimary, IComparable>> PrimaryField
        {
            get { return _primaryField; }
            set
            {
                _primaryField = value;
                PrimaryFunction = _primaryField.Compile();
                PrimaryFieldName = GetMember(_primaryField).Name;
            }
        }

        public Expression<Func<TForeign, IComparable>> ForeignField
        {
            get { return _foreignField; }
            set
            {
                _foreignField = value;
                ForeignFunction = _foreignField.Compile();
                ForeignFieldName = GetMember(_foreignField).Name;
            }
        }

        private MemberInfo GetMember<TInput, TReturn>(Expression<Func<TInput, TReturn>> property)
        {
            MemberInfo member;
            if (property.Body is UnaryExpression unary)
            {
                member = ((MemberExpression)unary.Operand).Member;
            }
            else
            {
                member = ((MemberExpression)property.Body).Member;
            }
            return member;
        }
    }
}
