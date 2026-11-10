/*
  MODULO - esquema final de nómina para Panamá.
  Script para una instalación limpia en SQL Server / LocalDB.
  Las tasas legales se almacenan en ReglasNominaPanama y deben validarse
  contra la normativa vigente antes de usar el sistema en producción.
*/

IF DB_ID(N'MODULO_Final') IS NULL CREATE DATABASE MODULO_Final;
GO
USE MODULO_Final;
GO
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
SET ANSI_PADDING ON;
SET ANSI_WARNINGS ON;
SET CONCAT_NULL_YIELDS_NULL ON;
SET ARITHABORT ON;
SET NUMERIC_ROUNDABORT OFF;
GO

/* Ejecutar en una base vacía. Para reinstalar, eliminar MODULO_Final previamente. */

CREATE TABLE dbo.Usuarios
(
	UsuarioID int IDENTITY PRIMARY KEY,
	NombreUsuario nvarchar(50) NOT NULL UNIQUE,
	ContrasenaHash nvarchar(255) NOT NULL,
	Correo nvarchar(150) NOT NULL UNIQUE,
	Rol nvarchar(20) NOT NULL CHECK (Rol IN (N'Administrador', N'Usuario')),
	Activo bit NOT NULL CONSTRAINT DF_Usuarios_Activo DEFAULT 1,
	IntentosFallidos tinyint NOT NULL CONSTRAINT DF_Usuarios_Intentos DEFAULT 0 CHECK (IntentosFallidos BETWEEN 0 AND 3),
	UltimoIntentoFallido datetime2(0) NULL,
	DebeCambiarContrasena bit NOT NULL CONSTRAINT DF_Usuarios_DebeCambiar DEFAULT 0,
	FechaCreacion datetime2(0) NOT NULL CONSTRAINT DF_Usuarios_Fecha DEFAULT SYSDATETIME()
);
GO

CREATE TABLE dbo.Bitacora
(
	BitacoraID int IDENTITY PRIMARY KEY,
	UsuarioID int NULL,
	Accion nvarchar(30) NOT NULL,
	Descripcion nvarchar(500) NOT NULL,
	TablaAfectada sysname NULL,
	RegistroID int NULL,
	FechaHora datetime2(0) NOT NULL CONSTRAINT DF_Bitacora_Fecha DEFAULT SYSDATETIME(),
	CONSTRAINT FK_Bitacora_Usuario FOREIGN KEY (UsuarioID) REFERENCES dbo.Usuarios(UsuarioID)
);
GO

CREATE TABLE dbo.Departamentos
(
	DepartamentoID int IDENTITY PRIMARY KEY,
	Nombre nvarchar(100) NOT NULL UNIQUE,
	Descripcion nvarchar(300) NULL,
	Activo bit NOT NULL CONSTRAINT DF_Departamentos_Activo DEFAULT 1
);
GO

CREATE TABLE dbo.Cargos
(
	CargoID int IDENTITY PRIMARY KEY,
	DepartamentoID int NOT NULL,
	Nombre nvarchar(100) NOT NULL,
	Descripcion nvarchar(300) NULL,
	SalarioMinimo decimal(18,2) NULL CHECK (SalarioMinimo IS NULL OR SalarioMinimo >= 0),
	SalarioMaximo decimal(18,2) NULL,
	Activo bit NOT NULL CONSTRAINT DF_Cargos_Activo DEFAULT 1,
	CONSTRAINT UQ_Cargos_Departamento_Nombre UNIQUE (DepartamentoID, Nombre),
	CONSTRAINT UQ_Cargos_Cargo_Departamento UNIQUE (CargoID, DepartamentoID),
	CONSTRAINT CK_Cargos_Rango CHECK (SalarioMaximo IS NULL OR SalarioMinimo IS NULL OR SalarioMaximo >= SalarioMinimo),
	CONSTRAINT FK_Cargos_Departamento FOREIGN KEY (DepartamentoID) REFERENCES dbo.Departamentos(DepartamentoID)
);
GO

CREATE TABLE dbo.Empleados
(
	EmpleadoID int IDENTITY PRIMARY KEY,
	Nombre nvarchar(150) NOT NULL,
	DepartamentoID int NOT NULL,
	CargoID int NOT NULL,
	SalarioBase decimal(18,2) NOT NULL CHECK (SalarioBase > 0),
	TipoContrato nvarchar(40) NOT NULL CHECK (TipoContrato IN (N'Permanente', N'ServiciosProfesionales')),
	AplicaDescuentos bit NOT NULL CONSTRAINT DF_Empleados_Descuentos DEFAULT 1,
	FechaInicioContrato date NOT NULL,
	Activo bit NOT NULL CONSTRAINT DF_Empleados_Activo DEFAULT 1,
	FechaCreacion datetime2(0) NOT NULL CONSTRAINT DF_Empleados_Fecha DEFAULT SYSDATETIME(),
	CONSTRAINT UQ_Empleados_Nombre UNIQUE (Nombre),
	CONSTRAINT FK_Empleados_Departamento FOREIGN KEY (DepartamentoID) REFERENCES dbo.Departamentos(DepartamentoID),
	CONSTRAINT FK_Empleados_Cargo_Departamento FOREIGN KEY (CargoID, DepartamentoID) REFERENCES dbo.Cargos(CargoID, DepartamentoID)
);
GO

CREATE TABLE dbo.Turnos
(
	TurnoID int IDENTITY PRIMARY KEY,
	Nombre nvarchar(80) NOT NULL UNIQUE,
	HoraInicio time(0) NOT NULL,
	HoraFin time(0) NOT NULL,
	TipoJornada nvarchar(20) NOT NULL CHECK (TipoJornada IN (N'Diurna', N'Nocturna', N'Mixta', N'Rotativa')),
	HorasProgramadas decimal(5,2) NOT NULL CHECK (HorasProgramadas > 0),
	Activo bit NOT NULL CONSTRAINT DF_Turnos_Activo DEFAULT 1
);
GO

