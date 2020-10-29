using SavingsApp.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SavingsApp.EventArgs
{
    public class UpdateWorkerStatusEventArgs : System.EventArgs
    {
        public EWorkerStatus Status { get; set; }

        public UpdateWorkerStatusEventArgs(EWorkerStatus status)
        {
            Status = status;
        }
    }
}
