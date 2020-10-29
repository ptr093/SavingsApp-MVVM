using SavingsApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SavingsApp.EventArgs
{
    class ExpensesFormListWorkPerformedEventArgs:System.EventArgs
    {
        public IList<ExpensesFormModel> ExpensesForms { get; set; }

        public ExpensesFormListWorkPerformedEventArgs(IList<ExpensesFormModel> expensesForms)
        {
            ExpensesForms = expensesForms;
        }
    }
}
