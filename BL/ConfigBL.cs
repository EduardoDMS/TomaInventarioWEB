using BE;
using DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    public class ConfigBL
    {
        public Response GetCorreo()
        {
            return new ConfigDAO().GetCorreo();
        }
        public Response GetEmpresa()
        {
            return new ConfigDAO().GetEmpresa();
        }
        public Response SaveEmpresa(EmpresaBE empresa)
        {
            return new ConfigDAO().SaveEmpresa(empresa);
        }
        public Response SaveCorreo(CorreoBE correo)
        {
            return new ConfigDAO().SaveCorreo(correo);
        }
    }
}
