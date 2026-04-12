using CommunityToolkit.Mvvm.Input;
using ProyectoFinal_Movil_BibliotecaPersonal.Models;
using ProyectoFinal_Movil_BibliotecaPersonal.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace ProyectoFinal_Movil_BibliotecaPersonal.ViewModels
{

    public class LibraryViewModel : BaseViewModel
    {
        private readonly DatabaseService _database;

        public ObservableCollection<Book> Books { get; set; } = new();

        private string _searchText = string.Empty;
        public string SearchText
        {
            get => _searchText;
            set
            {
                SetProperty(ref _searchText, value);
                _ = FilterBooksAsync();
            }
        }

        private string _selectedFilter = "Todos";
        public string SelectedFilter
        {
            get => _selectedFilter;
            set
            {
                SetProperty(ref _selectedFilter, value);
                _ = FilterBooksAsync();
            }
        }

        public int TotalBooks => Books.Count;
        public int ReadBooks => Books.Count(b => b.IsRead);
        public string ReadPercent => TotalBooks > 0
            ? $"{(ReadBooks * 100 / TotalBooks)}% leído"
            : "0% leído";

        public List<string> FilterOptions { get; } = new()
        { "Todos", "Leídos", "Pendientes" };

        public ICommand AddBookCommand { get; }
        public ICommand ViewDetailsCommand { get; }
        public ICommand MarkAsReadCommand { get; }
        public ICommand DeleteBookCommand { get; }
        public ICommand RefreshCommand { get; }

        public LibraryViewModel(DatabaseService database)
        {
            _database = database;
            Title = "Mi Biblioteca";

            AddBookCommand = new RelayCommand(async () =>
                await Shell.Current.GoToAsync(nameof(Views.AddBookPage)));

            ViewDetailsCommand = new RelayCommand<Book>(async (book) =>
            {
                if (book is null) return;
                await Shell.Current.GoToAsync($"{nameof(Views.BookDetailPage)}?bookId={book.Id}");
            });

            MarkAsReadCommand = new RelayCommand<Book>(async (book) =>
            {
                if (book is null) return;
                try
                {
                    await _database.UpdateReadStatusAsync(book.Id, !book.IsRead);
                    await LoadBooksAsync();
                }
                catch (Exception ex)
                {
                    await Shell.Current.DisplayAlertAsync("Error", ex.Message, "OK");
                }
            });

            DeleteBookCommand = new RelayCommand<Book>(async (book) =>
            {
                if (book is null) return;
                bool confirm = await Shell.Current.DisplayAlertAsync(
                    "Eliminar libro",
                    $"¿Eliminar \"{book.Title}\"?",
                    "Eliminar", "Cancelar");
                if (!confirm) return;
                try
                {
                    await _database.DeleteBookAsync(book);
                    Books.Remove(book);
                    OnPropertyChanged(nameof(TotalBooks));
                    OnPropertyChanged(nameof(ReadBooks));
                    OnPropertyChanged(nameof(ReadPercent));
                }
                catch (Exception ex)
                {
                    await Shell.Current.DisplayAlertAsync("Error", ex.Message, "OK");
                }
            });

            RefreshCommand = new RelayCommand(async () => await LoadBooksAsync());

            _ = LoadBooksAsync();
        }

        public async Task LoadBooksAsync()
        {
            try
            {
                IsBusy = true;
                await _database.SeedDataAsync();
                await FilterBooksAsync();
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Error", ex.Message, "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task FilterBooksAsync()
        {
            try
            {
                List<Book> books;

                if (!string.IsNullOrWhiteSpace(SearchText))
                    books = await _database.SearchLocalAsync(SearchText);
                else
                    books = SelectedFilter switch
                    {
                        "Leídos" => await _database.GetReadBooksAsync(),
                        "Pendientes" => await _database.GetUnreadBooksAsync(),
                        _ => await _database.GetBooksAsync()
                    };

                Books.Clear();
                foreach (var b in books)
                    Books.Add(b);

                OnPropertyChanged(nameof(TotalBooks));
                OnPropertyChanged(nameof(ReadBooks));
                OnPropertyChanged(nameof(ReadPercent));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Filter Error] {ex.Message}");
            }
        }
    }
}