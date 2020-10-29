using Caliburn.Micro;
using SavingsApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SavingsApp
{
    class Bootstrapper : BootstrapperBase
    {
      public  Bootstrapper()
        {
            Initialize();
        }
        private IWindowManager _windowManager = new WindowManager();


        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            this._windowManager.ShowWindow(new MainWindowViewModel(this._windowManager));
        }



    }
}
