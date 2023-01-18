using SavingsApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SavingsApp.EventArgs
{
    public class ChartsListWorkPerformedEventArgs : System.EventArgs
    {
        public IList<ChartsModel> Charts { get; set; }


        public ChartsListWorkPerformedEventArgs( IList<ChartsModel> charts)
        {

            Charts = charts;
        }


    }
}
