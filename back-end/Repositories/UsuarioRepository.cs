using System.Text.Json;
using Usuarioo.Models;

namespace Usuarioo.Repositories
{
    // Implementación de producción de IUsuarioRepository.
    // Contiene exactamente la misma lógica de lectura/escritura que antes vivía
    // directo dentro de UsuariosController, solo que ahora aislada en su propia clase.
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly string rutaUsuarios = @"..\back-end\Datos\usuarios.json";

        public List<Usuario> ObtenerTodos()
        {
            var usuarios = new List<Usuario>();

            if (File.Exists(rutaUsuarios))
            {
                var lineas = File.ReadAllLines(rutaUsuarios);
                foreach (var linea in lineas)
                {
                    if (!string.IsNullOrWhiteSpace(linea))
                    {
                        try
                        {
                            var u = JsonSerializer.Deserialize<Usuario>(linea);
                            if (u != null)
                                usuarios.Add(u);
                        }
                        catch
                        {
                            // Ignora líneas mal formateadas
                        }
                    }
                }
            }

            return usuarios;
        }

        public void Agregar(Usuario usuario)
        {
            usuario.Uuid = Guid.NewGuid().ToString();
            var usuarioJson = JsonSerializer.Serialize(usuario);
            File.AppendAllText(rutaUsuarios, usuarioJson + Environment.NewLine);
        }
    }
}
