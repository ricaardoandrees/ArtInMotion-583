using System;
using Xunit;

namespace back_end.Tests.Logica;

// Casos oficiales del Plan de Pruebas Unitarias v1.0, Bloque B2 (UT-B2-01 a UT-B2-04).
public class CatalogoValidacionesTests
{
    [Fact]
    public void UT_B2_01_CalcularNombreSubida_DevuelveElMayorNumeroMasUno()
    {
        // Arrange
        var existentes = new[] { "subida1.png", "subida3.png", "subida2.png" };

        // Act
        var resultado = CatalogoValidaciones.CalcularNombreSubida(existentes, ".png");

        // Assert
        Assert.Equal("subida4.png", resultado);
    }

    [Fact]
    public void UT_B2_02_CalcularNombreSubida_ConListaVacia_DevuelveSubida1()
    {
        // Arrange
        var existentes = Array.Empty<string>();

        // Act
        var resultado = CatalogoValidaciones.CalcularNombreSubida(existentes, ".png");

        // Assert
        Assert.Equal("subida1.png", resultado);
    }

    [Fact]
    public void UT_B2_03_ValidarExtensionArchivo_AceptaExtensionesPermitidas()
    {
        // Arrange & Act
        var resultado = CatalogoValidaciones.ValidarExtensionArchivo("foto.JPG");

        // Assert
        Assert.True(resultado, "Debe aceptar extensiones de imagen ignorando mayúsculas.");
    }

    [Fact]
    public void UT_B2_04_ValidarExtensionArchivo_RechazaExtensionesNoPermitidas()
    {
        // Arrange & Act
        var resultado = CatalogoValidaciones.ValidarExtensionArchivo("documento.pdf");

        // Assert
        Assert.False(resultado, "Debe rechazar extensiones fuera de la lista permitida.");
    }
}
