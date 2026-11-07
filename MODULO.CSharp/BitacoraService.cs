using System.Data;
using Microsoft.Data.SqlClient;

namespace MODULO.CSharp;

public static class BitacoraService
{
    public static void Registrar(
        string accion,
        string descripcion,
        string? tablaAfectada = null,
        int? registroId = null)
    {
        const string procedimiento = "sp_RegistrarBitacora";

        SqlParameter usuarioId = new("@UsuarioID", SqlDbType.Int)
        {
            Value = (object?)Sesion.UsuarioID ?? DBNull.Value
        };
        SqlParameter parametroAccion = new("@Accion", SqlDbType.NVarChar, 30)
        {
            Value = accion
        };
        SqlParameter parametroDescripcion = new("@Descripcion", SqlDbType.NVarChar, 500)
        {
            Value = descripcion
        };
        SqlParameter parametroTabla = new("@TablaAfectada", SqlDbType.NVarChar, 128)
        {
            Value = (object?)tablaAfectada ?? DBNull.Value
        };
        SqlParameter parametroRegistro = new("@RegistroID", SqlDbType.Int)
        {
            Value = (object?)registroId ?? DBNull.Value
        };

        ConexionBD.EjecutarProcedimiento(
            procedimiento,
            usuarioId,
            parametroAccion,
            parametroDescripcion,
            parametroTabla,
            parametroRegistro);
    }
}
