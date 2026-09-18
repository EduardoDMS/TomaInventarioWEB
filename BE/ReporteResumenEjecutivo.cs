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
        public decimal StockInicial { get; set; }
        public decimal StockFinal { get; set; }
        public decimal Diferencia { get; set; }
    }

    public class UsuarioParticipacionDto
    {
        public string CodUsuario { get; set; }
        public string NombreCompleto { get; set; }
        public int ProductosLecturados { get; set; }
    }

    // NUEVO AÑADIDO
    public class ProductoFueraUbicacionDto
    {
        public string CodProducto { get; set; }
        public string DscProducto { get; set; }

        public string UbicacionInicial { get; set; }
        public string UbicacionContada { get; set; }

        //public decimal Cantidad { get; set; }
    }

    public class FueraUbicacionResultDao
    {
        public int Total { get; set; }
        public List<ProductoFueraUbicacionDto> Productos { get; set; } = new List<ProductoFueraUbicacionDto>();
    }
}