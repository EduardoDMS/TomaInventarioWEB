using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE
{
    public class ProductoBE
    {
        public int idProducto { get; set; }
        public string vchCodProducto { get; set; }
        public string vchDescripcion { get; set; }
        public string vchActivo { get; set; }
        public int intActivo { get; set; }
        public int intUM { get; set; }
        public string vchCodUniMed { get; set; } // agregado, si ocurre un error, eliminar esta propiedad y usar intUM

        //AGREGADO _ ZEUS
        public decimal precioCosto { get; set; }


        public int idMoneda { get; set; }
        public string codMoneda { get; set; }
    }
    public class ProductoBEAPI
    {
        public string PRODUCTO_CODIGO { get; set; }
        public string PRODUCTO_NOMBRE { get; set; }
        public string UNIDADMEDIDA_CODIGO { get; set; }
    }

}
