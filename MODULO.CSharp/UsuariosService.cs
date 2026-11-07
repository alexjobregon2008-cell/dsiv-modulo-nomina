using System.Data;
using Microsoft.Data.SqlClient;

namespace MODULO.CSharp;

public static class UsuariosService
{
    public static int CrearUsuario(string nombreUsuario, string correo, string contrasenaTemporal, string rol = "Usuario")
    {
        ValidarContrasena(contrasenaTemporal);
        ValidarAdministrador();

        object? resultado = ConexionBD.EjecutarProcedimientoEscalar(
            "sp_CrearUsuario",
            new SqlParameter("@AdministradorID", SqlDbType.Int) { Value = Sesion.UsuarioID!.Value },
            new SqlParameter("@NombreUsuario", SqlDbType.NVarChar, 50) { Value = nombreUsuario.Trim() },
            new SqlParameter("@ContrasenaHash", SqlDbType.NVarChar, 255) { Value = BCrypt.Net.BCrypt.HashPassword(contrasenaTemporal) },
            new SqlParameter("@Correo", SqlDbType.NVarChar, 150) { Value = correo.Trim() },
            new SqlParameter("@Rol", SqlDbType.NVarChar, 20) { Value = rol });

        return Convert.ToInt32(resultado);
    }

    public static void RestablecerContrasena(int usuarioId, string nuevaContrasenaTemporal)
    {
        ValidarContrasena(nuevaContrasenaTemporal);
        ValidarAdministrador();

        ConexionBD.EjecutarProcedimientoNonQuery(
            "sp_RestablecerContrasenaUsuario",
            new SqlParameter("@AdministradorID", SqlDbType.Int) { Value = Sesion.UsuarioID!.Value },
            new SqlParameter("@UsuarioID", SqlDbType.Int) { Value = usuarioId },
            new SqlParameter("@ContrasenaHash", SqlDbType.NVarChar, 255) { Value = BCrypt.Net.BCrypt.HashPassword(nuevaContrasenaTemporal) });
    }

    public static void CambiarContrasena(string nuevaContrasena)
    {
        ValidarContrasena(nuevaContrasena);
        if (!Sesion.EstaIniciada)
            throw new UnauthorizedAccessException("Debe iniciar sesión para cambiar la contraseña.");

        ConexionBD.EjecutarProcedimientoNonQuery(
            "sp_CambiarContrasena",
            new SqlParameter("@UsuarioID", SqlDbType.Int) { Value = Sesion.UsuarioID!.Value },
            new SqlParameter("@ContrasenaHash", SqlDbType.NVarChar, 255) { Value = BCrypt.Net.BCrypt.HashPassword(nuevaContrasena) });
    }

    private static void ValidarAdministrador()
    {
        if (!Sesion.EstaIniciada || !string.Equals(Sesion.Rol, "Administrador", StringComparison.OrdinalIgnoreCase))
            throw new UnauthorizedAccessException("Solo un administrador activo puede realizar esta operación.");
    }

    private static void ValidarContrasena(string contrasena)
    {
        if (string.IsNullOrWhiteSpace(contrasena) || contrasena.Length < 8)
            throw new ArgumentException("La contraseña debe tener al menos 8 caracteres.", nameof(contrasena));
    }
}
