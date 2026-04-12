using ProyectoFinal_Movil_BibliotecaPersonal.Views;

namespace ProyectoFinal_Movil_BibliotecaPersonal
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(BookDetailPage), typeof(BookDetailPage));
            Routing.RegisterRoute(nameof(AddBookPage), typeof(AddBookPage));
        }
    }
}
