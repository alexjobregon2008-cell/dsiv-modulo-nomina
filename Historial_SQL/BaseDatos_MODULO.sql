/*
	Base de datos para el proyecto MODULO.
	Ejecutar este archivo en SQL Server Management Studio como usuario con permisos de creación.
*/

IF DB_ID(N'MODULO') IS NULL
BEGIN
	CREATE DATABASE MODULO;
END
GO

USE MODULO;
GO

SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
SET ANSI_PADDING ON;
SET ANSI_WARNINGS ON;
SET CONCAT_NULL_YIELDS_NULL ON;
SET ARITHABORT ON;
SET NUMERIC_ROUNDABORT OFF;
GO

IF OBJECT_ID(N'dbo.Auditoria', N'U') IS NULL
BEGIN
	CREATE TABLE dbo.Auditoria
	(
		AuditoriaID int IDENTITY(1,1) NOT NULL CONSTRAINT PK_Auditoria PRIMARY KEY,
		TablaAfectada sysname NOT NULL,
		Operacion char(1) NOT NULL CONSTRAINT CK_Auditoria_Operacion CHECK (Operacion IN ('I', 'U', 'D')),
		RegistroID int NULL,
		Fecha datetime2(0) NOT NULL CONSTRAINT DF_Auditoria_Fecha DEFAULT SYSDATETIME(),
		UsuarioSistema sysname NOT NULL CONSTRAINT DF_Auditoria_Usuario DEFAULT SUSER_SNAME(),
		Detalle nvarchar(4000) NULL
	);
END
GO

IF OBJECT_ID(N'dbo.Usuarios', N'U') IS NULL
BEGIN
	CREATE TABLE dbo.Usuarios
	(
		UsuarioID int IDENTITY(1,1) NOT NULL CONSTRAINT PK_Usuarios PRIMARY KEY,
		NombreUsuario nvarchar(50) NOT NULL,
		Contrasena nvarchar(255) NOT NULL,
		Correo nvarchar(150) NOT NULL,
		Tipo nvarchar(20) NOT NULL,
		Activo bit NOT NULL CONSTRAINT DF_Usuarios_Activo DEFAULT 1,
		EsFijo bit NOT NULL CONSTRAINT DF_Usuarios_EsFijo DEFAULT 0,
		IntentosFallidos int NOT NULL CONSTRAINT DF_Usuarios_Intentos DEFAULT 0,
		UltimoIntentoFallido datetime2(0) NULL,
		FechaCreacion datetime2(0) NOT NULL CONSTRAINT DF_Usuarios_Fecha DEFAULT SYSDATETIME(),
		CONSTRAINT CK_Usuarios_Tipo CHECK (Tipo IN (N'Administrador', N'Usuario')),
		CONSTRAINT CK_Usuarios_Intentos CHECK (IntentosFallidos >= 0),
		CONSTRAINT UQ_Usuarios_Nombre UNIQUE (NombreUsuario),
		CONSTRAINT UQ_Usuarios_Correo UNIQUE (Correo)
	);
END
GO

IF OBJECT_ID(N'dbo.Empleados', N'U') IS NULL
BEGIN
	CREATE TABLE dbo.Empleados
	(
		EmpleadoID int IDENTITY(1,1) NOT NULL CONSTRAINT PK_Empleados PRIMARY KEY,
		Nombre nvarchar(150) NOT NULL,
		Cargo nvarchar(100) NOT NULL,
		SalarioBase decimal(18,2) NOT NULL,
		TipoContrato nvarchar(40) NOT NULL,
		AplicaDescuentos bit NOT NULL CONSTRAINT DF_Empleados_Descuentos DEFAULT 0,
		FechaInicioContrato date NOT NULL,
		Activo bit NOT NULL CONSTRAINT DF_Empleados_Activo DEFAULT 1,
		FechaCreacion datetime2(0) NOT NULL CONSTRAINT DF_Empleados_Fecha DEFAULT SYSDATETIME(),
		CONSTRAINT CK_Empleados_Salario CHECK (SalarioBase > 0),
		CONSTRAINT CK_Empleados_TipoContrato CHECK (TipoContrato IN (N'Permanente', N'ServiciosProfesionales')),
		CONSTRAINT UQ_Empleados_NombreCargo UNIQUE (Nombre, Cargo)
	);
END
GO

