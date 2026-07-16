using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Usuarioo.Logica;
using Usuarioo.Models;
using Usuarioo.Repositories;

[ApiController]
[Route("api/usuarios")]
public class UsuariosController : ControllerBase
{
    private readonly IUsuarioRepository _usuarioRepository;

    public UsuariosController(IUsuarioRepository usuarioRepository)
    {
        _usuarioRepository = usuarioRepository;
    }

    [HttpPost("guardar")]
    public IActionResult GuardarUsuario([FromBody] Usuario usuario)
    {
        List<Usuario> usuarios = _usuarioRepository.ObtenerTodos();

        // Busca si ya existe un usuario con el mismo email (ignorando mayúsculas/minúsculas)
        bool existe = UsuarioValidaciones.DetectarEmailDuplicado(usuarios, usuario.Email);

        if (existe)
        {
            return BadRequest(new { mensaje = "emailAlreadyRegistered" });
        }

        // Si no existe, guarda el usuario
        _usuarioRepository.Agregar(usuario);

        return Ok(new { mensaje = "userSuccesfullyRegistered" });
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] JsonElement login)
    {
        // Extraer los valores del JSON recibido
        if (!login.TryGetProperty("Email", out JsonElement emailElement) ||
            !login.TryGetProperty("Contrasena", out JsonElement contrasenaElement))
        {
            return BadRequest(new { mensaje = "Faltan datos de login" });
        }

        string email = emailElement.GetString() ?? "";
        string contrasena = contrasenaElement.GetString() ?? "";

        List<Usuario> usuarios = _usuarioRepository.ObtenerTodos();

        // Buscar usuario por email (ignorando mayúsculas/minúsculas)
        var usuario = usuarios.FirstOrDefault(u =>
            u.Email.Trim().ToLower() == email.Trim().ToLower()
        );

        if (usuario == null)
        {
            return BadRequest(new { mensaje = "userNotFound" });
        }

        // Comparar contraseñas
        if (UsuarioValidaciones.VerificarContrasena(usuario.Contrasena, contrasena))
        {
            return Ok(new { mensaje = "loginSuccesful" });
        }
        else
        {
            return BadRequest(new { mensaje = "incorrectPassword" });
        }
    }


    [HttpPost("search")]
    public IActionResult SearchUser([FromBody] JsonElement login)
    {
        // Extraer los valores del JSON recibido
        if (!login.TryGetProperty("Email", out JsonElement emailElement) ||
            !login.TryGetProperty("Contrasena", out JsonElement contrasenaElement))
        {
            return BadRequest(new { mensaje = "Faltan datos de login" });
        }

        string email = emailElement.GetString() ?? "";
        string contrasena = contrasenaElement.GetString() ?? "";

        List<Usuario> usuarios = _usuarioRepository.ObtenerTodos();

        // Buscar usuario por email (ignorando mayúsculas/minúsculas)
        var usuario = usuarios.FirstOrDefault(u =>
            u.Email.Trim().ToLower() == email.Trim().ToLower()
        );

        if (usuario == null)
        {
            // Si no existe, retorna mensaje de usuario no encontrado
            return BadRequest(new { mensaje = "userNotFound" });
        }

        // Comparar contraseñas
        if (UsuarioValidaciones.VerificarContrasena(usuario.Contrasena, contrasena))
        {
            // Devuelve el objeto usuario al cliente
            return Ok(usuario);
        }
        else
        {
            return BadRequest(new { mensaje = "incorrectPassword" });
        }
    }

}
