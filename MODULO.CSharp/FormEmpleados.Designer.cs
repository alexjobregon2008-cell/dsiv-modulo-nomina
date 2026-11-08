namespace MODULO.CSharp;

partial class FormEmpleados
{
    private System.ComponentModel.IContainer? components = null;

    private Label lblIdTexto;
    private Label lblEmpleadoID;
    private Label lblNombre;
    private TextBox txtNombre;
    private Label lblDepartamento;
    private ComboBox cmbDepartamento;
    private Label lblCargo;
    private ComboBox cmbCargo;
    private Label lblRangoSalarial;
    private Label lblSalarioBase;
    private TextBox txtSalarioBase;
    private Label lblTipoContrato;
    private ComboBox cmbTipoContrato;
    private Label lblFechaInicio;
    private DateTimePicker dtpFechaInicio;
    private CheckBox chkAplicaDescuentos;
    private CheckBox chkActivo;
    private Button btnAgregar;
    private Button btnEditar;
    private Button btnEliminar;
    private Button btnLimpiar;
    private Button btnAyuda;
    private Button btnSalir;
    private Label lblBuscar;
    private TextBox txtBuscar;
    private DataGridView dgvEmpleados;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        lblIdTexto = new Label();
        lblEmpleadoID = new Label();
        lblNombre = new Label();
        txtNombre = new TextBox();
        lblDepartamento = new Label();
        cmbDepartamento = new ComboBox();
        lblCargo = new Label();
        cmbCargo = new ComboBox();
        lblRangoSalarial = new Label();
        lblSalarioBase = new Label();
        txtSalarioBase = new TextBox();
        lblTipoContrato = new Label();
        cmbTipoContrato = new ComboBox();
        lblFechaInicio = new Label();
        dtpFechaInicio = new DateTimePicker();
        chkAplicaDescuentos = new CheckBox();
        chkActivo = new CheckBox();
        btnAgregar = new Button();
        btnEditar = new Button();
        btnEliminar = new Button();
        btnLimpiar = new Button();
        btnAyuda = new Button();
        btnSalir = new Button();
        lblBuscar = new Label();
        txtBuscar = new TextBox();
        dgvEmpleados = new DataGridView();
        ((System.ComponentModel.ISupportInitialize)dgvEmpleados).BeginInit();
        SuspendLayout();

        // lblIdTexto
        lblIdTexto.AutoSize = true;
        lblIdTexto.Location = new Point(30, 15);
        lblIdTexto.Name = "lblIdTexto";
        lblIdTexto.Text = "ID:";

        // lblEmpleadoID
        lblEmpleadoID.AutoSize = true;
        lblEmpleadoID.Location = new Point(60, 15);
        lblEmpleadoID.Name = "lblEmpleadoID";

        // lblNombre
        lblNombre.AutoSize = true;
        lblNombre.Location = new Point(30, 45);
        lblNombre.Name = "lblNombre";
        lblNombre.Text = "Nombre (Apellido, Nombre):";

        // txtNombre
        txtNombre.Location = new Point(30, 66);
        txtNombre.MaxLength = 150;
        txtNombre.Name = "txtNombre";
        txtNombre.Size = new Size(250, 23);
        txtNombre.TabIndex = 0;

        // lblDepartamento
        lblDepartamento.AutoSize = true;
        lblDepartamento.Location = new Point(30, 100);
        lblDepartamento.Name = "lblDepartamento";
        lblDepartamento.Text = "Departamento:";

        // cmbDepartamento
        cmbDepartamento.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbDepartamento.FormattingEnabled = true;
        cmbDepartamento.Location = new Point(30, 121);
        cmbDepartamento.Name = "cmbDepartamento";
        cmbDepartamento.Size = new Size(250, 23);
        cmbDepartamento.TabIndex = 1;
        cmbDepartamento.SelectedIndexChanged += cmbDepartamento_SelectedIndexChanged;

        // lblCargo
        lblCargo.AutoSize = true;
        lblCargo.Location = new Point(30, 155);
        lblCargo.Name = "lblCargo";
        lblCargo.Text = "Cargo:";

