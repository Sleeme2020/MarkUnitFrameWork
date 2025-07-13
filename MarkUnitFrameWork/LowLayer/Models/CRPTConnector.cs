using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MarkUnitFrameWork.LowLayer.Models
{
    internal class CRPTConnector
    {
        
        string Host;
        IDictionary<string, string> headers;
        public CRPTConnector(IDictionary<string,string> Headers, string Host)
        {
            this.Host = Host;
            headers = Headers;
        }
       

        public Response SenderPost<Response>(string path)
        {
            using (var http = new HttpClient())
            {
                var request = new HttpRequestMessage(HttpMethod.Get, Host + path);
                foreach (var header in headers)
                {
                    request.Headers.Add(header.Key, header.Value);
                }
                var response = http.SendAsync(request);
                Task.WaitAll(response);
                if (response.Result.IsSuccessStatusCode)
                {
                    var responseContent = response.Result.Content.ReadAsStringAsync();
                    responseContent.Wait();

                    return JsonConvert.DeserializeObject<Response>(responseContent.Result.ToString());
                }
                else
                {
                    throw new Exception("Error in request");
                }
            }
        }

        public Response SenderPost<Response,Request>(Request request,string path)
        {
            using (var http = new HttpClient())
            {
                var requestString = JsonConvert.SerializeObject(request);
                
                var content = new StringContent(requestString, Encoding.UTF8, "application/json");
                foreach (var header in headers)
                {
                    content.Headers.Add(header.Key, header.Value);
                }                
                var response = http.PostAsync(Host+ path, content);
                Task.WaitAll(response);
                if (response.Result.IsSuccessStatusCode)
                {
                    var responseContent = response.Result.Content.ReadAsStringAsync();
                    responseContent.Wait();
                    
                    return JsonConvert.DeserializeObject<Response>(responseContent.Result.ToString());
                }
                else
                {
                    throw new Exception("Error in request");
                }
            }
        }

        public Response SenderGet<Response>(string path)
        {
            using (var http = new HttpClient())
            {
                var request = new HttpRequestMessage(HttpMethod.Get, Host + path);

                foreach (var header in headers)
                {
                    //request.Headers.Add(header.Key, header.Value);
                    http.DefaultRequestHeaders.Add(header.Key, header.Value);
                }
                var response = http.SendAsync(request);
                response.Wait();
                if(response.Result.IsSuccessStatusCode)
                {
                    var responseContent = response.Result.Content.ReadAsStringAsync();
                    responseContent.Wait();
                    return JsonConvert.DeserializeObject<Response>(responseContent.Result.ToString());
                }
                else
                {
                    throw new Exception("Error in request");
                }
            }
        }


    }
}
