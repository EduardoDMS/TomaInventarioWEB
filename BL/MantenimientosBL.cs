using BE;
using DAO;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using TomaInventario.BL.Licencias;

namespace BL
{
    public class MantenimientosBL
    {
        private readonly LicenciaService _licencia;

        public MantenimientosBL()
        {
            _licencia = new LicenciaService(new LicenciaDbProvider());
        }

        #region Mant Usuario
        public List<UsuarioBE> ListarUsuarios(string perfil, string usuario)
        {
            return new MantenimientosDAO().ListarUsuarios(perfil, usuario);
        }
        public Response CreateUsuario(string cod_Usuario, string nombre, string apellido, string clave, string perfil, List<string> ListidAlmacen)
        {
            int totalAdmin = new MantenimientosDAO().ContarUsuariosAdministrador();
            int totalOpe = new MantenimientosDAO().ContarUsuariosOperador();

            if (!_licencia.ValidarUsuarioAdministrador(totalAdmin) && perfil == "ADM")
            {
                return new Response
                {
                    HUBO_ERROR = true,
                    MENSAJE_ERROR = "Limite de usuarios administradores alcanzado"
                };
            }

            if (!_licencia.ValidarUsuarioOperador(totalOpe) && perfil == "OPE")
            {
                return new Response
                {
                    HUBO_ERROR = true,
                    MENSAJE_ERROR = "Limite de usuarios operadores alcanzado"
                };
            }

            string listaIdAlmacenes = string.Join(",", ListidAlmacen ?? new List<string>());
            return new MantenimientosDAO().CreateUsuario(cod_Usuario, nombre, apellido, clave, perfil, listaIdAlmacenes);
        }

        public Response GetUsuario(int idUsuario)
        {
            return new MantenimientosDAO().GetUsuario(idUsuario);
        }


        public Response UpdateUsuario(int idUsuario,string cod_usuario,string nombreUsuario,string apellidoUsuario,string clave,string perfil,
                List<string> ListidAlmacen,
                                        bool activo)
        {
            var dao = new MantenimientosDAO();

            //normaliza entradas
            string perfilNuevo = (perfil ?? string.Empty).Trim().ToUpper();
            string listaIdAlmacenes = string.Join(",", ListidAlmacen ?? new List<string>());

            //obtiene el usuario actual = lo guarda en respUsuario
            var respUsuario = dao.GetUsuario(idUsuario);

            if (respUsuario == null || respUsuario.HUBO_ERROR)
            {
                return new Response
                {
                    HUBO_ERROR = true,
                    MENSAJE_ERROR = respUsuario?.MENSAJE_ERROR ?? "Error al obtener usuario actual"
                };
            }

            var listaUsr = respUsuario.Entity as List<BE.UsuarioBE>;
            if (listaUsr == null || listaUsr.Count == 0)
            {
                return new Response
                {
                    HUBO_ERROR = true,
                    MENSAJE_ERROR = "Usuario no encontrado"
                };
            }

            var usuarioActual = listaUsr[0];
            string perfilActual = (usuarioActual.Perfil ?? string.Empty).Trim().ToUpper();

           
            //validar licencia NO IMPORTA SI ESTA ACTIVO O DESACTIVADO 3 manzanas son 3 manzanas y 4 peras son 4 peras >:v
            // Si cambia hacia ADM, validar límite de administradores
            if (perfilNuevo == "ADM" && perfilActual != "ADM")
            {
                int totalAdmin = dao.ContarUsuariosAdministrador();

                if (!_licencia.ValidarUsuarioAdministrador(totalAdmin))
                {
                    return new Response
                    {
                        HUBO_ERROR = true,
                        MENSAJE_ERROR = "Límite de usuarios administradores alcanzado"
                    };
                }
            }

            // igual pasa aca
            if (perfilNuevo == "OPE" && perfilActual != "OPE")
            {
                int totalOpe = dao.ContarUsuariosOperador();

                if (!_licencia.ValidarUsuarioOperador(totalOpe))
                {
                    return new Response
                    {
                        HUBO_ERROR = true,
                        MENSAJE_ERROR = "Límite de usuarios operadores alcanzado"
                    };
                }
            }
            return dao.UpdateUsuario(idUsuario: idUsuario,cod_usuario: cod_usuario,apellidoUsuario: apellidoUsuario,nombreUsuario: nombreUsuario,clave: clave,perfil: perfilNuevo,idAlmacen: listaIdAlmacenes,activo: activo
);
        }

