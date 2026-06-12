using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

public class ChatController : Controller
{
    private static readonly HttpClient client = new HttpClient();

    [GenerateNonce]
    public class ChatRequest
    {
        public string Message { get; set; }
        public string SessionId { get; set; }
    }

    public class ChatResponse
    {
        public string sessionId { get; set; }
        public string response { get; set; }
        public string emotion { get; set; }
    }

    [HttpPost]
    public async Task<ActionResult> EnviarMensaje(ChatRequest request)
    {
        var apiUrl = "https://neocortex.link/api/v2/chat";
        var apiKey = "sk_3bc6d8ca-ebe9-42db-885e-445fd4873bee";

        var body = new Dictionary<string, object>
        {
            { "characterId", "cmkbbt0470001ky04j2sge0py" },
            { "message", request.Message }
        };

        if (!string.IsNullOrEmpty(request.SessionId))
        {
            body.Add("sessionId", request.SessionId);
        }


        var json = JsonConvert.SerializeObject(body);

        //System.Diagnostics.Debug.WriteLine(json);

        var httpRequest = new HttpRequestMessage(HttpMethod.Post, apiUrl);
        httpRequest.Headers.Add("x-api-key", apiKey);
        httpRequest.Content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await client.SendAsync(httpRequest);
        var result = await response.Content.ReadAsStringAsync();

        //System.Diagnostics.Debug.WriteLine(result);

        return Content(result, "application/json");
    }

    [HttpPost]
    public async Task<ActionResult> ObtenerHistorial(string sessionId)
    {
        var apiUrl = "https://neocortex.link/api/v2/chat/session";
        var apiKey = "sk_3bc6d8ca-ebe9-42db-885e-445fd4873bee";

        var body = new
        {
            sessionId = sessionId,
            limit = 10
        };

        var json = JsonConvert.SerializeObject(body);

        var request = new HttpRequestMessage(HttpMethod.Post, apiUrl);
        request.Headers.Add("x-api-key", apiKey);
        request.Content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await client.SendAsync(request);
        var result = await response.Content.ReadAsStringAsync();

        return Content(result, "application/json");
    }
}