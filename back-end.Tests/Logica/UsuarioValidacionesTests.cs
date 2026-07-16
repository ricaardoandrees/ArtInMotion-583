using Usuarioo.Logica;
using Usuarioo.Models;
using Xunit;

namespace back_end.Tests.Logica;

// Casos oficiales del Plan de Pruebas Unitarias v1.0, Bloque B1 (UT-B1-01 a UT-B1-04).
// Pruebas puras: no tocan usuarios.json ni levantan el API, solo instancian
// datos en memoria y llaman al método estático directo.
public class UsuarioValidacionesTests
{
    private static Usuario CrearUsuario(string email)
    {
        return new Usuario
        {
            Email = email,
            Nombre = "Nombre de prueba",
            FechaNacimiento = "2000-01-01",
            Contrasena = "clave",
            Uuid = "uuid-1"
        };
    }

    [Fact]
    public void UT_B1_01_DetectarEmailDuplicado_IgnoraMayusculas()
    {
        // Arrange
        var usuarios = new List<Usuario> { CrearUsuario("usuario@correo.com") };

        // Act
        bool resultado = UsuarioValidaciones.DetectarEmailDuplicado(usuarios, "USUARIO@CORREO.COM");

        // Assert
        Assert.True(resultado, "Debe detectar el duplicado ignorando mayúsculas/minúsculas.");
    }

    [Fact]
    public void UT_B1_02_DetectarEmailDuplicado_IgnoraEspacios()
    {
        // Arrange
        var usuarios = new List<Usuario> { CrearUsuario("usuario@correo.com") };

        // Act
        bool resultado = UsuarioValidaciones.DetectarEmailDuplicado(usuarios, " usuario@correo.com ");

        // Assert
        Assert.True(resultado, "Debe detectar el duplicado ignorando espacios al inicio/final.");
    }

    [Fact]
    public void UT_B1_03_VerificarContrasena_AceptaCorrecta()
    {
        // Arrange
        string almacenada = "pass123";
        string ingresada = "pass123";

        // Act
        bool resultado = UsuarioValidaciones.VerificarContrasena(almacenada, ingresada);

        // Assert
        Assert.True(resultado, "Debe aceptar cuando la contraseña ingresada coincide exactamente.");
    }

    [Fact]
    public void UT_B1_04_VerificarContrasena_RechazaIncorrecta()
    {
        // Arrange
        string almacenada = "pass123";
        string ingresada = "wrongpass";

        // Act
        bool resultado = UsuarioValidaciones.VerificarContrasena(almacenada, ingresada);

        // Assert
        Assert.False(resultado, "Debe rechazar cuando la contraseña ingresada no coincide.");
    }
}
