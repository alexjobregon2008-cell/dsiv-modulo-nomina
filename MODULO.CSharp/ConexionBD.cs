using System.Data;
using Microsoft.Data.SqlClient;

namespace MODULO.CSharp;

public static class ConexionBD
{
    public const string CadenaConexion =
        "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=MODULO_Final;Integrated Security=True;TrustServerCertificate=True;";

    public static SqlConnection CrearConexion()
    {
        return new SqlConnection(CadenaConexion);
    }

    public static int EjecutarProcedimientoNonQuery(string nombreProcedimiento, params SqlParameter[] parametros)
    {
        using SqlConnection conexion = CrearConexion();
        using SqlCommand comando = CrearComando(conexion, nombreProcedimiento, parametros);
        comando.CommandType = CommandType.StoredProcedure;
        AbrirConexion(conexion);
        return comando.ExecuteNonQuery();
    }

    public static object? EjecutarProcedimientoEscalar(string nombreProcedimiento, params SqlParameter[] parametros)
    {
        using SqlConnection conexion = CrearConexion();
        using SqlCommand comando = CrearComando(conexion, nombreProcedimiento, parametros);
        comando.CommandType = CommandType.StoredProcedure;
        AbrirConexion(conexion);
        return comando.ExecuteScalar();
    }

    public static int EjecutarNonQuery(string consulta, params SqlParameter[] parametros)
    {
        using SqlConnection conexion = CrearConexion();
        using SqlCommand comando = CrearComando(conexion, consulta, parametros);
        AbrirConexion(conexion);
        return comando.ExecuteNonQuery();
    }

    public static object? EjecutarEscalar(string consulta, params SqlParameter[] parametros)
    {
        using SqlConnection conexion = CrearConexion();
        using SqlCommand comando = CrearComando(conexion, consulta, parametros);
        AbrirConexion(conexion);
        return comando.ExecuteScalar();
    }

    public static DataTable ObtenerDataTable(string consulta, params SqlParameter[] parametros)
    {
        using SqlConnection conexion = CrearConexion();
        using SqlCommand comando = CrearComando(conexion, consulta, parametros);
        AbrirConexion(conexion);
        using SqlDataAdapter adaptador = new(comando);
        DataTable tabla = new();
        adaptador.Fill(tabla);
        return tabla;
    }

    public static DataTable EjecutarProcedimiento(string nombreProcedimiento, params SqlParameter[] parametros)
    {
        using SqlConnection conexion = CrearConexion();
        using SqlCommand comando = CrearComando(conexion, nombreProcedimiento, parametros);
        comando.CommandType = CommandType.StoredProcedure;
        AbrirConexion(conexion);
        using SqlDataAdapter adaptador = new(comando);
        DataTable tabla = new();
        adaptador.Fill(tabla);
        return tabla;
    }

    private static SqlCommand CrearComando(SqlConnection conexion, string consulta, SqlParameter[] parametros)
    {
        SqlCommand comando = new(consulta, conexion);
        comando.Parameters.AddRange(parametros);
        return comando;
    }

    private static void AbrirConexion(SqlConnection conexion)
    {
        conexion.Open();
        using SqlCommand comandoContexto = new("sys.sp_set_session_context", conexion)
        {
            CommandType = CommandType.StoredProcedure
        };
        comandoContexto.Parameters.Add("@key", SqlDbType.NVarChar, 128).Value = "UsuarioID";
        comandoContexto.Parameters.Add("@value", SqlDbType.Int).Value = (object?)Sesion.UsuarioID ?? DBNull.Value;
        comandoContexto.ExecuteNonQuery();
    }
}
