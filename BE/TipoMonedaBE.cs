using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE
{
    // ZEUS
    public class TipoMonedaBE
    {
        public int idMoneda { get; set; }
        public string codMoneda { get; set; }
        public string dscMoneda { get; set; }
        public bool flgActivo { get; set; }
    }

    public class MonedaEXCELBE
    {
        public string codMoneda { get; set; }
        public string dscMoneda { get; set; }
    }
}
