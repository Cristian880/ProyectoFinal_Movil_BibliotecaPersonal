using System.Windows.Input;

namespace ProyectoFinal_Movil_BibliotecaPersonal.Helpers
{
    //medio entre accion de un boton y un metodo de ViewModel
    public class RelayCommand : ICommand
    {
        private readonly Func<bool>? _canExecute;//pregunta si se puede ejecutar la accion true o false
        private readonly Action? _execute;// se ejecuta la accion
        private readonly Func<Task>? _executeAsync;// avisa cuando cambia CanExecute
        private bool _isExecuting;

        public event EventHandler? CanExecuteChanged;

        public RelayCommand(Action execute, Func<bool>? canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public RelayCommand(Func<Task> executeAsync, Func<bool>? canExecute = null)
        {
            _executeAsync = executeAsync;
            _canExecute = canExecute;
        }
        public bool CanExecute(object? parameter) =>
            !_isExecuting // si ya está corriendo → false → botón desactivado
            && (_canExecute?.Invoke() ?? true);// evalúa la condición extra, si no hay ninguna → true
        public async void Execute(object? parameter)
        {
            if (!CanExecute(parameter)) return;// doble check de seguridad antes de empezar
            try
            {
                _isExecuting = true;// bloquea: nadie más puede ejecutar mientras corre
                RaiseCanExecuteChanged(); // avisa a la UI → el botón se desactiva visualmente

                if (_executeAsync != null)
                    await _executeAsync();// ejecuta el método async y espera que termine
                else
                    _execute?.Invoke();// o ejecuta el método síncrono
            }
            finally                            
            {
                _isExecuting = false;// desbloquea
                RaiseCanExecuteChanged();// avisa a la UI → el botón se reactiva
            }
        }

        public void RaiseCanExecuteChanged() =>
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
    public class RelayCommand<T> : ICommand
    {
        private readonly Func<T?, Task>? _executeAsync;
        private readonly Action<T?>? _execute;
        private readonly Func<T?, bool>? _canExecute;
        private bool _isExecuting;

        public event EventHandler? CanExecuteChanged;

        public RelayCommand(Action<T?> execute, Func<T?, bool>? canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public RelayCommand(Func<T?, Task> executeAsync, Func<T?, bool>? canExecute = null)
        {
            _executeAsync = executeAsync;
            _canExecute = canExecute;
        }

        public bool CanExecute(object? parameter) =>
            !_isExecuting && (_canExecute?.Invoke((T?)parameter) ?? true);

        public async void Execute(object? parameter)
        {
            if (!CanExecute(parameter)) return;
            try
            {
                _isExecuting = true;
                RaiseCanExecuteChanged();

                if (_executeAsync != null)
                    await _executeAsync((T?)parameter);
                else
                    _execute?.Invoke((T?)parameter);
            }
            finally
            {
                _isExecuting = false;
                RaiseCanExecuteChanged();
            }
        }

        public void RaiseCanExecuteChanged() =>
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}