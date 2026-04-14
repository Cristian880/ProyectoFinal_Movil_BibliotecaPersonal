namespace ProyectoFinal_Movil_BibliotecaPersonal.Drawables
{
    public class StatisticsDrawable : IDrawable
    {
        public int TotalBooks { get; set; }
        public int ReadBooks { get; set; }
        public int UnreadBooks { get; set; }
        public int TotalPages { get; set; }
        public Dictionary<string, int> BooksByGenre { get; set; } = new();

        public void Draw(ICanvas canvas, RectF dirtyRect) // metodo que se llama para crear los graficos
        {
            canvas.Antialias = true; // Suaviza los bordes
            DrawPieChart(canvas, dirtyRect);    // Dibuja el gráfico circular
            DrawBarChart(canvas, dirtyRect);    // Dibuja el gráfico de barras
            DrawStatNumbers(canvas, dirtyRect); // Dibuja los números grandes
        }

        private void DrawPieChart(ICanvas canvas, RectF dirtyRect)
        {
            float cx = dirtyRect.Width * 0.25f;
            float cy = 120f;
            float radius = 80f;

            // Fondo círculo gris
            canvas.FillColor = Color.FromArgb("#EEEEEE");
            canvas.FillCircle(cx, cy, radius);

            if (TotalBooks == 0) return;

            float readAngle = (float)(ReadBooks * 360.0 / TotalBooks);

            // Arco leídos (naranja)
            canvas.StrokeColor = Color.FromArgb("#FF9800");
            canvas.StrokeSize = 22;
            canvas.DrawArc(cx - radius, cy - radius, radius * 2, radius * 2,
                -90, -90 + readAngle, true, false);

            // Arco pendientes (verde)
            if (UnreadBooks > 0)
            {
                canvas.StrokeColor = Color.FromArgb("#4CAF50");
                canvas.StrokeSize = 22;
                canvas.DrawArc(cx - radius, cy - radius, radius * 2, radius * 2,
                    -90 + readAngle, 270, true, false);
            }

            // Texto centro
            canvas.FontColor = Color.FromArgb("#333333");
            canvas.FontSize = 14;
            canvas.DrawString($"{(TotalBooks > 0 ? ReadBooks * 100 / TotalBooks : 0)}%",//calculo porciento
                cx - 20, cy - 10, 40, 20, HorizontalAlignment.Center, VerticalAlignment.Center);

            // Leyenda
            canvas.FillColor = Color.FromArgb("#4CAF50");//cuadro verde
            canvas.FillRectangle(cx + radius + 12, cy - 20, 12, 12);

            canvas.FillColor = Color.FromArgb("#FF9800");
            canvas.FillRectangle(cx + radius + 12, cy, 12, 12);//cuadro naranja 

            canvas.FontColor = Color.FromArgb("#EEEEEE");//texto
            canvas.FontSize = 11;

            canvas.DrawString($"Leídos ({ReadBooks})", cx + radius + 26, cy - 22, 90, 16,
                HorizontalAlignment.Left, VerticalAlignment.Top);

            canvas.DrawString($"Pendientes ({UnreadBooks})", cx + radius + 26, cy - 2, 100, 16,
                HorizontalAlignment.Left, VerticalAlignment.Top);
        }

        private void DrawBarChart(ICanvas canvas, RectF dirtyRect)
        {
            if (BooksByGenre.Count == 0) return;

            // configuración de dimensiones
            int maxBars = Math.Min(BooksByGenre.Count, 10);//generos
            float gapBetweenBars = 28f;          // espacio entre barras
            float barWidth = 55f;               // ancho fijo de cada barra
            float startX = 30f;
            float startY = 260f;
            float chartHeight = 180f;            // barras más altas
            //float labelAreaHeight = 52f;         // zona reservada para el texto inferior (wrap)
            int maxVal = BooksByGenre.Values.Max();

            // Título sección
            canvas.FontColor = Color.FromArgb("#FF9800");
            canvas.FontSize = 15;
            canvas.DrawString("Libros por Género", 10, startY - 24,
                dirtyRect.Width - 20, 20,
                HorizontalAlignment.Left, VerticalAlignment.Top);

            // Línea base
            canvas.StrokeColor = Color.FromArgb("#CCCCCC");
            canvas.StrokeSize = 1;
            canvas.DrawLine(startX, startY + chartHeight,
                            dirtyRect.Width - 10, startY + chartHeight);

            var barColors = new[]
            {
                "#8B4513", "#D2691E", "#CD853F", "#DEB887", "#A0522D",
                "#6B3A2A", "#C4884E", "#E8A87C", "#9B6B47", "#B8860B"
            };

            int i = 0;
            foreach (var (genre, count) in BooksByGenre.Take(maxBars))
            {
                string barColor = barColors[i % barColors.Length];//selecciona uno de los los colores de BartColors
                float barHeight = maxVal > 0 
                                ? (float)count / maxVal * chartHeight 
                                : 4f;// altura maxima segun la vista

                float x = startX + i * (barWidth + gapBetweenBars);//margen donde empezara desde la izquierda x segun la vista o tamño disponible
                float y = startY + chartHeight - barHeight;// margen donde empezara desde la base vertical y

                // Barra 
                canvas.FillColor = Color.FromArgb(barColor);
                canvas.FillRoundedRectangle(x, y, barWidth, barHeight, 4); // esquinas redondeadas

                // Número encima de la barra (mismo color que la barra) 
                canvas.FontColor = Color.FromArgb(barColor);
                canvas.FontSize = 11;
                canvas.DrawString(count.ToString(),
                    x - 4, y - 16, barWidth + 8, 16,
                    HorizontalAlignment.Center, VerticalAlignment.Top);

                // Etiqueta inferior con "word wrap" manual 
                // Partimos el género en palabras y las distribuimos en líneas
                // que quepan dentro del ancho de la barra + gap
                float labelWidth = barWidth;
                float labelX = x;  // zona de texto

                float labelStartY = startY + chartHeight + 6f;
                float lineHeight = 12f;

                float fontSize = 12f;
                canvas.FontSize = fontSize;
                canvas.FontColor = Color.FromArgb(barColor);

                //division de palabras 
                var words = genre.Split(' ');
                var lines = new List<string>();
                string currentLine = "";

                foreach (var word in words)
                {
                    // Estimamos ~6.5 px por carácter a fontSize 9
                    string candidate = string.IsNullOrEmpty(currentLine)
                                    ? word
                                    : currentLine + " " + word;

                    var size = canvas.GetStringSize(candidate, null, fontSize);
                    float realWidth = size.Width;

                    if (realWidth <= labelWidth)
                    {
                        currentLine = candidate;
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(currentLine))
                            lines.Add(currentLine);

                        currentLine = word;
                    }
                }
                if (!string.IsNullOrEmpty(currentLine))
                    lines.Add(currentLine);

                // Máximo 4 líneas para que no se salga del canvas
                int maxLines = 3;

                for (int l = 0; l < Math.Min(lines.Count, maxLines); l++)
                {

                    canvas.DrawString(
                        lines[l],
                        labelX,          // centrado sobre barra + gap
                        labelStartY + l * lineHeight,
                        labelWidth,
                        lineHeight,
                        HorizontalAlignment.Center,
                        VerticalAlignment.Top);
                }

                i++;
            }
        }

        private void DrawStatNumbers(ICanvas canvas, RectF dirtyRect)
        {
            float y = 570f;   // bajado un poco para dar espacio al label wrap
            float w = dirtyRect.Width / 2f;

            var stats = new (string Label, string Value, string Color)[]
            {
                ("Total",   TotalBooks.ToString(), "#8B4513"),
                ("Leídos",  ReadBooks.ToString(),  "#4CAF50"),
            };

            for (int i = 0; i < stats.Length; i++)
            {
                var (label, val, color) = stats[i];
                float x = i * w;

                canvas.FillColor = Color.FromArgb("#F5F5F5");
                canvas.FillRoundedRectangle(x + 5, y, w - 10, 70, 8);

                canvas.FontColor = Color.FromArgb(color);
                canvas.FontSize = 28;
                canvas.DrawString(val, x + 5, y + 5, w - 10, 40,
                    HorizontalAlignment.Center, VerticalAlignment.Top);

                canvas.FontColor = Color.FromArgb("#666666");
                canvas.FontSize = 11;
                canvas.DrawString(label, x + 5, y + 45, w - 10, 20,
                    HorizontalAlignment.Center, VerticalAlignment.Top);
            }
        }
    }
}