        #endregion

        #region Mant Almacen
        public List<AlmacenBE> ListarAlmacenes(string dscAlmacen, string activo)
        {
            return new MantenimientosDAO().ListarAlmacenes(dscAlmacen, activo);
        }
        public Response CreateAlmacen(string codAlmacen, string descAlmacen)
        {
            int total = new MantenimientosDAO().ContarAlmacenes();

            if (!_licencia.ValidarAlmacenes(total))
            {
                return new Response
                {
                    HUBO_ERROR = true,
                    MENSAJE_ERROR = "Limite de almacenes alcanzado"
                };
            }

            return new MantenimientosDAO().CreateAlmacen(codAlmacen, descAlmacen);
        }
        public Response GetAlmacen(int idAlmacen)
        {
            return new MantenimientosDAO().GetAlmacen(idAlmacen);
        }
        public Response UpdateAlmacen(int idAlmacen, string codAlmacen, string dscAlmacen, bool activo)
        {
            return new MantenimientosDAO().UpdateAlmacen(idAlmacen, codAlmacen, dscAlmacen, activo);
        }
        public Response GetAPIAlmacen()
        {
            return new MantenimientosDAO().GetAPIAlmacen();
        }
        public Response CargarAlmacenesAPIExterna(List<AlmacenAPI> ListAlmacen)
        {
            MantenimientosDAO dao = new MantenimientosDAO();
            Response responseFinal = new Response();
            int error = 0;
            List<AlmacenAPI> ListAlmacenError = new List<AlmacenAPI>();
            for (int i = 0; i < ListAlmacen.Count; i++)
            {
                responseFinal = dao.CargarAlmacenesAPIExterna(ListAlmacen[i]);
                if (responseFinal.HUBO_ERROR == true)
                {
                    error++;
                    ListAlmacenError.Add(ListAlmacen[i]);
                }
            }
            if (error > 0)
            {
                responseFinal.HUBO_ERROR = true;
                responseFinal.MENSAJE_ERROR = "Hubo " + error.ToString() + " errores al registrar.";
                responseFinal.Entity = ListAlmacenError;
            }
            return responseFinal;
        }

        private string Almacen_ConvertListToXml(List<AlmacenAPI> lista)
        {
            XDocument xmlDocument = new XDocument(
                new XElement("Root",
                    lista.ConvertAll(item =>
                        new XElement("Item",
                            new XElement("COD_ALMACEN", item.ALMACEN_CODIGO),
                            new XElement("DSC_ALMACEN", item.ALMACEN_NOMBRE),
                            new XElement("COD_UBICACION", item.UBICACION_CODIGO),
                            new XElement("DSC_UBICACION", item.UBICACION_NOMBRE)

                        )
                    )
                )
            );

            return xmlDocument.ToString();
        }


        #endregion

        #region Mant Ubicacion
        //public List<UbicacionBE> ListarUbicaciones(string activo, string vchUbicacion, int idAlmacen)
        //{
        //    return new MantenimientosDAO().ListarUbicaciones(activo, vchUbicacion, idAlmacen);
        //}
        public Response ListarUbicaciones(string activo, string vchUbicacion, int idAlmacen, string start, string length, string order)
        {
            return new MantenimientosDAO().ListarUbicaciones(activo, vchUbicacion, idAlmacen, start, length, order);
        }

