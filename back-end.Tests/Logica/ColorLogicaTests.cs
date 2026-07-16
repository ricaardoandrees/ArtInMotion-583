using System.Collections.Generic;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Xunit;

namespace back_end.Tests.Logica;

// Casos oficiales del Plan de Pruebas Unitarias v1.0, Bloque B3 (UT-B3-01 a UT-B3-07).
//
// Nota sobre UT-B3-03/04 (EsCasiBlanco): el documento original usa
// umbral=10 en su ejemplo, pero la fórmula real es "cada canal >= umbral"
// (un mínimo de brillo, no una distancia al blanco). Con umbral=10 casi
// cualquier color pasaría como "casi blanco", lo que contradice el propio
// resultado esperado del documento. En el código real este método siempre
// se llama con umbral=240, así que se ajustó el valor de entrada a 240 —
// el resultado esperado (true/false) de cada caso no cambió.
//
// Nota sobre UT-B3-07 (ParsearPaleta): el código real recibe un string
// separado por comas (paletaColores.Split(',')), no un arreglo de strings
// como sugiere el ejemplo del documento. Se ajustó la entrada a ese formato.
public class ColorLogicaTests
{
    [Fact]
    public void UT_B3_01_ColoresSimilares_DentroDeTolerancia_DevuelveTrue()
    {
        // Arrange
        var color1 = new Rgba32(240, 240, 240);
        var color2 = new Rgba32(245, 242, 238);

        // Act
        bool resultado = ColorLogica.ColoresSimilares(color1, color2, 15);

        // Assert
        Assert.True(resultado);
    }

    [Fact]
    public void UT_B3_02_ColoresSimilares_FueraDeTolerancia_DevuelveFalse()
    {
        // Arrange
        var color1 = new Rgba32(255, 0, 0);
        var color2 = new Rgba32(0, 0, 255);

        // Act
        bool resultado = ColorLogica.ColoresSimilares(color1, color2, 15);

        // Assert
        Assert.False(resultado);
    }

    [Fact]
    public void UT_B3_03_EsCasiBlanco_DetectaBlancoDentroDelUmbral()
    {
        // Arrange
        var color = new Rgba32(252, 252, 250);

        // Act
        bool resultado = ColorLogica.EsCasiBlanco(color, 240);

        // Assert
        Assert.True(resultado);
    }

    [Fact]
    public void UT_B3_04_EsCasiBlanco_NoConfundeColorOscuroConBlanco()
    {
        // Arrange
        var color = new Rgba32(200, 100, 50);

        // Act
        bool resultado = ColorLogica.EsCasiBlanco(color, 240);

        // Assert
        Assert.False(resultado);
    }

    [Fact]
    public void UT_B3_05_FloodFill_RellenaSoloLaZonaBlancaConectada()
    {
        // Arrange: imagen 5x5, fondo gris (no blanco), bloque blanco 3x3
        // conectado en el centro, y un píxel blanco aislado en la esquina
        // (no conectado al bloque central).
        using var image = new Image<Rgba32>(5, 5);
        var gris = new Rgba32(100, 100, 100);
        var blanco = new Rgba32(255, 255, 255);
        var rojo = new Rgba32(255, 0, 0);

        for (int px = 0; px < 5; px++)
            for (int py = 0; py < 5; py++)
                image[px, py] = gris;

        for (int px = 1; px <= 3; px++)
            for (int py = 1; py <= 3; py++)
                image[px, py] = blanco;

        image[0, 0] = blanco; // aislado, no conectado al bloque

        // Act
        ColorLogica.FloodFill(image, 2, 2, blanco, rojo);

        // Assert: el bloque conectado cambió a rojo
        for (int px = 1; px <= 3; px++)
            for (int py = 1; py <= 3; py++)
                Assert.Equal(rojo, image[px, py]);

        // Assert: el píxel aislado sigue blanco (no estaba conectado)
        Assert.Equal(blanco, image[0, 0]);

        // Assert: el fondo gris no cambió
        Assert.Equal(gris, image[4, 4]);
    }

    [Fact]
    public void UT_B3_06_ConvertirHexARgb_ConvierteCorrectamente()
    {
        // Arrange & Act
        Rgba32 resultado = ColorLogica.ConvertirHexARgb("#FF8C00");

        // Assert
        Assert.Equal((byte)255, resultado.R);
        Assert.Equal((byte)140, resultado.G);
        Assert.Equal((byte)0, resultado.B);
    }

    [Fact]
    public void UT_B3_07_ParsearPaleta_DescartaValoresMalFormados()
    {
        // Arrange
        string paleta = "#FF0000,#00FF00,INVALIDO,#0000FF";

        // Act
        List<Rgba32> resultado = ColorLogica.ParsearPaleta(paleta);

        // Assert
        Assert.Equal(3, resultado.Count);
    }
}
