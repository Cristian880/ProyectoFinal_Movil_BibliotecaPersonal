using System.Globalization;

namespace ProyectoFinal_Movil_BibliotecaPersonal.Converters
{
    public class RatingToStarsConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is int rating)
                return string.Concat(Enumerable.Repeat("⭐", Math.Clamp(rating, 0, 5)));
            return "";
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
