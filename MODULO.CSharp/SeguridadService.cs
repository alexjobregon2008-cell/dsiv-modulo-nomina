using System.Data;
using Microsoft.Data.SqlClient;

namespace MODULO.CSharp;

public static class SeguridadService
{
    public static void DesbloquearUsuario(int usuarioId)
    {
        if (!Sesion.EstaIniciada || !string.Equals(Sesion.Rol, "Administrador", StringComparison.OrdinalIgnoreCase))
        {
            throw new UnauthorizedAccessException("Solo un administrador activo puede desbloquear usuarios.");
        }

        ConexionBD.EjecutarProcedimientoNonQuery(
            "sp_DesbloquearUsuario",
            new SqlParameter("@AdministradorID", SqlDbType.Int) { Value = Sesion.UsuarioID!.Value },
            new SqlParameter("@UsuarioID", SqlDbType.Int) { Value = usuarioId });
    }
}