        public Response CreateUbicacion(string codUbicacion, string dscUbicacion, List<string> ListidAlmacen)
        {
            int total = new MantenimientosDAO().ContarUbicaciones();
            string listaIdAlmacenes = "";

            if (!_licencia.ValidarUbicaciones(total))
            {
                return new Response
                {
                    HUBO_ERROR = true,
                    MENSAJE_ERROR = "Limite de ubicaciones alcanzado"
                };
            }

            if (ListidAlmacen != null)
            {
                for (int i = 0; ListidAlmacen.Count() > i; i++)
                {
                    listaIdAlmacenes = listaIdAlmacenes + "," + ListidAlmacen[i];
                }
            }

            return new MantenimientosDAO().CreateUbicacion(codUbicacion, dscUbicacion, listaIdAlmacenes);
        }
        public Response GetUbicacion(string CodUbicacion)
        {
            return new MantenimientosDAO().GetUbicacion(CodUbicacion);
        }
        public Response UpdateUbicacion(string codUbicacion, string DSCUbicacion, List<string> ListidAlmacen, bool activo)
        {
            string listaIdAlmacenes = "";
            if (ListidAlmacen != null)
            {
                for (int i = 0; ListidAlmacen.Count() > i; i++)
                {
                    listaIdAlmacenes = listaIdAlmacenes + "," + ListidAlmacen[i];
                }
            }
            return new MantenimientosDAO().UpdateUbicacion(codUbicacion, DSCUbicacion, listaIdAlmacenes, activo);
        }

