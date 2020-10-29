using SavingsApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SavingsApp.EventArgs
{
  public  class ExpensesListWorkPerformedEventArgs: System.EventArgs
    {
        public IList<ExpensesModel> Expenses { get; set; }

        public ExpensesListWorkPerformedEventArgs(IList<ExpensesModel> expenses)
        {
           Expenses = expenses;
        }


      
    }
}
