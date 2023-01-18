using SavingsApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SavingsApp.EventArgs
{
  public class ChartsCategoryListWorkPerformedEventArgscs : System.EventArgs
    {
        public IList<ChartsModel> CategoryCharts { get; set; }


        public ChartsCategoryListWorkPerformedEventArgscs(IList<ChartsModel> categoryCharts)
        {

            CategoryCharts = categoryCharts;
        }

    }
}
