using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ProyectoFinal_Movil_BibliotecaPersonal.ViewModels
{
    // Implementa INotifyPropertyChanged para que la UI se actualice automáticamente
    // cuando cambian los valores de las propiedades (binding en MVVM).
    public class BaseViewModel : INotifyPropertyChanged
    {
        // Evento que se dispara cuando una propiedad cambia.
        // La UI (XAML) se suscribe a este evento para refrescar los datos en pantalla.
        public event PropertyChangedEventHandler? PropertyChanged;

        private bool _isBusy;
        public bool IsBusy// este metodo/ propiedad esta ligado a INotifyPropertyChanged lo que permite que se pueda ejecutar ene el proyecto completo
        {
            get => _isBusy;// hace un get de la propiedad que tenga busy en el momento
            set => SetProperty(ref _isBusy, value);// hace el set de la propiedad de carga, en la vista se encarga del circulo de carga,
                                                   // se encarga de notoficar a la vista de que un proceso se esta ejecutando y cuando ya cargo( false o true)
                                                   // y evitar ejecuciones múltiples manualmente.
        }

        private string _title = string.Empty;
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        // Método reutilizable para asignar valores a propiedades de forma segura
        // y notificar automáticamente a la UI.
        protected bool SetProperty<T>(
             ref T storage,              // referencia al campo privado
            T value,                   // nuevo valor
            [CallerMemberName] string? propertyName = null) // nombre automático de la propiedad
        {
            // Si el valor nuevo es igual al actual, no hace nada
            if (EqualityComparer<T>.Default.Equals(storage, value)) return false;
            storage = value;// Asigna el nuevo valor
            OnPropertyChanged(propertyName);// Notifica a la UI que la propiedad cambió
            return true;
        }
        //metodo que se encarga de hacer que la vista cambie automaticamente la propiedad
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

}

