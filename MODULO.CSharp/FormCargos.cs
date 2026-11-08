using System.Data;
using System.Globalization;
using Microsoft.Data.SqlClient;

namespace MODULO.CSharp;

public partial class FormCargos : Form
{
    private const NumberStyles EstiloMonto =
        NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite;

    private const decimal MontoMaximoPermitido = 9_999_999_999_999m; // límite de decimal(18,2)

    private readonly BindingSource _origen = new();
    private int? _cargoId;

    /// <summary>Datos ya validados del formulario, listos para enviarse a la base.</summary>
    private sealed record DatosCargo(
        int DepartamentoId,
        string Nombre,
        string? Descripcion,
        decimal? SalarioMinimo,
        decimal? SalarioMaximo,
        bool Activo);

    public FormCargos()
    {
        InitializeComponent();
    }

    #region Carga inicial

    private void FormCargos_Load(object? sender, EventArgs e)
    {
        AplicarEstiloBotones();
        CargarDepartamentos();
        CargarCargos();
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

    private void CargarDepartamentos()
    {
        DataTable departamentos = ConexionBD.ObtenerDataTable(
            "SELECT DepartamentoID, Nombre FROM dbo.Departamentos WHERE Activo = 1 ORDER BY Nombre;");

        cmbDepartamento.DisplayMember = "Nombre";
        cmbDepartamento.ValueMember = "DepartamentoID";
        cmbDepartamento.DataSource = departamentos;
        cmbDepartamento.SelectedIndex = -1;
    }

    private void CargarCargos()
    {
        string filtro = txtBuscar.Text.Trim();

        DataTable datos = ConexionBD.ObtenerDataTable(
            "SELECT c.CargoID, c.DepartamentoID, d.Nombre AS Departamento, c.Nombre, c.Descripcion, " +
            "       c.SalarioMinimo, c.SalarioMaximo, c.Activo, " +
            "       (SELECT COUNT(*) FROM dbo.Empleados e WHERE e.CargoID = c.CargoID) AS Empleados " +
            "FROM dbo.Cargos c " +
            "INNER JOIN dbo.Departamentos d ON d.DepartamentoID = c.DepartamentoID " +
            "WHERE c.Nombre LIKE @Filtro OR d.Nombre LIKE @Filtro " +
            "ORDER BY d.Nombre, c.Nombre;",
            new SqlParameter("@Filtro", SqlDbType.NVarChar, 110) { Value = $"%{filtro}%" });

        _origen.DataSource = datos;
        dgvCargos.DataSource = _origen;

        if (dgvCargos.Columns["CargoID"] is DataGridViewColumn colId)
            colId.Visible = false;
        if (dgvCargos.Columns["DepartamentoID"] is DataGridViewColumn colDepto)
            colDepto.Visible = false;
        if (dgvCargos.Columns["SalarioMinimo"] is DataGridViewColumn colMin)
            colMin.DefaultCellStyle.Format = "N2";
        if (dgvCargos.Columns["SalarioMaximo"] is DataGridViewColumn colMax)
            colMax.DefaultCellStyle.Format = "N2";
    }

    #endregion

    #region Eventos de controles

    private void txtBuscar_TextChanged(object? sender, EventArgs e)
    {
        CargarCargos();
    }

    private void dgvCargos_CellClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0)
            return;

        DataGridViewRow fila = dgvCargos.Rows[e.RowIndex];