IF OBJECT_ID(N'dbo.Nominas', N'U') IS NULL
BEGIN
	CREATE TABLE dbo.Nominas
	(
		NominaID int IDENTITY(1,1) NOT NULL CONSTRAINT PK_Nominas PRIMARY KEY,
		EmpleadoID int NULL,
		FechaPago date NOT NULL,
		PeriodoPagado nvarchar(50) NOT NULL,
		Estado nvarchar(20) NOT NULL CONSTRAINT DF_Nominas_Estado DEFAULT N'Borrador',
		TotalBruto decimal(18,2) NOT NULL CONSTRAINT DF_Nominas_Bruto DEFAULT 0,
		TotalDescuentos decimal(18,2) NOT NULL CONSTRAINT DF_Nominas_Descuentos DEFAULT 0,
		TotalNeto decimal(18,2) NOT NULL CONSTRAINT DF_Nominas_Neto DEFAULT 0,
		FechaCreacion datetime2(0) NOT NULL CONSTRAINT DF_Nominas_Fecha DEFAULT SYSDATETIME(),
		CONSTRAINT CK_Nominas_Estado CHECK (Estado IN (N'Borrador', N'Emitida', N'Anulada')),
		CONSTRAINT FK_Nominas_Empleado FOREIGN KEY (EmpleadoID) REFERENCES dbo.Empleados(EmpleadoID)
	);
END
GO

IF OBJECT_ID(N'dbo.NominaDetalle', N'U') IS NULL
BEGIN
	CREATE TABLE dbo.NominaDetalle
	(
		NominaDetalleID int IDENTITY(1,1) NOT NULL CONSTRAINT PK_NominaDetalle PRIMARY KEY,
		NominaID int NOT NULL,
		EmpleadoID int NOT NULL,
		SalarioBruto decimal(18,2) NOT NULL,
		Descuentos decimal(18,2) NOT NULL CONSTRAINT DF_NominaDetalle_Descuentos DEFAULT 0,
		SalarioNeto AS (SalarioBruto - Descuentos) PERSISTED,
		CONSTRAINT CK_NominaDetalle_Bruto CHECK (SalarioBruto >= 0),
		CONSTRAINT CK_NominaDetalle_Descuentos CHECK (Descuentos >= 0 AND Descuentos <= SalarioBruto),
		CONSTRAINT UQ_NominaDetalle_NominaEmpleado UNIQUE (NominaID, EmpleadoID),
		CONSTRAINT FK_NominaDetalle_Nomina FOREIGN KEY (NominaID) REFERENCES dbo.Nominas(NominaID),
		CONSTRAINT FK_NominaDetalle_Empleado FOREIGN KEY (EmpleadoID) REFERENCES dbo.Empleados(EmpleadoID)
	);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Nominas_PeriodoPagado' AND object_id = OBJECT_ID(N'dbo.Nominas'))
	CREATE INDEX IX_Nominas_PeriodoPagado ON dbo.Nominas(PeriodoPagado);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_NominaDetalle_Empleado' AND object_id = OBJECT_ID(N'dbo.NominaDetalle'))
	CREATE INDEX IX_NominaDetalle_Empleado ON dbo.NominaDetalle(EmpleadoID);
GO

CREATE OR ALTER TRIGGER dbo.TR_Usuarios_Auditoria
ON dbo.Usuarios
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
	SET NOCOUNT ON;
	INSERT INTO dbo.Auditoria(TablaAfectada, Operacion, RegistroID, Detalle)
	SELECT N'Usuarios', CASE WHEN d.UsuarioID IS NULL THEN 'I' WHEN i.UsuarioID IS NULL THEN 'D' ELSE 'U' END,
		   COALESCE(i.UsuarioID, d.UsuarioID), N'Operación sobre usuario'
	FROM inserted i
	FULL OUTER JOIN deleted d ON d.UsuarioID = i.UsuarioID;
END
GO

CREATE OR ALTER TRIGGER dbo.TR_Empleados_ValidarYAudiar
ON dbo.Empleados
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
	SET NOCOUNT ON;

	IF EXISTS
	(
		SELECT 1
		FROM deleted d
		INNER JOIN dbo.NominaDetalle nd ON nd.EmpleadoID = d.EmpleadoID
		INNER JOIN dbo.Nominas n ON n.NominaID = nd.NominaID
		WHERE n.Estado = N'Emitida'
		  AND NOT EXISTS (SELECT 1 FROM inserted i WHERE i.EmpleadoID = d.EmpleadoID)
	)
	BEGIN
		ROLLBACK TRANSACTION;
		THROW 51001, 'No se puede eliminar o modificar un empleado con una nómina emitida.', 1;
	END;

	INSERT INTO dbo.Auditoria(TablaAfectada, Operacion, RegistroID, Detalle)
	SELECT N'Empleados', CASE WHEN d.EmpleadoID IS NULL THEN 'I' WHEN i.EmpleadoID IS NULL THEN 'D' ELSE 'U' END,
		   COALESCE(i.EmpleadoID, d.EmpleadoID), N'Operación sobre empleado'
	FROM inserted i
	FULL OUTER JOIN deleted d ON d.EmpleadoID = i.EmpleadoID;
