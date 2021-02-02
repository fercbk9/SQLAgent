using SQLAgent;
using SQLAgent.Criteria;
using SQLAgent.Models;
using System;
using System.Collections.Generic;
using System.Text;
using WpfExample.Services.Interfaces;

namespace WpfExample.Services
{
    public class BaseService<TModel> : IBaseService<TModel> where TModel : BaseModel,new ()
    {
        #region Properties
        private SQLManager Manager { get; set; }
        private CriteriaSet<TModel> QueryBuilder { get; set; }
        #endregion
        #region Ctor
        public BaseService()
        {
            Manager = new SQLManager();
            QueryBuilder = new CriteriaSet<TModel>();
        }
        #endregion
        public void Delete(TModel model)
        {
            Manager.Delete(model);
        }

        public void Insert(TModel model)
        {
            Manager.Insert(model);
        }

        public IEnumerable<TModel> SelectAll()
        {
            return QueryBuilder.GetEntities();
        }
    }
}
