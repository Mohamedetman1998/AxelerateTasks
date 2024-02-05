using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Task3.Command
{
    public class MyCommand : ICommand
    {
        #region Properties 
        public Action<object> DelegateForVoid { get; set; }
        public Predicate<object> DelegateForBool { get; set; }

        private readonly Action _act;

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        #endregion
        #region Constructor
        public MyCommand(Action<object> _execute, Predicate<object> _canExecute = null)
        {
            DelegateForVoid = _execute;
            DelegateForBool = _canExecute;
        }
        public MyCommand(Action act)
        {
            _act = act;
        }
        #endregion
        #region Methods
        public void Execute(object parameter = null)
        {
            if (_act != null) _act();
            else DelegateForVoid(parameter);
        }
        public bool CanExecute(object parameter) => DelegateForBool == null || DelegateForBool(parameter);
        #endregion


    }
}
