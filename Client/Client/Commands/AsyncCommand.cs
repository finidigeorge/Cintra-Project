using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Common;
using Serilog;
using WPFCustomMessageBox;

namespace Client.Commands
{
    public class AsyncCommand<T> : IAsyncCommand
    {
        protected readonly Predicate<T> _canExecute;
        protected Func<T, Task> _asyncExecute;

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public AsyncCommand(Func<T, Task> asyncExecute, Predicate<T> canExecute = null)
        {            
            _asyncExecute = asyncExecute;
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

        //fire and forget run
        public async void Execute(object parameter)
        {            
            await AsyncRunner(parameter);            
        }

        public async Task ExecuteAsync(object parameter)
        {
            await AsyncRunner(parameter);
        }

        protected virtual async Task AsyncRunner(object parameter)
        {
            try
            {
                await _asyncExecute((T) parameter).ConfigureAwait(false);
            } catch(Exception e)            
            {
                Log.Error(e, $"Messsage: {e.Message} Source: {e.Source}, Trace: {e.StackTrace}");
                
                var thread = new Thread(() =>
                {
                    CustomMessageBox.Show($"{Messages.COMMAND_ERROR_MSG} {e.Message}", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);

                    System.Windows.Threading.Dispatcher.Run();
                });

                thread.SetApartmentState(ApartmentState.STA);
                thread.Start();                
            }        
        }
    }


    public interface IAsyncCommand : ICommand
    {
        Task ExecuteAsync(object parameter);
    }
}
