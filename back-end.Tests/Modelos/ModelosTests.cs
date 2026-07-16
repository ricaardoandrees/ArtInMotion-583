using Usuarioo.Models;
using Xunit;

namespace back_end.Tests.Modelos;

// Casos oficiales del Plan de Pruebas Unitarias v1.0, Bloque B0 (UT-B0-01 a UT-B0-04).
// Verifican que los modelos base se inicializan correctamente y exponen
// las propiedades requeridas. Son la base de todos los demás bloques.
//
// Nota: el documento original usa "plantilla.Colores" como atajo; la
// propiedad real en el código es "PaletaColores". También usa
// "FechaNacimiento=DateTime.Now" de ejemplo, pero el modelo real define
// FechaNacimiento como string, así que se ajustó a un valor string.
public class ModelosTests
{
    [Fact]
    public void UT_B0_01_Plantilla_PaletaColoresIniciaVaciaNoNula()
    {
        // Arrange & Act
        var plantilla = new Plantilla();

        // Assert
        Assert.NotNull(plantilla.PaletaColores);
        Assert.Empty(plantilla.PaletaColores);
    }

    [Fact]
    public void UT_B0_02_Plantilla_ExponePropiedadesHeredadasDeActividad()
    {
        // Arrange & Act
        var plantilla = new Plantilla
        {
            NombreCreacion = "X",
            IdCreador = "U1",
            Puntaje = 5,
            ImagenUrl = "img.png"
        };

        // Assert
        Assert.Equal("X", plantilla.NombreCreacion);
        Assert.Equal("U1", plantilla.IdCreador);
        Assert.Equal(5, plantilla.Puntaje);
        Assert.Equal("img.png", plantilla.ImagenUrl);
    }

    [Fact]
    public void UT_B0_03_DibujoRequest_ExponePropiedadesHeredadasDeActividad()
    {
        // Arrange & Act
        var dibujo = new DibujoRequest
        {
            NombreCreacion = "Y",
            IdCreador = "U2",
            Puntaje = 3,
            ImagenUrl = "d.png"
        };

        // Assert
        Assert.Equal("Y", dibujo.NombreCreacion);
        Assert.Equal("U2", dibujo.IdCreador);
        Assert.Equal(3, dibujo.Puntaje);
        Assert.Equal("d.png", dibujo.ImagenUrl);
    }

    [Fact]
    public void UT_B0_04_Usuario_ExponeLosCincoCamposObligatorios()
    {
        // Arrange & Act
        var usuario = new Usuario
        {
            Email = "a@b.com",
            Nombre = "Ana",
            FechaNacimiento = "2000-01-01",
            Contrasena = "123",
            Uuid = "uuid-1"
        };

        // Assert
        Assert.Equal("a@b.com", usuario.Email);
        Assert.Equal("Ana", usuario.Nombre);
        Assert.Equal("2000-01-01", usuario.FechaNacimiento);
        Assert.Equal("123", usuario.Contrasena);
        Assert.Equal("uuid-1", usuario.Uuid);
    }
}
