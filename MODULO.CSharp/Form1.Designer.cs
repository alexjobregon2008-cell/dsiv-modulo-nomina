namespace MODULO.CSharp;

partial class Form1
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;
    private Label lblTitulo;
    private Label lblUsuario;
    private Label lblContrasena;
    private TextBox txtUsuario;
    private TextBox txtContrasena;
    private Button btnIniciarSesion;

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        lblTitulo = new Label();
        lblUsuario = new Label();
        lblContrasena = new Label();
        txtUsuario = new TextBox();
        txtContrasena = new TextBox();
        btnIniciarSesion = new Button();
        SuspendLayout();

        lblTitulo.AutoSize = true;
        lblTitulo.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
        lblTitulo.Location = new Point(105, 45);
        lblTitulo.Name = "lblTitulo";
        lblTitulo.Size = new Size(190, 32);
        lblTitulo.Text = "Iniciar sesión";

        lblUsuario.AutoSize = true;
        lblUsuario.Location = new Point(70, 115);
        lblUsuario.Name = "lblUsuario";
        lblUsuario.Text = "Usuario:";

        txtUsuario.Location = new Point(70, 140);
        txtUsuario.Name = "txtUsuario";
        txtUsuario.Size = new Size(260, 23);
        txtUsuario.TabIndex = 0;

        lblContrasena.AutoSize = true;
        lblContrasena.Location = new Point(70, 185);
        lblContrasena.Name = "lblContrasena";
        lblContrasena.Text = "Contraseña:";

        txtContrasena.Location = new Point(70, 210);
        txtContrasena.Name = "txtContrasena";
        txtContrasena.PasswordChar = '●';
        txtContrasena.Size = new Size(260, 23);
        txtContrasena.TabIndex = 1;

        btnIniciarSesion.Location = new Point(125, 265);
        btnIniciarSesion.Name = "btnIniciarSesion";
        btnIniciarSesion.Size = new Size(150, 35);
        btnIniciarSesion.TabIndex = 2;
        btnIniciarSesion.Text = "Iniciar sesión";
        btnIniciarSesion.UseVisualStyleBackColor = true;
        btnIniciarSesion.Click += btnIniciarSesion_Click;

        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(400, 370);
        Controls.Add(lblTitulo);
        Controls.Add(lblUsuario);
        Controls.Add(txtUsuario);
        Controls.Add(lblContrasena);
        Controls.Add(txtContrasena);
        Controls.Add(btnIniciarSesion);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "Form1";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Sistema de Nómina";
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion
}
