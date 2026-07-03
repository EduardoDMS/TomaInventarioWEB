public class LicenciaConfig
{
    // Campos de compatibilidad y mapeo con ASF_EMPRESA
    public string COD_EMPRESA { get; set; }
    public string DESC_EMPRESA { get; set; }
    public string FLG_ACTIVO { get; set; }
    public string RUC_EMPRESA { get; set; }
    public string DIR_EMPRESA { get; set; }
    public string TLF_EMPRESA { get; set; }
    public string RAZONSOCIAL_EMPRESA { get; set; }

    // Propiedades heredadas / compatibles (para no romper el BL actual) p
    public string CodigoCliente { get => COD_EMPRESA; set => COD_EMPRESA = value; }
    public string NombreEmpresa { get => DESC_EMPRESA; set => DESC_EMPRESA = value; }

    // Campos de Límites (ASF_CONFIG_LICENCIA)
    public int AlmacenesMax { get; set; }
    public int UbicacionesMax { get; set; }
    public int ProductosMax { get; set; }
    public int UsuariosAdmiMax { get; set; }
    public int UsuariosOpeMax { get; set; }
    public int InventariosPreparadosMax { get; set; }
    public bool Estado { get; set; }
}