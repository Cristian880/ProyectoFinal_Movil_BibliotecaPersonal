using ProyectoFinal_Movil_BibliotecaPersonal.ViewModels;

namespace ProyectoFinal_Movil_BibliotecaPersonal.Views;

public partial class AddBookPage : ContentPage
{
    public AddBookPage() : this(
       IPlatformApplication.Current!.Services.GetRequiredService<AddBookViewModel>())
    { }

    public AddBookPage(AddBookViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}