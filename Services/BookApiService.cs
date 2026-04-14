using ProyectoFinal_Movil_BibliotecaPersonal.Models;
using System.Text.Json;

namespace ProyectoFinal_Movil_BibliotecaPersonal.Services
{
    public class BookApiService
    {
        private readonly HttpClient _httpClient;
        private const string API_URL = "https://www.googleapis.com/books/v1/volumes";

        public BookApiService()
        {
            _httpClient = new HttpClient();
            _httpClient.Timeout = TimeSpan.FromSeconds(15);//si tarda mas de 15 segundos en conectarse manda una excepción 
        }

        public async Task<List<BookSearchResult>> SearchBooksAsync(string query)
        {
            try
            {
                var encoded = Uri.EscapeDataString(query);// convierte "don quijote" → "don%20quijote"
                                                          // necesario porque los espacios rompen la URL
                var url = $"{API_URL}?q={encoded}&maxResults=20&langRestrict=es";//enlce de busqueda del libro, el cual busca todos los libros parecidos
                //langRestrict=es se utiliza para indicar preferencia de libros en español
                
                var response = await _httpClient.GetStringAsync(url);//respues en json como string

                using var doc = JsonDocument.Parse(response);// parsea el string JSON a un árbol navegable
                var results = new List<BookSearchResult>();

                if (!doc.RootElement.TryGetProperty("items", out var items))
                    return results;//respuesta si no hay datos en el json (busqueda sin resultados)

                foreach (var item in items.EnumerateArray())
                {
                    var info = item.GetProperty("volumeInfo");// entra en el nodo de volumInfo, informacion
                    var result = new BookSearchResult
                    {
                        Id = item.GetProperty("id").GetString() ?? "",
                        Title = info.TryGetProperty("title", out var t) ? t.GetString() 
                        ?? "" : "Sin título",
                        Author = info.TryGetProperty("authors", out var a) && a.GetArrayLength() > 0
                            ? a[0].GetString() ?? "Autor desconocido"
                            : "Autor desconocido",
                    };

                    // Thumbnail: reemplaza http por https porque iOS/Android bloquean HTTP
                    if (info.TryGetProperty("imageLinks", out var imgs) &&
                        imgs.TryGetProperty("thumbnail", out var thumb))
                        result.ThumbnailUrl = thumb.GetString()?.Replace("http://", "https://") ?? "";

                    if (info.TryGetProperty("publisher", out var pub))
                        result.Publisher = pub.GetString() ?? "";

                    results.Add(result);
                }

                return results;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[API Error] {ex.Message}");
                return new List<BookSearchResult>();
            }
        }

        //hace una buscada igual pero por id para odtener los detalles de ese libro especifico 
        public async Task<BookDetail?> GetBookDetailAsync(string bookId)
        {
            try
            {
                var url = $"{API_URL}/{bookId}";
                var response = await _httpClient.GetStringAsync(url);

                using var doc = JsonDocument.Parse(response);
                var info = doc.RootElement.GetProperty("volumeInfo");

                var detail = new BookDetail
                {
                    Id = bookId,
                    Title = info.TryGetProperty("title", out var t) ? t.GetString() ?? "" : "Sin título",
                    Author = info.TryGetProperty("authors", out var a) && a.GetArrayLength() > 0
                        ? a[0].GetString() ?? "Autor desconocido"
                        : "Autor desconocido",
                    Description = info.TryGetProperty("description", out var d) ? d.GetString() ?? "" : "",
                    PageCount = info.TryGetProperty("pageCount", out var pg) ? pg.GetInt32() : 0,
                };

                if (info.TryGetProperty("publishedDate", out var pd))
                {
                    var dateStr = pd.GetString() ?? "";
                    if (dateStr.Length >= 4 && int.TryParse(dateStr[..4], out var year))
                        detail.PublishedYear = year;
                }

                if (info.TryGetProperty("imageLinks", out var imgs) &&
                    imgs.TryGetProperty("thumbnail", out var thumb))
                    detail.CoverUrl = thumb.GetString()?.Replace("http://", "https://") ?? "";

                if (info.TryGetProperty("industryIdentifiers", out var ids))
                {
                    foreach (var id in ids.EnumerateArray())
                    {
                        if (id.TryGetProperty("type", out var type) &&
                            (type.GetString() == "ISBN_13" || type.GetString() == "ISBN_10"))
                        {
                            detail.ISBN = id.TryGetProperty("identifier", out var isbn)
                                ? isbn.GetString() ?? ""
                                : "";
                            break;
                        }
                    }
                }

                if (info.TryGetProperty("categories", out var cats))
                    detail.Categories = cats.EnumerateArray()
                        .Select(c => c.GetString() ?? "")
                        .Where(c => !string.IsNullOrEmpty(c))
                        .ToList();

                return detail;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[API Detail Error] {ex.Message}");
                return null;
            }
        }
    }
}
