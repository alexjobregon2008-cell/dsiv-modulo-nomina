# Prompt para generar el diagrama E-R

Genera un diagrama entidad-relación profesional para un sistema de nómina de Panamá llamado `MODULO_Final`.

Usa notación Crow's Foot y muestra las entidades, claves primarias (PK), claves foráneas (FK), atributos principales y cardinalidades. No inventes entidades adicionales.

Entidades y relaciones:

- `Usuarios(UsuarioID PK, NombreUsuario UQ, ContrasenaHash, Correo, Rol, Activo, IntentosFallidos, UltimoIntentoFallido)`.
- `Bitacora(BitacoraID PK, UsuarioID FK, Accion, Descripcion, TablaAfectada, RegistroID, FechaHora)`; un usuario registra muchas entradas.
- `Departamentos(DepartamentoID PK, Nombre)`; un departamento tiene muchos empleados.
- `Cargos(CargoID PK, DepartamentoID FK, Nombre, SalarioMinimo, SalarioMaximo)`; un departamento tiene muchos cargos.
- `Empleados(EmpleadoID PK, UsuarioID FK opcional, DepartamentoID FK, CargoID FK, Cedula, Nombre, Apellido, FechaIngreso, SalarioBase, Activo)`; cada empleado pertenece a un departamento y cargo.
- `Turnos(TurnoID PK, Nombre, HoraEntrada, HoraSalida)`.
- `EmpleadoTurno(EmpleadoID FK, TurnoID FK, FechaInicio, FechaFin, PK compuesta)`; resuelve la relación muchos a muchos histórica entre empleados y turnos.
- `Feriados(FeriadoID PK, Fecha, Nombre, EsNacional)`.
- `Asistencias(AsistenciaID PK, EmpleadoID FK, Fecha, Estado, Observacion)`; un empleado tiene muchas asistencias.
- `AsistenciaHoras(AsistenciaHoraID PK, AsistenciaID FK, HoraEntrada, HoraSalida, HorasNormales, HorasExtras)`.
- `Nominas(NominaID PK, PeriodoInicio, PeriodoFin, FechaPago, Estado, TotalBruto, TotalDeducciones, TotalNeto)`.
- `NominaDetalle(NominaDetalleID PK, NominaID FK, EmpleadoID FK, SalarioBase, TotalIngresos, TotalDeducciones, NetoPagar)`; una nómina tiene muchos detalles.
- `ConceptosNomina(ConceptoID PK, Codigo UQ, Nombre, Tipo, EsDeduccion, Activo)`.
- `ReglasNominaPanama(ReglaID PK, Codigo, Nombre, Tasa, MontoDesde, MontoHasta, VigenciaDesde, VigenciaHasta, FuenteLegal, Activo)`; contiene reglas configurables y vigentes para Seguro Social, Seguro Educativo e ISR.
- `NominaConcepto(NominaDetalleID FK, ConceptoID FK, Monto, PK compuesta)`; relaciona detalles con conceptos.

Destaca las relaciones 1:N y N:M, las claves compuestas y las relaciones opcionales. Usa colores distintos para seguridad/auditoría, recursos humanos, asistencia y nómina. Incluye una leyenda y un título legible. Entrega el resultado como SVG o PNG de alta resolución y, si es posible, también el código Mermaid o PlantUML editable.
