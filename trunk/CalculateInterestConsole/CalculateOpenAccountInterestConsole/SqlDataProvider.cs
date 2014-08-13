using System;
using System.Data;
using Microsoft.ApplicationBlocks.Data;

using System.Web.Configuration;
using System.Configuration;

namespace BankProject.DataProvider
{
    public class SqlDataProvider : DataProvider
    {


        #region "Private Members"

        private string _connectionString;


        #endregion

        #region "Constructors"

        public SqlDataProvider()
        {

            _connectionString = ConfigurationManager.ConnectionStrings["defaultDB"].ConnectionString;

        }

        #endregion

        #region "Properties"

        public string ConnectionString
        {
            get { return _connectionString; }
        }


        #endregion

        #region "Public Methods"

        public override DataSet ndkExecuteDataset(string strsql, params object[] arrParams)
        {
            return (DataSet)SqlHelper.ExecuteDataset(ConnectionString, strsql, arrParams);
        }

        public override void ndkExecuteNonQuery(string strsql, params object[] arrParams)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, strsql, arrParams);
        }


        #endregion

    }
}
