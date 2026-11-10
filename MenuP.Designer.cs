namespace MODULO.CSharp
{
    partial class MenuP
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
            lblUsuario = new Label();
            btnEmpleados = new Button();
            btnCargos = new Button();
            btnDepartamentos = new Button();
            btnNomina = new Button();
            btnHistorial = new Button();
            btnCerrarSesion = new Button();
            SuspendLayout();
            // 
            // lblUsuario
            // 
            lblUsuario.AutoSize = true;
            lblUsuario.Location = new Point(66, 53);
            lblUsuario.Name = "lblUsuario";
            lblUsuario.Size = new Size(82, 15);
            lblUsuario.TabIndex = 0;
            lblUsuario.Text = "Usuario activo";
            // 
            // btnEmpleados
            // 
            btnEmpleados.Location = new Point(120, 115);
            btnEmpleados.Name = "btnEmpleados";
            btnEmpleados.Size = new Size(147, 75);
            btnEmpleados.TabIndex = 1;
            btnEmpleados.Text = "Empleados";
            btnEmpleados.UseVisualStyleBackColor = true;
            // 
            // btnCargos
            // 
            btnCargos.Location = new Point(273, 115);
            btnCargos.Name = "btnCargos";
            btnCargos.Size = new Size(157, 75);
            btnCargos.TabIndex = 2;
            btnCargos.Text = "Cargos";
            btnCargos.UseVisualStyleBackColor = true;
            // 
            // btnDepartamentos
            // 
            btnDepartamentos.Location = new Point(436, 115);
            btnDepartamentos.Name = "btnDepartamentos";
            btnDepartamentos.Size = new Size(143, 75);
            btnDepartamentos.TabIndex = 3;
            btnDepartamentos.Text = "Departamentos";
            btnDepartamentos.UseVisualStyleBackColor = true;
            // 
            // btnNomina
            // 
            btnNomina.Location = new Point(120, 196);
            btnNomina.Name = "btnNomina";
            btnNomina.Size = new Size(147, 75);
            btnNomina.TabIndex = 4;
            btnNomina.Text = "Crear nómina";
            btnNomina.UseVisualStyleBackColor = true;
            // 
            // btnHistorial
            // 
            btnHistorial.Location = new Point(273, 196);
            btnHistorial.Name = "btnHistorial";
            btnHistorial.Size = new Size(157, 75);
            btnHistorial.TabIndex = 5;
            btnHistorial.Text = "Historial de nóminas";
            btnHistorial.UseVisualStyleBackColor = true;
            // 
            // btnCerrarSesion
            // 
            btnCerrarSesion.Location = new Point(436, 196);
            btnCerrarSesion.Name = "btnCerrarSesion";
            btnCerrarSesion.Size = new Size(143, 75);
            btnCerrarSesion.TabIndex = 6;
            btnCerrarSesion.Text = "Cerrar sesión";
            btnCerrarSesion.UseVisualStyleBackColor = true;
            // 
            // MenuP
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(717, 462);
            Controls.Add(btnCerrarSesion);
            Controls.Add(btnHistorial);
            Controls.Add(btnNomina);
            Controls.Add(btnDepartamentos);
            Controls.Add(btnCargos);
            Controls.Add(btnEmpleados);
            Controls.Add(lblUsuario);
            Name = "MenuP";
            Text = "Menu Principal";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblUsuario;
        private Button btnEmpleados;
        private Button btnCargos;
        private Button btnDepartamentos;
        private Button btnNomina;
        private Button btnHistorial;
        private Button btnCerrarSesion;
    }
}