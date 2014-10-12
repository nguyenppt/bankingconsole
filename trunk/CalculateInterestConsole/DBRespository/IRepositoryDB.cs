using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BankProject.DBRespository
{
    /******************************************************************************
     * Description:
     *      Repository Interface it is interface for all Repository concerate.
     *      Don't recommend to change it, it is part of Framework
     * Created By: 
     *      Nghia Le
     ******************************************************************************/
    /**
     * *****HISTORY****
     * Date                 By                  Description of change
     * **************************************************************************
     * 10-Sep-2014          Nghia               Init code
     * */

    public interface IRepositoryDB<T>
    {
        IQueryable<T> GetAll();
        void Add(T entity);
        void Delete(T entity);
        void Update(T entity, T entryNew);
        void Commit();
        T GetById(Object id);
    }
}
