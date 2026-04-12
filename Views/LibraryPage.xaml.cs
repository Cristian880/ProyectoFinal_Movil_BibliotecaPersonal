using ProyectoFinal_Movil_BibliotecaPersonal.ViewModels;

namespace ProyectoFinal_Movil_BibliotecaPersonal.Views;

public partial class LibraryPage : ContentPage
{
	public LibraryPage(LibraryViewModel vm)
    {
		InitializeComponent();
        BindingContext = vm;
    }
    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is LibraryViewModel vm)
            _ = vm.LoadBooksAsync();
    }
}