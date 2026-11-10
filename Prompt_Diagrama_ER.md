# Prompt para generar el diagrama E-R

Genera un diagrama entidad-relación profesional para un sistema de nómina de Panamá llamado `MODULO_Final`.

Usa notación Crow's Foot y muestra las entidades, claves primarias (PK), claves foráneas (FK), atributos principales y cardinalidades. No inventes entidades adicionales.

Entidades y relaciones:

- `Usuarios(UsuarioID PK, NombreUsuario UQ, ContrasenaHash, Correo UQ, Rol, Activo, IntentosFallidos, UltimoIntentoFallido, DebeCambiarContrasena, FechaCreacion)`.
- `Bitacora(BitacoraID PK, UsuarioID FK, Accion, Descripcion, TablaAfectada, RegistroID, FechaHora)`; un usuario registra muchas entradas.
- `Departamentos(DepartamentoID PK, Nombre)`; un departamento tiene muchos empleados.
- `Cargos(CargoID PK, DepartamentoID FK, Nombre, Descripcion, SalarioMinimo, SalarioMaximo)`; un departamento tiene muchos cargos.
- `Empleados(EmpleadoID PK, Nombre, DepartamentoID FK, CargoID FK, SalarioBase, TipoContrato, AplicaDescuentos, FechaInicioContrato, Activo, FechaCreacion)`; cada empleado pertenece a un departamento y cargo.
- `Turnos(TurnoID PK, Nombre, HoraInicio, HoraFin, TipoJornada, HorasProgramadas, Activo)`.
- `EmpleadoTurno(EmpleadoTurnoID PK, EmpleadoID FK, TurnoID FK, FechaDesde, FechaHasta, Activo)`; conserva la asignación histórica de turnos.
- `Feriados(FeriadoID PK, Fecha UQ, Nombre, TipoFeriado, EsDiaNoLaborable, Activo)`.
- `Asistencias(AsistenciaID PK, EmpleadoID FK, TurnoID FK opcional, Fecha, TipoRegistro, Estado, horas y banderas de control, FeriadoID FK opcional, Observacion)`; un empleado tiene muchas asistencias.
- `AsistenciaHoras(AsistenciaHoraID PK, AsistenciaID FK, TipoHora, CantidadHoras, HorasAprobadas, AprobadoPorUsuarioID FK opcional, FechaAprobacion)`.
- `Nominas(NominaID PK, FechaPago, PeriodoDesde, PeriodoHasta, PeriodoPagado, Estado, TotalBruto, TotalDeducciones, TotalAportesPatronales, TotalNeto, FechaCreacion)`.
- `NominaDetalle(NominaDetalleID PK, NominaID FK, EmpleadoID FK, SalarioBase, TotalDevengado, TotalDeducciones, TotalAportesPatronales, SalarioNeto)`; una nómina tiene muchos detalles.
- `ConceptosNomina(ConceptoID PK, Codigo UQ, Nombre, TipoConcepto, Naturaleza, AplicaA, Activo)`.
- `ReglasNominaPanama(ReglaID PK, ConceptoID FK opcional, CodigoRegla, Descripcion, TipoCalculo, Porcentaje, Multiplicador, MontoDesde, MontoHasta, Valor, Periodicidad, VigenciaDesde, VigenciaHasta, FuenteLegal, Activo)`; contiene reglas configurables y vigentes para Seguro Social, Seguro Educativo e ISR.
- `NominaConcepto(NominaConceptoID PK, NominaDetalleID FK, ConceptoID FK, AsistenciaID FK opcional, AsistenciaHoraID FK opcional, BaseCalculo, Cantidad, PorcentajeAplicado, Monto, EsPatronal, Descripcion)`; relaciona detalles con conceptos.

Destaca las relaciones 1:N y N:M, las claves compuestas y las relaciones opcionales. Usa colores distintos para seguridad/auditoría, recursos humanos, asistencia y nómina. Incluye una leyenda y un título legible. Entrega el resultado como SVG o PNG de alta resolución y, si es posible, también el código Mermaid o PlantUML editable.
