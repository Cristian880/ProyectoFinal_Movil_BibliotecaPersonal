using ProyectoFinal_Movil_BibliotecaPersonal.ViewModels;

namespace ProyectoFinal_Movil_BibliotecaPersonal.Views;

public partial class BookDetailPage : ContentPage
{
    public BookDetailPage() : this(
        IPlatformApplication.Current!.Services.GetRequiredService<BookDetailViewModel>())
    { }
    public BookDetailPage(BookDetailViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}