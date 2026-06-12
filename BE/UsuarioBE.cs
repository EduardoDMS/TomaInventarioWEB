using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE
{
    public class UsuarioLoginBE
    {
        public int IdUsuario { get; set; }
        public string Usuario { get; set; }
        public string Perfil { get; set; }
        public bool Activo { get; set; }
    }
    public class UsuarioBE
    {
        public int IdUsuario { get; set; }
        public string Usuario { get; set; }
        public string Contraseña { get; set; }
        public string Perfil { get; set; }
        public int Activo { get; set; }
        public string vchActivo { get; set; }
        public int IdAlmacen { get; set; }
        public List<string> ListvchIdAlmacen { get; set; }
    }
}
