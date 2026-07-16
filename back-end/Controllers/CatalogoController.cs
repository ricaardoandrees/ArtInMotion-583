using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class CatalogoController : ControllerBase
{
    private readonly string _rutaImagenes = @"../back-end/Datos/Imagenes";

    // GET: api/catalogo
    [HttpGet]
    public ActionResult<List<string>> Get()
    {
        if (!Directory.Exists(_rutaImagenes))
            return Ok(new List<string>());

        var archivos = Directory.GetFiles(_rutaImagenes)
            .Select(nombreArchivo => "/Imagenes/" + Path.GetFileName(nombreArchivo))
            .ToList();

        return Ok(archivos);
    }

    // POST: api/catalogo/subir
    [HttpPost("subir")]
    public async Task<IActionResult> SubirPlantilla([FromForm] IFormFile archivo)
    {
        if (archivo == null || archivo.Length == 0)
            return BadRequest("No se seleccionó ningún archivo.");

        if (!CatalogoValidaciones.ValidarExtensionArchivo(archivo.FileName))
            return BadRequest("Solo se permiten archivos de imagen.");

        var extension = Path.GetExtension(archivo.FileName).ToLowerInvariant();

        // Crea la carpeta si no existe
        if (!Directory.Exists(_rutaImagenes))
            Directory.CreateDirectory(_rutaImagenes);

        // Busca el mayor número de subida ya existente
        var archivosExistentes = Directory.GetFiles(_rutaImagenes, "subida*.*");
        var nombreArchivo = CatalogoValidaciones.CalcularNombreSubida(archivosExistentes, extension);
        var rutaDestino = Path.Combine(_rutaImagenes, nombreArchivo);

        using (var stream = new FileStream(rutaDestino, FileMode.Create))
        {
            await archivo.CopyToAsync(stream);
        }

        // Devuelve la URL relativa
        return Ok(new { url = "/Imagenes/" + nombreArchivo });
    }
}
