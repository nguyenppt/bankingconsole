using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace BankProject.DBRespository
{

    /******************************************************************************
     * Description:
     *      It is used to return BD context, it will used when we want to invork StoreProc
     *      from DB context.
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
    public class StoreProRepository
    {
        public DBContextBase StoreProcessor()
        {
            return DBContextBase.getInstance();
        }

    }
}