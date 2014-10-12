using CalculateInterestConsole.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BankProject.DBRespository
{
    /******************************************************************************
     * Description:
     *      It is used to link to DB Context ().
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
    public class DBContextBase : VietVictoryCoreBankingEntities
    {
        private static DBContextBase context;

        private DBContextBase()
        {
        }

        public static DBContextBase getInstance()
        {
            if (context == null)
                context = new DBContextBase();

            return context;
        }
    }
}