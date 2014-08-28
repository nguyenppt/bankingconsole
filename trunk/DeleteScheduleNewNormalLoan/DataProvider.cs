using System;
using System.Data;

namespace BankProject.DataProvider
{
    public abstract class DataProvider
    {

        #region "Shared/Static Methods"

        /// <summary>
        /// singleton reference to the instantiated object 
        /// </summary>
        private static DataProvider objProvider = null;

        /// <summary>
        /// constructor
        /// </summary>
        //static DataProvider()
        //{
        //    CreateProvider();
        //}

        /// <summary>
        /// dynamically create provider 
        /// </summary>
        //private static void CreateProvider()
        //{
        //    object nn = new object();
        //    if (DateTime.Now.Month == 6)
        //        objProvider = (DataProvider)nn;
        //    else
        //        objProvider = (DataProvider)DotNetNuke.Framework.Reflection.CreateObject("data", "ndkDataProvider", "");
        //}

        /// <summary>
        /// return the provider 
        /// </summary>
        /// <returns></returns>
        public static DataProvider Instance()
        {
            return objProvider;
        }

        #endregion

        #region "Abstract methods"

        public abstract DataSet ndkExecuteDataset(string strsql, params object[] arrParams);
        public abstract void ndkExecuteNonQuery(string strsql, params object[] arrParams);

        #endregion

    }
}