CREATE TABLE dbo.EmpleadoTurno
(
	EmpleadoTurnoID int IDENTITY PRIMARY KEY,
	EmpleadoID int NOT NULL,
	TurnoID int NOT NULL,
	FechaDesde date NOT NULL,
	FechaHasta date NULL,
	Activo bit NOT NULL CONSTRAINT DF_EmpleadoTurno_Activo DEFAULT 1,
	CONSTRAINT CK_EmpleadoTurno_Fechas CHECK (FechaHasta IS NULL OR FechaHasta >= FechaDesde),
	CONSTRAINT FK_EmpleadoTurno_Empleado FOREIGN KEY (EmpleadoID) REFERENCES dbo.Empleados(EmpleadoID),
	CONSTRAINT FK_EmpleadoTurno_Turno FOREIGN KEY (TurnoID) REFERENCES dbo.Turnos(TurnoID)
);
GO

CREATE TABLE dbo.Feriados
(
	FeriadoID int IDENTITY PRIMARY KEY,
	Fecha date NOT NULL UNIQUE,
	Nombre nvarchar(150) NOT NULL,
	TipoFeriado nvarchar(30) NOT NULL,
	EsDiaNoLaborable bit NOT NULL CONSTRAINT DF_Feriados_NoLaborable DEFAULT 1,
	Activo bit NOT NULL CONSTRAINT DF_Feriados_Activo DEFAULT 1
);
GO

CREATE TABLE dbo.Asistencias
(
	AsistenciaID int IDENTITY PRIMARY KEY,
	EmpleadoID int NOT NULL,
	TurnoID int NULL,
	Fecha date NOT NULL,
	HoraEntrada time(0) NULL,
	HoraSalida time(0) NULL,
	TipoRegistro nvarchar(20) NOT NULL CHECK (TipoRegistro IN (N'Presente', N'Ausencia', N'Permiso', N'Incapacidad', N'Vacaciones', N'Feriado', N'Descanso')),
	Estado nvarchar(20) NOT NULL CHECK (Estado IN (N'Normal', N'Justificada', N'Injustificada', N'Pendiente')),
	HorasProgramadas decimal(5,2) NOT NULL CONSTRAINT DF_Asistencias_Programadas DEFAULT 0,
	HorasTrabajadas decimal(5,2) NOT NULL CONSTRAINT DF_Asistencias_Trabajadas DEFAULT 0,
	HorasNormales decimal(5,2) NOT NULL CONSTRAINT DF_Asistencias_Normales DEFAULT 0,
	EsDomingo bit NOT NULL CONSTRAINT DF_Asistencias_Domingo DEFAULT 0,
	EsFeriado bit NOT NULL CONSTRAINT DF_Asistencias_Feriado DEFAULT 0,
	FeriadoID int NULL,
	Justificada bit NOT NULL CONSTRAINT DF_Asistencias_Justificada DEFAULT 0,
	GeneraDescuento bit NOT NULL CONSTRAINT DF_Asistencias_Descuento DEFAULT 0,
	Observacion nvarchar(300) NULL,
	CONSTRAINT UQ_Asistencias_Empleado_Fecha UNIQUE (EmpleadoID, Fecha),
	CONSTRAINT CK_Asistencias_Horas CHECK (HorasProgramadas >= 0 AND HorasTrabajadas >= 0 AND HorasNormales >= 0),
	CONSTRAINT FK_Asistencias_Empleado FOREIGN KEY (EmpleadoID) REFERENCES dbo.Empleados(EmpleadoID),
	CONSTRAINT FK_Asistencias_Turno FOREIGN KEY (TurnoID) REFERENCES dbo.Turnos(TurnoID),
	CONSTRAINT FK_Asistencias_Feriado FOREIGN KEY (FeriadoID) REFERENCES dbo.Feriados(FeriadoID)
);
GO

CREATE TABLE dbo.AsistenciaHoras
(
	AsistenciaHoraID int IDENTITY PRIMARY KEY,
	AsistenciaID int NOT NULL,
	TipoHora nvarchar(30) NOT NULL CHECK (TipoHora IN (N'Normal', N'ExtraDiurna', N'ExtraNocturna', N'Domingo', N'Feriado', N'FeriadoNocturno', N'DescansoSemanal')),
	CantidadHoras decimal(5,2) NOT NULL CHECK (CantidadHoras > 0),
	HorasAprobadas bit NOT NULL CONSTRAINT DF_AsistenciaHoras_Aprobadas DEFAULT 0,
	AprobadoPorUsuarioID int NULL,
	FechaAprobacion datetime2(0) NULL,
	CONSTRAINT FK_AsistenciaHoras_Asistencia FOREIGN KEY (AsistenciaID) REFERENCES dbo.Asistencias(AsistenciaID),
	CONSTRAINT FK_AsistenciaHoras_Usuario FOREIGN KEY (AprobadoPorUsuarioID) REFERENCES dbo.Usuarios(UsuarioID)
);
GO

CREATE TABLE dbo.Nominas
(
	NominaID int IDENTITY PRIMARY KEY,
	FechaPago date NOT NULL,
	PeriodoDesde date NOT NULL,
	PeriodoHasta date NOT NULL,
	PeriodoPagado nvarchar(50) NOT NULL,
	Estado nvarchar(20) NOT NULL CONSTRAINT DF_Nominas_Estado DEFAULT N'Borrador' CHECK (Estado IN (N'Borrador', N'Emitida', N'Anulada')),
	TotalBruto decimal(18,2) NOT NULL CONSTRAINT DF_Nominas_Bruto DEFAULT 0,
	TotalDeducciones decimal(18,2) NOT NULL CONSTRAINT DF_Nominas_Deducciones DEFAULT 0,
	TotalAportesPatronales decimal(18,2) NOT NULL CONSTRAINT DF_Nominas_Aportes DEFAULT 0,
	TotalNeto decimal(18,2) NOT NULL CONSTRAINT DF_Nominas_Neto DEFAULT 0,
	FechaCreacion datetime2(0) NOT NULL CONSTRAINT DF_Nominas_Fecha DEFAULT SYSDATETIME(),
	CONSTRAINT CK_Nominas_Periodo CHECK (PeriodoHasta >= PeriodoDesde)
);
GO

