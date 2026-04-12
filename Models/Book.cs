using SQLite;

namespace ProyectoFinal_Movil_BibliotecaPersonal.Models
{

    [Table("books")]
    public class Book
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(100)]
        public string Author { get; set; } = string.Empty;

        [MaxLength(20)]
        public string ISBN { get; set; } = string.Empty;

        public int Year { get; set; }

        [MaxLength(50)]
        public string Genre { get; set; } = string.Empty;

        public int Pages { get; set; }

        [MaxLength(500)]
        public string CoverUrl { get; set; } = string.Empty;

        public bool IsRead { get; set; }

        public int Rating { get; set; } = 1; // 1-5 estrellas

        [MaxLength(1000)]
        public string Notes { get; set; } = string.Empty;

        public DateTime DateAdded { get; set; } = DateTime.Now;
    }
}

