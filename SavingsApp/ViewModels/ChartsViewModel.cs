using Caliburn.Micro;
using SavingsApp.Helpers;
using SavingsApp.Models;
using SavingsApp.Workers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SavingsApp.ViewModels
{
   public class ChartsViewModel:Screen
    {

      public ChartsViewModel()
        {
            ConnectionString = SqlServerHelper.Instance.MakeConnectionString(@".\SQLEXPRESS2017", "kasa", false, null, null);
            UpdateChartsList();
            UpdateMonthList();
        }


        private ObservableCollection<string> _chartType = new ObservableCollection<string>()
        {
            "Wykres 1","Wykres 2","Wykres 3"
        };
        public ObservableCollection<string> ChartType
        {
            get
            {
                return _chartType;
            }
            set
            {
                _chartType = value;
                NotifyOfPropertyChange(() => ChartType);
            }
        }

        private string _selectedchartType;

        private string _visibleChart="Collapsed";
        public string VisibleChart
        {
            get
            {
                return _visibleChart;
             
            }
            set
            {
                _visibleChart = value;
                NotifyOfPropertyChange(() => VisibleChart);
            }
        }


        private string _visibleChartCategories = "Collapsed";
        public string VisibleChartCategories
        {
            get
            {
                return _visibleChartCategories;

            }
            set
            {
                _visibleChartCategories = value;
                NotifyOfPropertyChange(() => VisibleChartCategories);
            }
        }


        private string _visibleMonthsMenu = "Collapsed";
        public string VisibleMonthsMenu
        {
            get
            {
                return _visibleMonthsMenu;

            }
            set
            {
                _visibleMonthsMenu = value;
                NotifyOfPropertyChange(() => VisibleMonthsMenu);
            }
        }

        public void Update()
        {
            if(SelectedChartType.Equals("Wykres 1"))
            {
                VisibleChart = "Visible";
                VisibleChartCategories = "Collapsed";
                VisibleMonthsMenu = "Collapsed";
                SelectedMonth = String.Empty;
                UpdateChartsList();

            }
            if(SelectedChartType.Equals("Wykres 2"))
            {
                VisibleChart = "Collapsed";
                VisibleChartCategories = "Visible";
                VisibleMonthsMenu = "Collapsed";
                SelectedMonth = String.Empty;
                UpdateCategoryColumn();
            }
            if(SelectedChartType.Equals("Wykres 3"))
            {
                VisibleChart = "Collapsed";
                VisibleChartCategories = "Visible";
                VisibleMonthsMenu = "Visible";
            }
           
        }


       


        public string SelectedChartType
        {
            get
            {
               
                return _selectedchartType;
               
             
            }
            set
            {
                _selectedchartType = value;
                NotifyOfPropertyChange(() => SelectedChartType);
                Update();

            }
        }

        public string ConnectionString { get; }

        private IList<ChartsModel> _generalSummary = new List<ChartsModel>();
        private IList<ChartsModel> GeneralSummary
        {
            get
            {
                return _generalSummary;
            }
            set
            {
                _generalSummary = value;
                NotifyOfPropertyChange(() => GeneralSummary);
            }
        }

        public void UpdateMonthList()
        {
            if (queryListWorker == null)
            {
                queryListWorker = new ExpensesWorkerAsync();

                queryListWorker.WorkPerformed2 += (s, e) =>
                {
                    MonthList = e.Months;

                };

              
            }
            queryListWorker.ConnectionString = ConnectionString;
            queryListWorker.RunAsync();


        }


        private IList<string> _monthList = new List<string>();
        public IList<string> MonthList
        {
            get
            {
                return _monthList;
            }
            set
            {
                _monthList = value;
                NotifyOfPropertyChange(() => MonthList);
            }
        }
        private ExpensesWorkerAsync queryListWorker = null;
        private string _selectedMonth = "";

        public string SelectedMonth
        {
            get
            {

                return _selectedMonth;
            }
            set
            {
                if (_selectedMonth == null)
                    return;

                _selectedMonth = value;

                NotifyOfPropertyChange(() => SelectedMonth);
            

            }
        }

        private IList<ChartsModel> _categoriesExpensesList = new List<ChartsModel>();

        public IList<ChartsModel> CategoriesExpensesList
        {
            get
            {
                return _categoriesExpensesList;
            }
            set
            {
                _categoriesExpensesList = value;
                NotifyOfPropertyChange(() => CategoriesExpensesList);
            }
        }

        private IList<ChartsModel> _chartsList = new List<ChartsModel>();
        public IList<ChartsModel> ChartsList
        {
            get
            {
                return _chartsList;
            }
            set
            {
                _chartsList = value;
                NotifyOfPropertyChange(() => ChartsList);
            }
        }




        public void UpdateCategoryColumn()
        {
            if (chartsListWorkerAsync == null)
            {
                chartsListWorkerAsync = new ChartsWorkerAsync();
            }
            chartsListWorkerAsync.WorkPerformedCategory += (s, e) =>
            {
                CategoriesExpensesList = e.CategoryCharts;

            };

       

            chartsListWorkerAsync.ConnectionString = ConnectionString;
            chartsListWorkerAsync.SelectedMonth = _selectedMonth;
            chartsListWorkerAsync.RunAsync();

         
        }


        public void UpdateChartsList()
        {
            if (chartsListWorkerAsync == null)
            {
                chartsListWorkerAsync = new ChartsWorkerAsync();
            }

            chartsListWorkerAsync.WorkPerformed += (s, e) =>
            {
                ChartsList = e.Charts;

            };
            chartsListWorkerAsync.WorkPerformedCategory += (s, e) =>
            {
                CategoriesExpensesList = e.CategoryCharts;
            };
            chartsListWorkerAsync.WorkPerformedGeneral += (s, e) =>
             {
                 GeneralSummary = e.Charts;
 
             };



            chartsListWorkerAsync.ConnectionString = ConnectionString;
            chartsListWorkerAsync.SelectedMonth = _selectedMonth;
            chartsListWorkerAsync.RunAsync();

        }


     
        ChartsWorkerAsync chartsListWorkerAsync = null;
    }
}
