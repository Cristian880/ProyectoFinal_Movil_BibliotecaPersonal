using System.Globalization;

namespace ProyectoFinal_Movil_BibliotecaPersonal.Converters
{
    public class RatingToStarsConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            //Convierte un número entero en una cadena de emojis de estrellas.
            if (value is int rating)
                return string.Concat(// Concatena N copias del emoji ⭐
                    Enumerable.Repeat("⭐", //Repite el string "⭐"
                    Math.Clamp(rating, 0, 5)));// las veces necesarias que se le dijo como minimo 0 y maximo 5
           //asegura que nunca salga del rango 0 - 5 aunque el dato esté corrupto
            return "";
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
            => throw new NotImplementedException();//de la vista hacia el modelo solo necesario en binding TwoWay
    }
}

