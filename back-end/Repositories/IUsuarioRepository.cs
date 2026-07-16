using Usuarioo.Models;

namespace Usuarioo.Repositories
{
    // Abstracción del acceso a datos de usuarios.
    // Permite que UsuariosController no dependa directamente del sistema de archivos,
    // lo cual habilita pruebas unitarias aisladas (con un mock/fake) sin tocar disco.
    public interface IUsuarioRepository
    {
        List<Usuario> ObtenerTodos();
        void Agregar(Usuario usuario);
    }
}
