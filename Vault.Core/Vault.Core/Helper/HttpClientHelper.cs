using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Vault.Core.Helper
{
    internal class HttpClientHelper
    {
        public static HttpResponseMessage GetResult(string requestUrl, Dictionary<string, string> httpHeader = null)
        {
            return GetAsync(requestUrl, httpHeader).Result;
        }

        public static async Task<HttpResponseMessage> GetAsync(string requestUrl, IDictionary<string, string> headers)
        {
            using var client = PrepareHttpClient(headers);

            try
            {
                return await client.GetAsync(requestUrl);
            }
            catch (Exception e)
            {
                var msg = $"Exception occurred inside GetAsync: {e.Message}";
                    
                return new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent(e.Message),
                    ReasonPhrase = e.Message
                };
            }
        }

        public static async Task<HttpResponseMessage> PostAsync(string requestUrl, IDictionary<string, string> headers, string payload)
        {
            using var client = PrepareHttpClient(headers);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            using var content = new StringContent(payload ?? string.Empty, Encoding.UTF8, "application/json");
            
            try
            {
                return await client.PostAsync(requestUrl, content);
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent(ex.Message),
                    ReasonPhrase = ex.Message
                };
            }
        }

        public static async Task<HttpResponseMessage> DeleteAsync(string requestUrl, IDictionary<string, string> headers)
        {
            using var client = PrepareHttpClient(headers);
            
            try
            {
                return await client.DeleteAsync(requestUrl);
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent(ex.Message),
                    ReasonPhrase = ex.Message
                };
            }
        }

        private static HttpClient PrepareHttpClient(IDictionary<string, string> headers)
        {
            var handler = new HttpClientHandler
            {
                UseDefaultCredentials = false
            };

            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            var client = new HttpClient(handler);
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            client.DefaultRequestHeaders.ExpectContinue = false;

            if (headers == null || !headers.Any()) return client;

            foreach (var header in headers)
            {
                client.DefaultRequestHeaders.Add(header.Key, header.Value);
            }

            return client;
        }
    }
}
