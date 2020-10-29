using SavingsApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using SavingsApp.Helpers;
using SavingsApp.Enums;
using SavingsApp.Workers;


namespace SavingsApp.ViewModels
{
    public class ExpensesViewModel : Screen
    {

        #region Constructor
      

        public ExpensesViewModel()
        {
            ConnectionString = SqlServerHelper.Instance.MakeConnectionString(@".\SQLEXPRESS2017", "kasa", false, null, null);
            UpdateMonthList();
           
        }
        #endregion


      
        #region User Interface
        private IList<ExpensesModel> _expenseList = new List<ExpensesModel>();
        public IList<ExpensesModel> ExpenseList
        {
            get
            {
                return _expenseList;
            }
            set
            {
                _expenseList = value;
                NotifyOfPropertyChange(() => ExpenseList);
            }
        }


        private IList<SummaryCosts> _summaryCostList = new List<SummaryCosts>();

        public IList<SummaryCosts> SummaryCostList
        {
            get
            {
                return _summaryCostList;
            }
            set
            {
                _summaryCostList = value;
                NotifyOfPropertyChange(() => SummaryCostList);
            }

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
        

        private string _selectedMonth="";

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

        private ExpensesModel _selectedExpense;


        public ExpensesModel SelectedExpense
        {
            get
            {
              
                return _selectedExpense;
               
            }
            set
            {
                _selectedExpense = value;
                NotifyOfPropertyChange(() => SelectedExpense);
            }
        }


        public void RefreshClick()
        {
            UpdateExpensesList();
        }


      
        private SqlServerHelper _sqlServerHelper = SqlServerHelper.Instance;

        private ExpensesWorkerAsync queryListWorker = null;

        private SummaryCostsWorkerAsync summaryCostListWorker = null;



        private EWorkerStatus _expenseListWorkerStatus = EWorkerStatus.Waiting;
        private EWorkerStatus ExpenseListWorkerStatus
        {
            get
            {
                return _expenseListWorkerStatus;
            }
            set
            {
                _expenseListWorkerStatus = value;
            }


        }

        public string ConnectionString { get;}

        public void UpdateExpensesList()
        {
            if (queryListWorker == null)
            {
                queryListWorker = new ExpensesWorkerAsync();
            }
                queryListWorker.WorkPerformed += (s, e) =>
                {
                    ExpenseList = e.Expenses;

                };

                queryListWorker.StatusUpdated += (s, e) =>
                {
                    ExpenseListWorkerStatus = e.Status;
                    /* NotifyOfPropertyChange(() => RefreshStatus)*/
                    ;
                };

            queryListWorker.ConnectionString = ConnectionString;
            queryListWorker.SelectedMonth = _selectedMonth;
            queryListWorker.RunAsync();

            if (SelectedMonth != "" | SelectedMonth != null)
            {
                UpdateSummaryCosts();
            }
        }

        public void UpdateSummaryCosts()
        {
            if (summaryCostListWorker == null)
            {
                summaryCostListWorker = new SummaryCostsWorkerAsync();
            }

                summaryCostListWorker.WorkPerformed += (s, e) =>
                {
                    SummaryCostList = e.Costs;

                };

                queryListWorker.StatusUpdated += (s, e) =>
                {
                    ExpenseListWorkerStatus = e.Status;
                
                };
            summaryCostListWorker.SelectedMonth = _selectedMonth;
            summaryCostListWorker.ConnectionString = ConnectionString;
            summaryCostListWorker.RunAsync();
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

                queryListWorker.StatusUpdated += (s, e) =>
                {
                    ExpenseListWorkerStatus = e.Status;
                    /* NotifyOfPropertyChange(() => RefreshStatus)*/
                    ;
                };
            }
            queryListWorker.ConnectionString = ConnectionString;
            queryListWorker.RunAsync();

            
        }



     


        #endregion




      
       

    }
}
