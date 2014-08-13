using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using System.Transactions;
using BankProject.DataProvider;

namespace CalculateInterestConsole
{
    class Program
    {
        static SqlDataProvider sqldata = new SqlDataProvider();
        private static readonly string _connectionString = ConfigurationManager.ConnectionStrings["defaultDB"].ConnectionString;


        public static DateTime SystemDate
        {
            get
            {
                return DateTime.Today;
                //return new DateTime(2014, 8, 31).AddDays(2);
                //return DateTime.Now.AddDays(2);
            }
        }
        static void Main(string[] args)
        {
            Console.WriteLine("Calculate daily interest");
            CalculateInterest();
        }

        private static void CalculateInterest()
        {
            sqldata.ndkExecuteNonQuery("BOPENACCOUNT_CalculatorInterestAmount", SystemDate);
        }

    }
}
