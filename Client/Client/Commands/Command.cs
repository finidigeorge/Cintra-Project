using Common;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WPFCustomMessageBox;

namespace Client.Commands
{
    public class Command<T> : ICommand
    {        
        private readonly Action _action;
        protected readonly Predicate<T> _canExecute;
        public Command(Action action, Predicate<T> canExecute = null)
        {
            _action = action;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            if (_canExecute == null)
            {
                return true;
            }

            return _canExecute((T)parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public void Execute(object parameter)
        {
            try
            {
                _action(); 
            }
            catch (Exception e)
            {
                Log.Error(e, $"Messsage: {e.Message} Source: {e.Source}, Trace: {e.StackTrace}");
                CustomMessageBox.Show($"{Messages.COMMAND_ERROR_MSG} {e.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
        }
    }    
}
