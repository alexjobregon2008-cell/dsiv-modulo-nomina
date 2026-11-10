USE MODULO_Final;
GO

CREATE OR ALTER PROCEDURE dbo.sp_DesbloquearUsuario
	@AdministradorID int,
	@UsuarioID int
AS
BEGIN
	SET NOCOUNT ON;

	IF NOT EXISTS (SELECT 1 FROM dbo.Usuarios WHERE UsuarioID=@AdministradorID AND Rol=N'Administrador' AND Activo=1)
		THROW 51004, 'Solo un administrador activo puede desbloquear usuarios.', 1;
	IF NOT EXISTS (SELECT 1 FROM dbo.Usuarios WHERE UsuarioID=@UsuarioID)
		THROW 51005, 'El usuario que se desea desbloquear no existe.', 1;

	UPDATE dbo.Usuarios
	SET Activo=1, IntentosFallidos=0, UltimoIntentoFallido=NULL
	WHERE UsuarioID=@UsuarioID;

	DECLARE @Descripcion nvarchar(500) = N'Desbloqueó al usuario '+CONVERT(nvarchar(20),@UsuarioID);
	EXEC dbo.sp_RegistrarBitacora
		@UsuarioID=@AdministradorID,
		@Accion=N'DESBLOQUEAR',
		@Descripcion=@Descripcion,
		@TablaAfectada=N'Usuarios',
		@RegistroID=@UsuarioID;
END;
GO
