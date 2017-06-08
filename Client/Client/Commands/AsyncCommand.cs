using System;
using System.Threading.Tasks;
using System.Windows.Input;

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
            await _asyncExecute((T)parameter);
        }
    }


    public interface IAsyncCommand : ICommand
    {
        Task ExecuteAsync(object parameter);
    }
}
