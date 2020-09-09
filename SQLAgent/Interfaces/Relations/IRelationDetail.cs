using System;
using System.Collections.Generic;
using System.Text;

namespace SQLAgent.Interfaces.Relations
{
    public interface IRelationDetail
    {
        #region Properties
        string PrimaryFieldName { get; set; }
        string ForeignFieldName { get; set; }
        #endregion
    }
}
