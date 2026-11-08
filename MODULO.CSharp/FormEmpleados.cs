using System.Data;
using System.Globalization;
using Microsoft.Data.SqlClient;

namespace MODULO.CSharp;

public partial class FormEmpleados : Form
{
    private const string TipoPermanente = "Permanente";
    private const string TipoServiciosProfesionales = "ServiciosProfesionales";

    private readonly BindingSource _origen = new();
    private int? _empleadoId;

    /// <summary>Datos ya validados del formulario, listos para enviarse a la base.</summary>
    private sealed record DatosEmpleado(
        string Nombre,
        int DepartamentoId,
        int CargoId,
        decimal SalarioBase,
        string TipoContrato,
        bool AplicaDescuentos,
        DateTime FechaInicioContrato,
        bool Activo);

    public FormEmpleados()
    {
        InitializeComponent();
    }

    #region Carga inicial

    private void FormEmpleados_Load(object? sender, EventArgs e)
    {
        AplicarEstiloBotones();
        CargarTiposContrato();
        CargarDepartamentos();
        CargarEmpleados();
        LimpiarCampos();
    }

    private void AplicarEstiloBotones()
    {
        foreach (Control control in Controls)
        {
            if (control is Button boton)
            {
                boton.BackColor = Color.FromArgb(254, 223, 231);
                boton.ForeColor = Color.Black;
                boton.FlatStyle = FlatStyle.Flat;
                boton.FlatAppearance.BorderSize = 0;
            }
        }
    }

    // Corrección: el original usaba Items.IndexOf(0), que siempre devuelve -1
    // porque busca el objeto 0 (entero) dentro de una lista de cadenas.
    private void CargarTiposContrato()
    {
        cmbTipoContrato.Items.Clear();
        cmbTipoContrato.Items.Add(TipoPermanente);
        cmbTipoContrato.Items.Add(TipoServiciosProfesionales);
        cmbTipoContrato.SelectedIndex = 0;
    }

    private void CargarDepartamentos()
    {
        DataTable departamentos = ConexionBD.ObtenerDataTable(
            "SELECT DepartamentoID, Nombre FROM dbo.Departamentos WHERE Activo = 1 ORDER BY Nombre;");

        cmbDepartamento.DisplayMember = "Nombre";
        cmbDepartamento.ValueMember = "DepartamentoID";
        cmbDepartamento.DataSource = departamentos;
        cmbDepartamento.SelectedIndex = -1;
    }

    // Los cargos dependen del departamento (FK compuesta CargoID + DepartamentoID).
    private void CargarCargos()
    {
        if (cmbDepartamento.SelectedValue is not int departamentoId)
        {
            cmbCargo.DataSource = null;
            lblRangoSalarial.Text = string.Empty;
            return;
        }

        DataTable cargos = ConexionBD.ObtenerDataTable(
            "SELECT CargoID, Nombre, SalarioMinimo, SalarioMaximo " +
            "FROM dbo.Cargos WHERE DepartamentoID = @DepartamentoID AND Activo = 1 ORDER BY Nombre;",
            new SqlParameter("@DepartamentoID", SqlDbType.Int) { Value = departamentoId });

        cmbCargo.DisplayMember = "Nombre";
        cmbCargo.ValueMember = "CargoID";
        cmbCargo.DataSource = cargos;
        cmbCargo.SelectedIndex = -1;
        lblRangoSalarial.Text = string.Empty;
    }

    private void CargarEmpleados()
    {
        string filtro = txtBuscar.Text.Trim();

        DataTable datos = ConexionBD.ObtenerDataTable(
            "SELECT e.EmpleadoID, e.Nombre, e.DepartamentoID, d.Nombre AS Departamento, " +
            "       e.CargoID, c.Nombre AS Cargo, e.SalarioBase, e.TipoContrato, " +
            "       e.AplicaDescuentos, e.FechaInicioContrato, e.Activo " +
            "FROM dbo.Empleados e " +
            "INNER JOIN dbo.Departamentos d ON d.DepartamentoID = e.DepartamentoID " +
            "INNER JOIN dbo.Cargos c ON c.CargoID = e.CargoID " +
            "WHERE e.Nombre LIKE @Filtro OR d.Nombre LIKE @Filtro OR c.Nombre LIKE @Filtro " +
            "ORDER BY e.Nombre;",
            new SqlParameter("@Filtro", SqlDbType.NVarChar, 160) { Value = $"%{filtro}%" });

        _origen.DataSource = datos;
        dgvEmpleados.DataSource = _origen;

        OcultarColumna("DepartamentoID");
        OcultarColumna("CargoID");
    }

    private void OcultarColumna(string nombre)
    {
        if (dgvEmpleados.Columns[nombre] is DataGridViewColumn columna)
            columna.Visible = false;
    }

