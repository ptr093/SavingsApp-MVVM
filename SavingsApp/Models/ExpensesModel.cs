using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SavingsApp.Models
{
   public class ExpensesModel
    {

        #region Singleton
        private static ExpensesModel instance = null;
        private static readonly object padlock = new object();

        public ExpensesModel()
        {
        }

        public static ExpensesModel Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (padlock)
                    {
                        if (instance == null)
                        {
                            instance = new ExpensesModel();
                        }
                    }
                }
                return instance;
            }

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