CREATE TABLE dbo.NominaDetalle
(
	NominaDetalleID int IDENTITY PRIMARY KEY,
	NominaID int NOT NULL,
	EmpleadoID int NOT NULL,
	SalarioBase decimal(18,2) NOT NULL CHECK (SalarioBase >= 0),
	TotalDevengado decimal(18,2) NOT NULL CONSTRAINT DF_NominaDetalle_Devengado DEFAULT 0,
	TotalDeducciones decimal(18,2) NOT NULL CONSTRAINT DF_NominaDetalle_Deducciones DEFAULT 0,
	TotalAportesPatronales decimal(18,2) NOT NULL CONSTRAINT DF_NominaDetalle_Aportes DEFAULT 0,
	SalarioNeto decimal(18,2) NOT NULL CONSTRAINT DF_NominaDetalle_Neto DEFAULT 0,
	CONSTRAINT UQ_NominaDetalle_Nomina_Empleado UNIQUE (NominaID, EmpleadoID),
	CONSTRAINT FK_NominaDetalle_Nomina FOREIGN KEY (NominaID) REFERENCES dbo.Nominas(NominaID),
	CONSTRAINT FK_NominaDetalle_Empleado FOREIGN KEY (EmpleadoID) REFERENCES dbo.Empleados(EmpleadoID)
);
GO

CREATE TABLE dbo.ConceptosNomina
(
	ConceptoID int IDENTITY PRIMARY KEY,
	Codigo varchar(50) NOT NULL UNIQUE,
	Nombre nvarchar(150) NOT NULL,
	TipoConcepto nvarchar(30) NOT NULL CHECK (TipoConcepto IN (N'Deduccion', N'AportePatronal', N'Devengo', N'Impuesto')),
	Naturaleza nvarchar(30) NOT NULL CHECK (Naturaleza IN (N'Legal', N'Voluntario', N'Salario', N'Beneficio')),
	AplicaA nvarchar(20) NOT NULL CHECK (AplicaA IN (N'Trabajador', N'Empleador', N'Ambos')),
	Activo bit NOT NULL CONSTRAINT DF_Conceptos_Activo DEFAULT 1
);
GO

CREATE TABLE dbo.ReglasNominaPanama
(
	ReglaID int IDENTITY PRIMARY KEY,
	ConceptoID int NULL,
	CodigoRegla varchar(60) NOT NULL,
	Descripcion nvarchar(300) NOT NULL,
	TipoCalculo nvarchar(30) NOT NULL CHECK (TipoCalculo IN (N'Porcentaje', N'Tramo', N'Multiplicador', N'Divisor', N'Monto')),
	Porcentaje decimal(12,8) NULL,
	Multiplicador decimal(12,8) NULL,
	MontoDesde decimal(18,2) NULL,
	MontoHasta decimal(18,2) NULL,
	Valor decimal(18,8) NULL,
	Periodicidad nvarchar(20) NOT NULL CHECK (Periodicidad IN (N'Mensual', N'Anual', N'PorHora', N'PorDia', N'Evento')),
	VigenciaDesde date NOT NULL,
	VigenciaHasta date NULL,
	FuenteLegal nvarchar(300) NOT NULL,
	Activo bit NOT NULL CONSTRAINT DF_Reglas_Activo DEFAULT 1,
	CONSTRAINT UQ_Reglas_Codigo_Vigencia UNIQUE (CodigoRegla, VigenciaDesde),
	CONSTRAINT CK_Reglas_Vigencia CHECK (VigenciaHasta IS NULL OR VigenciaHasta >= VigenciaDesde),
	CONSTRAINT FK_Reglas_Concepto FOREIGN KEY (ConceptoID) REFERENCES dbo.ConceptosNomina(ConceptoID)
);
GO

CREATE TABLE dbo.NominaConcepto
(
	NominaConceptoID int IDENTITY PRIMARY KEY,
	NominaDetalleID int NOT NULL,
	ConceptoID int NOT NULL,
	AsistenciaID int NULL,
	AsistenciaHoraID int NULL,
	BaseCalculo decimal(18,2) NOT NULL CONSTRAINT DF_NominaConcepto_Base DEFAULT 0,
	Cantidad decimal(12,4) NOT NULL CONSTRAINT DF_NominaConcepto_Cantidad DEFAULT 1,
	PorcentajeAplicado decimal(12,8) NULL,
	Monto decimal(18,2) NOT NULL,
	EsPatronal bit NOT NULL CONSTRAINT DF_NominaConcepto_Patronal DEFAULT 0,
	Descripcion nvarchar(300) NULL,
	CONSTRAINT FK_NominaConcepto_Detalle FOREIGN KEY (NominaDetalleID) REFERENCES dbo.NominaDetalle(NominaDetalleID),
	CONSTRAINT FK_NominaConcepto_Concepto FOREIGN KEY (ConceptoID) REFERENCES dbo.ConceptosNomina(ConceptoID),
	CONSTRAINT FK_NominaConcepto_Asistencia FOREIGN KEY (AsistenciaID) REFERENCES dbo.Asistencias(AsistenciaID),
	CONSTRAINT FK_NominaConcepto_AsistenciaHora FOREIGN KEY (AsistenciaHoraID) REFERENCES dbo.AsistenciaHoras(AsistenciaHoraID)
);
GO

CREATE INDEX IX_Bitacora_Fecha ON dbo.Bitacora(FechaHora);
CREATE INDEX IX_Asistencias_Empleado_Fecha ON dbo.Asistencias(EmpleadoID, Fecha);
CREATE INDEX IX_Nominas_Periodo ON dbo.Nominas(PeriodoDesde, PeriodoHasta);
CREATE INDEX IX_NominaConcepto_Detalle ON dbo.NominaConcepto(NominaDetalleID);
GO

