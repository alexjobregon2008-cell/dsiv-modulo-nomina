using System.Data;
using Microsoft.Data.SqlClient;

namespace MODULO.CSharp;

public partial class Form1 : Form
{
    public Form1()
    {
        InitializeComponent();
        AcceptButton = btnIniciarSesion;
    }

    private void btnIniciarSesion_Click(object? sender, EventArgs e)
    {
        string nombreUsuario = txtUsuario.Text.Trim();
        string contrasena = txtContrasena.Text;

        if (string.IsNullOrWhiteSpace(nombreUsuario) || string.IsNullOrWhiteSpace(contrasena))
        {
            MostrarMensaje("Escriba el usuario y la contraseña.", MessageBoxIcon.Warning);
            return;
        }

        try
        {
            DataTable resultado = ConexionBD.EjecutarProcedimiento(
                "sp_LoginUsuario",
                new SqlParameter("@NombreUsuario", SqlDbType.NVarChar, 50) { Value = nombreUsuario });

            if (resultado.Rows.Count == 0)
            {
                MostrarMensaje("Usuario o contraseña incorrectos.", MessageBoxIcon.Warning);
                return;
            }

            DataRow usuario = resultado.Rows[0];
            int usuarioId = Convert.ToInt32(usuario["UsuarioID"]);
            bool activo = Convert.ToBoolean(usuario["Activo"]);

            if (!activo)
            {
                MostrarMensaje("La cuenta está bloqueada o desactivada. Contacte al administrador.", MessageBoxIcon.Error);
                return;
            }

            string hash = usuario["ContrasenaHash"].ToString() ?? string.Empty;
            bool contrasenaValida = BCrypt.Net.BCrypt.Verify(contrasena, hash);

            if (!contrasenaValida)
            {
                ConexionBD.EjecutarProcedimientoNonQuery(
                    "sp_RegistrarIntentoFallido",
                    new SqlParameter("@UsuarioID", SqlDbType.Int) { Value = usuarioId });

                MostrarMensaje("Usuario o contraseña incorrectos. El usuario se desactiva después de 3 intentos fallidos.", MessageBoxIcon.Warning);
                return;
            }

            ConexionBD.EjecutarProcedimientoNonQuery(
                "sp_RegistrarLoginExitoso",
                new SqlParameter("@UsuarioID", SqlDbType.Int) { Value = usuarioId });

            string rol = usuario["Rol"].ToString() ?? string.Empty;
            bool debeCambiarContrasena = Convert.ToBoolean(usuario["DebeCambiarContrasena"]);
            Sesion.Iniciar(usuarioId, nombreUsuario, rol, debeCambiarContrasena);
            BitacoraService.Registrar("LOGIN", $"Inició sesión el usuario {nombreUsuario}.", "Usuarios", usuarioId);

            string mensaje = debeCambiarContrasena
                ? $"Bienvenido, {nombreUsuario}. Debe cambiar la contraseña temporal antes de continuar."
                : $"Bienvenido, {nombreUsuario}. Rol: {rol}.";
            MostrarMensaje(mensaje, MessageBoxIcon.Information);

            if (debeCambiarContrasena)
            {
                using CambioContrasenaForm formulario = new();
                if (formulario.ShowDialog(this) != DialogResult.OK)
                {
                    Sesion.Cerrar();
                    MostrarMensaje("Debe cambiar la contraseña temporal para continuar.", MessageBoxIcon.Warning);
                    return;
                }
            }

            txtContrasena.Clear();
        }
        catch (Exception ex)
        {
            MostrarMensaje($"No se pudo iniciar sesión: {ex.Message}", MessageBoxIcon.Error);
        }
    }

    private void MostrarMensaje(string mensaje, MessageBoxIcon icono)
    {
        MessageBox.Show(mensaje, "Inicio de sesión", MessageBoxButtons.OK, icono);
    }
}
