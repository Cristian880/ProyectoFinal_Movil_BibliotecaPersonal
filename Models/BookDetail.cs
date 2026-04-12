using System;
using System.Collections.Generic;
using System.Text;

namespace ProyectoFinal_Movil_BibliotecaPersonal.Models
{
    public class BookDetail
    {
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string ISBN { get; set; } = string.Empty;
        public int PublishedYear { get; set; }
        public int PageCount { get; set; }
        public string Description { get; set; } = string.Empty;
        public string CoverUrl { get; set; } = string.Empty;
        public List<string> Categories { get; set; } = new();
    }
}