        _cargoId = Convert.ToInt32(fila.Cells["CargoID"].Value);
        lblCargoID.Text = _cargoId.Value.ToString();
        cmbDepartamento.SelectedValue = Convert.ToInt32(fila.Cells["DepartamentoID"].Value);
        txtNombre.Text = fila.Cells["Nombre"].Value?.ToString() ?? string.Empty;
        txtDescripcion.Text = fila.Cells["Descripcion"].Value?.ToString() ?? string.Empty;
        txtSalarioMinimo.Text = fila.Cells["SalarioMinimo"].Value is decimal min
            ? min.ToString("0.00", CultureInfo.CurrentCulture)
            : string.Empty;
        txtSalarioMaximo.Text = fila.Cells["SalarioMaximo"].Value is decimal max
            ? max.ToString("0.00", CultureInfo.CurrentCulture)
            : string.Empty;
        chkActivo.Checked = Convert.ToBoolean(fila.Cells["Activo"].Value);
    }

    private void btnLimpiar_Click(object? sender, EventArgs e)
    {
        LimpiarCampos();
    }

    private void btnSalir_Click(object? sender, EventArgs e)
    {
        // Se enlazará con el menú principal cuando exista en el proyecto C#.
        Close();
    }

    private void btnAyuda_Click(object? sender, EventArgs e)
    {
        MessageBox.Show(
            "La ventana 'Cargos' permite administrar los puestos de la empresa. Cada cargo pertenece a un departamento " +
            "y puede tener un rango salarial (mínimo y máximo, ambos opcionales). Ese rango se usa al registrar empleados: " +
            "el salario base debe estar dentro del rango de su cargo.\n\n" +
            "El nombre del cargo no puede repetirse dentro del mismo departamento. " +
            "Un cargo con empleados asignados no se puede eliminar ni cambiar de departamento; en ese caso desmarque 'Activo' " +
            "para que deje de ofrecerse al registrar nuevos empleados.\n\n" +
            "Use el cuadro de búsqueda para filtrar por cargo o departamento.",
            "Ayuda - Cargos",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
    }

    #endregion

    #region CRUD

    private void btnAgregar_Click(object? sender, EventArgs e)
    {
        DatosCargo? datos = ValidarCampos();
        if (datos is null)
            return;

        try
        {
            if (CargoDuplicado(datos.DepartamentoId, datos.Nombre, null))
            {
                MostrarAdvertencia("Ya existe un cargo con ese nombre en el departamento seleccionado.");
                return;
            }

            object? resultado = ConexionBD.EjecutarEscalar(
                "INSERT INTO dbo.Cargos (DepartamentoID, Nombre, Descripcion, SalarioMinimo, SalarioMaximo, Activo) " +
                "VALUES (@DepartamentoID, @Nombre, @Descripcion, @SalarioMinimo, @SalarioMaximo, @Activo); " +
                "SELECT CAST(SCOPE_IDENTITY() AS int);",
                CrearParametros(datos).ToArray());

            int nuevoId = Convert.ToInt32(resultado);
            BitacoraService.Registrar("CREAR", $"Se creó el cargo {datos.Nombre}", "Cargos", nuevoId);

            CargarCargos();
            LimpiarCampos();
        }
        catch (SqlException ex) when (ex.Number is 2627 or 2601)
        {
            MostrarAdvertencia("Ya existe un cargo con ese nombre en el departamento seleccionado.");
        }
        catch (Exception ex)
        {
            MostrarError($"No se pudo agregar el cargo: {ex.Message}");
        }
    }

    private void btnEditar_Click(object? sender, EventArgs e)
    {
        if (_cargoId is not int cargoId)
        {
            MostrarAdvertencia("Seleccione un cargo de la lista para editarlo.");
            return;
        }

        DatosCargo? datos = ValidarCampos();
        if (datos is null)
            return;

        try
        {
            if (CargoDuplicado(datos.DepartamentoId, datos.Nombre, cargoId))
            {
                MostrarAdvertencia("Ya existe otro cargo con ese nombre en el departamento seleccionado.");
                return;
            }

            // Si el nuevo rango deja fuera a empleados que ya tienen este cargo, se avisa antes de guardar.
            int fueraDeRango = ContarEmpleadosFueraDeRango(cargoId, datos.SalarioMinimo, datos.SalarioMaximo);
            if (fueraDeRango > 0)
            {
                DialogResult continuar = MessageBox.Show(
                    $"{fueraDeRango} empleado(s) con este cargo tienen un salario fuera del nuevo rango. " +
                    "Sus salarios no se modificarán, pero no podrán editarse hasta ajustarlos al rango. ¿Desea continuar?",
                    "Rango salarial",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (continuar != DialogResult.Yes)
                    return;
            }

            List<SqlParameter> parametros = CrearParametros(datos);
            parametros.Add(new SqlParameter("@CargoID", SqlDbType.Int) { Value = cargoId });

            ConexionBD.EjecutarNonQuery(
                "UPDATE dbo.Cargos SET DepartamentoID = @DepartamentoID, Nombre = @Nombre, Descripcion = @Descripcion, " +
                "SalarioMinimo = @SalarioMinimo, SalarioMaximo = @SalarioMaximo, Activo = @Activo " +
                "WHERE CargoID = @CargoID;",
                parametros.ToArray());

            BitacoraService.Registrar("ACTUALIZAR", $"Se actualizó el cargo {datos.Nombre}", "Cargos", cargoId);

            CargarCargos();
            LimpiarCampos();
        }
        catch (SqlException ex) when (ex.Number is 2627 or 2601)
        {
            MostrarAdvertencia("Ya existe otro cargo con ese nombre en el departamento seleccionado.");
        }
        catch (SqlException ex) when (ex.Number == 547)
        {
            // La FK compuesta de Empleados (CargoID, DepartamentoID) impide mover un cargo con empleados.
            MostrarError("No se puede cambiar el departamento de un cargo que tiene empleados asignados.");
        }
        catch (Exception ex)
        {
            MostrarError($"No se pudo actualizar el cargo: {ex.Message}");
        }
    }

    private void btnEliminar_Click(object? sender, EventArgs e)
    {
        if (_cargoId is not int cargoId)
        {
            MostrarAdvertencia("Seleccione un cargo de la lista para eliminarlo.");
            return;
        }

        DialogResult confirmacion = MessageBox.Show(
            $"¿Desea eliminar el cargo '{txtNombre.Text.Trim()}'? Esta acción no se puede deshacer.",
            "Confirmar eliminación",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (confirmacion != DialogResult.Yes)
            return;

        try
        {
            object? empleados = ConexionBD.EjecutarEscalar(
                "SELECT COUNT(*) FROM dbo.Empleados WHERE CargoID = @CargoID;",
                new SqlParameter("@CargoID", SqlDbType.Int) { Value = cargoId });

            if (Convert.ToInt32(empleados) > 0)
            {
                MostrarError(
                    "No se puede eliminar este cargo porque tiene empleados asignados. " +
                    "Puede desmarcar 'Activo' para que deje de ofrecerse en nuevos registros.");
                return;
            }

            string nombre = txtNombre.Text.Trim();
            ConexionBD.EjecutarNonQuery(
                "DELETE FROM dbo.Cargos WHERE CargoID = @CargoID;",
                new SqlParameter("@CargoID", SqlDbType.Int) { Value = cargoId });

            BitacoraService.Registrar("ELIMINAR", $"Se eliminó el cargo {nombre}", "Cargos", cargoId);

            CargarCargos();
            LimpiarCampos();
            MessageBox.Show("El cargo ha sido eliminado correctamente.", "Información",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (SqlException ex) when (ex.Number == 547)
        {
            MostrarError("No se puede eliminar este cargo porque tiene registros asociados. Desmarque 'Activo' en su lugar.");
        }
        catch (Exception ex)
        {
            MostrarError($"No se pudo eliminar el cargo: {ex.Message}");
        }
    }

    #endregion

    #region Validación y utilidades

    private void LimpiarCampos()
    {
        _cargoId = null;
        lblCargoID.Text = string.Empty;
        cmbDepartamento.SelectedIndex = -1;
        txtNombre.Clear();
        txtDescripcion.Clear();
        txtSalarioMinimo.Clear();
        txtSalarioMaximo.Clear();
        chkActivo.Checked = true;
        dgvCargos.ClearSelection();
    }

    private DatosCargo? ValidarCampos()
    {
        if (cmbDepartamento.SelectedValue is not int departamentoId)
        {
            MostrarAdvertencia("Seleccione un departamento.");
            cmbDepartamento.Focus();
            return null;
        }

        string nombre = txtNombre.Text.Trim();
        if (string.IsNullOrWhiteSpace(nombre))
        {
            MostrarAdvertencia("El nombre del cargo es obligatorio.");
            txtNombre.Focus();
            return null;
        }

        if (nombre.Length > 100)
        {
            MostrarAdvertencia("El nombre del cargo no puede superar los 100 caracteres.");
            txtNombre.Focus();
            return null;
        }

        string descripcion = txtDescripcion.Text.Trim();
        if (descripcion.Length > 300)
        {
            MostrarAdvertencia("La descripción no puede superar los 300 caracteres.");
            txtDescripcion.Focus();
            return null;
        }

        if (!TryObtenerMonto(txtSalarioMinimo, "Salario mínimo", out decimal? minimo))
            return null;

        if (!TryObtenerMonto(txtSalarioMaximo, "Salario máximo", out decimal? maximo))
            return null;

        if (minimo.HasValue && maximo.HasValue && maximo < minimo)
        {
            MostrarAdvertencia("El salario máximo no puede ser menor que el salario mínimo.");
            txtSalarioMaximo.Focus();
            return null;
        }

        return new DatosCargo(
            departamentoId,
            nombre,
            descripcion.Length == 0 ? null : descripcion,
            minimo,
            maximo,
            chkActivo.Checked);
    }

    /// <summary>
    /// Lee un monto opcional: vacío = sin límite (NULL). Si trae texto, debe ser un número válido, 0 o mayor.
    /// Devuelve false (y avisa al usuario) solo cuando el texto es inválido.
    /// </summary>
    private bool TryObtenerMonto(TextBox caja, string nombreCampo, out decimal? monto)
    {
        monto = null;
        string texto = caja.Text.Trim();

        if (texto.Length == 0)
            return true;

        if (!decimal.TryParse(texto, EstiloMonto, CultureInfo.CurrentCulture, out decimal valor)
            || valor < 0
            || valor > MontoMaximoPermitido)
        {
            MostrarAdvertencia($"{nombreCampo} debe ser un número igual o mayor a 0, sin separador de miles (ejemplo: 1250.50), o quedar vacío.");
            caja.Focus();
            return false;
        }

        monto = Math.Round(valor, 2);
        return true;
    }

    private static bool CargoDuplicado(int departamentoId, string nombre, int? cargoIdExcluido)
    {
        object? resultado = ConexionBD.EjecutarEscalar(
            "SELECT COUNT(*) FROM dbo.Cargos WHERE DepartamentoID = @DepartamentoID AND Nombre = @Nombre AND CargoID <> @CargoID;",
            new SqlParameter("@DepartamentoID", SqlDbType.Int) { Value = departamentoId },
            new SqlParameter("@Nombre", SqlDbType.NVarChar, 100) { Value = nombre },
            new SqlParameter("@CargoID", SqlDbType.Int) { Value = cargoIdExcluido ?? 0 });

        return Convert.ToInt32(resultado) > 0;
    }

    private static int ContarEmpleadosFueraDeRango(int cargoId, decimal? minimo, decimal? maximo)
    {
        object? resultado = ConexionBD.EjecutarEscalar(
            "SELECT COUNT(*) FROM dbo.Empleados " +
            "WHERE CargoID = @CargoID " +
            "AND ((@SalarioMinimo IS NOT NULL AND SalarioBase < @SalarioMinimo) " +
            "  OR (@SalarioMaximo IS NOT NULL AND SalarioBase > @SalarioMaximo));",
            new SqlParameter("@CargoID", SqlDbType.Int) { Value = cargoId },
            CrearParametroMonto("@SalarioMinimo", minimo),
            CrearParametroMonto("@SalarioMaximo", maximo));

        return Convert.ToInt32(resultado);
    }

    private static SqlParameter CrearParametroMonto(string nombre, decimal? valor)
    {
        return new SqlParameter(nombre, SqlDbType.Decimal)
        {
            Precision = 18,
            Scale = 2,
            Value = (object?)valor ?? DBNull.Value
        };
    }

    private static List<SqlParameter> CrearParametros(DatosCargo datos)
    {
        return
        [
            new SqlParameter("@DepartamentoID", SqlDbType.Int) { Value = datos.DepartamentoId },
            new SqlParameter("@Nombre", SqlDbType.NVarChar, 100) { Value = datos.Nombre },
            new SqlParameter("@Descripcion", SqlDbType.NVarChar, 300) { Value = (object?)datos.Descripcion ?? DBNull.Value },
            CrearParametroMonto("@SalarioMinimo", datos.SalarioMinimo),
            CrearParametroMonto("@SalarioMaximo", datos.SalarioMaximo),
            new SqlParameter("@Activo", SqlDbType.Bit) { Value = datos.Activo }
        ];
    }

    private static void MostrarAdvertencia(string mensaje)
    {
        MessageBox.Show(mensaje, "Cargos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
    }

    private static void MostrarError(string mensaje)
    {
        MessageBox.Show(mensaje, "Cargos", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }

    #endregion
}