    #endregion

    #region Eventos de controles

    private void txtBuscar_TextChanged(object? sender, EventArgs e)
    {
        CargarEmpleados();
    }

    private void cmbDepartamento_SelectedIndexChanged(object? sender, EventArgs e)
    {
        CargarCargos();
    }

    private void cmbCargo_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (cmbCargo.SelectedItem is DataRowView cargo)
        {
            string minimo = cargo["SalarioMinimo"] is decimal min ? min.ToString("N2") : "—";
            string maximo = cargo["SalarioMaximo"] is decimal max ? max.ToString("N2") : "—";
            lblRangoSalarial.Text = $"Rango del cargo: {minimo} a {maximo}";
        }
        else
        {
            lblRangoSalarial.Text = string.Empty;
        }
    }

    private void dgvEmpleados_CellClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0)
            return;

        DataGridViewRow fila = dgvEmpleados.Rows[e.RowIndex];

        _empleadoId = Convert.ToInt32(fila.Cells["EmpleadoID"].Value);
        lblEmpleadoID.Text = _empleadoId.Value.ToString();
        txtNombre.Text = fila.Cells["Nombre"].Value?.ToString() ?? string.Empty;

        // Al fijar el departamento se recargan los cargos; luego se elige el cargo.
        cmbDepartamento.SelectedValue = Convert.ToInt32(fila.Cells["DepartamentoID"].Value);
        cmbCargo.SelectedValue = Convert.ToInt32(fila.Cells["CargoID"].Value);

        txtSalarioBase.Text = Convert.ToDecimal(fila.Cells["SalarioBase"].Value).ToString("0.00", CultureInfo.CurrentCulture);
        cmbTipoContrato.SelectedItem = fila.Cells["TipoContrato"].Value?.ToString();
        chkAplicaDescuentos.Checked = Convert.ToBoolean(fila.Cells["AplicaDescuentos"].Value);
        dtpFechaInicio.Value = Convert.ToDateTime(fila.Cells["FechaInicioContrato"].Value);
        chkActivo.Checked = Convert.ToBoolean(fila.Cells["Activo"].Value);
    }

    private void btnLimpiar_Click(object? sender, EventArgs e)
    {
        LimpiarCampos();
    }

    private void btnSalir_Click(object? sender, EventArgs e)
    {
        // El menú principal (MenuP) aún no existe en el proyecto C#;
        // por ahora se cierra el formulario. Se enlazará cuando esté el menú.
        Close();
    }

    private void btnAyuda_Click(object? sender, EventArgs e)
    {
        MessageBox.Show(
            "La ventana 'Empleados' permite gestionar el personal de la empresa: agregar, editar y eliminar empleados, " +
            "asignarles departamento y cargo, definir su salario base, tipo de contrato (Permanente o Servicios Profesionales), " +
            "fecha de inicio, si aplican descuentos de ley (Seguro Social y Seguro Educativo) y su estado (activo o inactivo).\n\n" +
            "El nombre debe escribirse en formato 'Apellido, Nombre'. El salario debe estar dentro del rango del cargo, si este lo define. " +
            "Si un empleado ya tiene nóminas emitidas no se puede eliminar; en ese caso desmárquelo como 'Activo'.\n\n" +
            "Use el cuadro de búsqueda para filtrar por nombre, departamento o cargo.",
            "Ayuda - Empleados",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
    }

    #endregion

    #region CRUD

    private void btnAgregar_Click(object? sender, EventArgs e)
    {
        DatosEmpleado? datos = ValidarCampos();
        if (datos is null)
            return;

        try
        {
            if (EmpleadoDuplicado(datos.Nombre, null))
            {
                MostrarAdvertencia("Ya existe un empleado con el mismo nombre.");
                return;
            }

            List<SqlParameter> parametros = CrearParametros(datos);
            object? resultado = ConexionBD.EjecutarEscalar(
                "INSERT INTO dbo.Empleados (Nombre, DepartamentoID, CargoID, SalarioBase, TipoContrato, AplicaDescuentos, FechaInicioContrato, Activo) " +
                "VALUES (@Nombre, @DepartamentoID, @CargoID, @SalarioBase, @TipoContrato, @AplicaDescuentos, @FechaInicioContrato, @Activo); " +
                "SELECT CAST(SCOPE_IDENTITY() AS int);",
                parametros.ToArray());

            int nuevoId = Convert.ToInt32(resultado);
            BitacoraService.Registrar("CREAR", $"Se creó el empleado {datos.Nombre}", "Empleados", nuevoId);

            CargarEmpleados();
            LimpiarCampos();
        }
        catch (SqlException ex) when (ex.Number is 2627 or 2601)
        {
            MostrarAdvertencia("Ya existe un empleado con el mismo nombre.");
        }
        catch (Exception ex)
        {
            MostrarError($"No se pudo agregar el empleado: {ex.Message}");
        }
    }

    private void btnEditar_Click(object? sender, EventArgs e)
    {
        if (_empleadoId is not int empleadoId)
        {
            MostrarAdvertencia("Seleccione un empleado de la lista para editarlo.");
            return;
        }

        DatosEmpleado? datos = ValidarCampos();
        if (datos is null)
            return;

        try
        {
            if (EmpleadoDuplicado(datos.Nombre, empleadoId))
            {
                MostrarAdvertencia("Ya existe otro empleado con el mismo nombre.");
                return;
            }

            List<SqlParameter> parametros = CrearParametros(datos);
            parametros.Add(new SqlParameter("@EmpleadoID", SqlDbType.Int) { Value = empleadoId });

            ConexionBD.EjecutarNonQuery(
                "UPDATE dbo.Empleados SET Nombre = @Nombre, DepartamentoID = @DepartamentoID, CargoID = @CargoID, " +
                "SalarioBase = @SalarioBase, TipoContrato = @TipoContrato, AplicaDescuentos = @AplicaDescuentos, " +
                "FechaInicioContrato = @FechaInicioContrato, Activo = @Activo " +
                "WHERE EmpleadoID = @EmpleadoID;",
                parametros.ToArray());

            BitacoraService.Registrar("ACTUALIZAR", $"Se actualizó el empleado {datos.Nombre}", "Empleados", empleadoId);

            CargarEmpleados();
            LimpiarCampos();
        }
        catch (SqlException ex) when (ex.Number is 2627 or 2601)
        {
            MostrarAdvertencia("Ya existe otro empleado con el mismo nombre.");
        }
        catch (Exception ex)
        {
            MostrarError($"No se pudo actualizar el empleado: {ex.Message}");
        }
    }

    // Corrección: Nominas.EmpleadoID ya no existe. La relación empleado-nómina
    // ahora pasa por NominaDetalle, así que se verifica ahí.
    private void btnEliminar_Click(object? sender, EventArgs e)
    {
        if (_empleadoId is not int empleadoId)
        {
            MostrarAdvertencia("Seleccione un empleado de la lista para eliminarlo.");
            return;
        }

        DialogResult confirmacion = MessageBox.Show(
            $"¿Desea eliminar al empleado '{txtNombre.Text.Trim()}'? Esta acción no se puede deshacer.",
            "Confirmar eliminación",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (confirmacion != DialogResult.Yes)
            return;

        try
        {
            object? emitidas = ConexionBD.EjecutarEscalar(
                "SELECT COUNT(*) FROM dbo.NominaDetalle nd " +
                "INNER JOIN dbo.Nominas n ON n.NominaID = nd.NominaID " +
                "WHERE nd.EmpleadoID = @EmpleadoID AND n.Estado = N'Emitida';",
                new SqlParameter("@EmpleadoID", SqlDbType.Int) { Value = empleadoId });

            if (Convert.ToInt32(emitidas) > 0)
            {
                MostrarError(
                    "No se puede eliminar este empleado porque ya tiene una nómina emitida. " +
                    "Puede desmarcar 'Activo' para que deje de incluirse en nuevas nóminas.");
                return;
            }

            string nombre = txtNombre.Text.Trim();
            ConexionBD.EjecutarNonQuery(
                "DELETE FROM dbo.Empleados WHERE EmpleadoID = @EmpleadoID;",
                new SqlParameter("@EmpleadoID", SqlDbType.Int) { Value = empleadoId });

            BitacoraService.Registrar("ELIMINAR", $"Se eliminó el empleado {nombre}", "Empleados", empleadoId);

            CargarEmpleados();
            LimpiarCampos();
            MessageBox.Show("El empleado ha sido eliminado correctamente.", "Información",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (SqlException ex) when (ex.Number == 547)
        {
            // Violación de FK: tiene turnos, asistencias u otros registros asociados.
            MostrarError(
                "No se puede eliminar este empleado porque tiene registros asociados (turnos, asistencias o nóminas). " +
                "Desmarque 'Activo' en su lugar.");
        }
        catch (Exception ex)
        {
            // Incluye el error del trigger TR_Empleados_Bitacora (nómina emitida).
            MostrarError($"No se pudo eliminar el empleado: {ex.Message}");
        }
    }

    #endregion

    #region Validación y utilidades

    private void LimpiarCampos()
    {
        _empleadoId = null;
        lblEmpleadoID.Text = string.Empty;
        txtNombre.Clear();
        cmbDepartamento.SelectedIndex = -1;
        cmbCargo.DataSource = null;
        lblRangoSalarial.Text = string.Empty;
        txtSalarioBase.Clear();
        cmbTipoContrato.SelectedIndex = 0;
        chkAplicaDescuentos.Checked = true;
        chkActivo.Checked = true;
        dtpFechaInicio.Value = DateTime.Today;
        dgvEmpleados.ClearSelection();
    }

    // Corrección: se usa decimal.TryParse en lugar de Decimal.Parse, que lanzaba
    // FormatException si el usuario escribía texto o dejaba un formato inválido.
    private DatosEmpleado? ValidarCampos()
    {
        string nombre = txtNombre.Text.Trim();

        if (string.IsNullOrWhiteSpace(nombre))
        {
            MostrarAdvertencia("El campo Nombre (Apellido, Nombre) es obligatorio.");
            txtNombre.Focus();
            return null;
        }

        if (!nombre.Contains(','))
        {
            MostrarAdvertencia("El campo Nombre debe estar en formato 'Apellido, Nombre'.");
            txtNombre.Focus();
            return null;
        }

        if (nombre.Length > 150)
        {
            MostrarAdvertencia("El nombre no puede superar los 150 caracteres.");
            txtNombre.Focus();
            return null;
        }

        if (cmbDepartamento.SelectedValue is not int departamentoId)
        {
            MostrarAdvertencia("Seleccione un departamento.");
            cmbDepartamento.Focus();
            return null;
        }

        if (cmbCargo.SelectedValue is not int cargoId || cmbCargo.SelectedItem is not DataRowView cargo)
        {
            MostrarAdvertencia("Seleccione un cargo.");
            cmbCargo.Focus();
            return null;
        }

        if (!decimal.TryParse(txtSalarioBase.Text.Trim(), NumberStyles.Number, CultureInfo.CurrentCulture, out decimal salario)
            || salario <= 0)
        {
            MostrarAdvertencia("El Salario Base debe ser un número mayor a 0.");
            txtSalarioBase.Focus();
            return null;
        }

        salario = Math.Round(salario, 2);

        if (cargo["SalarioMinimo"] is decimal minimo && salario < minimo)
        {
            MostrarAdvertencia($"El salario es menor al mínimo del cargo ({minimo:N2}).");
            txtSalarioBase.Focus();
            return null;
        }

        if (cargo["SalarioMaximo"] is decimal maximo && salario > maximo)
        {
            MostrarAdvertencia($"El salario supera el máximo del cargo ({maximo:N2}).");
            txtSalarioBase.Focus();
            return null;
        }

        if (cmbTipoContrato.SelectedItem is not string tipoContrato)
        {
            MostrarAdvertencia("Seleccione el tipo de contrato.");
            cmbTipoContrato.Focus();
            return null;
        }

        return new DatosEmpleado(
            nombre,
            departamentoId,
            cargoId,
            salario,
            tipoContrato,
            chkAplicaDescuentos.Checked,
            dtpFechaInicio.Value.Date,
            chkActivo.Checked);
    }

    private static bool EmpleadoDuplicado(string nombre, int? empleadoIdExcluido)
    {
        object? resultado = ConexionBD.EjecutarEscalar(
            "SELECT COUNT(*) FROM dbo.Empleados WHERE Nombre = @Nombre AND EmpleadoID <> @EmpleadoID;",
            new SqlParameter("@Nombre", SqlDbType.NVarChar, 150) { Value = nombre },
            new SqlParameter("@EmpleadoID", SqlDbType.Int) { Value = empleadoIdExcluido ?? 0 });

        return Convert.ToInt32(resultado) > 0;
    }

    private static List<SqlParameter> CrearParametros(DatosEmpleado datos)
    {
        return
        [
            new SqlParameter("@Nombre", SqlDbType.NVarChar, 150) { Value = datos.Nombre },
            new SqlParameter("@DepartamentoID", SqlDbType.Int) { Value = datos.DepartamentoId },
            new SqlParameter("@CargoID", SqlDbType.Int) { Value = datos.CargoId },
            new SqlParameter("@SalarioBase", SqlDbType.Decimal) { Precision = 18, Scale = 2, Value = datos.SalarioBase },
            new SqlParameter("@TipoContrato", SqlDbType.NVarChar, 40) { Value = datos.TipoContrato },
            new SqlParameter("@AplicaDescuentos", SqlDbType.Bit) { Value = datos.AplicaDescuentos },
            new SqlParameter("@FechaInicioContrato", SqlDbType.Date) { Value = datos.FechaInicioContrato },
            new SqlParameter("@Activo", SqlDbType.Bit) { Value = datos.Activo }
        ];
    }

    private static void MostrarAdvertencia(string mensaje)
    {
        MessageBox.Show(mensaje, "Empleados", MessageBoxButtons.OK, MessageBoxIcon.Warning);
    }

    private static void MostrarError(string mensaje)
    {
        MessageBox.Show(mensaje, "Empleados", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }

    #endregion
}