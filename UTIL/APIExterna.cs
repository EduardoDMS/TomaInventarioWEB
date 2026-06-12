using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace UTIL
{
    public class APIExterna 
    {
        public string ObtenerALMACENES_IQFARMA(string APIALmacen)
        {
            string apiUrl = APIALmacen;
            
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    // Establecer un timeout de 60 segundos
                    client.Timeout = TimeSpan.FromSeconds(60);

                    // Realizar la solicitud POST
                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, apiUrl);
                    HttpResponseMessage response = client.SendAsync(request).Result; // Realización síncrona

                    // Verificar si la solicitud fue exitosa
                    response.EnsureSuccessStatusCode();

                    // Leer y devolver el contenido de la respuesta
                    string responseBody = response.Content.ReadAsStringAsync().Result; // Lectura síncrona
                    return responseBody;
                }
                catch (HttpRequestException ex)
                {
                    // Manejar errores de solicitud HTTP
                    Console.WriteLine($"Error de solicitud HTTP: {ex.Message}");
                    return null;
                }
                catch (AggregateException ex)
                {
                    // Manejar excepciones de tareas canceladas (timeouts)
                    if (ex.InnerException is TaskCanceledException)
                    {
                        Console.WriteLine($"Error de timeout: {ex.InnerException.Message}");
                        return null;
                    }
                    else
                    {
                        // Manejar otros errores agregados
                        Console.WriteLine($"Ocurrió un error agregado: {ex.Message}");
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    // Manejar otros errores
                    Console.WriteLine($"Ocurrió un error: {ex.Message}");
                    return null;
                }
            }
        }

        
        public string ObtenerSTOCK_ALM_IQFARMA(string URLAPI, string almacenCodigo)
        {
            string apiUrl = URLAPI;

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    // Establecer un timeout de 60 segundos
                    client.Timeout = TimeSpan.FromSeconds(60);

                    // Crear el contenido de la solicitud
                    var content = new FormUrlEncodedContent(new[]
                    {
                new KeyValuePair<string, string>("Almacen_Codigo", almacenCodigo)
            });

                    // Realizar la solicitud POST con el contenido
                    HttpResponseMessage response = client.PostAsync(apiUrl, content).Result; // Realización síncrona

                    // Verificar si la solicitud fue exitosa
                    response.EnsureSuccessStatusCode();

                    // Leer y devolver el contenido de la respuesta
                    string responseBody = response.Content.ReadAsStringAsync().Result; // Lectura síncrona
                    return responseBody;
                }
                catch (HttpRequestException ex)
                {
                    // Manejar errores de solicitud HTTP
                    Console.WriteLine($"Error de solicitud HTTP: {ex.Message}");
                    return null;
                }
                catch (AggregateException ex)
                {
                    // Manejar excepciones de tareas canceladas (timeouts)
                    if (ex.InnerException is TaskCanceledException)
                    {
                        Console.WriteLine($"Error de timeout: {ex.InnerException.Message}");
                        return null;
                    }
                    else
                    {
                        // Manejar otros errores agregados
                        Console.WriteLine($"Ocurrió un error agregado: {ex.Message}");
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    // Manejar otros errores
                    Console.WriteLine($"Ocurrió un error: {ex.Message}");
                    return null;
                }
            }
        }


        public string ObtenerProductos(string APIALmacen)
        {
            string apiUrl = APIALmacen;

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    // Establecer un timeout de 60 segundos
                    client.Timeout = TimeSpan.FromSeconds(60);

                    // Realizar la solicitud POST
                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, apiUrl);
                    HttpResponseMessage response = client.SendAsync(request).Result; // Realización síncrona

                    // Verificar si la solicitud fue exitosa
                    response.EnsureSuccessStatusCode();

                    // Leer y devolver el contenido de la respuesta
                    string responseBody = response.Content.ReadAsStringAsync().Result; // Lectura síncrona
                    return responseBody;
                }
                catch (HttpRequestException ex)
                {
                    // Manejar errores de solicitud HTTP
                    Console.WriteLine($"Error de solicitud HTTP: {ex.Message}");
                    return null;
                }
                catch (AggregateException ex)
                {
                    // Manejar excepciones de tareas canceladas (timeouts)
                    if (ex.InnerException is TaskCanceledException)
                    {
                        Console.WriteLine($"Error de timeout: {ex.InnerException.Message}");
                        return null;
                    }
                    else
                    {
                        // Manejar otros errores agregados
                        Console.WriteLine($"Ocurrió un error agregado: {ex.Message}");
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    // Manejar otros errores
                    Console.WriteLine($"Ocurrió un error: {ex.Message}");
                    return null;
                }
            }
        }


        //public string ObtenerSTOCK_ALM_IQFARMA(string APIALmacen,string COD_ALM)
        //{
        //    string apiUrl = APIALmacen;

        //    using (HttpClient client = new HttpClient())
        //    {
        //        try
        //        {
        //            // Establecer un timeout de 60 segundos
        //            client.Timeout = TimeSpan.FromSeconds(60);

        //            // Realizar la solicitud POST
        //            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, apiUrl);
        //            HttpResponseMessage response = client.SendAsync(request).Result; // Realización síncrona

        //            // Verificar si la solicitud fue exitosa
        //            response.EnsureSuccessStatusCode();

        //            // Leer y devolver el contenido de la respuesta
        //            string responseBody = response.Content.ReadAsStringAsync().Result; // Lectura síncrona
        //            return responseBody;
        //        }
        //        catch (HttpRequestException ex)
        //        {
        //            // Manejar errores de solicitud HTTP
        //            Console.WriteLine($"Error de solicitud HTTP: {ex.Message}");
        //            return null;
        //        }
        //        catch (AggregateException ex)
        //        {
        //            // Manejar excepciones de tareas canceladas (timeouts)
        //            if (ex.InnerException is TaskCanceledException)
        //            {
        //                Console.WriteLine($"Error de timeout: {ex.InnerException.Message}");
        //                return null;
        //            }
        //            else
        //            {
        //                // Manejar otros errores agregados
        //                Console.WriteLine($"Ocurrió un error agregado: {ex.Message}");
        //                return null;
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            // Manejar otros errores
        //            Console.WriteLine($"Ocurrió un error: {ex.Message}");
        //            return null;
        //        }
        //    }
        //}
    }
}
