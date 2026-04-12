using CommunityToolkit.Mvvm.Input;
using ProyectoFinal_Movil_BibliotecaPersonal.Models;
using ProyectoFinal_Movil_BibliotecaPersonal.Services;
using System.Windows.Input;

namespace ProyectoFinal_Movil_BibliotecaPersonal.ViewModels
{

    public class AddBookViewModel : BaseViewModel
    {
        private readonly DatabaseService _database;

        public string BookTitle { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string ISBN { get; set; } = string.Empty;
        public string YearText { get; set; } = DateTime.Now.Year.ToString();
        public string Genre { get; set; } = string.Empty;
        public string PagesText { get; set; } = "0";
        public bool IsRead { get; set; }
        public int Rating { get; set; } = 3;
        public string Notes { get; set; } = string.Empty;

        public List<string> Genres { get; } = new()
        { "Ficción", "Fantasía", "Ciencia Ficción", "Misterio", "Historia",
          "Romance", "Biografía", "Clásico", "Distopía", "Terror", "Otro" };

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public AddBookViewModel(DatabaseService database)
        {
            _database = database;
            Title = "Agregar Libro";

            SaveCommand = new RelayCommand(async () =>
            {
                if (string.IsNullOrWhiteSpace(BookTitle))
                {
                    await Shell.Current.DisplayAlertAsync("Validación", "El título es obligatorio.", "OK");
                    return;
                }
                if (string.IsNullOrWhiteSpace(Author))
                {
                    await Shell.Current.DisplayAlertAsync("Validación", "El autor es obligatorio.", "OK");
                    return;
                }

                int.TryParse(YearText, out var year);
                int.TryParse(PagesText, out var pages);

                var book = new Book
                {
                    Title = BookTitle,
                    Author = Author,
                    ISBN = ISBN,
                    Year = year,
                    Genre = Genre,
                    Pages = pages,
                    IsRead = IsRead,
                    Rating = Math.Clamp(Rating, 1, 5),
                    Notes = Notes,
                    DateAdded = DateTime.Now
                };

                try
                {
                    IsBusy = true;
                    await _database.SaveBookAsync(book);
                    await Shell.Current.DisplayAlertAsync("Éxito", "Libro agregado a tu biblioteca.", "OK");
                    await Shell.Current.GoToAsync("..");
                }
                catch (Exception ex)
                {
                    await Shell.Current.DisplayAlertAsync("Error", ex.Message, "OK");
                }
                finally { IsBusy = false; }
            });

            CancelCommand = new RelayCommand(async () => await Shell.Current.GoToAsync(".."));
        }
    }
}