        public List<ImportBE> ImportarUbicaciones(string xml)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);
            int limiteUbicaciones = _licencia.ObtenerLimiteUbicaciones();
            return new MantenimientosDAO().ImportarUbicaciones(xmlDoc,limiteUbicaciones);
        }

        #endregion

        #region Mant Productos
        public Response ListarProductos(int activo, string vchProducto, string start, string length, string order)
        {
            return new MantenimientosDAO().ListarProductos(activo, vchProducto, start, length, order);
        }
        public Response CreateProducto(string codProducto, string descProducto, int UM,decimal costo, int idMoneda)
        {
            int total = new MantenimientosDAO().ContarProductos();

            if (!_licencia.ValidarProductos(total))
            {
                return new Response
                {
                    HUBO_ERROR = true,
                    MENSAJE_ERROR = "Limite de productos alcanzado"
                };
            }

            return new MantenimientosDAO().CreateProducto(codProducto, descProducto, UM,costo, idMoneda);
        }


        /// <summary>       Elocuencia de productos se agregara Se añadira el tema de Costos y el codigo de la moneda
        
        public Response GetProducto(int id)
        {
            return new MantenimientosDAO().GetProducto(id);
        }
        public Response UpdateProducto(int id, string codProducto, string descProducto, string usuario, bool activo,decimal costo,int idMoneda, int UM)
        {
            return new MantenimientosDAO().UpdateProducto(id, codProducto, descProducto, usuario, activo,costo, idMoneda, UM);
        }

        public List<ImportBE> ImportarProductos(string xml)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);
            int limiteProductos = _licencia.ObtenerProductosLimites();
            return new MantenimientosDAO().ImportarProductos(xmlDoc,limiteProductos);
        }


        /// </summary>

        //actualiza por medio de un excel 
        public List<ImportBE> ActualizarCostos_x_Productos(string xml)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);
            return new MantenimientosDAO().ActualizarCostos_x_Productos(xmlDoc);
        }

        public Response GetAPIProductos()
        {
            return new MantenimientosDAO().GetAPIProductos();
        }

        public Response CargarProductosAPIExterna(List<ProductoBEAPI> ListProductos)
        {
            MantenimientosDAO dao = new MantenimientosDAO();
            Response responseFinal = new Response();
            int error = 0;
            List<ProductoBEAPI> ListProductosError = new List<ProductoBEAPI>();
            for (int i = 0; i < ListProductos.Count; i++)
            {
                responseFinal = dao.CargarProductosAPIExterna(ListProductos[i]);
                if (responseFinal.HUBO_ERROR == true)
                {
                    error++;
                    ListProductosError.Add(ListProductos[i]);
                }
            }
            if (error > 0)
            {
                responseFinal.HUBO_ERROR = true;
                responseFinal.MENSAJE_ERROR = "Hubo " + error.ToString() + " errores al registrar.";
                responseFinal.Entity = ListProductosError;
            }
            return responseFinal;
        }

        #endregion

        #region Unidad Medida
        public Response ListarUnidadMedida(string vchUnidadMedida, string start, string length, string order)
        {
            return new MantenimientosDAO().ListarUnidadMedida(vchUnidadMedida, start, length, order);
        }

        public Response CreateUnidadMedida(UnidadMedidaBE obj)
        {
            return new MantenimientosDAO().CreateUnidadMedida(obj);
        }

        public Response GetUnidadMedida(int id)
        {
            return new MantenimientosDAO().GetUnidadMedida(id);
        }
        public Response UpdateUnidadMedida(UnidadMedidaBE obj)
        {
            return new MantenimientosDAO().UpdateUnidadMedida(obj);
        }

        public List<UMEXCELBE> ListarUnidadMedidaEXCEL()
        {
            return new MantenimientosDAO().ListarUnidadMedidaEXCEL();
        }
        #endregion

        #region Tipo de moneda
        public List<TipoMonedaBE> ListarTipoMonedas(string codMoneda , string dscMoneda, string activo)
        {
               return new MantenimientosDAO().ListarTipoMonedas(codMoneda , dscMoneda, activo);
        }

        public Response ActivarInactivarTipoMoneda(int idMoneda, bool flgActivo)
        {
            return new MantenimientosDAO().ActivarInactivarTipoMoneda(idMoneda, flgActivo);
        }

        public Response InsertarTipoMoneda(string codMoneda, string dscMoneda)
        {
            return new MantenimientosDAO().InsertarTipoMoneda(codMoneda, dscMoneda);
        }

        public Response EditarTipoMoneda(int idMoneda, string dscMoneda, bool flgActivo)
        {
            return new MantenimientosDAO().EditarTipoMoneda(idMoneda, dscMoneda, flgActivo);
        }

        public Response ObtenerTipoMoneda(int idMoneda)
        {
            return new MantenimientosDAO().ObtenerTipoMoneda(idMoneda);
        }


        public List<MonedaEXCELBE> ListarMonedaEXCEL()
        {
            return new MantenimientosDAO().ListarMonedaEXCEL();
        }


        #endregion

        #region Importación Maestro
        public List<ImportBE> ImportarAlmacen_Maestro(string xml)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);
            return new MantenimientosDAO().ImportarAlmacen_Maestro(xmlDoc);
        }

        public List<ImportBE> ImportarUsuarios_Maestro(string xml, string UserReg)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);
            return new MantenimientosDAO().ImportarUsuarios_Maestro(xmlDoc, UserReg);
        }
        public List<ImportBE> ImportarUsuariosXAlmacen_Maestro(string xml)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);
            return new MantenimientosDAO().ImportarUsuariosXAlmacen_Maestro(xmlDoc);
        }
        public List<ImportBE> ImportarUbicacion_Maestro(string xml)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);
            return new MantenimientosDAO().ImportarUbicacion_Maestro(xmlDoc);
        }
        #endregion

        #region Importación Inventario
        public List<ImportBE> ImportarInventario_Inventario(string xml, string UserReg)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);
            return new MantenimientosDAO().ImportarInventario_Inventario(xmlDoc, UserReg);
        }

        public List<ImportBE> ImportarDetInventario_Inventario(string xml, string UserReg, int pass)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);
            return new MantenimientosDAO().ImportarDetInventario_Inventario(xmlDoc, UserReg, pass);
        }

        public Response Select_DET_INV_IMPORT(string start, string length, string order, string search)
        {
            return new MantenimientosDAO().Select_DET_INV_IMPORT(start, length, order, search);
        }
        public void Insert_ASF_DETALLE_INVENTARIO(string UserReg)
        {
            new MantenimientosDAO().Insert_ASF_DETALLE_INVENTARIO(UserReg);
        }

        #endregion
    }
}