/* Auditoría y protección de nóminas emitidas. El UsuarioID se toma de SESSION_CONTEXT. */
CREATE OR ALTER TRIGGER dbo.TR_Usuarios_Bitacora ON dbo.Usuarios AFTER INSERT, UPDATE, DELETE AS
BEGIN
	SET NOCOUNT ON;
	INSERT dbo.Bitacora(UsuarioID, Accion, Descripcion, TablaAfectada, RegistroID)
	SELECT TRY_CONVERT(int, SESSION_CONTEXT(N'UsuarioID')),
		   CASE WHEN i.UsuarioID IS NULL THEN N'ELIMINAR' WHEN d.UsuarioID IS NULL THEN N'INSERTAR' ELSE N'ACTUALIZAR' END,
		   CASE WHEN i.UsuarioID IS NULL THEN N'Eliminó al usuario ' + CONVERT(nvarchar(20), d.UsuarioID)
				WHEN d.UsuarioID IS NULL THEN N'Creó al usuario ' + CONVERT(nvarchar(20), i.UsuarioID)
				ELSE N'Actualizó al usuario ' + CONVERT(nvarchar(20), i.UsuarioID) END,
		   N'Usuarios', COALESCE(i.UsuarioID, d.UsuarioID)
	FROM inserted i FULL JOIN deleted d ON d.UsuarioID = i.UsuarioID
	/* Un UPDATE solo se audita si cambió un dato relevante y no lo marcó como interno sp_RegistrarIntentoFallido. */
	WHERE i.UsuarioID IS NULL OR d.UsuarioID IS NULL
	   OR (ISNULL(TRY_CONVERT(int, SESSION_CONTEXT(N'OmitirAuditoriaUsuario')), 0) = 0
		   AND (i.NombreUsuario <> d.NombreUsuario OR i.ContrasenaHash <> d.ContrasenaHash
			 OR i.Correo <> d.Correo OR i.Rol <> d.Rol OR i.Activo <> d.Activo
			 OR i.DebeCambiarContrasena <> d.DebeCambiarContrasena));
END;
GO

CREATE OR ALTER TRIGGER dbo.TR_Empleados_Bitacora ON dbo.Empleados AFTER INSERT, UPDATE, DELETE AS
BEGIN
	SET NOCOUNT ON;
	IF EXISTS (SELECT 1 FROM deleted d INNER JOIN dbo.NominaDetalle nd ON nd.EmpleadoID=d.EmpleadoID INNER JOIN dbo.Nominas n ON n.NominaID=nd.NominaID WHERE n.Estado=N'Emitida' AND NOT EXISTS (SELECT 1 FROM inserted i WHERE i.EmpleadoID=d.EmpleadoID))
	BEGIN ROLLBACK; THROW 51001, 'No se puede eliminar un empleado con nómina emitida.', 1; END;
	INSERT dbo.Bitacora(UsuarioID, Accion, Descripcion, TablaAfectada, RegistroID)
	SELECT TRY_CONVERT(int, SESSION_CONTEXT(N'UsuarioID')),
		   CASE WHEN i.EmpleadoID IS NULL THEN N'ELIMINAR' WHEN d.EmpleadoID IS NULL THEN N'INSERTAR' ELSE N'ACTUALIZAR' END,
		   CASE WHEN i.EmpleadoID IS NULL THEN N'Eliminó al empleado ' + CONVERT(nvarchar(20), d.EmpleadoID)
				WHEN d.EmpleadoID IS NULL THEN N'Creó al empleado ' + CONVERT(nvarchar(20), i.EmpleadoID)
				ELSE N'Actualizó al empleado ' + CONVERT(nvarchar(20), i.EmpleadoID) END,
		   N'Empleados', COALESCE(i.EmpleadoID, d.EmpleadoID)
	FROM inserted i FULL JOIN deleted d ON d.EmpleadoID = i.EmpleadoID;
END;
GO

CREATE OR ALTER TRIGGER dbo.TR_Asistencias_Bitacora ON dbo.Asistencias AFTER INSERT, UPDATE, DELETE AS
BEGIN
	SET NOCOUNT ON;
	IF EXISTS (SELECT 1 FROM deleted d INNER JOIN dbo.NominaConcepto nc ON nc.AsistenciaID=d.AsistenciaID INNER JOIN dbo.NominaDetalle nd ON nd.NominaDetalleID=nc.NominaDetalleID INNER JOIN dbo.Nominas n ON n.NominaID=nd.NominaID WHERE n.Estado=N'Emitida')
	BEGIN ROLLBACK; THROW 51002, 'No se puede modificar una asistencia utilizada en nómina emitida.', 1; END;
	INSERT dbo.Bitacora(UsuarioID, Accion, Descripcion, TablaAfectada, RegistroID)
	SELECT TRY_CONVERT(int, SESSION_CONTEXT(N'UsuarioID')),
		   CASE WHEN i.AsistenciaID IS NULL THEN N'ELIMINAR' WHEN d.AsistenciaID IS NULL THEN N'INSERTAR' ELSE N'ACTUALIZAR' END,
		   CASE WHEN i.AsistenciaID IS NULL THEN N'Eliminó la asistencia ' + CONVERT(nvarchar(20), d.AsistenciaID)
				WHEN d.AsistenciaID IS NULL THEN N'Registró la asistencia ' + CONVERT(nvarchar(20), i.AsistenciaID)
				ELSE N'Actualizó la asistencia ' + CONVERT(nvarchar(20), i.AsistenciaID) END,
		   N'Asistencias', COALESCE(i.AsistenciaID, d.AsistenciaID)
	FROM inserted i FULL JOIN deleted d ON d.AsistenciaID = i.AsistenciaID;
END;
GO

