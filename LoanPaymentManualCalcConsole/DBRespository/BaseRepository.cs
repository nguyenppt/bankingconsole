using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Web;
using System.Data.Entity;
using LoanPaymentManualCalcConsole.DBContext;
using System.Data.Entity.Validation;
using System.Diagnostics;

namespace BankProject.DBRespository
{

    /******************************************************************************
     * Description:
     *      Abstract Base repository to have commont CRUD method.
     *      Don't recommend to change it, it is part of Framework
     * Created By: 
     *      Nghia Le
     ******************************************************************************/


    /**
     * *****HISTORY****
     * Date                 By                  Description of change
     * **************************************************************************
     * 10-Sep-2014          Nghia               Init code
     *
     * 
     * 
     * 
     * ****************************************************************************
     */
    public class BaseRepository<TEntity> : IRepositoryDB<TEntity> where TEntity : class
    {
        protected System.Data.Entity.DbContext context;
        protected DbSet<TEntity> dbSet;

        protected BaseRepository()
        {
            context = DBContextBase.getInstance();
            this.dbSet = context.Set<TEntity>();
        }


        public void Commit()
        {
            try
            {
                context.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        Trace.TraceInformation("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                    }
                }
            }
        }



        public IQueryable<TEntity> Find(System.Linq.Expressions.Expression<Func<TEntity, bool>> predicate)
        {
            return dbSet.Where(predicate);
       }


        public IQueryable<TEntity> GetAll()
        {
            return dbSet;
        }

        public void Add(TEntity entity)
        {
            dbSet.Add(entity);     
        }

        public void Delete(TEntity entity)
        {
            dbSet.Remove(entity);
        }

        public void Update(TEntity oldEntry,TEntity entity)
        {
            context.Entry<TEntity>(oldEntry).CurrentValues.SetValues(entity);
            //dbSet.Attach(entity);
        }

        public TEntity GetById(Object id)
        {
            return dbSet.Find(id);
        }

        public DateTime GetSystemDatetime()
        {
            DateTime dbDate = new DateTime();
            dbDate = DateTime.Now;
            return dbDate;
        }

        private bool Exists(TEntity entity){
            return true;
        }
    }
}