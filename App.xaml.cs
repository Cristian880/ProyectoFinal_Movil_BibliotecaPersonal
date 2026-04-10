using Microsoft.Extensions.DependencyInjection;

namespace ProyectoFinal_Movil_BibliotecaPersonal
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
    }
}