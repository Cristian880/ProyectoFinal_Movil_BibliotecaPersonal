using ProyectoFinal_Movil_BibliotecaPersonal.Helpers;
using ProyectoFinal_Movil_BibliotecaPersonal.Models;
using ProyectoFinal_Movil_BibliotecaPersonal.Services;
using System.Windows.Input;

namespace ProyectoFinal_Movil_BibliotecaPersonal.ViewModels
{

    [QueryProperty(nameof(BookId), "bookId")]
    public class BookDetailViewModel : BaseViewModel
    {
        private readonly DatabaseService _database;

        private int _bookId;
        public int BookId
        {
            get => _bookId;
            set
            {
                _bookId = value;
                _ = LoadBookAsync(value);
            }
        }

        private Book _book = new();
        public Book Book
        {
            get => _book;
            set => SetProperty(ref _book, value);
        }

        // Propiedades enlazadas individualmente para TwoWay binding
        public string BookTitle
        {
            get => _book.Title;
            set { _book.Title = value; OnPropertyChanged(); }
        }
        public string Author
        {
            get => _book.Author;
            set { _book.Author = value; OnPropertyChanged(); }
        }
        public string ISBN
        {
            get => _book.ISBN;
            set { _book.ISBN = value; OnPropertyChanged(); }
        }
        public int Year
        {
            get => _book.Year;
            set { _book.Year = value; OnPropertyChanged(); }
        }
        public string Genre
        {
            get => _book.Genre;
            set { _book.Genre = value; OnPropertyChanged(); }
        }
        public int Pages
        {
            get => _book.Pages;
            set { _book.Pages = value; OnPropertyChanged(); }
        }
        public bool IsRead
        {
            get => _book.IsRead;
            set { _book.IsRead = value; OnPropertyChanged(); }
        }
        public int Rating
        {
            get => _book.Rating;
            set { _book.Rating = Math.Clamp(value, 1, 5); OnPropertyChanged(); }
        }
        public string Notes
        {
            get => _book.Notes;
            set { _book.Notes = value; OnPropertyChanged(); }
        }

        public List<string> Genres { get; } = new()
        { "Ficción", "Fantasía", "Ciencia Ficción", "Misterio", "Historia",
          "Romance", "Biografía", "Clásico", "Distopía", "Terror", "Otro" };

        public ICommand SaveCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand CancelCommand { get; }

        public BookDetailViewModel(DatabaseService database)
        {
            _database = database;
            Title = "Detalle del libro";

            SaveCommand = new RelayCommand(async () =>
            {
                if (string.IsNullOrWhiteSpace(BookTitle))
                {
                    await Shell.Current.DisplayAlertAsync("Validación", "El título es obligatorio.", "OK");
                    return;
                }
                try
                {
                    IsBusy = true;
                    await _database.SaveBookAsync(_book);
                    await Shell.Current.DisplayAlertAsync("Éxito", "Libro guardado correctamente.", "OK");
                    await Shell.Current.GoToAsync("..");
                }
                catch (Exception ex)
                {
                    await Shell.Current.DisplayAlertAsync("Error", ex.Message, "OK");
                }
                finally { IsBusy = false; }
            });

            DeleteCommand = new RelayCommand(async () =>
            {
                bool confirm = await Shell.Current.DisplayAlertAsync(
                    "Eliminar", $"¿Eliminar \"{_book.Title}\"?", "Eliminar", "Cancelar");
                if (!confirm) return;
                try
                {
                    await _database.DeleteBookAsync(_book);
                    await Shell.Current.GoToAsync("..");
                }
                catch (Exception ex)
                {
                    await Shell.Current.DisplayAlertAsync("Error", ex.Message, "OK");
                }
            });

            CancelCommand = new RelayCommand(async () => await Shell.Current.GoToAsync(".."));
        }

        private async Task LoadBookAsync(int id)
        {
            try
            {
                IsBusy = true;
                var books = await _database.GetBooksAsync();
                var found = books.FirstOrDefault(b => b.Id == id);
                if (found is not null)
                {
                    Book = found;
                    OnPropertyChanged(nameof(BookTitle));
                    OnPropertyChanged(nameof(Author));
                    OnPropertyChanged(nameof(ISBN));
                    OnPropertyChanged(nameof(Year));
                    OnPropertyChanged(nameof(Genre));
                    OnPropertyChanged(nameof(Pages));
                    OnPropertyChanged(nameof(IsRead));
                    OnPropertyChanged(nameof(Rating));
                    OnPropertyChanged(nameof(Notes));
                }
            }
            finally { IsBusy = false; }
        }
    }
}
