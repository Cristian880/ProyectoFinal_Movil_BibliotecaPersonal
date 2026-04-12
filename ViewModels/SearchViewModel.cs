using CommunityToolkit.Mvvm.Input;
using ProyectoFinal_Movil_BibliotecaPersonal.Models;
using ProyectoFinal_Movil_BibliotecaPersonal.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace ProyectoFinal_Movil_BibliotecaPersonal.ViewModels
{
    public class SearchViewModel : BaseViewModel
    {
        private readonly BookApiService _apiService;
        private readonly DatabaseService _database;

        public ObservableCollection<BookSearchResult> SearchResults { get; } = new();

        private string _searchQuery = string.Empty;
        public string SearchQuery
        {
            get => _searchQuery;
            set => SetProperty(ref _searchQuery, value);
        }

        private string _statusMessage = "Busca un libro por título o autor";
        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        public ICommand SearchCommand { get; }
        public ICommand AddToLibraryCommand { get; }

        public SearchViewModel(BookApiService apiService, DatabaseService database)
        {
            _apiService = apiService;
            _database = database;
            Title = "Buscar Libros";

            SearchCommand = new RelayCommand(async () =>
            {
                if (string.IsNullOrWhiteSpace(SearchQuery))
                {
                    await Shell.Current.DisplayAlertAsync("Búsqueda", "Escribe un título o autor.", "OK");
                    return;
                }
                await PerformSearchAsync();
            });

            AddToLibraryCommand = new RelayCommand<BookSearchResult>(async (result) =>
            {
                if (result is null) return;
                try
                {
                    IsBusy = true;
                    var detail = await _apiService.GetBookDetailAsync(result.Id);

                    var book = new Book
                    {
                        Title = detail?.Title ?? result.Title,
                        Author = detail?.Author ?? result.Author,
                        ISBN = detail?.ISBN ?? "",
                        Year = detail?.PublishedYear ?? 0,
                        Pages = detail?.PageCount ?? 0,
                        CoverUrl = detail?.CoverUrl ?? result.ThumbnailUrl,
                        Genre = detail?.Categories.FirstOrDefault() ?? "",
                        IsRead = false,
                        Rating = 3,
                        DateAdded = DateTime.Now
                    };

                    await _database.SaveBookAsync(book);
                    await Shell.Current.DisplayAlertAsync("✅ Agregado", $"\"{book.Title}\" fue agregado a tu biblioteca.", "OK");
                }
                catch (Exception ex)
                {
                    await Shell.Current.DisplayAlertAsync("Error", ex.Message, "OK");
                }
                finally { IsBusy = false; }
            });
        }

        private async Task PerformSearchAsync()
        {
            try
            {
                IsBusy = true;
                StatusMessage = "Buscando...";
                SearchResults.Clear();

                var results = await _apiService.SearchBooksAsync(SearchQuery);

                if (results.Count == 0)
                {
                    StatusMessage = "No se encontraron resultados.";
                    return;
                }

                foreach (var r in results)
                    SearchResults.Add(r);

                StatusMessage = $"{results.Count} resultados encontrados";
            }
            catch (Exception ex)
            {
                StatusMessage = "Error al buscar. Verifica tu conexión.";
                Console.WriteLine($"[Search Error] {ex.Message}");
            }
            finally { IsBusy = false; }
        }
    }
}