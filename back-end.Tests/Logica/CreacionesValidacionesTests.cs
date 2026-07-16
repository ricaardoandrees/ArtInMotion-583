using System.Collections.Generic;
using Xunit;

namespace back_end.Tests.Logica;

// Casos oficiales del Plan de Pruebas Unitarias v1.0, Bloque B2 (UT-B2-05 y UT-B2-06).
//
// Nota sobre UT-B2-06: el documento original describe la entrada como una
// cadena delimitada por "|" (ej. "tipo=plantilla|imagen=flores.jpg|..."),
// pero el endpoint real (/api/creaciones/calificar) recibe un
// Dictionary<string, object> deserializado del body JSON — nunca parsea
// una cadena con ese formato. Se ajustó la entrada del test a un
// Dictionary para que corresponda con el código real, preservando el
// mismo objetivo: extraer Tipo, ImagenUrl, IdCreador y Puntaje.
public class CreacionesValidacionesTests
{
    [Fact]
    public void UT_B2_05_CriterioEliminarPlantilla_ComparaImagenUrlExacto()
    {
        // Arrange
        string imagenUrlPlantilla = "imgs/flores.jpg";
        string urlBuscada = "imgs/flores.jpg";

        // Act
        bool resultado = CreacionesValidaciones.CriterioEliminarPlantilla(imagenUrlPlantilla, urlBuscada);

        // Assert
        Assert.True(resultado, "Debe coincidir cuando el ImagenUrl es exactamente igual.");
    }

    [Fact]
    public void UT_B2_06_ParsearCalificacion_ExtraeTipoImagenCreadorYValor()
    {
        // Arrange
        var datos = new Dictionary<string, object>
        {
            { "tipo", "plantilla" },
            { "imagenurl", "flores.jpg" },
            { "idcreador", "uuid-1" },
            { "calificacion", 8 }
        };

        // Act
        var resultado = CreacionesValidaciones.ParsearCalificacion(datos);

        // Assert
        Assert.Equal("plantilla", resultado.Tipo);
        Assert.Equal("flores.jpg", resultado.ImagenUrl);
        Assert.Equal("uuid-1", resultado.IdCreador);
        Assert.Equal(8, resultado.Puntaje);
    }
}
