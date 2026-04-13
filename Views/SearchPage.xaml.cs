using ProyectoFinal_Movil_BibliotecaPersonal.ViewModels;

namespace ProyectoFinal_Movil_BibliotecaPersonal.Views;

public partial class SearchPage : ContentPage
{
    public SearchPage() : this(
       IPlatformApplication.Current!.Services.GetRequiredService<SearchViewModel>())
    { }
    public SearchPage(SearchViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}