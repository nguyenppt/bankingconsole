using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CalculateInterestConsole.DBContext;
using System.Linq.Expressions;

namespace CalculateInterestConsole.DBRespository
{
    class CheckBatchRunningRepository:BaseRepository<B_CheckBatchRunning>
    {
        public IQueryable<B_CheckBatchRunning> findBatchRunning(String batchName)
        {
            Expression<Func<B_CheckBatchRunning, bool>> batchRunning = t => t.BatchName.Equals(batchName);

            return Find(batchRunning);
        }
    }
}
