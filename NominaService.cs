using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace MODULO.CSharp
{
    public static class NominaService
    {
        public static int Crear(
            DateTime fechaPago,
            DateTime periodoDesde,
            DateTime periodoHasta,
            string periodoPagado)
        {
            object resultado = ConexionBD.EjecutarProcedimientoEscalar(
                "dbo.sp_CrearNomina",
                new SqlParameter("@FechaPago", SqlDbType.Date)
                {
                    Value = fechaPago.Date
                },
                new SqlParameter("@PeriodoDesde", SqlDbType.Date)
                {
                    Value = periodoDesde.Date
                },
                new SqlParameter("@PeriodoHasta", SqlDbType.Date)
                {
                    Value = periodoHasta.Date
                },
                new SqlParameter("@PeriodoPagado", SqlDbType.NVarChar, 50)
                {
                    Value = periodoPagado
                });

            return Convert.ToInt32(resultado);
        }

        public static DataTable Historial()
        {
            return ConexionBD.EjecutarProcedimiento(
                "dbo.sp_ReporteHistoricoNominas");
        }

        public static DataTable ReporteDetallado(int nominaId)
        {
            return ConexionBD.EjecutarProcedimiento(
                "dbo.sp_ReporteDetalladoNomina",
                new SqlParameter("@NominaID", SqlDbType.Int)
                {
                    Value = nominaId
                });
        }

        public static DataTable ResumenGeneral(int nominaId)
        {
            return ConexionBD.EjecutarProcedimiento(
                "dbo.sp_ResumenGeneralNomina",
                new SqlParameter("@NominaID", SqlDbType.Int)
                {
                    Value = nominaId
                });
        }
    }
}