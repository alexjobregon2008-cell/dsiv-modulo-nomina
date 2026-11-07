# Política de usuarios, contraseñas e intentos

## Creación de usuarios

Solo un administrador activo puede crear usuarios mediante `UsuariosService.CrearUsuario`. La contraseña temporal se convierte a un hash BCrypt en C# antes de enviarse a SQL Server. La base guarda `DebeCambiarContrasena = 1`, por lo que el nuevo usuario debe cambiarla después del primer inicio.

## Contraseña olvidada o cuenta bloqueada

El usuario no puede recuperar ni consultar la contraseña anterior porque el sistema solo conserva el hash. Debe contactar a un administrador.

El administrador utiliza `UsuariosService.RestablecerContrasena`. Esta operación:

1. Genera un hash BCrypt para una contraseña temporal nueva.
2. Activa la cuenta.
3. Reinicia `IntentosFallidos` y `UltimoIntentoFallido`.
4. Marca `DebeCambiarContrasena = 1`.
5. Registra la operación en `Bitacora`.

Al iniciar sesión con la contraseña temporal, el usuario debe establecer una nueva contraseña definitiva.

## Cambio de contraseña propia

Un usuario autenticado puede cambiar su contraseña con `UsuariosService.CambiarContrasena`. El sistema valida una longitud mínima de 8 caracteres y guarda únicamente el hash BCrypt.

## Intentos fallidos

- El contador pertenece a la cuenta, no a una sesión ni a un día calendario.
- Cada contraseña incorrecta incrementa el contador.
- Al tercer fallo, la cuenta se desactiva y queda bloqueada.
- Un inicio de sesión correcto ejecuta `sp_RegistrarLoginExitoso`, que deja `IntentosFallidos = 0` y `UltimoIntentoFallido = NULL`.
- Por tanto, si hoy el usuario inició correctamente y mañana falla una vez, ese fallo cuenta como el primero; no se arrastran intentos anteriores.
- Una cuenta bloqueada no puede iniciar sesión hasta que un administrador la restablezca o desbloquee.

## Roles

| Acción | Administrador | Usuario |
|---|---:|---:|
| Crear usuarios | Sí | No |
| Desbloquear cuentas | Sí | No |
| Restablecer contraseñas | Sí | No |
| Cambiar su propia contraseña | Sí | Sí |
| Consultar contraseñas | No | No |

## Seguridad

Las operaciones administrativas tienen doble validación: el servicio C# exige una sesión de administrador y el procedimiento almacenado vuelve a validar el rol y que la cuenta esté activa. Las tasas de nómina panameña permanecen configurables y con vigencia, conforme a las instrucciones del proyecto.
