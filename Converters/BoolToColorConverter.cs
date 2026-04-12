using System.Globalization;

namespace ProyectoFinal_Movil_BibliotecaPersonal.Converters
{
    public class BoolToColorConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool isRead)
                return isRead ? Color.FromArgb("#4CAF50") : Color.FromArgb("#FF9800");
            return Color.FromArgb("#FF9800");
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
