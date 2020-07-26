using System.Collections.Generic;

namespace SQLAgent.DataAccessObject
{
    public interface IBaseDAO<T> where T : Models.BaseModel
    {
        //CRUD
        int Insert(T model);
        int Update(T model);
        int Delete(T model);
        IEnumerable<T> GetAll();
        T GetByID(string id);

    }
}
