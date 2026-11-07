# Entrega Persona 1 — Sistema de nómina

## Alcance implementado

- Proyecto Windows Forms C# sobre .NET 10.
- Inicio de sesión contra SQL Server LocalDB mediante `sp_LoginUsuario`.
- Verificación de contraseñas con BCrypt en C#; nunca se compara ni almacena texto plano.
- Control de tres intentos fallidos y bloqueo de la cuenta.
- Registro de inicio de sesión, intentos y acciones en `Bitacora`.
- Desbloqueo administrativo mediante `sp_DesbloquearUsuario` y `SeguridadService`.
- Esquema normalizado para usuarios, empleados, departamentos, cargos, turnos, asistencias, nóminas y conceptos.
- Reglas de deducciones panameñas configurables con período de vigencia.

## Ejecución

1. Crear o seleccionar la base `MODULO_Final` en SQL Server.
2. Ejecutar únicamente `BaseDatos_MODULO_Final.sql` en una base vacía. Este es el script SQL consolidado de la entrega.
3. Verificar la cadena de conexión en `MODULO.CSharp/ConexionBD.cs`.
4. Compilar la solución `MODULO.sln` con .NET 10.
5. Iniciar sesión con el usuario administrativo entregado por el script y cambiar la contraseña según la política del proyecto.

## Modelo entidad-relación

El modelo editable se encuentra en `Modelo_ER.mmd` y usa Mermaid. Puede visualizarse en Mermaid Live Editor, Visual Studio Code con una extensión Mermaid o cualquier herramienta compatible. El archivo `Prompt_Diagrama_ER.md` contiene también el prompt para generar una versión gráfica en SVG o PNG.

## Archivos históricos

La carpeta `Historial_SQL` contiene scripts incrementales y el esquema antiguo únicamente como referencia. No deben ejecutarse para la instalación final.

## Desbloqueo administrativo

La pantalla administrativa debe invocar:

```csharp
SeguridadService.DesbloquearUsuario(usuarioId);
```

El servicio valida la sesión local y el procedimiento valida nuevamente en la base que el administrador esté activo. Después restablece `Activo`, `IntentosFallidos` y `UltimoIntentoFallido`, y registra la operación.

## Decisiones de seguridad

- BCrypt se verifica en la aplicación porque SQL Server no debe conocer contraseñas.
- La autorización se valida tanto en C# como en SQL para evitar omisiones desde otros clientes.
- La bitácora conserva quién realizó cada operación y sobre qué registro.
- `MODULO_Anterior` se conserva como respaldo y no forma parte del proyecto final.

## Consideraciones legales

Los porcentajes y límites de Seguro Social, Seguro Educativo e Impuesto Sobre la Renta deben revisarse contra la legislación vigente de Panamá. Se almacenan como reglas configurables con fecha de vigencia y fuente, no como números mágicos en el código.
