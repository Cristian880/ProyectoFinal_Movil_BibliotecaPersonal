using ProyectoFinal_Movil_BibliotecaPersonal.ViewModels;

namespace ProyectoFinal_Movil_BibliotecaPersonal.Views;

public partial class StatisticsPage : ContentPage
{
    public StatisticsPage() : this(
       IPlatformApplication.Current!.Services.GetRequiredService<StatisticsViewModel>())
    { }

    public StatisticsPage(StatisticsViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is StatisticsViewModel vm)
            _ = vm.LoadStatisticsAsync();
    }
}