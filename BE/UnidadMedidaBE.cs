using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE
{
    public class UnidadMedidaBE
    {
        public int IntUniMed { get; set; }
        public string vchCodUniMed { get; set; }
        public string vchDesUniMed { get; set; } 
        public int IntEstado { get; set; }
        public string vchEstado { get; set; }
        public bool bitCantidad { get; set; }
        public int IntCantidad { get; set; }

    }

    public class UMEXCELBE
    {
        public string vchCodUniMed { get; set; }
        public string vchDesUniMed { get; set; }
        public int IntCantidad { get; set; }

    }

}