END
GO

CREATE OR ALTER TRIGGER dbo.TR_NominaDetalle_ActualizarTotales
ON dbo.NominaDetalle
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
	SET NOCOUNT ON;
	;WITH NominasAfectadas AS
	(
		SELECT NominaID FROM inserted
		UNION
		SELECT NominaID FROM deleted
	)
	UPDATE n
	   SET TotalBruto = ISNULL(x.TotalBruto, 0),
		   TotalDescuentos = ISNULL(x.TotalDescuentos, 0),
		   TotalNeto = ISNULL(x.TotalBruto - x.TotalDescuentos, 0)
	FROM dbo.Nominas n
	INNER JOIN NominasAfectadas a ON a.NominaID = n.NominaID
	OUTER APPLY
	(
		SELECT SUM(SalarioBruto) AS TotalBruto, SUM(Descuentos) AS TotalDescuentos
		FROM dbo.NominaDetalle nd
		WHERE nd.NominaID = n.NominaID
	) x;
END
GO

CREATE OR ALTER TRIGGER dbo.TR_Nominas_ProtegerEmitida
ON dbo.Nominas
AFTER UPDATE, DELETE
AS
BEGIN
	SET NOCOUNT ON;
	IF EXISTS
	(
		SELECT 1
		FROM deleted d
		LEFT JOIN inserted i ON i.NominaID = d.NominaID
		WHERE d.Estado = N'Emitida'
		  AND (i.NominaID IS NULL OR ISNULL(i.Estado, N'') <> d.Estado)
	)
	BEGIN
		ROLLBACK TRANSACTION;
		THROW 51002, 'Una nómina emitida no puede eliminarse ni cambiar de estado.', 1;
	END;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_VerificarUsuario
	@NombreUsuario nvarchar(50),
	@Contrasena nvarchar(255),
	@Tipo nvarchar(20) OUTPUT,
	@Resultado int OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	SET @Tipo = NULL;

	IF NULLIF(LTRIM(RTRIM(@NombreUsuario)), N'') IS NULL OR NULLIF(@Contrasena, N'') IS NULL
	BEGIN
		SET @Resultado = 4;
		RETURN;
	END;

	DECLARE @UsuarioID int, @Activo bit, @ContrasenaBD nvarchar(255);
	SELECT @UsuarioID = UsuarioID, @Activo = Activo, @ContrasenaBD = Contrasena, @Tipo = Tipo
	FROM dbo.Usuarios WHERE NombreUsuario = @NombreUsuario;

	IF @UsuarioID IS NULL
	BEGIN
		SET @Resultado = 2;
		SET @Tipo = NULL;
		RETURN;
	END;

	IF @Activo = 0
	BEGIN
		SET @Resultado = 0;
		RETURN;
	END;

	IF @ContrasenaBD <> @Contrasena
	BEGIN
		UPDATE dbo.Usuarios SET IntentosFallidos = IntentosFallidos + 1, UltimoIntentoFallido = SYSDATETIME()
		WHERE UsuarioID = @UsuarioID;
		SET @Resultado = 3;
		RETURN;
	END;

	UPDATE dbo.Usuarios SET IntentosFallidos = 0, UltimoIntentoFallido = NULL WHERE UsuarioID = @UsuarioID;
	SET @Resultado = 1;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_EditarUsuario
	@UsuarioID int,
	@NombreUsuario nvarchar(50),
	@Correo nvarchar(150),
	@Tipo nvarchar(20),
	@Activo bit,
	@EsFijo bit,
	@Contrasena nvarchar(255) = NULL
AS
BEGIN
	SET NOCOUNT ON;
	UPDATE dbo.Usuarios
	   SET NombreUsuario = @NombreUsuario,
		   Correo = @Correo,
		   Tipo = @Tipo,
		   Activo = @Activo,
		   EsFijo = @EsFijo,
		   Contrasena = COALESCE(NULLIF(@Contrasena, N''), Contrasena)
	 WHERE UsuarioID = @UsuarioID;
