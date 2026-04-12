using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;
using ProyectoFinal_Movil_BibliotecaPersonal.ViewModels;
using ProyectoFinal_Movil_BibliotecaPersonal.Services;
using ProyectoFinal_Movil_BibliotecaPersonal.Views;

namespace ProyectoFinal_Movil_BibliotecaPersonal
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            }).UseMauiCommunityToolkit();

            builder.Services.AddSingleton<DatabaseService>();
            builder.Services.AddSingleton<BookApiService>();

            builder.Services.AddTransient<LibraryViewModel>();
            builder.Services.AddTransient<BookDetailViewModel>();
            builder.Services.AddTransient<SearchViewModel>();
            builder.Services.AddTransient<StatisticsViewModel>();
            builder.Services.AddTransient<AddBookViewModel>();

            builder.Services.AddTransient<LibraryPage>();
            builder.Services.AddTransient<BookDetailPage>();
            builder.Services.AddTransient<SearchPage>();
            builder.Services.AddTransient<StatisticsPage>();
            builder.Services.AddTransient<AddBookPage>();

            return builder.Build();
        }
    }
}