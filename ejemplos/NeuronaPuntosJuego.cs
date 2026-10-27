using System;

public class Program
{
    // Una neurona con dos entradas y activación lineal: f(z) = z.
    static double Predecir(double monedas, double gemas,
                           double w1, double w2, double sesgo)
    {
        return monedas * w1 + gemas * w2 + sesgo;
    }

    public static void Main()
    {
        // Cada fila contiene: monedas, gemas y puntos reales de la partida.
        double[,] datos =
        {
            { 0, 0,  3 },
            { 1, 0,  5 },
            { 0, 1,  8 },
            { 2, 1, 12 },
            { 1, 2, 15 },
            { 3, 3, 24 }
        };

        // Los valores que la neurona debe aprender empiezan en cero.
        double w1 = 0, w2 = 0, sesgo = 0;
        double tasa = 0.02; // Tamaño del paso de cada ajuste.
        int epocas = 6000;  // Número de pasadas por todos los datos.
        int cantidad = datos.GetLength(0);

        for (int epoca = 0; epoca < epocas; epoca++)
        {
            // Acumulamos gradientes antes de actualizar los parámetros.
            double gradW1 = 0, gradW2 = 0, gradSesgo = 0;

            for (int i = 0; i < cantidad; i++)
            {
                double x1 = datos[i, 0]; // Monedas de esta partida.
                double x2 = datos[i, 1]; // Gemas de esta partida.
                double y = datos[i, 2];  // Puntos que debería predecir.

                double prediccion = Predecir(x1, x2, w1, w2, sesgo);
                double error = prediccion - y;

                // Derivadas de la pérdida: L = suma(error²) / (2 * cantidad).
                gradW1 += error * x1;
                gradW2 += error * x2;
                gradSesgo += error;
            }

            // Descenso de gradiente: restamos el gradiente medio de cada parámetro.
            w1 -= tasa * gradW1 / cantidad;
            w2 -= tasa * gradW2 / cantidad;
            sesgo -= tasa * gradSesgo / cantidad;
        }

        // El entrenamiento aproxima w1 = 2, w2 = 5 y sesgo = 3.
        Console.WriteLine("w1 = {0:F4}, w2 = {1:F4}, sesgo = {2:F4}",
                          w1, w2, sesgo);

        // Esta combinación no aparece en los datos de entrenamiento.
        double puntos = Predecir(4, 2, w1, w2, sesgo);
        Console.WriteLine("4 monedas y 2 gemas: {0:F2} puntos", puntos);
    }
}