END
GO

CREATE OR ALTER PROCEDURE dbo.CrearNomina
	@FechaPago date,
	@PeriodoPagado nvarchar(50)
AS
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;
	BEGIN TRANSACTION;

	INSERT INTO dbo.Nominas(FechaPago, PeriodoPagado, Estado)
	VALUES (@FechaPago, @PeriodoPagado, N'Emitida');

	DECLARE @NominaID int = SCOPE_IDENTITY();
	INSERT INTO dbo.NominaDetalle(NominaID, EmpleadoID, SalarioBruto, Descuentos)
	SELECT @NominaID, EmpleadoID, SalarioBase, 0
	FROM dbo.Empleados
	WHERE Activo = 1;

	UPDATE nd
	   SET Descuentos = CASE WHEN e.AplicaDescuentos = 1 THEN ROUND(nd.SalarioBruto * 0.095, 2) ELSE 0 END
	FROM dbo.NominaDetalle nd
	INNER JOIN dbo.Empleados e ON e.EmpleadoID = nd.EmpleadoID
	WHERE nd.NominaID = @NominaID;

	COMMIT TRANSACTION;
	SELECT @NominaID AS NominaID;
END
GO

CREATE OR ALTER PROCEDURE dbo.CalcularDescuentos
AS
BEGIN
	SET NOCOUNT ON;
	UPDATE nd
	   SET Descuentos = CASE WHEN e.AplicaDescuentos = 1 THEN ROUND(nd.SalarioBruto * 0.095, 2) ELSE 0 END
	FROM dbo.NominaDetalle nd
	INNER JOIN dbo.Empleados e ON e.EmpleadoID = nd.EmpleadoID
	INNER JOIN dbo.Nominas n ON n.NominaID = nd.NominaID
	WHERE n.Estado <> N'Anulada';
END
GO

CREATE OR ALTER PROCEDURE dbo.ReporteHistoricoNominas
AS
BEGIN
	SET NOCOUNT ON;
	SELECT n.NominaID, n.FechaPago, n.PeriodoPagado, n.Estado,
		   n.TotalBruto, n.TotalDescuentos, n.TotalNeto
	FROM dbo.Nominas n
	ORDER BY n.FechaPago DESC, n.NominaID DESC;
END
GO

CREATE OR ALTER PROCEDURE dbo.ReporteDetalladoNomina
	@NominaID int
AS
BEGIN
	SET NOCOUNT ON;
	SELECT n.NominaID, n.PeriodoPagado, n.FechaPago,
		   e.EmpleadoID, e.Nombre, e.Cargo, e.TipoContrato,
		   nd.SalarioBruto, nd.Descuentos, nd.SalarioNeto
	FROM dbo.Nominas n
	INNER JOIN dbo.NominaDetalle nd ON nd.NominaID = n.NominaID
	INNER JOIN dbo.Empleados e ON e.EmpleadoID = nd.EmpleadoID
	WHERE n.NominaID = @NominaID
	ORDER BY e.Nombre;
END
GO

CREATE OR ALTER PROCEDURE dbo.ResumenGeneralNomina
	@NominaID int
AS
BEGIN
	SET NOCOUNT ON;
	SELECT e.Cargo AS Departamento,
		   COUNT(*) AS TotalEmpleados,
		   SUM(nd.SalarioNeto) AS CostoTotal
	FROM dbo.NominaDetalle nd
	INNER JOIN dbo.Empleados e ON e.EmpleadoID = nd.EmpleadoID
	WHERE nd.NominaID = @NominaID
	GROUP BY e.Cargo
	ORDER BY e.Cargo;
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Usuarios WHERE NombreUsuario = N'admin')
BEGIN
	INSERT INTO dbo.Usuarios(NombreUsuario, Contrasena, Correo, Tipo, Activo, EsFijo)
	VALUES (N'admin', N'admin123', N'admin@local.test', N'Administrador', 1, 1);
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Empleados)
BEGIN
	INSERT INTO dbo.Empleados(Nombre, Cargo, SalarioBase, TipoContrato, AplicaDescuentos, FechaInicioContrato, Activo)
	VALUES (N'Administrador, Usuario', N'Administrador', 1000.00, N'Permanente', 0, CAST(GETDATE() AS date), 1);
END
GO
