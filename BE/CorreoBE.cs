using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE
{
    public class CorreoBE
    {
        public string SERVIDOR { get; set; }
        public string PUERTO { get; set; }
        public string REMITENTE { get; set; }
        public string PRIORIDAD { get; set; }
        public int FLG_HABILITAR { get; set; }
        public int FLG_AUTENTICACION { get; set; }
        public string USUARIO { get; set; }
        public string CONTRASEÑA { get; set; }
    }
}
