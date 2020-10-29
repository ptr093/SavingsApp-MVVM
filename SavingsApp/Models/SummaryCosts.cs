using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SavingsApp.Models
{
   public class SummaryCosts
    {
        #region Singleton

        private  static SummaryCosts instance = null;
        private static readonly object padlock = new object();

        public SummaryCosts()
        {
        }

        public static SummaryCosts Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (padlock)
                    {
                        if (instance == null)
                        {
                            instance = new SummaryCosts();
                        }
                    }
                }
                return instance;
            }
        }


        #endregion

        #region Methods
        public decimal Icnome { get; set; }

        public decimal Expenses { get; set; }

        public decimal Total { get; set; }

        #endregion
    }
}
