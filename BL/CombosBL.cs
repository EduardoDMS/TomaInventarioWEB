using BE;
using DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    public class CombosBL
    {
        public Response cbxAlmacenes() {
            return new CombosDAO().cbxAlmacenes();
        }
        public Response cbxInventario()
        {
            return new CombosDAO().cbxInventario();
        }
        public Response cbxUbicacion(int id_almacen)
        {
            return new CombosDAO().cbxUbicacion(id_almacen);
        }
        public Response cbxProducto(string dsc_prod)
        {
            return new CombosDAO().cbxProducto(dsc_prod);
        }
        public Response cbxUM()
        {
            return new CombosDAO().cbxUM();
        }
        public Response cbxInventariosCerrados()
        {
            return new CombosDAO().cbxInventariosCerrados();
        }
    }
}
