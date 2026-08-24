using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE
{
    public class EstadoSesionBE
    {
        public bool FlgOnline {  get; set; }
        public Guid? TokenSesion {  get; set; }
    }
}
