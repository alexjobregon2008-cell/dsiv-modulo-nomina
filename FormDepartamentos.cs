#nullable enable

using System;
using System.Data;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace MODULO.CSharp
{
    public partial class FormDepartamentos : Form
    {
        private int? _departamentoId;

        public FormDepartamentos()
        {
            InitializeComponent();

            Load += FormDepartamentos_Load;
            btnNuevo.Click += btnNuevo_Click;
            btnGuardar.Click += btnGuardar_Click;
            btnEliminar.Click += btnEliminar_Click;
            dgvDepartamentos.CellClick += dgvDepartamentos_CellClick;

            dgvDepartamentos.ReadOnly = true;
            dgvDepartamentos.AllowUserToAddRows = false;
            dgvDepartamentos.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;
            dgvDepartamentos.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void FormDepartamentos_Load(object? sender, EventArgs e)
        {
            CargarDepartamentos();
        }

        private void CargarDepartamentos()
        {
            try
            {
                dgvDepartamentos.DataSource =
                    ConexionBD.ObtenerDataTable(
                        "SELECT DepartamentoID, Nombre, Descripcion, Activo " +
                        "FROM dbo.Departamentos ORDER BY Nombre;");

                if (dgvDepartamentos.Columns["DepartamentoID"] != null)
                {
                    dgvDepartamentos.Columns["DepartamentoID"].Visible = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "No se pudieron cargar los departamentos: " + ex.Message,
                    "Departamentos",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void dgvDepartamentos_CellClick(
            object? sender,
            DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            DataGridViewRow fila = dgvDepartamentos.Rows[e.RowIndex];

            _departamentoId =
                Convert.ToInt32(fila.Cells["DepartamentoID"].Value);

            txtNombre.Text =
                Convert.ToString(fila.Cells["Nombre"].Value) ?? "";

            object? descripcion = fila.Cells["Descripcion"].Value;
            txtDescripcion.Text =
                descripcion == null || descripcion == DBNull.Value
                    ? ""
                    : Convert.ToString(descripcion) ?? "";

            chkActivo.Checked =
                Convert.ToBoolean(fila.Cells["Activo"].Value);
        }

        private void btnNuevo_Click(object? sender, EventArgs e)
        {
            LimpiarCampos();
        }

        private void btnGuardar_Click(object? sender, EventArgs e)
        {
            string nombre = txtNombre.Text.Trim();
            string descripcion = txtDescripcion.Text.Trim();

            if (string.IsNullOrWhiteSpace(nombre) || nombre.Length > 100)
            {
                MessageBox.Show(
                    "El nombre es obligatorio y admite hasta 100 caracteres.");
                txtNombre.Focus();
                return;
            }

            if (descripcion.Length > 300)
            {
                MessageBox.Show(
                    "La descripción no puede superar los 300 caracteres.");
                txtDescripcion.Focus();
                return;
            }

            SqlParameter parametroDescripcion =
                new SqlParameter("@Descripcion", SqlDbType.NVarChar, 300)
                {
                    Value = string.IsNullOrWhiteSpace(descripcion)
                        ? DBNull.Value
                        : descripcion
                };

            try
            {
                if (_departamentoId.HasValue)
                {
                    ConexionBD.EjecutarNonQuery(
                        "UPDATE dbo.Departamentos " +
                        "SET Nombre = @Nombre, Descripcion = @Descripcion, " +
                        "Activo = @Activo " +
                        "WHERE DepartamentoID = @DepartamentoID;",
                        new SqlParameter("@Nombre", SqlDbType.NVarChar, 100)
                        {
                            Value = nombre
                        },
                        parametroDescripcion,
                        new SqlParameter("@Activo", SqlDbType.Bit)
                        {
                            Value = chkActivo.Checked
                        },
                        new SqlParameter("@DepartamentoID", SqlDbType.Int)
                        {
                            Value = _departamentoId.Value
                        });
                }
                else
                {
                    ConexionBD.EjecutarNonQuery(
                        "INSERT INTO dbo.Departamentos " +
                        "(Nombre, Descripcion, Activo) " +
                        "VALUES (@Nombre, @Descripcion, @Activo);",
                        new SqlParameter("@Nombre", SqlDbType.NVarChar, 100)
                        {
                            Value = nombre
                        },
                        parametroDescripcion,
                        new SqlParameter("@Activo", SqlDbType.Bit)
                        {
                            Value = chkActivo.Checked
                        });
                }

                CargarDepartamentos();
                LimpiarCampos();

                MessageBox.Show(
                    "Departamento guardado correctamente.",
                    "Departamentos",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (SqlException ex) when (ex.Number == 2601 || ex.Number == 2627)
            {
                MessageBox.Show(
                    "Ya existe un departamento con ese nombre.",
                    "Departamentos",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "No se pudo guardar el departamento: " + ex.Message,
                    "Departamentos",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void btnEliminar_Click(object? sender, EventArgs e)
        {
            if (!_departamentoId.HasValue)
            {
                MessageBox.Show("Seleccione un departamento de la tabla.");
                return;
            }

            DialogResult respuesta = MessageBox.Show(
                "¿Desea eliminar el departamento seleccionado?",
                "Confirmar eliminación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (respuesta != DialogResult.Yes)
                return;

            try
            {
                ConexionBD.EjecutarNonQuery(
                    "DELETE FROM dbo.Departamentos " +
                    "WHERE DepartamentoID = @DepartamentoID;",
                    new SqlParameter("@DepartamentoID", SqlDbType.Int)
                    {
                        Value = _departamentoId.Value
                    });

                CargarDepartamentos();
                LimpiarCampos();
            }
            catch (SqlException ex) when (ex.Number == 547)
            {
                MessageBox.Show(
                    "El departamento tiene empleados o cargos asociados. " +
                    "Desmárquelo como Activo para deshabilitarlo.",
                    "Departamentos",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "No se pudo eliminar el departamento: " + ex.Message,
                    "Departamentos",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void LimpiarCampos()
        {
            _departamentoId = null;
            txtNombre.Clear();
            txtDescripcion.Clear();
            chkActivo.Checked = true;
            dgvDepartamentos.ClearSelection();
            txtNombre.Focus();
        }
    }
}