CREATE OR ALTER TRIGGER dbo.TR_NominaConcepto_ActualizarTotales ON dbo.NominaConcepto AFTER INSERT, UPDATE, DELETE AS
BEGIN
	SET NOCOUNT ON;
	;WITH A AS (SELECT NominaDetalleID FROM inserted UNION SELECT NominaDetalleID FROM deleted)
	UPDATE d SET TotalDevengado=ISNULL(x.Devengado,0), TotalDeducciones=ISNULL(x.Deducciones,0), TotalAportesPatronales=ISNULL(x.Patronales,0), SalarioNeto=ISNULL(d.SalarioBase,0)+ISNULL(x.Devengado,0)-ISNULL(x.Deducciones,0)
	FROM dbo.NominaDetalle d INNER JOIN A ON A.NominaDetalleID=d.NominaDetalleID
	OUTER APPLY (SELECT SUM(CASE WHEN c.TipoConcepto='Devengo' THEN nc.Monto ELSE 0 END) Devengado, SUM(CASE WHEN c.TipoConcepto IN ('Deduccion','Impuesto') AND nc.EsPatronal=0 THEN nc.Monto ELSE 0 END) Deducciones, SUM(CASE WHEN nc.EsPatronal=1 THEN nc.Monto ELSE 0 END) Patronales FROM dbo.NominaConcepto nc INNER JOIN dbo.ConceptosNomina c ON c.ConceptoID=nc.ConceptoID WHERE nc.NominaDetalleID=d.NominaDetalleID) x;
	UPDATE n SET TotalBruto=ISNULL(x.Bruto,0), TotalDeducciones=ISNULL(x.Deducciones,0), TotalAportesPatronales=ISNULL(x.Patronales,0), TotalNeto=ISNULL(x.Bruto,0)-ISNULL(x.Deducciones,0)
	FROM dbo.Nominas n INNER JOIN (SELECT DISTINCT NominaID FROM dbo.NominaDetalle d INNER JOIN A ON A.NominaDetalleID=d.NominaDetalleID) a ON a.NominaID=n.NominaID
	OUTER APPLY (SELECT SUM(d.TotalDevengado+d.SalarioBase) Bruto, SUM(d.TotalDeducciones) Deducciones, SUM(d.TotalAportesPatronales) Patronales FROM dbo.NominaDetalle d WHERE d.NominaID=n.NominaID) x;
END;
GO

CREATE OR ALTER TRIGGER dbo.TR_Nominas_ProtegerEmitida ON dbo.Nominas AFTER UPDATE, DELETE AS
BEGIN
	SET NOCOUNT ON;
	IF EXISTS (SELECT 1 FROM deleted d LEFT JOIN inserted i ON i.NominaID=d.NominaID WHERE d.Estado=N'Emitida' AND (i.NominaID IS NULL OR i.Estado<>N'Emitida'))
	BEGIN ROLLBACK; THROW 51003, 'Una nómina emitida no puede eliminarse ni cambiar de estado.', 1; END;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_LoginUsuario
	@NombreUsuario nvarchar(50)
AS
BEGIN
	SET NOCOUNT ON;
	SELECT UsuarioID, NombreUsuario, ContrasenaHash, Rol, Activo, IntentosFallidos, DebeCambiarContrasena
	FROM dbo.Usuarios
	WHERE NombreUsuario=@NombreUsuario;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_RegistrarBitacora @UsuarioID int,@Accion nvarchar(30),@Descripcion nvarchar(500),@TablaAfectada sysname=NULL,@RegistroID int=NULL AS
BEGIN SET NOCOUNT ON; INSERT dbo.Bitacora(UsuarioID,Accion,Descripcion,TablaAfectada,RegistroID) VALUES(@UsuarioID,@Accion,@Descripcion,@TablaAfectada,@RegistroID); END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_CrearUsuario
	@AdministradorID int,
	@NombreUsuario nvarchar(50),
	@ContrasenaHash nvarchar(255),
	@Correo nvarchar(150),
	@Rol nvarchar(20) = N'Usuario'
AS
BEGIN
	SET NOCOUNT ON;
	IF NOT EXISTS (SELECT 1 FROM dbo.Usuarios WHERE UsuarioID=@AdministradorID AND Rol=N'Administrador' AND Activo=1)
		THROW 51006, 'Solo un administrador activo puede crear usuarios.', 1;
	IF @Rol NOT IN (N'Administrador', N'Usuario')
		THROW 51007, 'El rol indicado no es válido.', 1;
	IF EXISTS (SELECT 1 FROM dbo.Usuarios WHERE NombreUsuario=@NombreUsuario OR Correo=@Correo)
		THROW 51008, 'El nombre de usuario o correo ya existe.', 1;

	INSERT dbo.Usuarios(NombreUsuario,ContrasenaHash,Correo,Rol,DebeCambiarContrasena)
	VALUES(@NombreUsuario,@ContrasenaHash,@Correo,@Rol,1);
	DECLARE @UsuarioID int = CONVERT(int, SCOPE_IDENTITY());
	DECLARE @Descripcion nvarchar(500) = N'Creó al usuario ' + @NombreUsuario;
	EXEC dbo.sp_RegistrarBitacora @AdministradorID,N'CREAR_USUARIO',@Descripcion,N'Usuarios',@UsuarioID;
	SELECT @UsuarioID AS UsuarioID;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_RestablecerContrasenaUsuario
	@AdministradorID int,
	@UsuarioID int,
	@ContrasenaHash nvarchar(255)
