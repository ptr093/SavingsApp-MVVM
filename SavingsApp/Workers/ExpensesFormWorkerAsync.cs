using SavingsApp.Enums;
using SavingsApp.EventArgs;
using SavingsApp.Helpers;
using SavingsApp.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SavingsApp.Workers
{
    class ExpensesFormWorkerAsync : BaseWorkerAsync
    {



        private SqlServerHelper _sqlServerHelper = SqlServerHelper.Instance;

        public string ConnectionString { get; set; }

        public ObservableCollection<ExpensesFormModel> ExpensesFormModel { get; set; }

        public List<string> Transactions { get; set; }
        protected override void ThreadWorker()
        {
            OnStatusUpdated(EWorkerStatus.Running);

            try
            {
               
                using (SqlConnection cnn = new SqlConnection(ConnectionString))
                {
                    cnn.Open();

                    

                   for (int i = 0; i < ExpensesFormModel.Count(); i++)
                    {
                            InsertDataToDataBase(cnn, ExpensesFormModel[i].Amount, ExpensesFormModel[i].TransactionName, ExpensesFormModel[i].TransactionType, ExpensesFormModel[i].Date);
                    }
                 

                    cnn.Close();
                }
                
                OnStatusUpdated(EWorkerStatus.Success);
              
            }
            catch (Exception ex)
            {
                OnStatusUpdated(EWorkerStatus.Failed);
            }
        }



        private void InsertDataToDataBase(SqlConnection cnn, decimal Amount, string TransactionName, string TransactionType, string Data)
        {
            string command =  String.Format("INSERT INTO dbo.Wydatkii VALUES ({0},'{1}','{2}','{3}')", Amount,TransactionName,TransactionType,Data);
            _sqlServerHelper.ExecuteQuery(command, cnn);
        }

     
    }
}
