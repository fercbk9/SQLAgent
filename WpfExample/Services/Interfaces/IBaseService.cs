using SQLAgent;
using SQLAgent.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace WpfExample.Services.Interfaces
{
    public interface IBaseService<TModel> where TModel : BaseModel
    {
        IEnumerable<TModel> SelectAll();
        void Insert(TModel model);
        void Delete(TModel model);
    }
}