AS
BEGIN
	SET NOCOUNT ON;
	IF NOT EXISTS (SELECT 1 FROM dbo.Usuarios WHERE UsuarioID=@AdministradorID AND Rol=N'Administrador' AND Activo=1)
		THROW 51009, 'Solo un administrador activo puede restablecer contraseñas.', 1;
	IF NOT EXISTS (SELECT 1 FROM dbo.Usuarios WHERE UsuarioID=@UsuarioID)
		THROW 51005, 'El usuario no existe.', 1;

	UPDATE dbo.Usuarios
	SET ContrasenaHash=@ContrasenaHash, Activo=1, IntentosFallidos=0,
		UltimoIntentoFallido=NULL, DebeCambiarContrasena=1
	WHERE UsuarioID=@UsuarioID;
	DECLARE @Descripcion nvarchar(500) = N'Restableció la contraseña del usuario ' + CONVERT(nvarchar(20),@UsuarioID);
	EXEC dbo.sp_RegistrarBitacora @AdministradorID,N'RESTABLECER_CLAVE',@Descripcion,N'Usuarios',@UsuarioID;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_CambiarContrasena
	@UsuarioID int,
	@ContrasenaHash nvarchar(255)
AS
BEGIN
	SET NOCOUNT ON;
	IF NOT EXISTS (SELECT 1 FROM dbo.Usuarios WHERE UsuarioID=@UsuarioID AND Activo=1)
		THROW 51010, 'El usuario no está activo.', 1;
	UPDATE dbo.Usuarios SET ContrasenaHash=@ContrasenaHash, DebeCambiarContrasena=0 WHERE UsuarioID=@UsuarioID;
	EXEC dbo.sp_RegistrarBitacora @UsuarioID,N'CAMBIAR_CLAVE',N'El usuario cambió su contraseña.',N'Usuarios',@UsuarioID;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_RegistrarIntentoFallido @UsuarioID int AS
BEGIN
	SET NOCOUNT ON;
	DECLARE @IntentosFallidos tinyint;
	DECLARE @NombreUsuario nvarchar(50);
	DECLARE @Descripcion nvarchar(500);
	DECLARE @Filas int;

	/* Evita que TR_Usuarios_Bitacora genere una fila genérica ACTUALIZAR por este cambio de seguridad. */
	EXEC sys.sp_set_session_context @key = N'OmitirAuditoriaUsuario', @value = 1;

	BEGIN TRY
		UPDATE dbo.Usuarios
		   SET IntentosFallidos = CASE WHEN IntentosFallidos < 3 THEN IntentosFallidos + 1 ELSE 3 END,
			   UltimoIntentoFallido = SYSDATETIME(),
			   Activo = CASE WHEN IntentosFallidos + 1 >= 3 THEN 0 ELSE Activo END
		 WHERE UsuarioID = @UsuarioID AND Activo = 1;
		SET @Filas = @@ROWCOUNT;
	END TRY
	BEGIN CATCH
		EXEC sys.sp_set_session_context @key = N'OmitirAuditoriaUsuario', @value = 0;
		THROW;
	END CATCH;

	EXEC sys.sp_set_session_context @key = N'OmitirAuditoriaUsuario', @value = 0;

	IF @Filas = 0
		RETURN;

	SELECT @IntentosFallidos = IntentosFallidos, @NombreUsuario = NombreUsuario
	FROM dbo.Usuarios
	WHERE UsuarioID = @UsuarioID;

	/* Un login fallido no tiene usuario autenticado: UsuarioID queda NULL y el usuario afectado va en RegistroID. */
	SET @Descripcion = N'Intento de inicio de sesión fallido para el usuario ' + @NombreUsuario
		+ N'. Intentos acumulados: ' + CONVERT(nvarchar(10), @IntentosFallidos) + N'.';
	EXEC dbo.sp_RegistrarBitacora
		@UsuarioID = NULL,
		@Accion = N'INTENTO_FALLIDO',
		@Descripcion = @Descripcion,
		@TablaAfectada = N'Usuarios',
		@RegistroID = @UsuarioID;

	IF @IntentosFallidos >= 3
	BEGIN
		SET @Descripcion = N'La cuenta del usuario ' + @NombreUsuario
			+ N' fue bloqueada después de alcanzar 3 intentos fallidos.';
		EXEC dbo.sp_RegistrarBitacora
			@UsuarioID = NULL,
			@Accion = N'CUENTA_BLOQUEADA',
			@Descripcion = @Descripcion,
			@TablaAfectada = N'Usuarios',
			@RegistroID = @UsuarioID;
	END;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_RegistrarLoginExitoso @UsuarioID int AS
BEGIN
	SET NOCOUNT ON;
	UPDATE dbo.Usuarios SET IntentosFallidos=0, UltimoIntentoFallido=NULL WHERE UsuarioID=@UsuarioID;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_DesbloquearUsuario
	@AdministradorID int,
	@UsuarioID int
AS
BEGIN
	SET NOCOUNT ON;

	IF NOT EXISTS
	(
		SELECT 1
		FROM dbo.Usuarios
		WHERE UsuarioID = @AdministradorID
		  AND Rol = N'Administrador'
		  AND Activo = 1
	)
		THROW 51004, 'Solo un administrador activo puede desbloquear usuarios.', 1;

	IF NOT EXISTS (SELECT 1 FROM dbo.Usuarios WHERE UsuarioID = @UsuarioID)
		THROW 51005, 'El usuario que se desea desbloquear no existe.', 1;

	UPDATE dbo.Usuarios
	   SET Activo = 1,
		   IntentosFallidos = 0,
		   UltimoIntentoFallido = NULL
	 WHERE UsuarioID = @UsuarioID;

	DECLARE @Descripcion nvarchar(500) = N'Desbloqueó al usuario ' + CONVERT(nvarchar(20), @UsuarioID);
	EXEC dbo.sp_RegistrarBitacora
		@UsuarioID = @AdministradorID,
		@Accion = N'DESBLOQUEAR',
		@Descripcion = @Descripcion,
		@TablaAfectada = N'Usuarios',
		@RegistroID = @UsuarioID;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_CalcularDescuentosAusencias @NominaID int AS
