using Caliburn.Micro;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using SavingsApp.Enums;
using SavingsApp.Helpers;
using SavingsApp.Models;
using SavingsApp.Workers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SavingsApp.ViewModels
{
    public class ExpensesFormViewModel : Screen
    {

        #region Constructor
        

        public ExpensesFormViewModel()
        {
            ConnectionString = SqlServerHelper.Instance.MakeConnectionString(@".\SQLEXPRESS2017", "kasa", false, null, null);
            GetTransactionsList();
            _visible = "Collapsed";
            _visibleSummaryCost = "Collapsed";
            


        }
        #endregion
        #region User Interface
        private string _selectedActualTransaction;

        public string SelectedActualTransaction
        {
            get
            {
                return _selectedActualTransaction;


            }
            set
            {
                _selectedActualTransaction = value;
                OnPropertyChanged();
                DataSelected();
            }
        }

        private string _visibleSummaryCost;
        public string VisibleSummaryCost
        {
            get
            {
                return _visibleSummaryCost;
            }
            set
            {
                _visibleSummaryCost = value;
                NotifyOfPropertyChange(() => VisibleSummaryCost);
            }
        }

        bool UpdateTransactionName = false;

        private ObservableCollection<string> _actualTransactions = new ObservableCollection<string>();

        public ObservableCollection<string> ActualTransactions
        {
            get
            {
                return _actualTransactions;
            }
            set
            {
                _actualTransactions = value;
                OnPropertyChanged();
            }
        }
        private IList<string> _transactions = new List<string>();

        public IList<string> Transactions
        {
            get
            {
                return _transactions;
            }
            set
            {
                _transactions = value;
                NotifyOfPropertyChange(() => SummaryCostList);
            }
        }
        public IList<SummaryCosts> _summaryCostList = new List<SummaryCosts>();

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
        private string _visible;
        public string Visible
        {
            get
            {
                return _visible;
            }
            set
            {
                _visible = value;
                NotifyOfPropertyChange(() => Visible);
            }
        }

        private ObservableCollection<string> _transactionType = new ObservableCollection<string>()
        {
            "Przychod","Wydatek"
        };
        public ObservableCollection<string> TransactionType
        {
            get
            {
                return _transactionType;
            }
            set
            {
                _transactionType = value;
                OnPropertyChanged();
            }
        }

        private string _selectedTransactionType;

        public string SelectedTransactionType
        {
            get
            {
                return _selectedTransactionType;
            }
            set
            {
                _selectedTransactionType = value;
                OnPropertyChanged();

            }
        }



        private bool _isEditedItem;
        public bool IsEditedItem
        {
            get
            {
                return _isEditedItem;
            }
            set
            {
                _isEditedItem = value;

                OnPropertyChanged();
            }
        }
        public int SelectedIndex { get; set; }

        private ObservableCollection<ExpensesFormModel> _expenseList = new ObservableCollection<ExpensesFormModel>();
        public ObservableCollection<ExpensesFormModel> ExpenseList
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

        private ExpensesFormModel _selectedExpense;


        private decimal _amount;
        public decimal Amount
        {
            get
            {
                return _amount;
            }
            set
            {
                _amount = value;
                NotifyOfPropertyChange(() => Amount);
            }

        }

        private string _transactionName;
        public string TransactionName
        {
            get
            {
                return _transactionName;
            }
            set
            {

                _transactionName = value;
                NotifyOfPropertyChange(() => TransactionName);
            }
        }


        private string _date;
        public string Date
        {
            get
            {
                return _date;
            }
            set
            {

                _date = value;
                NotifyOfPropertyChange(() => Date);
            }
        }

        public ExpensesFormModel SelectedExpense
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

        private EWorkerStatus _transactionsListWorkerStatus = EWorkerStatus.Waiting;
        private EWorkerStatus TransactionsListWorkerStatus
        {
            get
            {
                return _transactionsListWorkerStatus;
            }
            set
            {
                _transactionsListWorkerStatus = value;
            }
        }

        private EWorkerStatus _transactionUpdaterWorkerAync = EWorkerStatus.Waiting;

        private EWorkerStatus TransactionUpdaterWorkerAync
        {
            get
            {
                return _transactionsListWorkerStatus;
            }
            set
            {
                _transactionsListWorkerStatus = value;
            }
        }



        private void DataSelected()
        {
            if (!String.IsNullOrEmpty(_selectedActualTransaction) && UpdateTransactionName)
            {
                TransactionName = _selectedActualTransaction;
                SelectedActualTransaction = string.Empty;
                Visible = "Collapsed";
            }
        }

       

        public void ExecuteFilterView(ActionExecutionContext context)
        {
            // Clear evrytime when someone put the Key
            ActualTransactions.Clear();

            var Query = TransactionName;

            if(!String.IsNullOrEmpty(Query))
                Visible = "Visible";
            else
                Visible= "Collapsed";
            UpdateTransactionName = true;
         
            if (!String.IsNullOrEmpty(Query))
            {
                foreach (var item in Transactions)
                {
                    if (item.ToLower().StartsWith(Query.ToLower()) && Query != "")
                    {
                        ActualTransactions.Add(item);
                    }

                }
                if(ActualTransactions.Count() == 0)
                {
                    ActualTransactions.Add("Nie ma takiej transackji");
                    UpdateTransactionName = false;
                }
            }

        }

        public void UpdateList()
        {
            

            SummaryCosts costs = new SummaryCosts();

            foreach (var item in ExpenseList)
            {
                costs.Icnome += item.TransactionType =="Przychod"? item.Amount : 0;
                costs.Expenses += item.TransactionType == "Wydatek" ? item.Amount : 0;
            }
            costs.Total = costs.Icnome - costs.Expenses;

            List<SummaryCosts> tempSummaryCosts = new List<SummaryCosts>();
            tempSummaryCosts.Add(costs);
            SummaryCostList = tempSummaryCosts;
        }
 

       

        public void DeleteransactionFromList()
        {
            var DeleteItem = _selectedExpense;

            ExpenseList.Remove(DeleteItem);
            UpdateList();
            VisibleSummaryCost = ExpenseList.Count() > 0 ? "Visible" : "Collapsed";
        }

       public void EditTransaction()
        {
            var SelectedItem = SelectedExpense;
            IsEditedItem = true;
            SelectedIndex = ExpenseList.IndexOf(SelectedItem);

            if (SelectedIndex != -1)
            {
                Amount = SelectedItem.Amount;
                TransactionName = SelectedItem.TransactionName;
                Date = SelectedItem.Date;
            }
        }

        TransactionUpdaterWorkerAync transactionFormWorkerAsync = null;





        public void GetTransactionsList()
        {
            if (transactionFormWorkerAsync == null)
            {
                transactionFormWorkerAsync = new TransactionUpdaterWorkerAync();
            }

            transactionFormWorkerAsync.WorkPerformed += (s, e) =>
            {
                Transactions = e.Transactions;

            };

            transactionFormWorkerAsync.StatusUpdated += (s, e) =>
            {
                TransactionsListWorkerStatus = e.Status;
                /* NotifyOfPropertyChange(() => RefreshStatus)*/
                ;
            };

            transactionFormWorkerAsync.ConnectionString = ConnectionString;
            transactionFormWorkerAsync.RunAsync();
        }


        public void SaveTransactionsToList()
        {
            if (expensesFormWorkerAsync == null)
            {
                expensesFormWorkerAsync = new ExpensesFormWorkerAsync();
            }
            expensesFormWorkerAsync.ConnectionString = ConnectionString;
            expensesFormWorkerAsync.ExpensesFormModel = ExpenseList;
            expensesFormWorkerAsync.RunAsync();


        }

        public void AddTransactionToList()
        {
            if (IsEditedItem == false)
            {
                var item = new ExpensesFormModel(Amount, TransactionName, SelectedTransactionType, Date);

                ExpenseList.Add(item);
                UpdateList();


            }
            else
            {
                ExpenseList[SelectedIndex] = new ExpensesFormModel(Amount, TransactionName, SelectedTransactionType, Date);
                IsEditedItem = false;
                UpdateList();

            }
            VisibleSummaryCost = ExpenseList.Count() > 0 ? "Visible" : "Collapsed";

        }
        private ExpensesFormWorkerAsync expensesFormWorkerAsync = null;
        private string ConnectionString { get;}

     
        #endregion
        #region PropertyChangedEventHandler

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

      
        #endregion



    }
}
