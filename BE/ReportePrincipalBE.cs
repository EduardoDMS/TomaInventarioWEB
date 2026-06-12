using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE
{


        public class TblReportePrincipalBE
        {
            public string Cod_Empresa { get; set; }
            public string Dsc_Empresa { get; set; }
            public string Cod_Almacen { get; set; }
            public string Dsc_Almacen { get; set; }
            public string Cod_Inventario { get; set; }
            public int Id_producto { get; set; }
            public string Cod_Producto { get; set; }
            public string Dsc_Producto { get; set; }
            public string Lote_Producto { get; set; }
            public string Serie_Producto { get; set; }
            //public int Stock_inicial { get; set; }

            public decimal Stock_inicial { get; set; }
            public decimal Conteo_1 { get; set; }
            public decimal Conteo_2 { get; set; }
            public decimal Conteo_3 { get; set; }
            public decimal Stock_Final { get; set; }
            public decimal Stock_Diferencial { get; set; }

            public decimal Id_ubicacion { get; set; }
            public string Fch_inicio { get; set; }
            public string Fch_fin { get; set; }
            public string Cod_ubicacion { get; set; }
            public string Dsc_ubicacion { get; set; }
            public string FLG_ESNUEVO { get; set; }
            //IQFARMA
            //public string DSC_ANALISIS { get; set; }
            //public string DSC_OBSERVACION { get; set; }
            //public string DSC_BALANZA { get; set; }

            //REPORTE PRINCIPAL
            public int Ultimo_Conteo { get; set; }
            public string Estado_Ajuste { get; set; }
            public string Cod_Usuario_Registro { get; set; }
            public string Fch_Registro_Inventario { get; set; }
            public string Fch_Cierre { get; set; }
            public string DSC_ANALISIS { get; set; }
            public string DSC_OBSERVACION { get; set; }
            public string ESTADO_DESCRIPCION { get; set; }


            //public string NombreAlmacen { get; set; }
            //public string FechaInicio { get; set; }
            //public string FechaCierre { get; set; }

        }



}
