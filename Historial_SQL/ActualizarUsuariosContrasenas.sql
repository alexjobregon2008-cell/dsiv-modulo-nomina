USE MODULO_Final;
GO
IF COL_LENGTH(N'dbo.Usuarios', N'DebeCambiarContrasena') IS NULL
	ALTER TABLE dbo.Usuarios ADD DebeCambiarContrasena bit NOT NULL CONSTRAINT DF_Usuarios_DebeCambiar DEFAULT 0;
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
CREATE OR ALTER PROCEDURE dbo.sp_RegistrarLoginExitoso @UsuarioID int AS
BEGIN
	SET NOCOUNT ON;
	UPDATE dbo.Usuarios SET IntentosFallidos=0, UltimoIntentoFallido=NULL WHERE UsuarioID=@UsuarioID;
END;
GO
CREATE OR ALTER PROCEDURE dbo.sp_CrearUsuario
	@AdministradorID int, @NombreUsuario nvarchar(50), @ContrasenaHash nvarchar(255),
	@Correo nvarchar(150), @Rol nvarchar(20)=N'Usuario'
AS
BEGIN
	SET NOCOUNT ON;
	IF NOT EXISTS (SELECT 1 FROM dbo.Usuarios WHERE UsuarioID=@AdministradorID AND Rol=N'Administrador' AND Activo=1)
		THROW 51006, 'Solo un administrador activo puede crear usuarios.', 1;
	IF @Rol NOT IN (N'Administrador', N'Usuario') THROW 51007, 'El rol indicado no es válido.', 1;
	IF EXISTS (SELECT 1 FROM dbo.Usuarios WHERE NombreUsuario=@NombreUsuario OR Correo=@Correo)
		THROW 51008, 'El nombre de usuario o correo ya existe.', 1;
	INSERT dbo.Usuarios(NombreUsuario,ContrasenaHash,Correo,Rol,DebeCambiarContrasena)
	VALUES(@NombreUsuario,@ContrasenaHash,@Correo,@Rol,1);
	DECLARE @UsuarioID int=CONVERT(int,SCOPE_IDENTITY());
	DECLARE @Descripcion nvarchar(500)=N'Creó al usuario '+@NombreUsuario;
	EXEC dbo.sp_RegistrarBitacora @AdministradorID,N'CREAR_USUARIO',@Descripcion,N'Usuarios',@UsuarioID;
	SELECT @UsuarioID AS UsuarioID;
END;
GO
CREATE OR ALTER PROCEDURE dbo.sp_RestablecerContrasenaUsuario
	@AdministradorID int, @UsuarioID int, @ContrasenaHash nvarchar(255)
AS
BEGIN
	SET NOCOUNT ON;
	IF NOT EXISTS (SELECT 1 FROM dbo.Usuarios WHERE UsuarioID=@AdministradorID AND Rol=N'Administrador' AND Activo=1)
		THROW 51009, 'Solo un administrador activo puede restablecer contraseñas.', 1;
	IF NOT EXISTS (SELECT 1 FROM dbo.Usuarios WHERE UsuarioID=@UsuarioID) THROW 51005, 'El usuario no existe.', 1;
	UPDATE dbo.Usuarios SET ContrasenaHash=@ContrasenaHash,Activo=1,IntentosFallidos=0,UltimoIntentoFallido=NULL,DebeCambiarContrasena=1 WHERE UsuarioID=@UsuarioID;
	DECLARE @Descripcion nvarchar(500)=N'Restableció la contraseña del usuario '+CONVERT(nvarchar(20),@UsuarioID);
	EXEC dbo.sp_RegistrarBitacora @AdministradorID,N'RESTABLECER_CLAVE',@Descripcion,N'Usuarios',@UsuarioID;
END;
GO
CREATE OR ALTER PROCEDURE dbo.sp_CambiarContrasena @UsuarioID int,@ContrasenaHash nvarchar(255) AS
BEGIN
	SET NOCOUNT ON;
	IF NOT EXISTS (SELECT 1 FROM dbo.Usuarios WHERE UsuarioID=@UsuarioID AND Activo=1) THROW 51010, 'El usuario no está activo.', 1;
	UPDATE dbo.Usuarios SET ContrasenaHash=@ContrasenaHash,DebeCambiarContrasena=0 WHERE UsuarioID=@UsuarioID;
	EXEC dbo.sp_RegistrarBitacora @UsuarioID,N'CAMBIAR_CLAVE',N'El usuario cambió su contraseña.',N'Usuarios',@UsuarioID;
END;
GO
DECLARE @UsuarioPrueba int=(SELECT UsuarioID FROM dbo.Usuarios WHERE NombreUsuario=N'usuario.prueba');
IF @UsuarioPrueba IS NOT NULL
BEGIN
	UPDATE dbo.Usuarios SET Activo=1,IntentosFallidos=2,UltimoIntentoFallido=SYSDATETIME() WHERE UsuarioID=@UsuarioPrueba;
	EXEC dbo.sp_RegistrarLoginExitoso @UsuarioID=@UsuarioPrueba;
	SELECT NombreUsuario,Activo,IntentosFallidos,DebeCambiarContrasena FROM dbo.Usuarios WHERE UsuarioID=@UsuarioPrueba;
END;
GO
