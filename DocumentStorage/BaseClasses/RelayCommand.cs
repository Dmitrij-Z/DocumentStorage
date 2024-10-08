using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace DocumentStorage.BaseClasses
{
    public class RelayCommand : ICommand
    {
        private readonly CanExecuteHandler canExecute;
        private readonly ExecuteHandler execute;
        private readonly EventHandler requerySuggested;

        private readonly Dispatcher dispatcher;

        public event EventHandler CanExecuteChanged;

        private void Invalidate() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        public RelayCommand(ExecuteHandler execute, CanExecuteHandler canExecute = null)
        {
            dispatcher = Application.Current.Dispatcher;

            this.execute = execute ?? throw new ArgumentNullException(nameof(execute));
            this.canExecute = canExecute;

            requerySuggested = (o, e) => Invalidate();
            CommandManager.RequerySuggested += requerySuggested;
        }

        public RelayCommand(Action execute, Func<bool> canExecute = null)
            : this
            (
                  p => execute(),
                  p => canExecute?.Invoke() ?? true
            )
        { }

        private void RaiseCanExecuteChanged()
        {
            if (dispatcher.CheckAccess())
                Invalidate();
            else
                dispatcher.BeginInvoke((Action)Invalidate);
        }

        public bool CanExecute(object parameter) => canExecute?.Invoke(parameter) ?? true;

        public void Execute(object parameter) => execute?.Invoke(parameter);
    }
}
