using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TomaInventario.BL.Licencias;

namespace BL.Licencias
{
    public class LicenciaInitializer
    {
        public void Inicializar()
        {
            var jsonProvider = new LicenciaJsonProvider();

            LicenciaConfig licencia = jsonProvider.Obtener();

            new LicenciaBL().InicializarLicencia(licencia);
        }
    }
}
