using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using SixLabors.ImageSharp;//permite la manipulacion de imagenes
using SixLabors.ImageSharp.PixelFormats;//define formato de pixeles
using System.Text.Json;
[ApiController]
[Route("api/imagen")]
[EnableCors("PermitirTodo")]
public class ImagenController : ControllerBase
{
  private readonly string rutaImagenes = @"../back-end/Datos/Imagenes";
  private readonly string rutaImagenesUso = @"../back-end/Datos/ImagenesUso";

  //POST crea una copia de la imagen para que no se este reiniciando 
  [HttpPost("restaurar")]
  public IActionResult RestaurarImagenAUso([FromQuery] string nombreImagen, string idUsuario)
  {
    var rutaOriginal = Path.Combine(rutaImagenes, nombreImagen);
    var extension = Path.GetExtension(nombreImagen);
    var nombreSinExtension = Path.GetFileNameWithoutExtension(nombreImagen);
    var timestamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
    var nombreNuevo = nombreSinExtension + "_restaurada_" + idUsuario + "_" + timestamp + extension;
    var rutaCopia = Path.Combine(rutaImagenesUso, nombreNuevo);
    try
    {
      System.IO.File.Copy(rutaOriginal, rutaCopia, true);
    }
    catch (Exception ex)
    {
      return BadRequest(new { mensaje = "Error al restaurar la imagen: " + ex.Message });
    }
    return Ok(new { mensaje = "Imagen restaurada correctamente.", nombre = nombreNuevo });
  }
  //GET: Obtener imagen desde la carpeta de imagenes en uso 
  [HttpGet("obtener")]
  public IActionResult ObtenerImagen([FromQuery] string nombreImagen)
  {
    var rutaCompleta = Path.Combine(rutaImagenesUso, nombreImagen);
    if (!System.IO.File.Exists(rutaCompleta))
      return NotFound(new { mensaje = "La imagen no existe en la carpeta Imagenes." });
    var imageBytes = System.IO.File.ReadAllBytes(rutaCompleta);
    Response.Headers.Append("Access-Control-Allow-Origin", "*");
    Response.Headers.Append("Cache-Control", "no-cache, no-store, must-revalidate");
    Response.Headers.Append("Pragma", "no-cache");
    Response.Headers.Append("Expires", "0");
    return File(imageBytes, "image/png");
  }
  //Endpoint SEGUNDO SPRINT
  [HttpGet("imagen-sin-cache")]
  public IActionResult ObtenerImagenSinCache([FromQuery] string nombreImagen)
  {
    var rutaCompleta = Path.Combine(rutaImagenesUso, nombreImagen);
    if (!System.IO.File.Exists(rutaCompleta))
      return NotFound(new { mensaje = "La imagen no existe en la carpeta Imagenes." });
    
    var imageBytes = System.IO.File.ReadAllBytes(rutaCompleta);
    var timestamp = DateTime.Now.Ticks;
    Response.Headers.Append("Access-Control-Allow-Origin", "*");
    Response.Headers.Append("Cache-Control", "no-cache, no-store, must-revalidate");
    Response.Headers.Append("Pragma", "no-cache");
    Response.Headers.Append("Expires", "0");
    Response.Headers.Append("Last-Modified", DateTime.UtcNow.ToString("R"));  
    return File(imageBytes, "image/png");
  }
  /////////////////////////////////
  //POST:funciona para pintar las zonas que estan en blanco y elimina los colores de la paleta ya que los pinta de blanco
  [HttpPost("pintaryborra")]
  public IActionResult PintarYBorrar([FromQuery] string nombreImagen, [FromQuery] string colorHex, [FromQuery] int x, [FromQuery] int y, [FromQuery] string paletaColores)
  {
    var rutaCompleta = Path.Combine(rutaImagenesUso, nombreImagen);
    if (!System.IO.File.Exists(rutaCompleta))
      return NotFound("La imagen no existe en Imagenes.");
    using (var image = Image.Load<Rgba32>(rutaCompleta))
    {
      Rgba32 fillColor = ColorLogica.ConvertirHexARgb(colorHex);
      if (x < 0 || x >= image.Width || y < 0 || y >= image.Height)
        return BadRequest("Coordenadas fuera de la imagen.");
      Rgba32 targetColor = image[x, y];
      List<Rgba32> coloresPaleta = ColorLogica.ParsearPaleta(paletaColores);
      // Si el color es blanco, solo borra si el pixel es de la paleta
      if (fillColor.R == 255 && fillColor.G == 255 && fillColor.B == 255)
      {
        ColorLogica.FloodFillSoloColoresPaleta(image, x, y, targetColor, fillColor, coloresPaleta.ToArray());
        image.Save(rutaCompleta);
      }
      else if (targetColor.R > 240 && targetColor.G > 240 && targetColor.B > 240)
      {
        ColorLogica.FloodFill(image, x, y, targetColor, fillColor);
        image.Save(rutaCompleta);
      }
      using (var ms = new MemoryStream())
      {
        image.Save(ms, new SixLabors.ImageSharp.Formats.Png.PngEncoder());
        return File(ms.ToArray(), "image/png");
      }
    }
  }
  [HttpPost("renombrar")]
  public IActionResult RenombrarImagen([FromQuery] string nombreActual, [FromQuery] string nombreNuevo)
  {
    var rutaActual = Path.Combine(rutaImagenesUso, nombreActual);
    var rutaDestino = Path.Combine(rutaImagenesUso, nombreNuevo);
    if (!System.IO.File.Exists(rutaActual))
      return NotFound(new { mensaje = "El archivo original no existe." });
    if (System.IO.File.Exists(rutaDestino))
      return BadRequest(new { mensaje = "Ya existe un archivo con el nombre nuevo." });
    try
    {
      System.IO.File.Move(rutaActual, rutaDestino);
      return Ok(new { mensaje = "Archivo renombrado correctamente.", nombre = nombreNuevo });
    }
    catch (Exception ex)
    {
      return BadRequest(new { mensaje = "Error al renombrar el archivo: " + ex.Message });
    }
  }

