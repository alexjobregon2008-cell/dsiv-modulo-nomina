namespace MODULO.CSharp
{
    partial class FormNomina
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
            dtpFechaPago = new DateTimePicker();
            dtpPeriodoDesde = new DateTimePicker();
            dtpPeriodoHasta = new DateTimePicker();
            txtPeriodoPagado = new TextBox();
            btnCrearNomina = new Button();
            btnHistorial = new Button();
            dgvHistorial = new DataGridView();
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            ((System.ComponentModel.ISupportInitialize)dgvHistorial).BeginInit();
            SuspendLayout();
            // 
            // dtpFechaPago
            // 
            dtpFechaPago.Location = new Point(85, 107);
            dtpFechaPago.Name = "dtpFechaPago";
            dtpFechaPago.Size = new Size(200, 23);
            dtpFechaPago.TabIndex = 0;
            // 
            // dtpPeriodoDesde
            // 
            dtpPeriodoDesde.Location = new Point(85, 169);
            dtpPeriodoDesde.Name = "dtpPeriodoDesde";
            dtpPeriodoDesde.Size = new Size(200, 23);
            dtpPeriodoDesde.TabIndex = 1;
            // 
            // dtpPeriodoHasta
            // 
            dtpPeriodoHasta.Location = new Point(85, 232);
            dtpPeriodoHasta.Name = "dtpPeriodoHasta";
            dtpPeriodoHasta.Size = new Size(200, 23);
            dtpPeriodoHasta.TabIndex = 2;
            // 
            // txtPeriodoPagado
            // 
            txtPeriodoPagado.Location = new Point(85, 287);
            txtPeriodoPagado.Name = "txtPeriodoPagado";
            txtPeriodoPagado.Size = new Size(199, 23);
            txtPeriodoPagado.TabIndex = 3;
            // 
            // btnCrearNomina
            // 
            btnCrearNomina.Location = new Point(100, 409);
            btnCrearNomina.Name = "btnCrearNomina";
            btnCrearNomina.Size = new Size(154, 47);
            btnCrearNomina.TabIndex = 4;
            btnCrearNomina.Text = "Crear nómina";
            btnCrearNomina.UseVisualStyleBackColor = true;
            // 
            // btnHistorial
            // 
            btnHistorial.Location = new Point(100, 350);
            btnHistorial.Name = "btnHistorial";
            btnHistorial.Size = new Size(154, 53);
            btnHistorial.TabIndex = 5;
            btnHistorial.Text = "Ver historial";
            btnHistorial.UseVisualStyleBackColor = true;
            // 
            // dgvHistorial
            // 
            dgvHistorial.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvHistorial.Location = new Point(320, 40);
            dgvHistorial.Name = "dgvHistorial";
            dgvHistorial.Size = new Size(797, 530);
            dgvHistorial.TabIndex = 6;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(85, 77);
            label1.Name = "label1";
            label1.Size = new Size(87, 15);
            label1.TabIndex = 7;
            label1.Text = "Fecha de Pago:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(85, 142);
            label2.Name = "label2";
            label2.Size = new Size(82, 15);
            label2.TabIndex = 8;
            label2.Text = "Periodo desde";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(85, 205);
            label3.Name = "label3";
            label3.Size = new Size(81, 15);
            label3.TabIndex = 9;
            label3.Text = "Periodo Hasta";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(81, 269);
            label4.Name = "label4";
            label4.Size = new Size(91, 15);
            label4.TabIndex = 10;
            label4.Text = "Periodo Pagado";
            // 
            // FormNomina
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1158, 592);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(dgvHistorial);
            Controls.Add(btnHistorial);
            Controls.Add(btnCrearNomina);
            Controls.Add(txtPeriodoPagado);
            Controls.Add(dtpPeriodoHasta);
            Controls.Add(dtpPeriodoDesde);
            Controls.Add(dtpFechaPago);
            Name = "FormNomina";
            Text = "Gestión de Nomina";
            ((System.ComponentModel.ISupportInitialize)dgvHistorial).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DateTimePicker dtpFechaPago;
        private DateTimePicker dtpPeriodoDesde;
        private DateTimePicker dtpPeriodoHasta;
        private TextBox txtPeriodoPagado;
        private Button btnCrearNomina;
        private Button btnHistorial;
        private DataGridView dgvHistorial;
        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
    }
}