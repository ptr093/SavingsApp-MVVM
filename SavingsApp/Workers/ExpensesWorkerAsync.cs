using SavingsApp.Enums;
using SavingsApp.EventArgs;
using SavingsApp.Helpers;
using SavingsApp.Models;
using SavingsApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SavingsApp.Workers
{
    public class ExpensesWorkerAsync : BaseWorkerAsync
    {

        #region Events
        public event EventHandler<ExpensesListWorkPerformedEventArgs> WorkPerformed;
        public event EventHandler<MonthsListWorkPerformedEventArgs> WorkPerformed2;
        protected virtual void OnWorkPerformed(IList<ExpensesModel> expenses, IList<string> months)
        {
            var handler = WorkPerformed as EventHandler<ExpensesListWorkPerformedEventArgs>;
            var handler2 = WorkPerformed2 as EventHandler<MonthsListWorkPerformedEventArgs>;
       

            if (handler != null)
            {
                handler(this, new ExpensesListWorkPerformedEventArgs(expenses));

            }
            if (handler2 != null)
            {
                handler2(this, new MonthsListWorkPerformedEventArgs(months));

            }
        }
        #endregion

        private SqlServerHelper _sqlServerHelper = SqlServerHelper.Instance;

        public string ConnectionString { get; set; }
        public string SelectedMonth { get; set; }

        


        protected override void ThreadWorker()
        {
            OnStatusUpdated(EWorkerStatus.Running);

            try
            {
                IList<ExpensesModel> ExpensesList = new List<ExpensesModel>();
                IList<string> MonthsList = new List<string>();

                using (SqlConnection cnn = new SqlConnection(ConnectionString))
                {
                    cnn.Open();

                    
                    IList<string[]> getMonths = _sqlServerHelper.GetMonthsList(cnn);

                    var index = 0;
                    var MonthName = string.Empty;
                    var YearName = string.Empty;
                    if (SelectedMonth != null)
                    {
                        index = SelectedMonth.IndexOf(' ');
                        MonthName= SelectedMonth.Remove(index,SelectedMonth.Count() - index);
                        YearName = SelectedMonth.Remove(0, index);
                    }
                    IList<string[]> getExpenses = _sqlServerHelper.GetExpensesList(cnn, MonthName,YearName);
                    foreach (string[] sqlCommand in getExpenses)
                    {
                        ExpensesModel expenses = GetExpnsesList(cnn, sqlCommand);
                        ExpensesList.Add(expenses);
                   
                    }

                    
                    foreach (string [] sqlCommand in getMonths)
                    {
                      
                        var months = GetMonths(cnn, sqlCommand);
                     
                        MonthsList.Add(months);
                    }
                    cnn.Close();
                }

                OnWorkPerformed(ExpensesList,MonthsList);

               
                OnStatusUpdated(EWorkerStatus.Success);
            }
            catch (Exception ex)
            {
                OnStatusUpdated(EWorkerStatus.Failed);
            }
        }

        private ExpensesModel GetExpnsesList(SqlConnection cnn, string[] sqlCommand)
        {
            ExpensesModel expensesModel = new ExpensesModel();

            expensesModel.Amount = decimal.Parse(sqlCommand[0]);
            expensesModel.Date = sqlCommand[3];
            expensesModel.TransactionName = sqlCommand[1];
            expensesModel.TransactionType = sqlCommand[2];
       
            return expensesModel;

        }


        private string GetMonths(SqlConnection cnn, string [] sqlCommand)
        {
            var Monthname = sqlCommand[0];
            return Monthname;

        }
    }
}
