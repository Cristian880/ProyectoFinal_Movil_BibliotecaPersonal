using CommunityToolkit.Mvvm.Input;
using ProyectoFinal_Movil_BibliotecaPersonal.Drawables;
using ProyectoFinal_Movil_BibliotecaPersonal.Services;
using System.Windows.Input;

namespace ProyectoFinal_Movil_BibliotecaPersonal.ViewModels
{
    public class StatisticsViewModel : BaseViewModel
    {
        private readonly DatabaseService _database;

        private int _totalBooks;
        public int TotalBooks { get => _totalBooks; set => SetProperty(ref _totalBooks, value); }

        private int _readBooks;
        public int ReadBooks { get => _readBooks; set => SetProperty(ref _readBooks, value); }

        private int _unreadBooks;
        public int UnreadBooks { get => _unreadBooks; set => SetProperty(ref _unreadBooks, value); }

        private int _totalPages;
        public int TotalPages { get => _totalPages; set => SetProperty(ref _totalPages, value); }

        private string _readPercent = "0%";
        public string ReadPercent { get => _readPercent; set => SetProperty(ref _readPercent, value); }

        private StatisticsDrawable _drawable = new();
        public StatisticsDrawable Drawable { get => _drawable; set => SetProperty(ref _drawable, value); }

        public ICommand RefreshCommand { get; }

        public StatisticsViewModel(DatabaseService database)
        {
            _database = database;
            Title = "Estadísticas";
            RefreshCommand = new RelayCommand(async () => await LoadStatisticsAsync());
            _ = LoadStatisticsAsync();
        }

        public async Task LoadStatisticsAsync()
        {
            try
            {
                IsBusy = true;
                var stats = await _database.GetStatisticsAsync();

                TotalBooks = stats.Total;
                ReadBooks = stats.Read;
                UnreadBooks = stats.Unread;
                TotalPages = stats.TotalPages;
                ReadPercent = TotalBooks > 0
                    ? $"{(ReadBooks * 100 / TotalBooks)}%"
                    : "0%";

                Drawable = new StatisticsDrawable
                {
                    TotalBooks = stats.Total,
                    ReadBooks = stats.Read,
                    UnreadBooks = stats.Unread,
                    BooksByGenre = stats.ByGenre
                };
            }
            finally { IsBusy = false; }
        }
    }
}