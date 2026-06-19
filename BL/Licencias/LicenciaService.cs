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

        // ALMACENES
        public bool ValidarAlmacenes(int totalActual)
        {
            return totalActual < _config.AlmacenesMax;
        }

        // UBICACIONES
        public bool ValidarUbicaciones(int totalActual)
        {
            return totalActual < _config.UbicacionesMax;
        }

        //  OBTENER UBICACIONES 
        public int ObtenerLimiteUbicaciones()
        {
            return _config.UbicacionesMax;
        }

        // PRODUCTOS
        public bool ValidarProductos(int totalActual)
        {
            return totalActual < _config.ProductosMax;
        }

        //OBTENER PRODUCTOS LIMITE
        public int ObtenerProductosLimites()
        {
            return _config.ProductosMax;
        }

        // USUARIOS OPERADORES
        public bool ValidarUsuarioOperador(int totalActual)
        {
            return totalActual < _config.UsuariosOpeMax;
        }

        // USUARIOS ADMINISTRADORES
        public bool ValidarUsuarioAdministrador(int totalActual)
        {
            return totalActual < _config.UsuariosAdmiMax;
        }

        // INVENTARIOS PREPARADOS
        public bool ValidarInventariosPreparados(int totalActual)
        {
            return totalActual < _config.InventariosPreparadosMax;
        }

        


        // VALIDARPRODUCTOS
        //public bool ValidarAlmacenes()
        //{
        //    int totalActual = new AlmacenBL().Contar();

        //    return totalActual < _licencia.AlmacenesMax;
        //}
    }
}