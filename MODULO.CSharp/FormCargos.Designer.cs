namespace MODULO.CSharp;

partial class FormCargos
{
    private System.ComponentModel.IContainer? components = null;

    private Label lblIdTexto;
    private Label lblCargoID;
    private Label lblDepartamento;
    private ComboBox cmbDepartamento;
    private Label lblNombre;
    private TextBox txtNombre;
    private Label lblDescripcion;
    private TextBox txtDescripcion;
    private Label lblSalarioMinimo;
    private TextBox txtSalarioMinimo;
    private Label lblSalarioMaximo;
    private TextBox txtSalarioMaximo;
    private CheckBox chkActivo;
    private Button btnAgregar;
    private Button btnEditar;
    private Button btnEliminar;
    private Button btnLimpiar;
    private Button btnAyuda;
    private Button btnSalir;
    private Label lblBuscar;
    private TextBox txtBuscar;
    private DataGridView dgvCargos;

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
        lblCargoID = new Label();
        lblDepartamento = new Label();
        cmbDepartamento = new ComboBox();
        lblNombre = new Label();
        txtNombre = new TextBox();
        lblDescripcion = new Label();
        txtDescripcion = new TextBox();
        lblSalarioMinimo = new Label();
        txtSalarioMinimo = new TextBox();
        lblSalarioMaximo = new Label();
        txtSalarioMaximo = new TextBox();
        chkActivo = new CheckBox();
        btnAgregar = new Button();
        btnEditar = new Button();
        btnEliminar = new Button();
        btnLimpiar = new Button();
        btnAyuda = new Button();
        btnSalir = new Button();
        lblBuscar = new Label();
        txtBuscar = new TextBox();
        dgvCargos = new DataGridView();
        ((System.ComponentModel.ISupportInitialize)dgvCargos).BeginInit();
        SuspendLayout();

        // lblIdTexto
        lblIdTexto.AutoSize = true;
        lblIdTexto.Location = new Point(30, 15);
        lblIdTexto.Name = "lblIdTexto";
        lblIdTexto.Text = "ID:";

        // lblCargoID
        lblCargoID.AutoSize = true;
        lblCargoID.Location = new Point(60, 15);
        lblCargoID.Name = "lblCargoID";

        // lblDepartamento
        lblDepartamento.AutoSize = true;
        lblDepartamento.Location = new Point(30, 45);
        lblDepartamento.Name = "lblDepartamento";
        lblDepartamento.Text = "Departamento:";

        // cmbDepartamento
        cmbDepartamento.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbDepartamento.FormattingEnabled = true;
        cmbDepartamento.Location = new Point(30, 66);
        cmbDepartamento.Name = "cmbDepartamento";
        cmbDepartamento.Size = new Size(250, 23);
        cmbDepartamento.TabIndex = 0;

        // lblNombre
        lblNombre.AutoSize = true;
        lblNombre.Location = new Point(30, 100);
        lblNombre.Name = "lblNombre";
        lblNombre.Text = "Nombre del cargo:";

        // txtNombre
        txtNombre.Location = new Point(30, 121);
        txtNombre.MaxLength = 100;
        txtNombre.Name = "txtNombre";
        txtNombre.Size = new Size(250, 23);
        txtNombre.TabIndex = 1;

        // lblDescripcion
        lblDescripcion.AutoSize = true;
        lblDescripcion.Location = new Point(30, 155);
        lblDescripcion.Name = "lblDescripcion";
        lblDescripcion.Text = "Descripción (opcional):";

        // txtDescripcion
        txtDescripcion.Location = new Point(30, 176);
        txtDescripcion.MaxLength = 300;
        txtDescripcion.Multiline = true;
        txtDescripcion.Name = "txtDescripcion";
        txtDescripcion.ScrollBars = ScrollBars.Vertical;
        txtDescripcion.Size = new Size(250, 60);
        txtDescripcion.TabIndex = 2;

        // lblSalarioMinimo
        lblSalarioMinimo.AutoSize = true;
        lblSalarioMinimo.Location = new Point(30, 250);
        lblSalarioMinimo.Name = "lblSalarioMinimo";
        lblSalarioMinimo.Text = "Salario mínimo (opcional):";

        // txtSalarioMinimo
        txtSalarioMinimo.Location = new Point(30, 271);
        txtSalarioMinimo.Name = "txtSalarioMinimo";
        txtSalarioMinimo.Size = new Size(250, 23);
        txtSalarioMinimo.TabIndex = 3;

        // lblSalarioMaximo
        lblSalarioMaximo.AutoSize = true;
        lblSalarioMaximo.Location = new Point(30, 305);
        lblSalarioMaximo.Name = "lblSalarioMaximo";
        lblSalarioMaximo.Text = "Salario máximo (opcional):";

