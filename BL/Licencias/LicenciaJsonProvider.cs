using Newtonsoft.Json;
using System.IO;
using System.Web.Hosting;

namespace TomaInventario.BL.Licencias
{
    public class LicenciaJsonProvider : ILicenciaProvider
    {
        public LicenciaConfig Obtener()
        {
            string ruta = HostingEnvironment.MapPath("~/licencia.json");

            if (!File.Exists(ruta))
            {
                throw new System.Exception("No se encontro el archivo licencia.json");
            }

            string json = File.ReadAllText(ruta);

            return JsonConvert.DeserializeObject<LicenciaConfig>(json);
        }
    }
}