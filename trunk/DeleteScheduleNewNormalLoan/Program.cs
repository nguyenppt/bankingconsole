using BankProject.DataProvider;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeleteScheduleNewNormalLoan
{
    class Program
    {
        static SqlDataProvider sqldata = new SqlDataProvider();
        private static readonly string _connectionString = ConfigurationManager.ConnectionStrings["defaultDB"].ConnectionString;

        static void Main(string[] args)
        {
            Console.WriteLine("Delete schedule new normal load");
            BNEWNORMALLOAN_DeleteSchedule();
        }

        private static void BNEWNORMALLOAN_DeleteSchedule()
        {
            sqldata.ndkExecuteNonQuery("BNEWNORMALLOAN_DeleteSchedule");
        }
    }
}
