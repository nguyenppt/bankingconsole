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

namespace CalculateInterestConsole
{
    class Program
    {
        private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static readonly string _connectionString = ConfigurationManager.ConnectionStrings["defaultDB"].ConnectionString;

        private static readonly string PREPARE_DATA_FOR_ARREAR = @"INSERT INTO [BSAVING_ACC_INTEREST]([RefId], [SavingAccType], [CustomerId], [CustomerName], [Currency], [Principal]
	                                                                    , [StartDate], [EndDate], [InterestRate], [NonInterestRate], [AZRolloverPR], TermInterestAmt, NonTermInterestAmt)
                                                                    SELECT a.RefId, 'AREAR', a.CustomerId, a.CustomerName, a.Currency
                                                                        ,  a.AZPrincipal +  ISNULL((SELECT SUM(ISNULL(TermInterestAmt,0)) FROM BSAVING_ACC_INTEREST WHERE RefId = a.RefId),0) as AZPrincipal
                                                                        , a.AZPreMaturityDate, a.AZMaturityDate, a.AZInterestRate
                                                                        , (SELECT CASE a.Currency 
		                                                                            WHEN 'VND' THEN VND  
		                                                                            WHEN 'USD' THEN USD
		                                                                            ELSE EUR END FROM BINTEREST_RATE WHERE Term = 'NON') as NonVNDInterestRate
                                                                        , a.AZRolloverPR, 0, 0
                                                                    FROM  BSAVING_ACC_ARREAR a
                                                                    LEFT JOIN [BSAVING_ACC_INTEREST] i
	                                                                    ON a.RefId = i.RefId  AND a.AZPreMaturityDate = i.[StartDate] AND a.AZMaturityDate = i.[EndDate]
                                                                    WHERE Status = 'AUT' AND (CloseStatus is null OR CloseStatus != 'AUT') 
                                                                            AND i.RefId is null AND a.AZPreMaturityDate is not NULL AND a.AZMaturityDate is not NULL";

