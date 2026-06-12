using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TomaInventarioWEB.Assets.Clases
{
    public class CspNonceHeaderModule : IHttpModule
    {
        public void Init(HttpApplication context)
        {
            context.PreSendRequestHeaders += OnPreSendRequestHeaders;
        }

        public void Dispose() { }

        void OnPreSendRequestHeaders(object sender, EventArgs e)
        {
            HttpApplication app = (HttpApplication)sender;
            string nonce = (string)app.Context.Items["ScriptNonce"];

            string cspValue;
            if (!string.IsNullOrEmpty(nonce))
            {
                cspValue = $"default-src 'self'; script-src 'nonce-{nonce}' 'self' https://cdn.datatables.net; style-src 'self' 'unsafe-inline'; style-src-attr 'unsafe-inline'; img-src 'self' data:; connect-src 'self' http://190.187.232.45 http://172.16.3.12/WsInventarioAndroid https://neocortex.link; frame-ancestors 'none'; form-action 'self'; font-src 'self'; media-src 'self'; object-src 'none'; manifest-src 'self'; worker-src 'self'; frame-src 'self' https://www.youtube-nocookie.com https://www.youtube.com;";
            }
            else
            {
                cspValue = $"default-src 'self'; script-src 'self' https://cdn.datatables.net; style-src 'self'; style-src-attr 'unsafe-inline'; img-src 'self' data:; connect-src 'self' http://190.187.232.45 http://172.16.3.12/WsInventarioAndroid https://neocortex.link; frame-ancestors 'none'; form-action 'self'; font-src 'self'; media-src 'self'; object-src 'none'; manifest-src 'self'; worker-src 'self'; frame-src 'self' https://www.youtube-nocookie.com https://www.youtube.com;";
            }

            app.Response.Headers.Set("Content-Security-Policy", cspValue);
        }
    }
}