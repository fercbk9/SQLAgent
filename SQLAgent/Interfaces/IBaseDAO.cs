using System.Collections.Generic;
using System.Data;

namespace SQLAgent.Interfaces
{
    public interface IBaseDAO<T> where T : Models.BaseModel
    {
        //CRUD
        int Insert(T model, CommandType? typeCommand = null, IDbTransaction transaction = null, bool openConnection = true);
        int Update(T model, CommandType? typeCommand = null, IDbTransaction transaction = null, bool openConnection = true);
        int Delete(T model, CommandType? typeCommand = null, IDbTransaction transaction = null, bool openConnection = true);
        IEnumerable<T> GetAll();
        T GetByID(string id);

    }
}
