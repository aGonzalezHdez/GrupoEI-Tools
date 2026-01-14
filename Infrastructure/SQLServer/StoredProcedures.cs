namespace Infrastructure.SQLServer;

public static class StoredProcedures
{
    public static class Reporting
    {
        public const string VerificacionCfdi = "dbo.NET_REPORTING_VERIFICACION_DE_COMPROBANTE_FISCAL_DIGITAL";
        
    }

    public static class Usuarios
    {
        public const string DuplicaVentanaUsuario = "dbo.NET_DUPLICA_USUARIO";
        public const string ActualizaPuesto = "Perfiles.NET_ACTUALIZAR_PUESTO";
    }
}