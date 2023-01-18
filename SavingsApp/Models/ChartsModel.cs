using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SavingsApp.Models
{
   public class ChartsModel
    {

        #region Singleton
        private static ChartsModel instance = null;
        private static readonly object padlock = new object();


        public ChartsModel() { }


        public static ChartsModel Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (padlock)
                    {
                        if (instance == null)
                        {
                            instance = new ChartsModel();
                        }
                    }
                }
                return instance;
            }

        }

        #endregion

        public string MonthName { get; set; }
        public decimal Total { get; set; }

        public decimal Income { get; set; }

        public decimal Outcome { get; set; }

        public string CategoryName { get; set;}

        public int Year { get; set; }




    }
}
