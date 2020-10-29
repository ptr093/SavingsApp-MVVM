using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SavingsApp.EventArgs
{
    class TransactionsNameListWorkPerformedEventArgs
    {
        public IList<string> Transactions { get; set; }

        public TransactionsNameListWorkPerformedEventArgs(IList<string> transactions)
        {
            Transactions = transactions;
        }
    }
}