        private static readonly string PREPARE_DATA_FOR_PERIODIC = @"INSERT INTO [BSAVING_ACC_INTEREST]([RefId], [SavingAccType], [CustomerId], [CustomerName], [Currency], [Principal]
	                                                                    , [StartDate], [EndDate], [InterestRate], [NonInterestRate], [AZRolloverPR], TermInterestAmt, NonTermInterestAmt)
                                                                    SELECT a.RefId, 'PERIODIC', a.CustomerId, a.CustomerName, a.Currency
                                                                        ,  a.AZPrincipal +  ISNULL((SELECT SUM(ISNULL(TermInterestAmt,0)) FROM BSAVING_ACC_INTEREST WHERE RefId = a.RefId),0) as AZPrincipal
                                                                        , a.AZPreMaturityDate, a.AZMaturityDate, a.AZInterestRate
                                                                        , (SELECT CASE a.Currency 
		                                                                        WHEN 'VND' THEN VND  
		                                                                        WHEN 'USD' THEN USD
		                                                                        ELSE EUR END FROM BINTEREST_RATE WHERE Term = 'NON') as NonVNDInterestRate
                                                                        , '1', 0, 0
                                                                    FROM  BSAVING_ACC_PERIODIC a
                                                                    LEFT JOIN [BSAVING_ACC_INTEREST] i
	                                                                    ON a.RefId = i.RefId  AND a.AZPreMaturityDate = i.[StartDate] AND a.AZMaturityDate = i.[EndDate]
                                                                    WHERE Status = 'AUT' AND (CloseStatus is null OR CloseStatus != 'AUT') AND i.RefId is null AND a.AZPreMaturityDate is not NULL AND a.AZMaturityDate is not NULL";

        public static DateTime SystemDate
        {
            get;
            private set;
        }
        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            _logger.Info(string.Format("- Start calculate interest at {0} with mode {1}", DateTime.Now.ToLongDateString(), args[0]));            
            if (args[0] == "schedule")
            {
                SystemDate = DateTime.Now;
                Process();
            }
            else if (args[0] == "input")
            {
                try
                {                    
                    SystemDate = new DateTime(int.Parse(args[1]), int.Parse(args[2]), int.Parse(args[3]));
                    Console.WriteLine(SystemDate.ToShortDateString());
                    Process();
                }
                catch (Exception ex)
                {
                    _logger.Error(ex.Message, ex);
                    Console.WriteLine(ex.Message);
                    Console.Read();
                }                
            }
            else if (args[0] == "manual")
            {
                string exit = "N";
                do
                {
                    try
                    {
                        Console.Write("Day:");
                        int day = int.Parse(Console.ReadLine());
                        Console.Write("Month:");
                        int month = int.Parse(Console.ReadLine());
                        Console.Write("Year:");
                        int year = int.Parse(Console.ReadLine());

                        SystemDate = new DateTime(year, month, day);

                        Process();
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex.Message, ex);
                        Console.WriteLine(ex.Message);
                    }

                    Console.Write("Do you want to quit (Y/N)?");
                    exit = Console.ReadLine();
                }
                while (exit.ToUpper() != "Y");
            }

            _logger.Info("Completed !");
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = (Exception)e.ExceptionObject;
            _logger.Error(ex.Message, ex);
        }

        private static void Process()
        {
            //Console.WriteLine("Get non term interest.");
            //GetNonInterestRate();
            Console.WriteLine("Prepare data for arrear");
            PrepareDataForArrear();
            Console.WriteLine("Prepare data for arrear");
            PrepareDataForPeriodic();
            Console.WriteLine("Calculate daily interest");
            CalculateInterest();
        }

        //private static void GetNonInterestRate()
        //{
        //    using (var conn = new SqlConnection(_connectionString))
        //    {
        //        conn.Open();
        //        using (var command = new SqlCommand(@"SELECT VND FROM BINTEREST_RATE WHERE TERM ='NON'", conn))
        //        {
        //            var result = command.ExecuteScalar();
        //            if (result != null)
        //            {
        //                _nonVNDInterestRate = (decimal)result;
        //            }
        //        }
        //    }
        //}

        private static void CalculateInterest()
        {
            var option = new TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted
            };
            using (var scope = new TransactionScope(TransactionScopeOption.Required, option))
            {
                var savingAccInterestTb = new DataTable();
                using (var conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    using (var command = new SqlCommand(@"SELECT [RefId],[SavingAccType],[CustomerId],[CustomerName],[Currency]
                                                        ,[Principal],[StartDate],[EndDate],[AZRolloverPR],[InterestRate]
                                                        ,[TermInterestAmt],[NonInterestRate],[NonTermInterestAmt], LastCalcInterstDate
                                                    FROM [dbo].[BSAVING_ACC_INTEREST] 
                                                    WHERE (LastCalcInterstDate is null OR DATEDIFF(day, LastCalcInterstDate, @systemDate) > 0)
                                                        AND [RefId] IN  (SELECT [RefId] FROM BSAVING_ACC_ARREAR WHERE CloseStatus is null OR CloseStatus != 'AUT' 
		                                                                        UNION SELECT [RefId] FROM BSAVING_ACC_PERIODIC WHERE CloseStatus is null OR CloseStatus != 'AUT' )", conn))
                    {
                        command.Parameters.AddWithValue("@systemDate", SystemDate);
                        using (var adapter = new SqlDataAdapter(command))
                        {
                            adapter.Fill(savingAccInterestTb);

                            foreach (DataRow row in savingAccInterestTb.Rows)
                            {
                                if (DateAndTime.DateDiff(DateInterval.Day, SystemDate, (DateTime)row["StartDate"]) > 0)
                                {
                                    continue;
                                }
                                //decimal nonTermInterestAmt = row["NonTermInterestAmt"] == DBNull.Value ? 0 : (decimal)row["NonTermInterestAmt"];
                                var principal = (decimal)row["Principal"]; // +nonTermInterestAmt;
                                decimal interestAmt = 0;

                                if (DateAndTime.DateDiff(DateInterval.Day, SystemDate, (DateTime)row["EndDate"]) <= 0)
                                {
                                    //Trung ngay dao han
                                    interestAmt = (decimal)row["InterestRate"] / 100 / 360 * DateAndTime.DateDiff(DateInterval.Day, (DateTime)row["StartDate"], (DateTime)row["EndDate"]) * principal;
                                    row["TermInterestAmt"] = interestAmt;
                                    row["NonTermInterestAmt"] = 0;
                                    row["LastCalcInterstDate"] = SystemDate;

                                    UpdateMaturityDate((string)row["RefId"], (string)row["SavingAccType"], conn);
                                }
                                else if (DateAndTime.DateDiff(DateInterval.Day, SystemDate, (DateTime)row["EndDate"]) > 0)
                                {
                                    interestAmt = (decimal)row["NonInterestRate"] / 100 / 360 * DateAndTime.DateDiff(DateInterval.Day, (DateTime)row["StartDate"], SystemDate) * principal;
                                    row["NonTermInterestAmt"] = interestAmt;
                                    row["LastCalcInterstDate"] = SystemDate;
                                }
                            }

                            var commandBuilder = new SqlCommandBuilder(adapter);
                            adapter.Update(savingAccInterestTb);
                        }
                    }
                }
                scope.Complete();
            }
        }

        private static void UpdateMaturityDate(string refId, string savingAccType, SqlConnection connection)
        {
            var resultTb = new DataTable();
            using (var adapter = new SqlDataAdapter())
            {
                switch (savingAccType.ToUpper())
                {
                    case "AREAR":
                        adapter.SelectCommand = new SqlCommand(@"SELECT AZTerm, AZMaturityDate FROM BSAVING_ACC_ARREAR WHERE RefId =@RefId", connection);
                        adapter.SelectCommand.Parameters.AddWithValue("RefId", refId);
                        adapter.Fill(resultTb);
                        break;
                    case "PERIODIC":
                        adapter.SelectCommand = new SqlCommand(@"SELECT AZTerm, AZMaturityDate FROM BSAVING_ACC_PERIODIC WHERE RefId =@RefId", connection);
                        adapter.SelectCommand.Parameters.AddWithValue("RefId", refId);
                        adapter.Fill(resultTb);
                        break;
                }
                if (resultTb.Rows.Count > 0)
                {
                    var term = (string)resultTb.Rows[0]["AZTerm"];
                    var preMaturityDate = (DateTime)resultTb.Rows[0]["AZMaturityDate"];
                    DateTime maturityDate;
                    if (term.Substring(term.Length - 1, 1).ToUpper() == "D")
                    {
                        maturityDate = preMaturityDate.AddDays(Convert.ToInt32(term.Substring(0, term.Length - 1)));
                    }
                    else
                    {
                        maturityDate = preMaturityDate.AddMonths(Convert.ToInt32(term.Substring(0, term.Length - 1)));
                    }
                    using (var command = new SqlCommand())
                    {
                        command.Connection = connection;
                        command.Parameters.AddWithValue("RefId", refId);
                        command.Parameters.AddWithValue("AZPreMaturityDate", preMaturityDate);
                        command.Parameters.AddWithValue("AZMaturityDate", maturityDate);
                        switch (savingAccType.ToUpper())
                        {
                            case "AREAR":
                                command.CommandText = "UPDATE BSAVING_ACC_ARREAR SET AZPreMaturityDate = @AZPreMaturityDate, AZMaturityDate = @AZMaturityDate WHERE RefId = @RefId";
                                command.ExecuteNonQuery();
                                break;
                            case "PERIODIC":
                                command.CommandText = "UPDATE BSAVING_ACC_PERIODIC SET AZPreMaturityDate = @AZPreMaturityDate, AZMaturityDate = @AZMaturityDate WHERE RefId = @RefId";
                                command.ExecuteNonQuery();
                                break;
                        }
                    }
                }
            }
        }

        private static void PrepareDataForArrear()
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (var command = new SqlCommand(PREPARE_DATA_FOR_ARREAR, conn))
                {
                    //command.Parameters.AddWithValue("@nonVNDInterestRate", _nonVNDInterestRate);
                    command.ExecuteNonQuery();
                }
            }
        }

        private static void PrepareDataForPeriodic()
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (var command = new SqlCommand(PREPARE_DATA_FOR_PERIODIC, conn))
                {
                    //command.Parameters.AddWithValue("@nonVNDInterestRate", _nonVNDInterestRate);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