        // txtSalarioMaximo
        txtSalarioMaximo.Location = new Point(30, 326);
        txtSalarioMaximo.Name = "txtSalarioMaximo";
        txtSalarioMaximo.Size = new Size(250, 23);
        txtSalarioMaximo.TabIndex = 4;

        // chkActivo
        chkActivo.AutoSize = true;
        chkActivo.Checked = true;
        chkActivo.CheckState = CheckState.Checked;
        chkActivo.Location = new Point(30, 365);
        chkActivo.Name = "chkActivo";
        chkActivo.TabIndex = 5;
        chkActivo.Text = "Activo";

        // btnAgregar
        btnAgregar.Location = new Point(30, 400);
        btnAgregar.Name = "btnAgregar";
        btnAgregar.Size = new Size(120, 38);
        btnAgregar.TabIndex = 6;
        btnAgregar.Text = "Agregar";
        btnAgregar.UseVisualStyleBackColor = true;
        btnAgregar.Click += btnAgregar_Click;

        // btnEditar
        btnEditar.Location = new Point(160, 400);
        btnEditar.Name = "btnEditar";
        btnEditar.Size = new Size(120, 38);
        btnEditar.TabIndex = 7;
        btnEditar.Text = "Editar";
        btnEditar.UseVisualStyleBackColor = true;
        btnEditar.Click += btnEditar_Click;

        // btnEliminar
        btnEliminar.Location = new Point(30, 448);
        btnEliminar.Name = "btnEliminar";
        btnEliminar.Size = new Size(120, 38);
        btnEliminar.TabIndex = 8;
        btnEliminar.Text = "Eliminar";
        btnEliminar.UseVisualStyleBackColor = true;
        btnEliminar.Click += btnEliminar_Click;

        // btnLimpiar
        btnLimpiar.Location = new Point(160, 448);
        btnLimpiar.Name = "btnLimpiar";
        btnLimpiar.Size = new Size(120, 38);
        btnLimpiar.TabIndex = 9;
        btnLimpiar.Text = "Limpiar";
        btnLimpiar.UseVisualStyleBackColor = true;
        btnLimpiar.Click += btnLimpiar_Click;

        // btnAyuda
        btnAyuda.Location = new Point(30, 496);
        btnAyuda.Name = "btnAyuda";
        btnAyuda.Size = new Size(120, 38);
        btnAyuda.TabIndex = 10;
        btnAyuda.Text = "Ayuda";
        btnAyuda.UseVisualStyleBackColor = true;
        btnAyuda.Click += btnAyuda_Click;

        // btnSalir
        btnSalir.Location = new Point(160, 496);
        btnSalir.Name = "btnSalir";
        btnSalir.Size = new Size(120, 38);
        btnSalir.TabIndex = 11;
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
        txtBuscar.PlaceholderText = "Cargo o departamento";
        txtBuscar.Size = new Size(300, 23);
        txtBuscar.TabIndex = 12;
        txtBuscar.TextChanged += txtBuscar_TextChanged;

        // dgvCargos
        dgvCargos.AllowUserToAddRows = false;
        dgvCargos.AllowUserToDeleteRows = false;
        dgvCargos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
        dgvCargos.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        dgvCargos.Location = new Point(320, 50);
        dgvCargos.MultiSelect = false;
        dgvCargos.Name = "dgvCargos";
        dgvCargos.ReadOnly = true;
        dgvCargos.RowHeadersWidth = 51;
        dgvCargos.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvCargos.Size = new Size(820, 484);
        dgvCargos.TabIndex = 13;
        dgvCargos.CellClick += dgvCargos_CellClick;

        // FormCargos
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1160, 560);
        Controls.Add(dgvCargos);
        Controls.Add(txtBuscar);
        Controls.Add(lblBuscar);
        Controls.Add(btnSalir);
        Controls.Add(btnAyuda);
        Controls.Add(btnLimpiar);
        Controls.Add(btnEliminar);
        Controls.Add(btnEditar);
        Controls.Add(btnAgregar);
        Controls.Add(chkActivo);
        Controls.Add(txtSalarioMaximo);
        Controls.Add(lblSalarioMaximo);
        Controls.Add(txtSalarioMinimo);
        Controls.Add(lblSalarioMinimo);
        Controls.Add(txtDescripcion);
        Controls.Add(lblDescripcion);
        Controls.Add(txtNombre);
        Controls.Add(lblNombre);
        Controls.Add(cmbDepartamento);
        Controls.Add(lblDepartamento);
        Controls.Add(lblCargoID);
        Controls.Add(lblIdTexto);
        Name = "FormCargos";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Cargos";
        Load += FormCargos_Load;
        ((System.ComponentModel.ISupportInitialize)dgvCargos).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion
}