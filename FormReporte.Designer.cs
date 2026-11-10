namespace MODULO.CSharp
{
    partial class FormReporte
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
            lblTitulo = new Label();
            lblPeriodoNomina = new Label();
            cmbTipoReporte = new ComboBox();
            cmbPeriodoNomina = new ComboBox();
            btnExportar = new Button();
            btnSalir = new Button();
            dgvReporte = new DataGridView();
            ((System.ComponentModel.ISupportInitialize)dgvReporte).BeginInit();
            SuspendLayout();
            // 
            // lblTitulo
            // 
            lblTitulo.AutoSize = true;
            lblTitulo.Location = new Point(41, 109);
            lblTitulo.Name = "lblTitulo";
            lblTitulo.Size = new Size(91, 15);
            lblTitulo.TabIndex = 0;
            lblTitulo.Text = "Tipo de Reporte";
            // 
            // lblPeriodoNomina
            // 
            lblPeriodoNomina.AutoSize = true;
            lblPeriodoNomina.BackColor = SystemColors.Control;
            lblPeriodoNomina.Location = new Point(41, 170);
            lblPeriodoNomina.Name = "lblPeriodoNomina";
            lblPeriodoNomina.Size = new Size(96, 15);
            lblPeriodoNomina.TabIndex = 1;
            lblPeriodoNomina.Text = "Nómina/Periodo";
            // 
            // cmbTipoReporte
            // 
            cmbTipoReporte.FormattingEnabled = true;
            cmbTipoReporte.Location = new Point(41, 136);
            cmbTipoReporte.Name = "cmbTipoReporte";
            cmbTipoReporte.Size = new Size(200, 23);
            cmbTipoReporte.TabIndex = 2;
            // 
            // cmbPeriodoNomina
            // 
            cmbPeriodoNomina.FormattingEnabled = true;
            cmbPeriodoNomina.Location = new Point(41, 197);
            cmbPeriodoNomina.Name = "cmbPeriodoNomina";
            cmbPeriodoNomina.Size = new Size(200, 23);
            cmbPeriodoNomina.TabIndex = 3;
            // 
            // btnExportar
            // 
            btnExportar.Location = new Point(41, 264);
            btnExportar.Name = "btnExportar";
            btnExportar.Size = new Size(200, 50);
            btnExportar.TabIndex = 4;
            btnExportar.Text = "Exportar CSV";
            btnExportar.UseVisualStyleBackColor = true;
            // 
            // btnSalir
            // 
            btnSalir.Location = new Point(41, 339);
            btnSalir.Name = "btnSalir";
            btnSalir.Size = new Size(200, 50);
            btnSalir.TabIndex = 5;
            btnSalir.Text = "Cerrar";
            btnSalir.UseVisualStyleBackColor = true;
            // 
            // dgvReporte
            // 
            dgvReporte.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvReporte.Location = new Point(306, 29);
            dgvReporte.Name = "dgvReporte";
            dgvReporte.Size = new Size(798, 527);
            dgvReporte.TabIndex = 6;
            // 
            // FormReporte
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1159, 594);
            Controls.Add(dgvReporte);
            Controls.Add(btnSalir);
            Controls.Add(btnExportar);
            Controls.Add(cmbPeriodoNomina);
            Controls.Add(cmbTipoReporte);
            Controls.Add(lblPeriodoNomina);
            Controls.Add(lblTitulo);
            Name = "FormReporte";
            Text = "Reporte de Nominas";
            ((System.ComponentModel.ISupportInitialize)dgvReporte).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblTitulo;
        private Label lblPeriodoNomina;
        private ComboBox cmbTipoReporte;
        private ComboBox cmbPeriodoNomina;
        private Button btnExportar;
        private Button btnSalir;
        private DataGridView dgvReporte;
    }
}