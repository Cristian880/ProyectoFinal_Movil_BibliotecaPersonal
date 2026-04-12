using ProyectoFinal_Movil_BibliotecaPersonal.ViewModels;

namespace ProyectoFinal_Movil_BibliotecaPersonal.Views;

public partial class AddBookPage : ContentPage
{
    public AddBookPage(AddBookViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}