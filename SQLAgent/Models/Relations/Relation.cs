using SQLAgent.Interfaces.Relations;
using System;
using System.Collections.Generic;
using System.Text;

namespace SQLAgent.Models.Relations
{
    public class Relation<TPrimary, TForeign> : IRelation
        where TPrimary : BaseModel
        where TForeign : BaseModel
    {
        public string Name { get; set; }
        public Type PrimaryType { get; set; }
        public Type ForeignType { get; set; }
        public IEnumerable<IRelationDetail> Details
        {
            get { return _details; }
            set { }
        }
        private readonly List<RelationDetail<TPrimary, TForeign>> _details = new List<RelationDetail<TPrimary, TForeign>>();


        public Relation(string name)
        {
            Name = name;
            PrimaryType = typeof(TPrimary);
            ForeignType = typeof(TForeign);
            Details = new List<RelationDetail<TPrimary, TForeign>>();
        }

        public IRelation Add(IRelationDetail detail)
        {
            _details.Add((RelationDetail<TPrimary, TForeign>)detail);
            return this;
        }

        public IRelation Remove(IRelationDetail detail)
        {
            _details.Remove((RelationDetail<TPrimary, TForeign>)detail);
            return this;
        }
    }
}
