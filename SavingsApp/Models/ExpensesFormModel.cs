using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SavingsApp.Models
{
    public class ExpensesFormModel
    {
        #region Singleton
        private static ExpensesFormModel instance = null;
        private static readonly object padlock = new object();

        public ExpensesFormModel(decimal amount, string transactionName, string transactionType, string date)
        {
            Amount = amount;
            TransactionName = transactionName;
            TransactionType = transactionType;
            Date = date;
        }

                #endregion
        #region Methods

        public decimal Amount { get; set; }

        public string TransactionName { get; set; }

        public string TransactionType { get; set; }

        public string Date { get; set; }

        public string MonthName { get; set; }



        #endregion
    }
}

