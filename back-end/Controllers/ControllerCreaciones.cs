using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

[ApiController]
[Route("api/[controller]")]
public class CreacionesController : ControllerBase
{
    private readonly string _rutaPlantillasJson = @"../back-end/Datos/plantilla.json";
    private readonly string _rutaDibujosJson = @"../back-end/Datos/Dibujo/dibujos.json";
    private readonly string _carpetaDibujos = @"../back-end/Datos/Dibujo";
    private readonly string _carpetaPlantillas = @"../back-end/Datos/ImagenesUso";

    [HttpGet("plantillas")]
    public ActionResult<List<Plantilla>> GetPlantillas()
    {
        if (!System.IO.File.Exists(_rutaPlantillasJson))
            return Ok(new List<Plantilla>());

        var json = System.IO.File.ReadAllText(_rutaPlantillasJson);
        var plantillas = JsonConvert.DeserializeObject<List<Plantilla>>(json) ?? new List<Plantilla>();
        return Ok(plantillas);
    }

    [HttpGet("dibujos")]
    public ActionResult<List<DibujoRequest>> GetDibujos()
    {
        if (!System.IO.File.Exists(_rutaDibujosJson))
            return Ok(new List<DibujoRequest>());

        var json = System.IO.File.ReadAllText(_rutaDibujosJson);
        var dibujos = JsonConvert.DeserializeObject<List<DibujoRequest>>(json) ?? new List<DibujoRequest>();
        return Ok(dibujos);
    }

    // Eliminar una creación (plantilla o dibujo) y su archivo local
    [HttpDelete("{tipo}/{nombre}")]
    public IActionResult Delete(string tipo, string nombre)
{
    if (tipo == "plantilla")
    {
        var json = System.IO.File.ReadAllText(_rutaPlantillasJson);
        var plantillas = JsonConvert.DeserializeObject<List<Plantilla>>(json) ?? new List<Plantilla>();
        var plantilla = plantillas.FirstOrDefault(p => CreacionesValidaciones.CriterioEliminarPlantilla(p.ImagenUrl, nombre));
        if (plantilla == null) return NotFound();

        // Eliminar del JSON
        plantillas.Remove(plantilla);
        System.IO.File.WriteAllText(_rutaPlantillasJson, JsonConvert.SerializeObject(plantillas));

        // Eliminar el archivo físico de la imagen
        if (!string.IsNullOrEmpty(plantilla.ImagenUrl))
        {
            var rutaImagen = Path.Combine(_carpetaPlantillas, plantilla.ImagenUrl);
            if (System.IO.File.Exists(rutaImagen))
            {
                System.IO.File.Delete(rutaImagen);
            }
        }
        return NoContent();
    }
    else if (tipo == "dibujo")
    {
        var json = System.IO.File.ReadAllText(_rutaDibujosJson);
        var dibujos = JsonConvert.DeserializeObject<List<DibujoRequest>>(json) ?? new List<DibujoRequest>();
        var dibujo = dibujos.FirstOrDefault(d =>!string.IsNullOrEmpty(d.ImagenUrl) && d.ImagenUrl.Split('/').Last().Equals(nombre, StringComparison.OrdinalIgnoreCase));
        if (dibujo == null) return NotFound();

        // Eliminar del JSON
        dibujos.Remove(dibujo);
        System.IO.File.WriteAllText(_rutaDibujosJson, JsonConvert.SerializeObject(dibujos));

        // Eliminar el archivo físico de la imagen
        if (!string.IsNullOrEmpty(dibujo.ImagenUrl)){
            var rutaImagen = Path.Combine(_carpetaDibujos, dibujo.ImagenUrl.Split('/').Last());
            if (System.IO.File.Exists(rutaImagen)){
                System.IO.File.Delete(rutaImagen);
            }
        }
        return NoContent();
    }
    return BadRequest("Tipo no válido");
}
    [HttpPost("calificar")]
    public IActionResult Calificar([FromBody] Dictionary<string, object> calificacion)
    {
        var datos = CreacionesValidaciones.ParsearCalificacion(calificacion);

        if (string.IsNullOrWhiteSpace(datos.Tipo) || string.IsNullOrWhiteSpace(datos.ImagenUrl) || string.IsNullOrWhiteSpace(datos.IdCreador))
            return BadRequest("Faltan datos obligatorios.");

        if (datos.Tipo.Equals("plantilla", System.StringComparison.OrdinalIgnoreCase))
        {
            var json = System.IO.File.ReadAllText(_rutaPlantillasJson);
            var plantillas = JsonConvert.DeserializeObject<List<Plantilla>>(json) ?? new List<Plantilla>();
            var plantilla = plantillas.FirstOrDefault(p =>
                p.ImagenUrl.Trim().Equals(datos.ImagenUrl.Trim(), System.StringComparison.OrdinalIgnoreCase) &&
                p.IdCreador.Trim().Equals(datos.IdCreador.Trim(), System.StringComparison.OrdinalIgnoreCase));
            if (plantilla == null) return NotFound();

            plantilla.Puntaje = datos.Puntaje;
            System.IO.File.WriteAllText(_rutaPlantillasJson, JsonConvert.SerializeObject(plantillas));
            return Ok(new { mensaje = $"Puntaje actualizado a {plantilla.Puntaje} para {plantilla.ImagenUrl}" });
        }
        else if (datos.Tipo.Equals("dibujo", System.StringComparison.OrdinalIgnoreCase))
        {
            var json = System.IO.File.ReadAllText(_rutaDibujosJson);
            var dibujos = JsonConvert.DeserializeObject<List<DibujoRequest>>(json) ?? new List<DibujoRequest>();
            var dibujo = dibujos.FirstOrDefault(d =>
                d.ImagenUrl.Trim().Equals(datos.ImagenUrl.Trim(), System.StringComparison.OrdinalIgnoreCase) &&
                d.IdCreador.Trim().Equals(datos.IdCreador.Trim(), System.StringComparison.OrdinalIgnoreCase));
            if (dibujo == null) return NotFound();

            dibujo.Puntaje = datos.Puntaje;
            System.IO.File.WriteAllText(_rutaDibujosJson, JsonConvert.SerializeObject(dibujos));
            return Ok(new { mensaje = $"Puntaje actualizado a {dibujo.Puntaje} para {dibujo.ImagenUrl}" });
        }
        return BadRequest("Tipo no válido");
    }
}
