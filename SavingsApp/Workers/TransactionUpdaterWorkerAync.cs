using SavingsApp.Enums;
using SavingsApp.EventArgs;
using SavingsApp.Helpers;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SavingsApp.Workers
{
    class TransactionUpdaterWorkerAync : BaseWorkerAsync
    {
        #region Events
        public event EventHandler<TransactionsNameListWorkPerformedEventArgs> WorkPerformed;

        protected virtual void OnWorkPerformed(IList<string> transactions)
        {
            var handler = WorkPerformed as EventHandler<TransactionsNameListWorkPerformedEventArgs>;



            if (handler != null)
            {
                handler(this, new TransactionsNameListWorkPerformedEventArgs(transactions));

            }


        }
        #endregion


        private SqlServerHelper _sqlServerHelper = SqlServerHelper.Instance;

        public string ConnectionString { get; set; }

       

        public List<string> Transactions { get; set; }
        protected override void ThreadWorker()
        {
            OnStatusUpdated(EWorkerStatus.Running);

            try
            {
                IList<string> TransactionsList = new List<string>();
                using (SqlConnection cnn = new SqlConnection(ConnectionString))
                {
                    cnn.Open();



                    IList<string[]> getTransactions = _sqlServerHelper.GetTransactionsList(cnn);

                    foreach (string[] sqlCommand in getTransactions)
                    {
                        var itemTransaction = GetTransactions(cnn, sqlCommand);
                        TransactionsList.Add(itemTransaction);

                    }

                                      

                    cnn.Close();
                }

                OnWorkPerformed(TransactionsList);

                OnStatusUpdated(EWorkerStatus.Success);

            }
            catch (Exception ex)
            {
                OnStatusUpdated(EWorkerStatus.Failed);
            }
        }

        private string GetTransactions(SqlConnection cnn, string[] sqlCommand)
        {
            var transactionName = sqlCommand[0];
            return transactionName;

        }
    }
}
