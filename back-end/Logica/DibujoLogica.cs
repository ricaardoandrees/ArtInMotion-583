using System;
using System.Linq;
using System.Text.RegularExpressions;

// Lógica de negocio de DibujoController extraída a métodos estáticos
// puros, según el Plan de Pruebas Unitarias v1.0 (Bloque B4).
public static class DibujoLogica
{
    // UT-B4-01 / UT-B4-02: extrae el UUID del creador a partir del nombre
    // ya "limpiado" (después de LimpiarNombre). Si no hay guion bajo,
    // se asume que no se pudo determinar el creador.
    public static string ExtraerUUIDDeNombre(string nombre)
    {
        var partes = nombre.Split('_');
        if (partes.Length > 1)
            return partes.Last();
        return "desconocido";
    }

    // UT-B4-03: deja solo caracteres alfanuméricos, guion y guion bajo.
    public static string LimpiarNombre(string nombre)
    {
        return Regex.Replace(nombre, @"[^a-zA-Z0-9_-]", "");
    }

    // UT-B4-04: extrae los bytes de imagen desde un data URI completo
    // (ej. "data:image/png;base64,xxxx"). Nota: requiere el prefijo
    // "data:image/..." tal como lo manda el frontend; un base64 "pelado"
    // sin ese prefijo no matchea el patrón real usado en producción.
    public static byte[] DecodificarBase64(string imagenBase64)
    {
        var base64Data = Regex.Match(imagenBase64, @"data:image/(?<type>.+?),(?<data>.+)").Groups["data"].Value;
        return Convert.FromBase64String(base64Data);
    }
}
