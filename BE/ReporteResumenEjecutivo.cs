using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//clases dto
namespace BE
{
    public class ProductoDiferenciaDto
    {
        public string CodProducto { get; set; }
        public string DscProducto { get; set; }
        public decimal Diferencia { get; set; }
    }

    public class UbicacionDiferenciaDto
    {
        public string Codigo { get; set; }
        public string Ubicacion { get; set; }
        public decimal Diferencia { get; set; }
    }

    public class UsuarioParticipacionDto
    {
        public string CodUsuario { get; set; }
        public string NombreCompleto { get; set; }
        public int CantLecturas { get; set; }
    }
}