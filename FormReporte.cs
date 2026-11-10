using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MODULO.CSharp
{
    public partial class FormReporte : Form
    {
        private string _tipoInicial = "Reporte Histórico";
        private int _nominaIdInicial;
        private bool _cargando;

        public FormReporte()
        {
            InitializeComponent();
            ConectarEventos();
        }

        public FormReporte(string tipoReporteSeleccionado, int nominaIdSeleccionado)
        {
            InitializeComponent();

            _tipoInicial = tipoReporteSeleccionado;
            _nominaIdInicial = nominaIdSeleccionado;

            ConectarEventos();
        }

        private void ConectarEventos()
        {
            Load += FormReporte_Load;
            cmbTipoReporte.SelectedIndexChanged += cmbTipoReporte_SelectedIndexChanged;
            cmbPeriodoNomina.SelectedIndexChanged += cmbPeriodoNomina_SelectedIndexChanged;
            btnExportar.Click += btnExportar_Click;
            btnSalir.Click += btnSalir_Click;

            dgvReporte.ReadOnly = true;
            dgvReporte.AllowUserToAddRows = false;
            dgvReporte.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void FormReporte_Load(object? sender, EventArgs e)
        {
            _cargando = true;

            cmbTipoReporte.Items.Clear();
            cmbTipoReporte.Items.Add("Reporte Histórico");
            cmbTipoReporte.Items.Add("Resumen General");
            cmbTipoReporte.Items.Add("Reporte Detallado");

            int indice = cmbTipoReporte.Items.IndexOf(_tipoInicial);
            cmbTipoReporte.SelectedIndex = indice >= 0 ? indice : 0;

            try
            {
                DataTable periodos = NominaService.Historial();

                cmbPeriodoNomina.DisplayMember = "PeriodoPagado";
                cmbPeriodoNomina.ValueMember = "NominaID";
                cmbPeriodoNomina.DataSource = periodos;

                if (_nominaIdInicial > 0)
                    cmbPeriodoNomina.SelectedValue = _nominaIdInicial;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "No se pudieron cargar los periodos: " + ex.Message,
                    "Reportes",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }

            _cargando = false;
            ActualizarReporte();
        }

        private void cmbTipoReporte_SelectedIndexChanged(
            object? sender, EventArgs e)
        {
            ActualizarReporte();
        }

        private void cmbPeriodoNomina_SelectedIndexChanged(
            object? sender, EventArgs e)
        {
            ActualizarReporte();
        }

        private void ActualizarReporte()
        {
            if (_cargando || cmbTipoReporte.SelectedItem == null)
                return;

            string tipo = cmbTipoReporte.SelectedItem.ToString() ?? "";
            bool esHistorico = tipo == "Reporte Histórico";

            lblTitulo.Text = "Mostrando: " + tipo;
            lblPeriodoNomina.Visible = !esHistorico;
            cmbPeriodoNomina.Visible = !esHistorico;

            try
            {
                if (esHistorico)
                {
                    dgvReporte.DataSource = NominaService.Historial();
                    return;
                }

                int nominaId = ObtenerNominaIdSeleccionado();

                if (nominaId <= 0)
                {
                    dgvReporte.DataSource = null;
                    return;
                }

                DataTable datos;

                if (tipo == "Resumen General")
                {
                    datos = NominaService.ResumenGeneral(nominaId);

                    if (datos.Rows.Count > 0)
                        AgregarTotales(datos);
                }
                else
                {
                    datos = NominaService.ReporteDetallado(nominaId);
                }

                dgvReporte.DataSource = datos;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "No se pudo cargar el reporte: " + ex.Message,
                    "Reportes",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private int ObtenerNominaIdSeleccionado()
        {
            if (cmbPeriodoNomina.SelectedValue is int id)
                return id;

            if (cmbPeriodoNomina.SelectedItem is DataRowView fila &&
                int.TryParse(fila["NominaID"].ToString(), out id))
            {
                return id;
            }

            return 0;
        }

        private static void AgregarTotales(DataTable datos)
        {
            DataRow total = datos.NewRow();

            if (datos.Columns.Contains("Departamento"))
                total["Departamento"] = "Total general";

            string[] columnas =
            {
                "TotalEmpleados",
                "TotalDevengado",
                "TotalDeducciones",
                "CostoTotal"
            };

            foreach (string columna in columnas)
            {
                if (datos.Columns.Contains(columna))
                {
                    total[columna] =
                        datos.Compute("SUM([" + columna + "])", "");
                }
            }

            datos.Rows.Add(total);
        }

        private void btnExportar_Click(object? sender, EventArgs e)
        {
            if (dgvReporte.DataSource is not DataTable datos ||
                datos.Rows.Count == 0)
            {
                MessageBox.Show("No hay datos para exportar.");
                return;
            }

            using SaveFileDialog dialogo = new SaveFileDialog
            {
                Filter = "Archivo CSV (*.csv)|*.csv",
                FileName = "ReporteNomina.csv"
            };

            if (dialogo.ShowDialog(this) != DialogResult.OK)
                return;

            try
            {
                StringBuilder contenido = new StringBuilder();

                contenido.AppendLine(string.Join(",",
                    datos.Columns
                        .Cast<DataColumn>()
                        .Select(columna => FormatearCsv(columna.ColumnName))));

                foreach (DataRow fila in datos.Rows)
                {
                    contenido.AppendLine(string.Join(",",
                        datos.Columns
                            .Cast<DataColumn>()
                            .Select(columna =>
                                FormatearCsv(Convert.ToString(fila[columna]) ?? ""))));
                }

                File.WriteAllText(
                    dialogo.FileName,
                    contenido.ToString(),
                    new UTF8Encoding(true));

                MessageBox.Show("Reporte exportado correctamente.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "No se pudo exportar el reporte: " + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private static string FormatearCsv(string valor)
        {
            return "\"" + valor.Replace("\"", "\"\"") + "\"";
        }

        private void btnSalir_Click(object? sender, EventArgs e)
        {
            Close();
        }
    }
}