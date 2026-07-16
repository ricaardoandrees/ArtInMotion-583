using System.Collections.Generic;
using System.IO;
using System.Linq;

// Lógica de negocio de CatalogoController extraída a métodos estáticos
// puros, según el Plan de Pruebas Unitarias v1.0 (Bloque B2).
// No dependen de disco: reciben los datos ya leídos y devuelven un resultado.
public static class CatalogoValidaciones
{
    private static readonly string[] ExtensionesPermitidas =
        { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };

    // UT-B2-03 / UT-B2-04: acepta o rechaza una extensión de archivo.
    public static bool ValidarExtensionArchivo(string nombreArchivo)
    {
        var extension = Path.GetExtension(nombreArchivo).ToLowerInvariant();
        return ExtensionesPermitidas.Contains(extension);
    }

    // UT-B2-01 / UT-B2-02: calcula el siguiente nombre "subidaN.ext"
    // según el mayor número ya usado en los archivos existentes.
    public static string CalcularNombreSubida(IEnumerable<string> archivosExistentes, string extension)
    {
        int maxNum = 0;
        foreach (var archivoExistente in archivosExistentes)
        {
            var nombre = Path.GetFileNameWithoutExtension(archivoExistente);
            if (nombre.StartsWith("subida"))
            {
                var numeroStr = nombre.Substring(6); // "subida".Length == 6
                if (int.TryParse(numeroStr, out int num))
                {
                    if (num > maxNum) maxNum = num;
                }
            }
        }
        int nuevoNum = maxNum + 1;
        return $"subida{nuevoNum}{extension}";
    }
}
