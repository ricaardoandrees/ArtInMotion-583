using System;
using System.Collections.Generic;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

// Lógica de color y flood fill de ImagenController extraída a métodos
// estáticos, según el Plan de Pruebas Unitarias v1.0 (Bloque B3).
// Operan sobre Image<Rgba32> en memoria (SixLabors.ImageSharp permite
// crear imágenes sin tocar disco), por lo que son pruebas unitarias reales.
public static class ColorLogica
{
    // UT-B3-01 / UT-B3-02
    public static bool ColoresSimilares(Rgba32 c1, Rgba32 c2, int tolerancia = 30)
    {
        return Math.Abs(c1.R - c2.R) <= tolerancia &&
               Math.Abs(c1.G - c2.G) <= tolerancia &&
               Math.Abs(c1.B - c2.B) <= tolerancia;
    }

    // UT-B3-03 / UT-B3-04
    // Nota: "umbral" es un mínimo de brillo por canal (no una distancia
    // al blanco). En el código real siempre se llama con umbral=240.
    public static bool EsCasiBlanco(Rgba32 color, int umbral = 240)
    {
        return color.R >= umbral && color.G >= umbral && color.B >= umbral;
    }

    // UT-B3-06
    public static Rgba32 ConvertirHexARgb(string colorHex)
    {
        return new Rgba32(
            Convert.ToByte(colorHex.Substring(1, 2), 16),
            Convert.ToByte(colorHex.Substring(3, 2), 16),
            Convert.ToByte(colorHex.Substring(5, 2), 16)
        );
    }

    // UT-B3-07
    public static List<Rgba32> ParsearPaleta(string paletaColores)
    {
        var coloresPaleta = new List<Rgba32>();
        if (!string.IsNullOrEmpty(paletaColores))
        {
            var colores = paletaColores.Split(',');
            foreach (var hex in colores)
            {
                if (hex.Length == 7 && hex[0] == '#')
                {
                    coloresPaleta.Add(ConvertirHexARgb(hex));
                }
            }
        }
        return coloresPaleta;
    }

    // UT-B3-05
    public static void FloodFill(Image<Rgba32> image, int x, int y, Rgba32 targetColor, Rgba32 fillColor, int tolerancia = 30, bool borrarTodo = false)
    {
        if (ColoresSimilares(targetColor, fillColor, tolerancia)) return;
        var pixels = new Queue<(int, int)>();
        pixels.Enqueue((x, y));
        while (pixels.Count > 0)
        {
            var (px, py) = pixels.Dequeue();
            if (px < 0 || px >= image.Width || py < 0 || py >= image.Height)
                continue;
            Rgba32 currentColor = image[px, py];
            if ((borrarTodo || EsCasiBlanco(currentColor, 240)) && !ColoresSimilares(currentColor, fillColor, 10))
            {
                image[px, py] = fillColor;
                pixels.Enqueue((px + 1, py));
                pixels.Enqueue((px - 1, py));
                pixels.Enqueue((px, py + 1));
                pixels.Enqueue((px, py - 1));
            }
        }
    }

    // Usado por "pintaryborra" cuando el color a pintar es blanco (borrador)
    public static void FloodFillSoloColoresPaleta(Image<Rgba32> image, int x, int y, Rgba32 targetColor, Rgba32 fillColor, Rgba32[] coloresPaleta, int tolerancia = 10)
    {
        int width = image.Width;
        int height = image.Height;
        bool[,] visitado = new bool[width, height];
        var pixels = new Queue<(int, int)>();
        pixels.Enqueue((x, y));
        while (pixels.Count > 0)
        {
            var (px, py) = pixels.Dequeue();
            if (px < 0 || px >= width || py < 0 || py >= height)
                continue;
            if (visitado[px, py])
                continue;
            Rgba32 currentColor = image[px, py];
            bool esPaleta = false;
            foreach (var color in coloresPaleta)
            {
                if (ColoresSimilares(currentColor, color, tolerancia))
                {
                    esPaleta = true;
                    break;
                }
            }
            if (esPaleta)
            {
                image[px, py] = fillColor;
                visitado[px, py] = true;
                pixels.Enqueue((px + 1, py));
                pixels.Enqueue((px - 1, py));
                pixels.Enqueue((px, py + 1));
                pixels.Enqueue((px, py - 1));
            }
        }
    }
}
