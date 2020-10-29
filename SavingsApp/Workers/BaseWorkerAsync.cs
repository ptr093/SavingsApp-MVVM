using SavingsApp.Enums;
using SavingsApp.EventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SavingsApp.Workers
{
    public abstract class BaseWorkerAsync
    {
        #region Events
        public event EventHandler<UpdateWorkerStatusEventArgs> StatusUpdated;
        protected virtual void OnStatusUpdated(EWorkerStatus status)
        {
            var handler = StatusUpdated as EventHandler<UpdateWorkerStatusEventArgs>;
            if (handler != null)
            {
                handler(this, new UpdateWorkerStatusEventArgs(status));
            }
        }
        #endregion

        protected bool _running = false;
        public bool Running { get { return _running; } }


        public void RunAsync()
        {
            if (!_running)
            {
                var thread = new Thread(ThreadWorkerDelegate);
                thread.Start();
            }
        }

        private void ThreadWorkerDelegate()
        {
            _running = true;

            ThreadWorker();

            _running = false;
        }

        protected abstract void ThreadWorker();
    }
}
