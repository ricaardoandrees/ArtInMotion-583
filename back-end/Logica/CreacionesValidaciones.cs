using System;
using System.Collections.Generic;

// Lógica de negocio de CreacionesController (Calificar / Delete) extraída
// a métodos estáticos puros, según el Plan de Pruebas Unitarias v1.0 (Bloque B2).
public static class CreacionesValidaciones
{
    // UT-B2-05: coincidencia exacta de ImagenUrl para identificar
    // la plantilla a eliminar (criterio real usado en Delete()).
    public static bool CriterioEliminarPlantilla(string imagenUrlPlantilla, string nombreBuscado)
    {
        return imagenUrlPlantilla == nombreBuscado;
    }

    // UT-B2-06: extrae tipo, imagen, creador y valor del dato de
    // calificación recibido en el body (Dictionary del JSON real,
    // no la cadena delimitada por "|" que describe el documento original).
    public static (string Tipo, string ImagenUrl, string IdCreador, int Puntaje) ParsearCalificacion(
        Dictionary<string, object> calificacion)
    {
        string tipo = null, imagenUrl = null, idCreador = null;
        int puntaje = 0;

        foreach (var key in calificacion.Keys)
        {
            if (key.Equals("tipo", StringComparison.OrdinalIgnoreCase))
                tipo = calificacion[key]?.ToString();
            else if (key.Equals("imagenurl", StringComparison.OrdinalIgnoreCase))
                imagenUrl = calificacion[key]?.ToString();
            else if (key.Equals("idcreador", StringComparison.OrdinalIgnoreCase))
                idCreador = calificacion[key]?.ToString();
            else if (key.Equals("calificacion", StringComparison.OrdinalIgnoreCase))
            {
                var val = calificacion[key];
                if (val is long l) puntaje = (int)l;
                else if (val is int i) puntaje = i;
                else if (int.TryParse(val?.ToString(), out int parsed)) puntaje = parsed;
            }
        }

        return (tipo, imagenUrl, idCreador, puntaje);
    }
}
