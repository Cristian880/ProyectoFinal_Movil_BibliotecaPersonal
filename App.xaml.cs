namespace ProyectoFinal_Movil_BibliotecaPersonal
{
    public partial class App : Application
    {
        public App(AppShell shell)
        {
            InitializeComponent();
            _shell = shell;
        }
        private readonly AppShell _shell;
        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(_shell);
        }
    }
}