  // //Endpoint SEGUNDO SPRINT
  [HttpGet("buscar")]
  public IActionResult BuscarPlantillaPorImagen([FromQuery] string imagen)
  {
    string ruta = "../back-end/Datos/plantilla.json";

    if (string.IsNullOrWhiteSpace(imagen))
        return BadRequest("El parámetro 'imagen' es obligatorio.");
    if (!System.IO.File.Exists(ruta))
        return NotFound("El archivo JSON no existe.");
    try
    {
      var contenido = System.IO.File.ReadAllText(ruta);
      var listaPlantillas = JsonSerializer.Deserialize<List<Plantilla>>(contenido) ?? new List<Plantilla>();
      for (int i = 0; i < listaPlantillas.Count; i++)
      {
        var p = listaPlantillas[i];
      }
      var plantilla = listaPlantillas.FirstOrDefault(p =>
      {
        if (string.IsNullOrEmpty(p.ImagenUrl)) return false;
           if (string.Equals(p.ImagenUrl.Trim(), imagen.Trim(), StringComparison.OrdinalIgnoreCase))
              return true;
            var nombreImagenJson = Path.GetFileNameWithoutExtension(p.ImagenUrl);
            var nombreImagenBusqueda = Path.GetFileNameWithoutExtension(imagen);
            if (string.Equals(nombreImagenJson, nombreImagenBusqueda, StringComparison.OrdinalIgnoreCase))
                return true;
            return false;
        });
        if (plantilla != null)
        {
          return Ok(plantilla);
        }
        return NotFound($"No se encontró plantilla para la imagen: {imagen}");
    }
    catch (Exception ex)
    {
      return StatusCode(500, $"Error al procesar el archivo JSON: {ex.Message}");
    }
}
  /////////////////////////////
}



[ApiController]
[Route("api/plantilla")]
public class ControllerPlantilla : ControllerBase
{
  [HttpPost("guardar")]
  public IActionResult GuardarPlantilla([FromBody] Plantilla plantilla)
  {
    string ruta = "../back-end/Datos/plantilla.json";
    List<Plantilla> listaPlantillas;
    // Leer el archivo si existe, si no, crear una nueva lista
    if (System.IO.File.Exists(ruta))
    {
      var contenido = System.IO.File.ReadAllText(ruta);
      if (!string.IsNullOrWhiteSpace(contenido))
        listaPlantillas = JsonSerializer.Deserialize<List<Plantilla>>(contenido) ?? new List<Plantilla>();
      else
        listaPlantillas = new List<Plantilla>();
    }
    else
    {
      listaPlantillas = new List<Plantilla>();
    }
    // Agregar la nueva plantilla
    listaPlantillas.Add(plantilla);
    // Guardar la lista actualizada
    var json = JsonSerializer.Serialize(listaPlantillas, new JsonSerializerOptions { WriteIndented = true });
    System.IO.File.WriteAllText(ruta, json);
    return Ok(new { mensaje = "Plantilla guardada correctamente" });
  }

