using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Usuarioo.Models;
using Usuarioo.Repositories;
using Xunit;

namespace back_end.Tests.Controllers;

// Pruebas unitarias del bloque B1 (Usuario).
//
// Patrón utilizado: AAA (Arrange - Act - Assert) en cada prueba.
// Aislamiento: se usa Moq para simular IUsuarioRepository, así que estas pruebas
// nunca tocan el filesystem ni el archivo usuarios.json real. El componente bajo
// prueba es exclusivamente UsuariosController; su dependencia (el repositorio)
// queda controlada por completo desde el test.
//
// Las pruebas que sí ejercitan UsuarioRepository contra un archivo real (I/O real)
// se documentan aparte como pruebas de integración (IT-B1-*), no acá.
public class UsuariosControllerTests
{
    private readonly Mock<IUsuarioRepository> _repositoryMock;
    private readonly UsuariosController _controller;

    public UsuariosControllerTests()
    {
        _repositoryMock = new Mock<IUsuarioRepository>();
        _controller = new UsuariosController(_repositoryMock.Object);
    }

    private static JsonElement CrearLoginJson(string email, string contrasena)
    {
        var json = JsonSerializer.Serialize(new { Email = email, Contrasena = contrasena });
        return JsonDocument.Parse(json).RootElement;
    }

    private static Usuario CrearUsuario(string email, string contrasena, string uuid = "")
    {
        return new Usuario
        {
            Email = email,
            Nombre = "Ricardo",
            FechaNacimiento = "2003-01-01",
            Contrasena = contrasena,
            Uuid = uuid
        };
    }

    // ---------- GuardarUsuario ----------

    [Fact]
    public void GuardarUsuario_ConEmailNuevo_RegistraElUsuarioYRetornaOk()
    {
        // Arrange
        _repositoryMock.Setup(r => r.ObtenerTodos()).Returns(new List<Usuario>());
        var nuevoUsuario = CrearUsuario("ricardo@ucab.edu.ve", "clave123");

        // Act
        var resultado = _controller.GuardarUsuario(nuevoUsuario);

        // Assert
        Assert.IsType<OkObjectResult>(resultado);
        _repositoryMock.Verify(r => r.Agregar(nuevoUsuario), Times.Once);
    }

    [Fact]
    public void GuardarUsuario_ConEmailYaRegistrado_RetornaBadRequestYNoGuardaNada()
    {
        // Arrange
        var existente = CrearUsuario("ricardo@ucab.edu.ve", "clave123", uuid: "uuid-1");
        _repositoryMock.Setup(r => r.ObtenerTodos()).Returns(new List<Usuario> { existente });
        var nuevoUsuario = CrearUsuario("ricardo@ucab.edu.ve", "otraClave");

        // Act
        var resultado = _controller.GuardarUsuario(nuevoUsuario);

        // Assert
        Assert.IsType<BadRequestObjectResult>(resultado);
        _repositoryMock.Verify(r => r.Agregar(It.IsAny<Usuario>()), Times.Never);
    }

    [Fact]
    public void GuardarUsuario_ComparaEmailsIgnorandoMayusculasYEspacios_DetectaDuplicado()
    {
        // Arrange: el email existente está en minúsculas y sin espacios,
        // el nuevo llega con mayúsculas y espacios extra (caso real de formulario web)
        var existente = CrearUsuario("ricardo@ucab.edu.ve", "clave123", uuid: "uuid-1");
        _repositoryMock.Setup(r => r.ObtenerTodos()).Returns(new List<Usuario> { existente });
        var nuevoUsuario = CrearUsuario("  RICARDO@UCAB.EDU.VE  ", "otraClave");

        // Act
        var resultado = _controller.GuardarUsuario(nuevoUsuario);

        // Assert
        Assert.IsType<BadRequestObjectResult>(resultado);
    }

    // ---------- Login ----------

    [Fact]
    public void Login_ConCredencialesFaltantes_RetornaBadRequestSinConsultarRepositorio()
    {
        // Arrange
        var jsonIncompleto = JsonDocument.Parse("{\"Email\":\"ricardo@ucab.edu.ve\"}").RootElement;

        // Act
        var resultado = _controller.Login(jsonIncompleto);

        // Assert
        Assert.IsType<BadRequestObjectResult>(resultado);
        _repositoryMock.Verify(r => r.ObtenerTodos(), Times.Never);
    }

    [Fact]
    public void Login_ConUsuarioInexistente_RetornaBadRequest()
    {
        // Arrange
        _repositoryMock.Setup(r => r.ObtenerTodos()).Returns(new List<Usuario>());
        var login = CrearLoginJson("nadie@ucab.edu.ve", "clave123");

        // Act
        var resultado = _controller.Login(login);

        // Assert
        Assert.IsType<BadRequestObjectResult>(resultado);
    }

    [Fact]
    public void Login_ConContrasenaIncorrecta_RetornaBadRequest()
    {
        // Arrange
        var existente = CrearUsuario("ricardo@ucab.edu.ve", "claveCorrecta", uuid: "uuid-1");
        _repositoryMock.Setup(r => r.ObtenerTodos()).Returns(new List<Usuario> { existente });
        var login = CrearLoginJson("ricardo@ucab.edu.ve", "claveIncorrecta");

        // Act
        var resultado = _controller.Login(login);

        // Assert
        Assert.IsType<BadRequestObjectResult>(resultado);
    }

    [Fact]
    public void Login_ConCredencialesCorrectas_RetornaOk()
    {
        // Arrange
        var existente = CrearUsuario("ricardo@ucab.edu.ve", "claveCorrecta", uuid: "uuid-1");
        _repositoryMock.Setup(r => r.ObtenerTodos()).Returns(new List<Usuario> { existente });
        var login = CrearLoginJson("ricardo@ucab.edu.ve", "claveCorrecta");

        // Act
        var resultado = _controller.Login(login);

        // Assert
        Assert.IsType<OkObjectResult>(resultado);
    }

    // ---------- SearchUser ----------

    [Fact]
    public void SearchUser_ConCredencialesCorrectas_RetornaElUsuarioCompleto()
    {
        // Arrange
        var existente = CrearUsuario("ricardo@ucab.edu.ve", "claveCorrecta", uuid: "uuid-1");
        _repositoryMock.Setup(r => r.ObtenerTodos()).Returns(new List<Usuario> { existente });
        var login = CrearLoginJson("ricardo@ucab.edu.ve", "claveCorrecta");

        // Act
        var resultado = _controller.SearchUser(login);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(resultado);
        var usuarioDevuelto = Assert.IsType<Usuario>(ok.Value);
        Assert.Equal(existente.Email, usuarioDevuelto.Email);
    }

    [Fact]
    public void SearchUser_ConUsuarioInexistente_RetornaBadRequest()
    {
        // Arrange
        _repositoryMock.Setup(r => r.ObtenerTodos()).Returns(new List<Usuario>());
        var login = CrearLoginJson("nadie@ucab.edu.ve", "clave123");

        // Act
        var resultado = _controller.SearchUser(login);

        // Assert
        Assert.IsType<BadRequestObjectResult>(resultado);
    }
}