        // cmbCargo
        cmbCargo.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbCargo.FormattingEnabled = true;
        cmbCargo.Location = new Point(30, 176);
        cmbCargo.Name = "cmbCargo";
        cmbCargo.Size = new Size(250, 23);
        cmbCargo.TabIndex = 2;
        cmbCargo.SelectedIndexChanged += cmbCargo_SelectedIndexChanged;

        // lblRangoSalarial
        lblRangoSalarial.AutoSize = true;
        lblRangoSalarial.ForeColor = Color.DimGray;
        lblRangoSalarial.Location = new Point(30, 203);
        lblRangoSalarial.Name = "lblRangoSalarial";

        // lblSalarioBase
        lblSalarioBase.AutoSize = true;
        lblSalarioBase.Location = new Point(30, 230);
        lblSalarioBase.Name = "lblSalarioBase";
        lblSalarioBase.Text = "Salario base:";

        // txtSalarioBase
        txtSalarioBase.Location = new Point(30, 251);
        txtSalarioBase.Name = "txtSalarioBase";
        txtSalarioBase.Size = new Size(250, 23);
        txtSalarioBase.TabIndex = 3;

        // lblTipoContrato
        lblTipoContrato.AutoSize = true;
        lblTipoContrato.Location = new Point(30, 285);
        lblTipoContrato.Name = "lblTipoContrato";
        lblTipoContrato.Text = "Tipo de contrato:";

        // cmbTipoContrato
        cmbTipoContrato.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbTipoContrato.FormattingEnabled = true;
        cmbTipoContrato.Location = new Point(30, 306);
        cmbTipoContrato.Name = "cmbTipoContrato";
        cmbTipoContrato.Size = new Size(250, 23);
        cmbTipoContrato.TabIndex = 4;

        // lblFechaInicio
        lblFechaInicio.AutoSize = true;
        lblFechaInicio.Location = new Point(30, 340);
        lblFechaInicio.Name = "lblFechaInicio";
        lblFechaInicio.Text = "Inicio de contrato:";

        // dtpFechaInicio
        dtpFechaInicio.Format = DateTimePickerFormat.Short;
        dtpFechaInicio.Location = new Point(30, 361);
        dtpFechaInicio.Name = "dtpFechaInicio";
        dtpFechaInicio.Size = new Size(250, 23);
        dtpFechaInicio.TabIndex = 5;

        // chkAplicaDescuentos
        chkAplicaDescuentos.AutoSize = true;
        chkAplicaDescuentos.Checked = true;
        chkAplicaDescuentos.CheckState = CheckState.Checked;
        chkAplicaDescuentos.Location = new Point(30, 398);
        chkAplicaDescuentos.Name = "chkAplicaDescuentos";
        chkAplicaDescuentos.TabIndex = 6;
        chkAplicaDescuentos.Text = "Aplica descuentos";

        // chkActivo
        chkActivo.AutoSize = true;
        chkActivo.Checked = true;
        chkActivo.CheckState = CheckState.Checked;
        chkActivo.Location = new Point(190, 398);
        chkActivo.Name = "chkActivo";
        chkActivo.TabIndex = 7;
        chkActivo.Text = "Activo";

        // btnAgregar
        btnAgregar.Location = new Point(30, 435);
        btnAgregar.Name = "btnAgregar";
        btnAgregar.Size = new Size(120, 38);
        btnAgregar.TabIndex = 8;
        btnAgregar.Text = "Agregar";
        btnAgregar.UseVisualStyleBackColor = true;
        btnAgregar.Click += btnAgregar_Click;

        // btnEditar
        btnEditar.Location = new Point(160, 435);
        btnEditar.Name = "btnEditar";
        btnEditar.Size = new Size(120, 38);
        btnEditar.TabIndex = 9;
        btnEditar.Text = "Editar";
        btnEditar.UseVisualStyleBackColor = true;
        btnEditar.Click += btnEditar_Click;

