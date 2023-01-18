using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SavingsApp.ViewModels
{
  public class MainWindowViewModel : Conductor<Screen>
    {
        
        
      
        public MainWindowViewModel(IWindowManager windowManager)
        {
            _windowManager = windowManager;
            AddExpenses();

        }

        private IWindowManager _windowManager;

        public void AddExpenses()
        {
          
            ActivateItem(new ExpensesFormViewModel());

        }
        public void ListOfExpenses()
        {
         
            ActivateItem(new ExpensesViewModel());
        }

        public void Charts()
        {
            ActivateItem(new ChartsViewModel());
        }
    }
}