BEGIN
	SET NOCOUNT ON;
	DECLARE @Desde date,@Hasta date; SELECT @Desde=PeriodoDesde,@Hasta=PeriodoHasta FROM dbo.Nominas WHERE NominaID=@NominaID;
	INSERT dbo.NominaConcepto(NominaDetalleID,ConceptoID,AsistenciaID,BaseCalculo,Cantidad,Monto,Descripcion)
	SELECT nd.NominaDetalleID,c.ConceptoID,a.AsistenciaID,nd.SalarioBase,1,ROUND((nd.SalarioBase/r.Valor),2),N'Descuento por ausencia injustificada del '+CONVERT(nvarchar(10),a.Fecha,23)
	FROM dbo.NominaDetalle nd INNER JOIN dbo.Asistencias a ON a.EmpleadoID=nd.EmpleadoID AND a.Fecha BETWEEN @Desde AND @Hasta CROSS JOIN (SELECT TOP 1 Valor FROM dbo.ReglasNominaPanama WHERE CodigoRegla='DIVISOR_SALARIO_DIARIO' AND Activo=1 ORDER BY VigenciaDesde DESC) r CROSS JOIN (SELECT TOP 1 ConceptoID FROM dbo.ConceptosNomina WHERE Codigo='AUSENCIA_INJUSTIFICADA') c
	WHERE nd.NominaID=@NominaID AND a.TipoRegistro='Ausencia' AND a.Estado='Injustificada' AND a.GeneraDescuento=1;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_CrearNomina
	@FechaPago date, @PeriodoDesde date, @PeriodoHasta date, @PeriodoPagado nvarchar(50)
AS
BEGIN
	SET NOCOUNT ON; SET XACT_ABORT ON;
	BEGIN TRANSACTION;
	INSERT dbo.Nominas(FechaPago,PeriodoDesde,PeriodoHasta,PeriodoPagado,Estado)
	VALUES(@FechaPago,@PeriodoDesde,@PeriodoHasta,@PeriodoPagado,N'Borrador');
	DECLARE @NominaID int=SCOPE_IDENTITY();
	INSERT dbo.NominaDetalle(NominaID,EmpleadoID,SalarioBase,TotalDevengado)
	SELECT @NominaID,EmpleadoID,SalarioBase,SalarioBase FROM dbo.Empleados WHERE Activo=1;

	DECLARE @Css decimal(12,8)=(SELECT TOP 1 Porcentaje FROM dbo.ReglasNominaPanama WHERE CodigoRegla='CSS_TRABAJADOR' AND Activo=1 ORDER BY VigenciaDesde DESC);
	DECLARE @Edu decimal(12,8)=(SELECT TOP 1 Porcentaje FROM dbo.ReglasNominaPanama WHERE CodigoRegla='SEGURO_EDUCATIVO_TRABAJADOR' AND Activo=1 ORDER BY VigenciaDesde DESC);
	DECLARE @CssID int=(SELECT ConceptoID FROM dbo.ConceptosNomina WHERE Codigo='CSS_TRABAJADOR');
	DECLARE @EduID int=(SELECT ConceptoID FROM dbo.ConceptosNomina WHERE Codigo='SEGURO_EDUCATIVO_TRABAJADOR');
	INSERT dbo.NominaConcepto(NominaDetalleID,ConceptoID,BaseCalculo,PorcentajeAplicado,Monto,Descripcion)
	SELECT NominaDetalleID,@CssID,SalarioBase,@Css,ROUND(SalarioBase*@Css,2),N'Seguro Social del trabajador' FROM dbo.NominaDetalle WHERE NominaID=@NominaID;
	INSERT dbo.NominaConcepto(NominaDetalleID,ConceptoID,BaseCalculo,PorcentajeAplicado,Monto,Descripcion)
	SELECT NominaDetalleID,@EduID,SalarioBase,@Edu,ROUND(SalarioBase*@Edu,2),N'Seguro Educativo del trabajador' FROM dbo.NominaDetalle WHERE NominaID=@NominaID;
	EXEC dbo.sp_CalcularDescuentosAusencias @NominaID;
	UPDATE dbo.Nominas SET Estado=N'Emitida' WHERE NominaID=@NominaID;
	COMMIT TRANSACTION;
	SELECT @NominaID AS NominaID;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_ReporteHistoricoNominas AS
BEGIN SET NOCOUNT ON; SELECT NominaID,FechaPago,PeriodoDesde,PeriodoHasta,PeriodoPagado,Estado,TotalBruto,TotalDeducciones,TotalAportesPatronales,TotalNeto FROM dbo.Nominas ORDER BY FechaPago DESC,NominaID DESC; END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_ReporteDetalladoNomina @NominaID int AS
BEGIN SET NOCOUNT ON; SELECT n.NominaID,n.PeriodoPagado,e.EmpleadoID,e.Nombre,dpt.Nombre AS Departamento,c.Nombre AS Cargo,nd.SalarioBase,nd.TotalDevengado,nd.TotalDeducciones,nd.TotalAportesPatronales,nd.SalarioNeto FROM dbo.Nominas n INNER JOIN dbo.NominaDetalle nd ON nd.NominaID=n.NominaID INNER JOIN dbo.Empleados e ON e.EmpleadoID=nd.EmpleadoID INNER JOIN dbo.Departamentos dpt ON dpt.DepartamentoID=e.DepartamentoID INNER JOIN dbo.Cargos c ON c.CargoID=e.CargoID WHERE n.NominaID=@NominaID ORDER BY e.Nombre; END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_ResumenGeneralNomina @NominaID int AS
BEGIN SET NOCOUNT ON; SELECT dpt.Nombre AS Departamento,COUNT(*) AS TotalEmpleados,SUM(nd.TotalDevengado) AS TotalDevengado,SUM(nd.TotalDeducciones) AS TotalDeducciones,SUM(nd.SalarioNeto) AS CostoTotal FROM dbo.NominaDetalle nd INNER JOIN dbo.Empleados e ON e.EmpleadoID=nd.EmpleadoID INNER JOIN dbo.Departamentos dpt ON dpt.DepartamentoID=e.DepartamentoID WHERE nd.NominaID=@NominaID GROUP BY dpt.Nombre ORDER BY dpt.Nombre; END;
GO

