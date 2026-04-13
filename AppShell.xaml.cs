using ProyectoFinal_Movil_BibliotecaPersonal.Views;

namespace ProyectoFinal_Movil_BibliotecaPersonal;

public partial class AppShell : Shell
{
    private readonly IServiceProvider _services;

    public AppShell(IServiceProvider services)
    {
        _services = services;
        InitializeComponent();
        Routing.RegisterRoute(nameof(BookDetailPage), typeof(BookDetailPage));
        Routing.RegisterRoute(nameof(AddBookPage), typeof(AddBookPage));
    }
}                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                