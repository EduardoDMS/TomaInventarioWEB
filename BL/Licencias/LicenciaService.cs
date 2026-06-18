namespace TomaInventario.BL.Licencias
{
    public class LicenciaService
    {
        private readonly ILicenciaProvider _provider;
        private readonly LicenciaConfig _config;

        public LicenciaService(ILicenciaProvider provider)
        {
            _provider = provider;
            _config = _provider.Obtener();
        }

        // VALIDACIONES
        public bool ValidarAlmacenes(int totalActual)
        {
            return totalActual < _config.AlmacenesMax;
        }

        // VALIDARPRODUCTOS
        //public bool ValidarAlmacenes()
        //{
        //    int totalActual = new AlmacenBL().Contar();

        //    return totalActual < _licencia.AlmacenesMax;
        //}
    }
}