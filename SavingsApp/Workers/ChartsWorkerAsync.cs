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
   public class ChartsWorkerAsync:BaseWorkerAsync
    {
        #region Events
        public event EventHandler<ChartsListWorkPerformedEventArgs> WorkPerformed;

        public event EventHandler<ChartsCategoryListWorkPerformedEventArgscs> WorkPerformedCategory;

        public event EventHandler<ChartsListWorkPerformedEventArgs> WorkPerformedGeneral;

        public event EventHandler<MonthsListWorkPerformedEventArgs> WorkPerformedMonths;
        protected virtual void OnWorkPerformed(IList<ChartsModel> charts)
        {
            var handler = WorkPerformed as EventHandler<ChartsListWorkPerformedEventArgs>;



            if (handler != null)
            {
                handler(this, new ChartsListWorkPerformedEventArgs(charts));

            }

        }

        protected virtual void OnWorkPerformed(IList<ChartsModel> categoryCharts, IList<string> months)
        {
            var handlerCategory = WorkPerformedCategory as EventHandler<ChartsCategoryListWorkPerformedEventArgscs>;
            var handler2 = WorkPerformedMonths as EventHandler<MonthsListWorkPerformedEventArgs>;


            if (handlerCategory != null)
            {
                handlerCategory(this, new ChartsCategoryListWorkPerformedEventArgscs(categoryCharts));

            }
            if (handler2 != null)
            {
                handler2(this, new MonthsListWorkPerformedEventArgs(months));

            }
        }




        public string SelectedMonth { get; set; }
        protected virtual void OnWorkPerformed(IList<ChartsModel> charts, IList<ChartsModel> categoryCharts, IList<ChartsModel> generalCharts)
        {
            var handler = WorkPerformed as EventHandler<ChartsListWorkPerformedEventArgs>;

            var handlerCategory = WorkPerformedCategory as EventHandler<ChartsCategoryListWorkPerformedEventArgscs>;

            var handlerGeneral = WorkPerformedGeneral as EventHandler<ChartsListWorkPerformedEventArgs>;

            if (handler != null)
            {
                handler(this, new ChartsListWorkPerformedEventArgs(charts));

            }
            if (handlerCategory != null)
            {
                handlerCategory(this, new ChartsCategoryListWorkPerformedEventArgscs(categoryCharts));
            }

            if(handlerGeneral != null)
            {
                handlerGeneral(this, new ChartsListWorkPerformedEventArgs(generalCharts));
            }
        }
        #endregion

        private SqlServerHelper _sqlServerHelper = SqlServerHelper.Instance;
        protected override void ThreadWorker()
        {
            OnStatusUpdated(EWorkerStatus.Running);

            try
            {
                IList<ChartsModel> ChartsList = new List<ChartsModel>();
                IList<ChartsModel> ChartsCategory = new List<ChartsModel>();
                IList<ChartsModel> ChartsCategoryByMonth = new List<ChartsModel>();
                IList<ChartsModel> ChartsGeneral = new List<ChartsModel>();
                using (SqlConnection cnn = new SqlConnection(ConnectionString))
                {
                    cnn.Open();

                    var index = 0;
                    var MonthName = string.Empty;
                    var YearName = string.Empty;
                    if (!String.IsNullOrEmpty(SelectedMonth))
                    {
                        index = SelectedMonth.IndexOf(' ');
                        MonthName = SelectedMonth.Remove(index, SelectedMonth.Count() - index);
                        YearName = SelectedMonth.Remove(0, index);
                    }

                    IList<string[]> getChartsElements = _sqlServerHelper.GetStatistic(cnn);
                    IList<string[]> getCategoriesExpenses = !String.IsNullOrEmpty(SelectedMonth)? _sqlServerHelper.GetCategoryCostsByMonth(cnn,MonthName,YearName) : _sqlServerHelper.GetCategoryCosts(cnn);
                    IList<string[]> getGeneralSummary = _sqlServerHelper.GetGeneralSummary(cnn);
                  
                    foreach (string[] sqlCommand in getChartsElements)
                    {
                        ChartsModel chartsElements = GetChartsElements(cnn, sqlCommand);
                        ChartsList.Add(chartsElements);

                    }
                    foreach (string [] sqlCommandCategory in getCategoriesExpenses)
                    {
                        ChartsModel categoriesExpenses = GetCategoryExpenses(cnn, sqlCommandCategory);
                        ChartsCategory.Add(categoriesExpenses);
                    }
                    foreach  (string [] sqlGeneral in getGeneralSummary)
                    {
                        ChartsModel generalSummary = GetGeneralSummary(cnn, sqlGeneral);
                        ChartsGeneral.Add(generalSummary);
                    }

                    cnn.Close();
                }

                OnWorkPerformed(ChartsList,ChartsCategory,ChartsGeneral);


                OnStatusUpdated(EWorkerStatus.Success);
            }
            catch (Exception ex)
            {
                OnStatusUpdated(EWorkerStatus.Failed);
            }
        }

        private ChartsModel GetGeneralSummary(SqlConnection cnn, string[] sqlCommand)
        {
            ChartsModel generalSummary = new ChartsModel();
            if (!String.IsNullOrEmpty(sqlCommand[0]))
                generalSummary.Income = Convert.ToDecimal(sqlCommand[0]);
            if (!String.IsNullOrEmpty(sqlCommand[1]))
                generalSummary.Outcome = Convert.ToDecimal(sqlCommand[1]);
            if (!String.IsNullOrEmpty(sqlCommand[2]))
                generalSummary.Total = Convert.ToDecimal(sqlCommand[2]);
            if (!String.IsNullOrEmpty(sqlCommand[3]))
                generalSummary.Year = Convert.ToInt32(sqlCommand[3]);
                return generalSummary;


           }

        private ChartsModel GetCategoryExpenses(SqlConnection cnn, string [] sqlCommand)
        {
            ChartsModel categoryCharts = new ChartsModel();
            if(!String.IsNullOrEmpty(sqlCommand[0]))
            categoryCharts.CategoryName = sqlCommand[0];
            if (!String.IsNullOrEmpty(sqlCommand[1]))
            categoryCharts.Total = Convert.ToDecimal(sqlCommand[1]);

            return categoryCharts;
        }

        private ChartsModel GetChartsElements(SqlConnection cnn, string[] sqlCommand)
        {
            ChartsModel charts = new ChartsModel();
            charts.MonthName = sqlCommand[0];
            if(!String.IsNullOrEmpty(sqlCommand[1]))
            charts.Income = Convert.ToDecimal(sqlCommand[1]);
            if (!String.IsNullOrEmpty(sqlCommand[2]))
            charts.Outcome = Convert.ToDecimal(sqlCommand[2]);
            if (!String.IsNullOrEmpty(sqlCommand[3]))
            charts.Total = Convert.ToDecimal(sqlCommand[3]);
           return charts;
        }
        public string ConnectionString { get; set; }
    }
}
