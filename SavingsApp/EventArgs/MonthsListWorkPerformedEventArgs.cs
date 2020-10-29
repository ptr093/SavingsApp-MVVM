using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SavingsApp.EventArgs
{
   public class MonthsListWorkPerformedEventArgs :System.EventArgs
    {
        public IList<string> Months { get; set; }

        public MonthsListWorkPerformedEventArgs(IList<string> months)
        {
            Months = months;
        }
    }
}
