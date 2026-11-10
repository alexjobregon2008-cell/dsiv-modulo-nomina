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
