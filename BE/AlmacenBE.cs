using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE
{
    public class AlmacenBE
    {
        public int idAlmacen { get; set; }
        public string vchcodAlmacen { get; set; }
        public string vchdscAlmacen { get; set; }
        public string vchActivo { get; set; }
        public int intActivo { get; set; }
        public string vchTipoAlmacen { get; set; }
    }

    public class AlmacenAPI
    {
        public int idAlmacen { get; set; }
        public string ALMACEN_CODIGO { get; set; }
        public string ALMACEN_NOMBRE { get; set; }
        public string UBICACION_CODIGO { get; set; }
        public string UBICACION_NOMBRE { get; set; }
    }

}
