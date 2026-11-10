using System;
using System.Windows.Forms;

namespace MODULO.CSharp
{
    public partial class MenuP : Form
    {
        private Form1? _formLogin;

        public MenuP()
        {
            InitializeComponent();
            ConectarEventos();
        }

        public MenuP(Form1 formLogin) : this()
        {
            _formLogin = formLogin;
        }

        private void ConectarEventos()
        {
            btnEmpleados.Click += (sender, e) =>
                AbrirFormulario(new FormEmpleados());

            btnCargos.Click += (sender, e) =>
                AbrirFormulario(new FormCargos());

            btnDepartamentos.Click += (sender, e) =>
                AbrirFormulario(new FormDepartamentos());

            btnNomina.Click += (sender, e) =>
                AbrirFormulario(new FormNomina());

            btnHistorial.Click += (sender, e) =>
                AbrirFormulario(new FormReporte("Reporte Histórico", 0));

            btnCerrarSesion.Click += btnCerrarSesion_Click;

            lblUsuario.Text = "Usuario: " + Sesion.NombreUsuario;
        }

        private void AbrirFormulario(Form formulario)
        {
            using (formulario)
            {
                formulario.ShowDialog(this);
            }
        }

        private void btnCerrarSesion_Click(object? sender, EventArgs e)
        {
            Sesion.Cerrar();

            if (_formLogin != null)
            {
                _formLogin.Show();
                _formLogin.Activate();
            }

            Close();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            if (Sesion.EstaIniciada)
            {
                Sesion.Cerrar();

                if (_formLogin != null)
                {
                    _formLogin.Show();
                    _formLogin.Activate();
                }
            }

            base.OnFormClosed(e);
        }
    }
}