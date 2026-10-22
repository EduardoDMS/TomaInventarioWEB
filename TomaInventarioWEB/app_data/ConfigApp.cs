using BL;
using System.Threading.Tasks;
public static class ConfigApp
{
    private static readonly object _lock = new object();
    private static bool? _mostrarLote;

    public static bool MostrarLote
    {
        get
        {
            if (_mostrarLote.HasValue) return _mostrarLote.Value;
            lock (_lock)
            {
                if (_mostrarLote.HasValue) return _mostrarLote.Value;

                try
                {
                    _mostrarLote = ConfigGlobalBL.MostrarLote;
                }
                catch
                {
                    return true;
                }
                return _mostrarLote.Value;
            }
        }


    }


    public static async Task SetMostrarLoteAsync(bool valor)
    {
        await ConfigGlobalBL.SetMostrarLoteAsync(valor); // si falla, lanza y no toca el caché
        lock (_lock) { _mostrarLote = valor; }
    }
}


