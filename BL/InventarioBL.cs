using BE;
using DAO;
using System.Collections.Generic;
using System.Xml;
using TomaInventario.BL.Licencias;

namespace BL
{
    public class InventarioBL
    {
        private readonly LicenciaService _licencia;

        public InventarioBL()
        {
            _licencia = new LicenciaService(new LicenciaDbProvider());//newbdProvider
        }

        public Response ListarInventario(string COD_INVENTARIO, int NRO_CONTEO_1, int NRO_CONTEO_2, int NRO_CONTEO_3, string start, string legnth, string order, string search)
        {
            return new InventarioDAO().ListarInventario(COD_INVENTARIO, NRO_CONTEO_1, NRO_CONTEO_2, NRO_CONTEO_3, start, legnth, order, search);
        }

        public Response GetInventario(string idEmpresa, int Almacen, string CodInventario, string CodEstado, string flg_filtroFecha, string fch_inicio, string fch_fin)
        {
            return new InventarioDAO().GetInventario(idEmpresa, Almacen, CodInventario, CodEstado, flg_filtroFecha, fch_inicio, fch_fin);
        }

        public Response CerrarIventario(string codInventario, int conteo)
        {
            return new InventarioDAO().CerrarIventario(codInventario, conteo);
        }

        public Response ConteoDiferencial(string codInventario, int conteo)
        {
            return new InventarioDAO().ConteoDiferencial(codInventario, conteo);
        }

        public Response ConteoReinicio(string codInventario, int conteo)
        {
            return new InventarioDAO().ConteoReinicio(codInventario, conteo);
        }

        public List<UsuarioBE> ListarUsuariosAsociados(string dscAlmacen)
        {
            return new InventarioDAO().ListarUsuariosAsociados(dscAlmacen);
        }

        //public DataTable ExportInventario(string COD_INVENTARIO, int NRO_CONTEO_1, int NRO_CONTEO_2, int NRO_CONTEO_3)
        //{
        //    _ = new DataTable();
        //    DataTable dt = new InventarioDAO().ExportInventario(COD_INVENTARIO, NRO_CONTEO_1, NRO_CONTEO_2, NRO_CONTEO_3).Tables[0];
        //    return dt;
        //}

        public Response ExportInventario(string COD_INVENTARIO, int NRO_CONTEO_1, int NRO_CONTEO_2, int NRO_CONTEO_3)
        {
            return new InventarioDAO().ExportInventario(COD_INVENTARIO, NRO_CONTEO_1, NRO_CONTEO_2, NRO_CONTEO_3);
        }

        public Response DrawnBarChart(string CodInventario, int tipoGrafico)
        {
            return new InventarioDAO().DrawnBarChart(CodInventario, tipoGrafico);
        }

        public Response DrawnBarChart2(string CodInventario)
        {
            return new InventarioDAO().DrawnBarChart2(CodInventario);
        }

        public Response ReportesInventario_WEB(int tipoReporte, string Cod_inventario, string DatoFiltro, string NConteo, string Usuario,
            string start, string length, string order, string search)
        {
            return new InventarioDAO().ReportesInventario_WEB(tipoReporte, Cod_inventario, DatoFiltro, NConteo, Usuario, start, length, order, search);
        }

        public Response ReportesInventario(int tipoReporte, string Cod_inventario, string DatoFiltro, string NConteo, string Usuario)
        {
            return new InventarioDAO().ReportesInventario(tipoReporte, Cod_inventario, DatoFiltro, NConteo, Usuario);
        }

        public Response ValidarDatosInventario(string Cod_Inv, int id_almacen)
        {
            return new InventarioDAO().ValidarDatosInventario(Cod_Inv, id_almacen);
        }

        public Response InsertInv_InvDetalle(string xmlData, string UserReg, string CodInv, int Id_Almacen)
        {
            int total = new InventarioDAO().ContarInventarios();

            if (!_licencia.ValidarInventariosPreparados(total))
            {
                return new Response
                {
                    HUBO_ERROR = true,
                    MENSAJE_ERROR = "Limite de inventarios preparados alcanzado"
                };
            }

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlData);
            return new InventarioDAO().InsertInv_InvDetalle(xmlDoc, UserReg, CodInv, Id_Almacen);
        }

        public List<DetInventarioImportBE> ImportarDetalles(string xml, int IdAlmacen)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);
            return new InventarioDAO().ImportarDetalles(xmlDoc, IdAlmacen);
        }

        public Response GetAPI_StockALM()
        {
            return new InventarioDAO().GetAPI_StockALM();
        }

        public int? CLI_SP_CREAR_INVENTARIO(string COD_ALMACEN, out bool huboError, out string mensaje)
        {
            return new InventarioDAO().CLI_SP_CREAR_INVENTARIO(COD_ALMACEN, out huboError, out mensaje);
        }

        public void ImportarDetInventario_Inventario(string xml, string UserReg, int IDInventario)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);
            var dao = new InventarioDAO();
            dao.ImportarDetInventario_Movil(xmlDoc, UserReg, IDInventario);
        }

        public void Insert_ASF_DETALLE_INVENTARIO(string UserReg)
        {
            var dao = new InventarioDAO();
            dao.Insert_ASF_DETALLE_INVENTARIO(UserReg);
        }
    }
}
