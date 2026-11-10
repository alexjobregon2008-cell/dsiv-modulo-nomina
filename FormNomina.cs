using System;
using System.Windows.Forms;

namespace MODULO.CSharp
{
    public partial class FormNomina : Form
    {
        public FormNomina()
        {
            InitializeComponent();

            Load += FormNomina_Load;
            btnHistorial.Click += btnHistorial_Click;
            btnCrearNomina.Click += btnCrearNomina_Click;

            dgvHistorial.ReadOnly = true;
            dgvHistorial.AllowUserToAddRows = false;
            dgvHistorial.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void FormNomina_Load(object? sender, EventArgs e)
        {
            CargarHistorial();
        }

        private void btnCrearNomina_Click(object? sender, EventArgs e)
        {
            string periodo = txtPeriodoPagado.Text.Trim();

            if (string.IsNullOrWhiteSpace(periodo))
            {
                MessageBox.Show("Escriba el periodo pagado.");
                txtPeriodoPagado.Focus();
                return;
            }

            if (dtpPeriodoDesde.Value.Date > dtpPeriodoHasta.Value.Date)
            {
                MessageBox.Show(
                    "La fecha Desde debe ser anterior o igual a la fecha Hasta.");
                return;
            }

            try
            {
                int nominaId = NominaService.Crear(
                    dtpFechaPago.Value,
                    dtpPeriodoDesde.Value,
                    dtpPeriodoHasta.Value,
                    periodo);

                MessageBox.Show(
                    "Nómina creada correctamente. ID: " + nominaId,
                    "Nómina",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                using (FormReporte reporte =
                       new FormReporte("Reporte Detallado", nominaId))
                {
                    reporte.ShowDialog(this);
                }

                CargarHistorial();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "No se pudo crear la nómina: " + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void btnHistorial_Click(object? sender, EventArgs e)
        {
            using (FormReporte reporte =
                   new FormReporte("Reporte Histórico", 0))
            {
                reporte.ShowDialog(this);
            }

            CargarHistorial();
        }

        private void CargarHistorial()
        {
            try
            {
                dgvHistorial.DataSource = NominaService.Historial();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "No se pudo cargar el historial: " + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
    }
}