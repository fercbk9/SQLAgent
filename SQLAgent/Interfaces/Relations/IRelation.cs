using System;
using System.Collections.Generic;
using System.Text;

namespace SQLAgent.Interfaces.Relations
{
    public interface IRelation
    {
        #region Properties
        string Name
        {
            get;
            set;
        }

        Type PrimaryType
        {
            get;
            set;
        }

        Type ForeignType
        {
            get;
            set;
        }

        public IEnumerable<IRelationDetail> Details
        {
            get; set;
        }
        #endregion
    }
}
