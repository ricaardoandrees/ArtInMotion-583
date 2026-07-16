using Xunit;

namespace back_end.Tests.Logica;

// Casos oficiales del Plan de Pruebas Unitarias v1.0, Bloque B4 (UT-B4-01 a UT-B4-04).
//
// Nota sobre UT-B4-04: el documento describe la entrada como "cadena
// base64 válida", pero el código real (DecodificarBase64) usa un patrón
// que exige el prefijo "data:image/...;base64," tal como lo manda el
// frontend — un base64 "pelado" sin ese prefijo no matchea y produciría
// un arreglo vacío, no el resultado esperado. Se ajustó la entrada del
// test para incluir ese prefijo, preservando la intención del caso.
public class DibujoLogicaTests
{
    [Fact]
    public void UT_B4_01_ExtraerUUIDDeNombre_ConGuionBajo_DevuelveUltimoSegmento()
    {
        // Arrange
        string nombre = "dibujo_uuid-abc123";

        // Act
        string resultado = DibujoLogica.ExtraerUUIDDeNombre(nombre);

        // Assert
        Assert.Equal("uuid-abc123", resultado);
    }

    [Fact]
    public void UT_B4_02_ExtraerUUIDDeNombre_SinGuionBajo_DevuelveDesconocido()
    {
        // Arrange
        string nombre = "dibujosinguion";

        // Act
        string resultado = DibujoLogica.ExtraerUUIDDeNombre(nombre);

        // Assert
        Assert.Equal("desconocido", resultado);
    }

    [Fact]
    public void UT_B4_03_LimpiarNombre_EliminaCaracteresNoPermitidos()
    {
        // Arrange
        string nombre = "Mi Dibujo #1 / Arte!";

        // Act
        string resultado = DibujoLogica.LimpiarNombre(nombre);

        // Assert
        Assert.Equal("MiDibujo1Arte", resultado);
    }

    [Fact]
    public void UT_B4_04_DecodificarBase64_ConDataUriValido_DevuelveBytesNoVacios()
    {
        // Arrange: PNG mínimo de 1x1 píxel, con el prefijo data-URI real
        // que el frontend efectivamente envía.
        string dataUri = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR42mNkYAAAAAYAAjCB0C8AAAAASUVORK5CYII=";

        // Act
        byte[] resultado = DibujoLogica.DecodificarBase64(dataUri);

        // Assert
        Assert.NotNull(resultado);
        Assert.True(resultado.Length > 0);
    }
}
