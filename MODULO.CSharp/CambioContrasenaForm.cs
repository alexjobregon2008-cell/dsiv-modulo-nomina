using System.Drawing;

namespace MODULO.CSharp;

public sealed class CambioContrasenaForm : Form
{
    private readonly TextBox txtNueva = new() { PasswordChar = '●', Width = 250 };
    private readonly TextBox txtConfirmacion = new() { PasswordChar = '●', Width = 250 };
    private readonly Button btnGuardar = new() { Text = "Guardar contraseña", AutoSize = true };

    public CambioContrasenaForm()
    {
        Text = "Cambiar contraseña";
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        ClientSize = new Size(360, 190);

        Label lblNueva = new() { Text = "Nueva contraseña:", AutoSize = true, Location = new Point(25, 25) };
        Label lblConfirmacion = new() { Text = "Confirmar contraseña:", AutoSize = true, Location = new Point(25, 70) };
        txtNueva.Location = new Point(25, 45);
        txtConfirmacion.Location = new Point(25, 90);
        btnGuardar.Location = new Point(25, 135);
        btnGuardar.Click += Guardar_Click;
        AcceptButton = btnGuardar;

        Controls.AddRange([lblNueva, txtNueva, lblConfirmacion, txtConfirmacion, btnGuardar]);
    }

    private void Guardar_Click(object? sender, EventArgs e)
    {
        if (txtNueva.Text != txtConfirmacion.Text)
        {
            MessageBox.Show("Las contraseñas no coinciden.", "Contraseña", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        try
        {
            UsuariosService.CambiarContrasena(txtNueva.Text);
            Sesion.MarcarContrasenaActualizada();
            DialogResult = DialogResult.OK;
            Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Contraseña", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}