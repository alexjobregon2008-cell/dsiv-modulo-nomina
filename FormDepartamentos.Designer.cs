namespace MODULO.CSharp
{
    partial class FormDepartamentos
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            lblNombre = new Label();
            lblDescripcion = new Label();
            txtNombre = new TextBox();
            txtDescripcion = new TextBox();
            chkActivo = new CheckBox();
            btnNuevo = new Button();
            btnGuardar = new Button();
            btnEliminar = new Button();
            dgvDepartamentos = new DataGridView();
            ((System.ComponentModel.ISupportInitialize)dgvDepartamentos).BeginInit();
            SuspendLayout();
            // 
            // lblNombre
            // 
            lblNombre.AutoSize = true;
            lblNombre.Location = new Point(26, 120);
            lblNombre.Name = "lblNombre";
            lblNombre.Size = new Size(51, 15);
            lblNombre.TabIndex = 0;
            lblNombre.Text = "Nombre";
            // 
            // lblDescripcion
            // 
            lblDescripcion.AutoSize = true;
            lblDescripcion.Location = new Point(26, 158);
            lblDescripcion.Name = "lblDescripcion";
            lblDescripcion.Size = new Size(69, 15);
            lblDescripcion.TabIndex = 1;
            lblDescripcion.Text = "Descripción";
            // 
            // txtNombre
            // 
            txtNombre.Location = new Point(112, 120);
            txtNombre.Name = "txtNombre";
            txtNombre.Size = new Size(200, 23);
            txtNombre.TabIndex = 2;
            // 
            // txtDescripcion
            // 
            txtDescripcion.Location = new Point(112, 158);
            txtDescripcion.Name = "txtDescripcion";
            txtDescripcion.Size = new Size(200, 23);
            txtDescripcion.TabIndex = 3;
            // 
            // chkActivo
            // 
            chkActivo.AllowDrop = true;
            chkActivo.AutoSize = true;
            chkActivo.Location = new Point(26, 199);
            chkActivo.Name = "chkActivo";
            chkActivo.Size = new Size(60, 19);
            chkActivo.TabIndex = 4;
            chkActivo.Text = "Activo";
            chkActivo.UseVisualStyleBackColor = true;
            // 
            // btnNuevo
            // 
            btnNuevo.Location = new Point(26, 265);
            btnNuevo.Name = "btnNuevo";
            btnNuevo.Size = new Size(103, 23);
            btnNuevo.TabIndex = 5;
            btnNuevo.Text = "Nuevo/Limpiar";
            btnNuevo.UseVisualStyleBackColor = true;
            // 
            // btnGuardar
            // 
            btnGuardar.Location = new Point(135, 265);
            btnGuardar.Name = "btnGuardar";
            btnGuardar.Size = new Size(82, 23);
            btnGuardar.TabIndex = 6;
            btnGuardar.Text = "Guardar";
            btnGuardar.UseVisualStyleBackColor = true;
            // 
            // btnEliminar
            // 
            btnEliminar.Location = new Point(223, 265);
            btnEliminar.Name = "btnEliminar";
            btnEliminar.Size = new Size(89, 23);
            btnEliminar.TabIndex = 7;
            btnEliminar.Text = "Eliminar";
            btnEliminar.UseVisualStyleBackColor = true;
            // 
            // dgvDepartamentos
            // 
            dgvDepartamentos.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvDepartamentos.Location = new Point(339, 34);
            dgvDepartamentos.Name = "dgvDepartamentos";
            dgvDepartamentos.Size = new Size(751, 445);
            dgvDepartamentos.TabIndex = 8;
            // 
            // FormDepartamentos
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1140, 517);
            Controls.Add(dgvDepartamentos);
            Controls.Add(btnEliminar);
            Controls.Add(btnGuardar);
            Controls.Add(btnNuevo);
            Controls.Add(chkActivo);
            Controls.Add(txtDescripcion);
            Controls.Add(txtNombre);
            Controls.Add(lblDescripcion);
            Controls.Add(lblNombre);
            Name = "FormDepartamentos";
            Text = "Gestión de Departamentos";
            ((System.ComponentModel.ISupportInitialize)dgvDepartamentos).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblNombre;
        private Label lblDescripcion;
        private TextBox txtNombre;
        private TextBox txtDescripcion;
        private CheckBox chkActivo;
        private Button btnNuevo;
        private Button btnGuardar;
        private Button btnEliminar;
        private DataGridView dgvDepartamentos;
    }
}