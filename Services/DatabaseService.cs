using ProyectoFinal_Movil_BibliotecaPersonal.Models;
using SQLite;

namespace ProyectoFinal_Movil_BibliotecaPersonal.Services
{
    public class DatabaseService
    {
        private SQLiteAsyncConnection? _database;
        private bool _initialized = false;

        private async Task InitializeAsync()
        {
            if (_initialized) return;

            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "biblioteca.db3");
            _database = new SQLiteAsyncConnection(dbPath);
            await _database.CreateTableAsync<Book>();
            _initialized = true;
        }

        public async Task<int> SaveBookAsync(Book book)
        {
            await InitializeAsync();
            if (book.Id != 0)
                return await _database!.UpdateAsync(book);
            else
            {
                book.DateAdded = DateTime.Now;
                return await _database!.InsertAsync(book);
            }
        }

        public async Task<List<Book>> GetBooksAsync()
        {
            await InitializeAsync();
            return await _database!.Table<Book>().OrderByDescending(b => b.DateAdded).ToListAsync();
        }

        public async Task<List<Book>> GetBooksByGenreAsync(string genre)
        {
            await InitializeAsync();
            return await _database!.Table<Book>()
                .Where(b => b.Genre == genre)
                .ToListAsync();
        }

        public async Task<List<Book>> GetReadBooksAsync()
        {
            await InitializeAsync();
            return await _database!.Table<Book>()
                .Where(b => b.IsRead == true)
                .ToListAsync();
        }

        public async Task<List<Book>> GetUnreadBooksAsync()
        {
            await InitializeAsync();
            return await _database!.Table<Book>()
                .Where(b => b.IsRead == false)
                .ToListAsync();
        }

        public async Task<int> DeleteBookAsync(Book book)
        {
            await InitializeAsync();
            return await _database!.DeleteAsync(book);
        }

        public async Task<int> UpdateReadStatusAsync(int bookId, bool isRead)
        {
            await InitializeAsync();
            var book = await _database!.Table<Book>().FirstOrDefaultAsync(b => b.Id == bookId);
            if (book == null) return 0;
            book.IsRead = isRead;
            return await _database.UpdateAsync(book);
        }

        public async Task<LibraryStats> GetStatisticsAsync()
        {
            await InitializeAsync();
            var books = await GetBooksAsync();
            return new LibraryStats
            {
                Total = books.Count,
                Read = books.Count(b => b.IsRead),
                Unread = books.Count(b => !b.IsRead),
                ByGenre = books
                    .Where(b => !string.IsNullOrEmpty(b.Genre))
                    .GroupBy(b => b.Genre)
                    .ToDictionary(g => g.Key, g => g.Count()),
                TotalPages = books.Where(b => b.IsRead).Sum(b => b.Pages)
            };
        }

        public async Task<List<Book>> SearchLocalAsync(string query)
        {
            await InitializeAsync();
            var lower = query.ToLowerInvariant();
            return await _database!.Table<Book>()
                .Where(b => b.Title.ToLower().Contains(lower) || b.Author.ToLower().Contains(lower))
                .ToListAsync();
        }

        public async Task SeedDataAsync()
        {
            await InitializeAsync();
            var count = await _database!.Table<Book>().CountAsync();
            if (count > 0) return;

            var books = new List<Book>
        {
            new Book { Title = "Cien años de soledad", Author = "Gabriel García Márquez", Genre = "Ficción", Year = 1967, Pages = 417, IsRead = true, Rating = 5, Notes = "Obra maestra del realismo mágico" },
            new Book { Title = "Don Quijote de la Mancha", Author = "Miguel de Cervantes", Genre = "Clásico", Year = 1605, Pages = 1008, IsRead = true, Rating = 5 },
            new Book { Title = "1984", Author = "George Orwell", Genre = "Distopía", Year = 1949, Pages = 328, IsRead = true, Rating = 5 },
            new Book { Title = "El señor de los anillos", Author = "J.R.R. Tolkien", Genre = "Fantasía", Year = 1954, Pages = 1200, IsRead = false, Rating = 4 },
            new Book { Title = "Harry Potter y la piedra filosofal", Author = "J.K. Rowling", Genre = "Fantasía", Year = 1997, Pages = 309, IsRead = true, Rating = 4 },
            new Book { Title = "Sapiens", Author = "Yuval Noah Harari", Genre = "Historia", Year = 2011, Pages = 443, IsRead = true, Rating = 4 },
            new Book { Title = "El principito", Author = "Antoine de Saint-Exupéry", Genre = "Ficción", Year = 1943, Pages = 96, IsRead = true, Rating = 5 },
            new Book { Title = "Orgullo y prejuicio", Author = "Jane Austen", Genre = "Romance", Year = 1813, Pages = 432, IsRead = false, Rating = 3 },
            new Book { Title = "La sombra del viento", Author = "Carlos Ruiz Zafón", Genre = "Misterio", Year = 2001, Pages = 560, IsRead = false, Rating = 4 },
            new Book { Title = "El nombre del viento", Author = "Patrick Rothfuss", Genre = "Fantasía", Year = 2007, Pages = 662, IsRead = false, Rating = 5 },
            new Book { Title = "Dune", Author = "Frank Herbert", Genre = "Ciencia Ficción", Year = 1965, Pages = 688, IsRead = false, Rating = 4 },
            new Book { Title = "El código Da Vinci", Author = "Dan Brown", Genre = "Misterio", Year = 2003, Pages = 454, IsRead = true, Rating = 3 },
        };

            foreach (var book in books)
            {
                book.DateAdded = DateTime.Now.AddDays(-new Random().Next(1, 365));
                await _database.InsertAsync(book);
            }
        }
    }
    public class LibraryStats
    {
        public int Total { get; set; }
        public int Read { get; set; }
        public int Unread { get; set; }
        public int TotalPages { get; set; }
        public Dictionary<string, int> ByGenre { get; set; } = new();
    }
}