/* Datos de prueba. El hash BCrypt debe ser reemplazado por el generado en C#. */
INSERT dbo.Usuarios(NombreUsuario,ContrasenaHash,Correo,Rol) VALUES (N'admin',N'$2a$12$P6v1znf8rxZ0QL2PzW8eQeF7vkJcxK.oRlNGzXSniaH75jg6F3Cny',N'admin@local.test',N'Administrador');
INSERT dbo.Departamentos(Nombre,Descripcion) VALUES (N'Recursos Humanos',N'Gestión del personal'),(N'Finanzas',N'Administración financiera'),(N'Tecnología',N'Sistemas');
INSERT dbo.Cargos(DepartamentoID,Nombre,Descripcion) SELECT DepartamentoID,N'Analista',N'Analista del área' FROM dbo.Departamentos WHERE Nombre=N'Recursos Humanos';
INSERT dbo.Empleados(Nombre,DepartamentoID,CargoID,SalarioBase,TipoContrato,FechaInicioContrato)
SELECT N'Administrador, Usuario',d.DepartamentoID,c.CargoID,1000.00,N'Permanente',CAST(GETDATE() AS date)
FROM dbo.Departamentos d INNER JOIN dbo.Cargos c ON c.DepartamentoID=d.DepartamentoID
WHERE d.Nombre=N'Recursos Humanos' AND c.Nombre=N'Analista';
INSERT dbo.Turnos(Nombre,HoraInicio,HoraFin,TipoJornada,HorasProgramadas) VALUES (N'Diurno', '08:00','17:00',N'Diurna',8),(N'Nocturno','22:00','06:00',N'Nocturna',8);
INSERT dbo.Feriados(Fecha,Nombre,TipoFeriado) VALUES ('2026-01-01',N'Año Nuevo',N'Nacional'),('2026-05-01',N'Día del Trabajo',N'Nacional');
INSERT dbo.ConceptosNomina(Codigo,Nombre,TipoConcepto,Naturaleza,AplicaA) VALUES
('CSS_TRABAJADOR',N'Seguro Social trabajador',N'Deduccion',N'Legal',N'Trabajador'),
('SEGURO_EDUCATIVO_TRABAJADOR',N'Seguro Educativo trabajador',N'Deduccion',N'Legal',N'Trabajador'),
('IMPUESTO_RENTA',N'Impuesto Sobre la Renta',N'Impuesto',N'Legal',N'Trabajador'),
('CSS_PATRONAL',N'Seguro Social patronal',N'AportePatronal',N'Legal',N'Empleador'),
('SEGURO_EDUCATIVO_PATRONAL',N'Seguro Educativo patronal',N'AportePatronal',N'Legal',N'Empleador'),
('AUSENCIA_INJUSTIFICADA',N'Ausencia injustificada',N'Deduccion',N'Legal',N'Trabajador'),
('HORAS_EXTRA_DIURNAS',N'Horas extra diurnas',N'Devengo',N'Legal',N'Trabajador'),
('HORAS_EXTRA_NOCTURNAS',N'Horas extra nocturnas',N'Devengo',N'Legal',N'Trabajador'),
('PAGO_FERIADO',N'Pago o recargo por feriado',N'Devengo',N'Legal',N'Trabajador');
/* Valores iniciales de referencia: validar antes de la sustentación y producción. */
INSERT dbo.ReglasNominaPanama(CodigoRegla,Descripcion,TipoCalculo,Porcentaje,MontoDesde,MontoHasta,Valor,Periodicidad,VigenciaDesde,VigenciaHasta,FuenteLegal)
VALUES
('CSS_TRABAJADOR',N'Tasa de referencia CSS del trabajador',N'Porcentaje',0.0975,NULL,NULL,NULL,N'Mensual','2026-01-01',NULL,N'Validar con CSS vigente'),
('SEGURO_EDUCATIVO_TRABAJADOR',N'Tasa de referencia Seguro Educativo trabajador',N'Porcentaje',0.0125,NULL,NULL,NULL,N'Mensual','2026-01-01',NULL,N'Validar con normativa vigente'),
('DIVISOR_SALARIO_DIARIO',N'Divisor para obtener salario diario',N'Divisor',NULL,NULL,NULL,30,N'PorDia','2026-01-01',NULL,N'Validar según período y legislación vigente'),
('DIVISOR_SALARIO_HORA',N'Divisor para obtener valor hora',N'Divisor',NULL,NULL,NULL,240,N'PorHora','2026-01-01',NULL,N'Validar según jornada aplicable'),
('ISR_TRAMO_1',N'ISR tramo exento',N'Tramo',0,NULL,11000,NULL,N'Anual','2026-01-01',NULL,N'Validar con DGI vigente'),
('ISR_TRAMO_2',N'ISR segundo tramo',N'Tramo',0.15,11000,50000,NULL,N'Anual','2026-01-01',NULL,N'Validar con DGI vigente'),
('ISR_TRAMO_3',N'ISR excedente tercer tramo',N'Tramo',0.25,50000,NULL,NULL,N'Anual','2026-01-01',NULL,N'Validar con DGI vigente'),
('HORAS_EXTRA_DIURNAS',N'Recargo de hora extra diurna',N'Multiplicador',NULL,NULL,NULL,1,N'PorHora','2026-01-01',NULL,N'Validar con Código de Trabajo vigente'),
('HORAS_EXTRA_NOCTURNAS',N'Recargo de hora extra nocturna',N'Multiplicador',NULL,NULL,NULL,1,N'PorHora','2026-01-01',NULL,N'Validar con Código de Trabajo vigente'),
('PAGO_FERIADO',N'Recargo por trabajo en feriado',N'Multiplicador',NULL,NULL,NULL,1,N'PorHora','2026-01-01',NULL,N'Validar con Código de Trabajo vigente');
GO
