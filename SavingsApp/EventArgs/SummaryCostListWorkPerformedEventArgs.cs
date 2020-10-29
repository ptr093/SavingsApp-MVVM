using SavingsApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SavingsApp.EventArgs
{
   public class SummaryCostListWorkPerformedEventArgs:System.EventArgs
    {
        public IList<SummaryCosts> Costs;

        public SummaryCostListWorkPerformedEventArgs(IList<SummaryCosts> costs)
        {
            Costs = costs;
        }
    }
}
