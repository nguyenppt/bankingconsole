using BankProject.DBRespository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoanPaymentManualCalcConsole
{
    class Program
    {
        private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static DateTime SystemDate
        {
            get;
            private set;
        }

        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            _logger.Info(string.Format("- Start calculate Loan payment at {0} with mode {1}", DateTime.Now.ToLongDateString(), args[0]));
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

        private static void Process()
        {
            CalculatePaymnet();
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = (Exception)e.ExceptionObject;
            _logger.Error(ex.Message, ex);
        }

        private static void CalculatePaymnet()
        {

            _logger.Info("CalculatePaymnet_" + SystemDate);
            StoreProRepository facade = new StoreProRepository();
            facade.StoreProcessor().B_Normal_Loan_Process_Payment(SystemDate);

        }
    }
}
