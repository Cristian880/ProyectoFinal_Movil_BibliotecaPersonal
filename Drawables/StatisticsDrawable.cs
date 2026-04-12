namespace ProyectoFinal_Movil_BibliotecaPersonal.Drawables
{
    public class StatisticsDrawable : IDrawable
    {
        public int TotalBooks { get; set; }
        public int ReadBooks { get; set; }
        public int UnreadBooks { get; set; }
        public Dictionary<string, int> BooksByGenre { get; set; } = new();

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            canvas.Antialias = true;
            DrawPieChart(canvas, dirtyRect);
            DrawBarChart(canvas, dirtyRect);
            DrawStatNumbers(canvas, dirtyRect);
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

            // Arco leídos (verde)
            canvas.StrokeColor = Color.FromArgb("#4CAF50");
            canvas.StrokeSize = 22;
            canvas.DrawArc(cx - radius, cy - radius, radius * 2, radius * 2,
                -90, -90 + readAngle, true, false);

            // Arco pendientes (naranja)
            if (UnreadBooks > 0)
            {
                canvas.StrokeColor = Color.FromArgb("#FF9800");
                canvas.StrokeSize = 22;
                canvas.DrawArc(cx - radius, cy - radius, radius * 2, radius * 2,
                    -90 + readAngle, 270, true, false);
            }

            // Texto centro
            canvas.FontColor = Color.FromArgb("#333333");
            canvas.FontSize = 14;
            canvas.DrawString($"{(TotalBooks > 0 ? ReadBooks * 100 / TotalBooks : 0)}%",
                cx - 20, cy - 10, 40, 20, HorizontalAlignment.Center, VerticalAlignment.Center);

            // Leyenda
            canvas.FillColor = Color.FromArgb("#4CAF50");
            canvas.FillRectangle(cx + radius + 10, cy - 20, 12, 12);
            canvas.FontColor = Color.FromArgb("#333333");
            canvas.FontSize = 11;
            canvas.DrawString($"Leídos ({ReadBooks})", cx + radius + 26, cy - 22, 90, 16,
                HorizontalAlignment.Left, VerticalAlignment.Top);

            canvas.FillColor = Color.FromArgb("#FF9800");
            canvas.FillRectangle(cx + radius + 10, cy, 12, 12);
            canvas.DrawString($"Pendientes ({UnreadBooks})", cx + radius + 26, cy - 2, 100, 16,
                HorizontalAlignment.Left, VerticalAlignment.Top);
        }

        private void DrawBarChart(ICanvas canvas, RectF dirtyRect)
        {
            if (BooksByGenre.Count == 0) return;

            float startY = 240f;
            float chartHeight = 160f;
            float barWidth = Math.Min(40f, (dirtyRect.Width - 40) / BooksByGenre.Count);
            int maxVal = BooksByGenre.Values.Max();
            float startX = 30f;

            // Título
            canvas.FontColor = Color.FromArgb("#333333");
            canvas.FontSize = 13;
            canvas.DrawString("Libros por Género", 10, startY - 20, dirtyRect.Width - 20, 20,
                HorizontalAlignment.Left, VerticalAlignment.Top);

            // Línea base
            canvas.StrokeColor = Color.FromArgb("#CCCCCC");
            canvas.StrokeSize = 1;
            canvas.DrawLine(startX, startY + chartHeight, dirtyRect.Width - 10, startY + chartHeight);

            var colors = new[] { "#8B4513", "#D2691E", "#CD853F", "#DEB887", "#A0522D",
                             "#6B3A2A", "#C4884E", "#E8A87C", "#9B6B47", "#B8860B" };
            int i = 0;
            foreach (var (genre, count) in BooksByGenre.Take(10))
            {
                float barHeight = maxVal > 0 ? (float)count / maxVal * chartHeight : 0;
                float x = startX + i * (barWidth + 5);
                float y = startY + chartHeight - barHeight;

                canvas.FillColor = Color.FromArgb(colors[i % colors.Length]);
                canvas.FillRectangle(x, y, barWidth, barHeight);

                // Número encima
                canvas.FontColor = Color.FromArgb("#333333");
                canvas.FontSize = 10;
                canvas.DrawString(count.ToString(), x, y - 14, barWidth, 14,
                    HorizontalAlignment.Center, VerticalAlignment.Top);

                // Etiqueta abajo (rotada no disponible en ICanvas básico, ponemos abreviatura)
                var label = genre.Length > 5 ? genre[..5] : genre;
                canvas.FontSize = 9;
                canvas.DrawString(label, x, startY + chartHeight + 2, barWidth, 14,
                    HorizontalAlignment.Center, VerticalAlignment.Top);

                i++;
            }
        }

        private void DrawStatNumbers(ICanvas canvas, RectF dirtyRect)
        {
            float y = 450f;
            float w = dirtyRect.Width / 3f;

            var stats = new (string Label, string Value, string Color)[]
            {
            ("Total", TotalBooks.ToString(), "#8B4513"),
            ("Leídos", ReadBooks.ToString(), "#4CAF50"),
            ("Páginas", TotalPages.ToString(), "#2196F3"),
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