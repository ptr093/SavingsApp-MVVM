using SavingsApp.Enums;
using SavingsApp.EventArgs;
using SavingsApp.Helpers;
using SavingsApp.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SavingsApp.Workers
{
    public class SummaryCostsWorkerAsync : BaseWorkerAsync
    {
        #region Events
        public event EventHandler<SummaryCostListWorkPerformedEventArgs> WorkPerformed;
      
        protected virtual void OnWorkPerformed(IList<SummaryCosts> costs)
        {
            var handler = WorkPerformed as EventHandler<SummaryCostListWorkPerformedEventArgs>;
        


            if (handler != null)
            {
                handler(this, new SummaryCostListWorkPerformedEventArgs(costs));

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
                IList<SummaryCosts> SummaryCostsList = new List<SummaryCosts>();
                using (SqlConnection cnn = new SqlConnection(ConnectionString))
                {
                    cnn.Open();

                    var index = 0;
                    var MonthName = string.Empty;
                    var YearName = string.Empty;
                    if (SelectedMonth != null)
                    {
                        index = SelectedMonth.IndexOf(' ');
                        MonthName = SelectedMonth.Remove(index, SelectedMonth.Count() - index);
                        YearName = SelectedMonth.Remove(0, index);
                    };
                    IList<string[]> getSummaryCosts = _sqlServerHelper.GetSummaryCosts(cnn, MonthName,YearName);
                    foreach (string[] sqlCommand in getSummaryCosts)
                    {
                        SummaryCosts summaryCosts = GetSummaryCosts(cnn, sqlCommand);
                        SummaryCostsList.Add(summaryCosts);

                    }

                    cnn.Close();
                }

                OnWorkPerformed(SummaryCostsList);


                OnStatusUpdated(EWorkerStatus.Success);
            }
            catch (Exception ex)
            {
                OnStatusUpdated(EWorkerStatus.Failed);
            }
        }

        private SummaryCosts GetSummaryCosts(SqlConnection cnn, string[] sqlCommand)
        {
            SummaryCosts summaryCosts = new SummaryCosts();



            if (!string.IsNullOrEmpty(sqlCommand[0]))
            summaryCosts.Icnome = decimal.Parse(sqlCommand[0]);
            if(!string.IsNullOrEmpty(sqlCommand[1]))
            summaryCosts.Expenses = decimal.Parse(sqlCommand[1]);
            if (!string.IsNullOrEmpty(sqlCommand[2]))
            summaryCosts.Total = decimal.Parse(sqlCommand[2]);
            return summaryCosts;

        }
    }
}
