# Convenciones del proyecto MODULO

## Nombres de código

- Clases y formularios: `PascalCase` (`SeguridadService`, `Form1`).
- Métodos y propiedades: `PascalCase` (`DesbloquearUsuario`).
- Variables y parámetros locales: `camelCase`.
- Campos privados: `_camelCase`.
- Constantes: `PascalCase`.
- Un tipo público principal por archivo; el nombre del archivo coincide con el tipo.

## Base de datos

- Tablas y columnas: `PascalCase`, en singular lógico cuando representa una entidad (`Usuarios`, `Empleados`).
- Claves primarias: `<Entidad>ID` (`UsuarioID`).
- Claves foráneas: el mismo nombre de la clave primaria referenciada.
- Procedimientos almacenados: prefijo `sp_` y verbo descriptivo (`sp_LoginUsuario`, `sp_DesbloquearUsuario`).
- Triggers: prefijo `TR_` y tabla/evento (`TR_Usuarios_Bitacora`).
- Restricciones: prefijos `PK_`, `FK_`, `CK_`, `UQ_`.
- Parámetros SQL: prefijo `@` y nombre equivalente a la columna o concepto.

## Seguridad

- Las contraseñas se almacenan únicamente como hashes BCrypt.
- Las consultas se ejecutan con parámetros; no se concatena entrada del usuario en SQL.
- Los cambios relevantes se registran en `Bitacora`.
- Las tasas de nómina panameña son configurables y deben tener vigencia y fuente legal.
