using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CalculateInterestConsole.DBContext;
using System.Linq.Expressions;

namespace CalculateInterestConsole.DBRespository
{
    class BatchMaintenanceRepository : BaseRepository<B_BATCH_MAINTENANCE>
    {
        public IQueryable<B_BATCH_MAINTENANCE> findBatchRunning(String batchName)
        {
            Expression<Func<B_BATCH_MAINTENANCE, bool>> batchRunning = t => t.BatchName.Equals(batchName);

            return Find(batchRunning);
        }
    }
}