        // btnEliminar
        btnEliminar.Location = new Point(30, 483);
        btnEliminar.Name = "btnEliminar";
        btnEliminar.Size = new Size(120, 38);
        btnEliminar.TabIndex = 10;
        btnEliminar.Text = "Eliminar";
        btnEliminar.UseVisualStyleBackColor = true;
        btnEliminar.Click += btnEliminar_Click;

        // btnLimpiar
        btnLimpiar.Location = new Point(160, 483);
        btnLimpiar.Name = "btnLimpiar";
        btnLimpiar.Size = new Size(120, 38);
        btnLimpiar.TabIndex = 11;
        btnLimpiar.Text = "Limpiar";
        btnLimpiar.UseVisualStyleBackColor = true;
        btnLimpiar.Click += btnLimpiar_Click;

        // btnAyuda
        btnAyuda.Location = new Point(30, 531);
        btnAyuda.Name = "btnAyuda";
        btnAyuda.Size = new Size(120, 38);
        btnAyuda.TabIndex = 12;
        btnAyuda.Text = "Ayuda";
        btnAyuda.UseVisualStyleBackColor = true;
        btnAyuda.Click += btnAyuda_Click;

        // btnSalir
        btnSalir.Location = new Point(160, 531);
        btnSalir.Name = "btnSalir";
        btnSalir.Size = new Size(120, 38);
        btnSalir.TabIndex = 13;
        btnSalir.Text = "Salir";
        btnSalir.UseVisualStyleBackColor = true;
        btnSalir.Click += btnSalir_Click;

        // lblBuscar
        lblBuscar.AutoSize = true;
        lblBuscar.Location = new Point(320, 18);
        lblBuscar.Name = "lblBuscar";
        lblBuscar.Text = "Buscar:";

        // txtBuscar
        txtBuscar.Location = new Point(375, 15);
        txtBuscar.Name = "txtBuscar";
        txtBuscar.PlaceholderText = "Nombre, departamento o cargo";
        txtBuscar.Size = new Size(300, 23);
        txtBuscar.TabIndex = 14;
        txtBuscar.TextChanged += txtBuscar_TextChanged;

        // dgvEmpleados
        dgvEmpleados.AllowUserToAddRows = false;
        dgvEmpleados.AllowUserToDeleteRows = false;
        dgvEmpleados.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
        dgvEmpleados.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        dgvEmpleados.Location = new Point(320, 50);
        dgvEmpleados.MultiSelect = false;
        dgvEmpleados.Name = "dgvEmpleados";
        dgvEmpleados.ReadOnly = true;
        dgvEmpleados.RowHeadersWidth = 51;
        dgvEmpleados.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvEmpleados.Size = new Size(820, 519);
        dgvEmpleados.TabIndex = 15;
        dgvEmpleados.CellClick += dgvEmpleados_CellClick;

        // FormEmpleados
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1160, 590);
        Controls.Add(dgvEmpleados);
        Controls.Add(txtBuscar);
        Controls.Add(lblBuscar);
        Controls.Add(btnSalir);
        Controls.Add(btnAyuda);
        Controls.Add(btnLimpiar);
        Controls.Add(btnEliminar);
        Controls.Add(btnEditar);
        Controls.Add(btnAgregar);
        Controls.Add(chkActivo);
        Controls.Add(chkAplicaDescuentos);
        Controls.Add(dtpFechaInicio);
        Controls.Add(lblFechaInicio);
        Controls.Add(cmbTipoContrato);
        Controls.Add(lblTipoContrato);
        Controls.Add(txtSalarioBase);
        Controls.Add(lblSalarioBase);
        Controls.Add(lblRangoSalarial);
        Controls.Add(cmbCargo);
        Controls.Add(lblCargo);
        Controls.Add(cmbDepartamento);
        Controls.Add(lblDepartamento);
        Controls.Add(txtNombre);
        Controls.Add(lblNombre);
        Controls.Add(lblEmpleadoID);
        Controls.Add(lblIdTexto);
        Name = "FormEmpleados";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Empleados";
        Load += FormEmpleados_Load;
        ((System.ComponentModel.ISupportInitialize)dgvEmpleados).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion
}