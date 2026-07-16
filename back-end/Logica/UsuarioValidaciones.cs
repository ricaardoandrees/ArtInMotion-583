using Usuarioo.Models;

namespace Usuarioo.Logica
{
    // Lógica de negocio de Usuario extraída a métodos estáticos puros,
    // según lo pedido en el Plan de Pruebas Unitarias v1.0 (Sección 5, Bloque B1):
    // "Extraer los métodos de validación del UsuariosController a una clase
    // de servicio o métodos estáticos antes de probarlos. Estos casos no
    // deben tocar usuarios.json."
    //
    // No dependen de disco ni de ningún servicio externo: reciben los datos
    // ya cargados y devuelven un resultado. Eso es lo que los hace
    // pruebas unitarias "puras" (ni siquiera necesitan un mock).
    public static class UsuarioValidaciones
    {
        // UT-B1-01 / UT-B1-02: detecta el email repetido ignorando
        // mayúsculas/minúsculas y espacios al inicio/final.
        public static bool DetectarEmailDuplicado(List<Usuario> usuarios, string emailBuscado)
        {
            string emailNormalizado = emailBuscado.Trim().ToLower();
            return usuarios.Any(u => u.Email.Trim().ToLower() == emailNormalizado);
        }

        // UT-B1-03 / UT-B1-04: compara la contraseña almacenada contra
        // la ingresada en el login.
        public static bool VerificarContrasena(string contrasenaAlmacenada, string contrasenaIngresada)
        {
            return contrasenaAlmacenada == contrasenaIngresada;
        }
    }
}
