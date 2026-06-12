using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE
{
    public class UbicacionBE
    {
        public int intIdUbicacion { get; set; }
        public int intIdAlmacen { get; set; }
        public string vchCOD_Almacen { get; set; }
        public string vchDSC_Almacen { get; set; }
        public string vchCod_Ubicacion { get; set; }
        public string vchDSC_Ubicacion { get; set; }
        public int intActivo { get; set; }
        public string vchActivo { get; set; }
    }
}