  //Endpoint SEGUNDO SPRINT
  [HttpGet("listar")]
  public IActionResult ListarTodasLasPlantillas()
  {
    string ruta = "../back-end/Datos/plantilla.json";

    if (!System.IO.File.Exists(ruta))
      return NotFound("El archivo JSON no existe.");

    try
    {
      var contenido = System.IO.File.ReadAllText(ruta);
      var listaPlantillas = JsonSerializer.Deserialize<List<Plantilla>>(contenido) ?? new List<Plantilla>();

      return Ok(new
      {
        total = listaPlantillas.Count,
        plantillas = listaPlantillas
      });
    }
    catch (Exception ex)
    {
      return StatusCode(500, $"Error al leer el archivo JSON: {ex.Message}");
    }
  }

  [HttpGet("debug")]
  public IActionResult DebugBusqueda([FromQuery] string imagen)
  {
    string ruta = "../back-end/Datos/plantilla.json";

    if (string.IsNullOrWhiteSpace(imagen))
      return BadRequest("El parámetro 'imagen' es obligatorio.");

    if (!System.IO.File.Exists(ruta))
      return NotFound("El archivo JSON no existe.");

    try
    {
      var contenido = System.IO.File.ReadAllText(ruta);
      var listaPlantillas = JsonSerializer.Deserialize<List<Plantilla>>(contenido) ?? new List<Plantilla>();

      var resultado = new
      {
        imagenBuscada = imagen,
        totalPlantillas = listaPlantillas.Count,
        todasLasPlantillas = listaPlantillas.Select((p, i) => new
        {
          indice = i,
          nombreCreacion = p.NombreCreacion,
          imagenUrl = p.ImagenUrl,
          idCreador = p.IdCreador,
          puntaje = p.Puntaje,
          paletaColores = p.PaletaColores?.Count ?? 0
        }).ToList(),
        coincidenciasExactas = listaPlantillas.Where(p =>
          string.Equals(p.ImagenUrl?.Trim(), imagen.Trim(), StringComparison.OrdinalIgnoreCase)).ToList(),
        coincidenciasParciales = listaPlantillas.Where(p =>
          p.ImagenUrl?.Contains(imagen, StringComparison.OrdinalIgnoreCase) == true).ToList()
      };

      return Ok(resultado);
    }
    catch (Exception ex)
    {
      return StatusCode(500, $"Error al procesar el archivo JSON: {ex.Message}");
    }
  }

  [HttpPut("actualizar")]
  public IActionResult ActualizarPlantilla([FromBody] Plantilla plantillaActualizada)
  {
    string ruta = "../back-end/Datos/plantilla.json";

    if (!System.IO.File.Exists(ruta))
      return NotFound("El archivo JSON no existe.");

    try
    {
      var contenido = System.IO.File.ReadAllText(ruta);
      var listaPlantillas = JsonSerializer.Deserialize<List<Plantilla>>(contenido) ?? new List<Plantilla>();

      // Buscar la plantilla por ImagenUrl
      var plantillaExistente = listaPlantillas.FirstOrDefault(p =>
        string.Equals(p.ImagenUrl?.Trim(), plantillaActualizada.ImagenUrl?.Trim(), StringComparison.OrdinalIgnoreCase));

      if (plantillaExistente == null)
        return NotFound($"No se encontró la plantilla con imagen: {plantillaActualizada.ImagenUrl}");

      // Actualizar los datos de la plantilla
      plantillaExistente.PaletaColores = plantillaActualizada.PaletaColores;
      plantillaExistente.Puntaje = plantillaActualizada.Puntaje;
      plantillaExistente.NombreCreacion = plantillaActualizada.NombreCreacion;
      plantillaExistente.IdCreador = plantillaActualizada.IdCreador;

      // Guardar la lista actualizada
      var json = JsonSerializer.Serialize(listaPlantillas, new JsonSerializerOptions { WriteIndented = true });
      System.IO.File.WriteAllText(ruta, json);

      return Ok(new { mensaje = "Plantilla actualizada correctamente" });
    }
    catch (Exception ex)
    {
      return StatusCode(500, $"Error al actualizar la plantilla: {ex.Message}");
    }
  }
  ////////////////////////////////////////

}
