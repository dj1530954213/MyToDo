using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MyToDo.Shared.Contact;
using Newtonsoft.Json;
using RestSharp;

namespace MyToDo.Service
{
    public class HttpRestClient
    {
        private readonly string _apiUrl;
        protected  RestClient client;

        public HttpRestClient(string apiUrl)
        {
            _apiUrl = apiUrl;
            client = new RestClient();
        }

        public async Task<ApiResponse> ExcuteAsync(BaseRequest baseRequest)
        {
            var request = new RestRequest(){Method = baseRequest.Method};
            request.AddHeader("Content-Tpye", baseRequest.ContentType);
            if (baseRequest.Parameter != null)
            {
                string body = JsonConvert.SerializeObject(baseRequest.Parameter);
                request.AddStringBody(body, ContentType.Json);
            }
            client = new RestClient(new RestClientOptions(new Uri(_apiUrl + baseRequest.Route)));
            var response = await client.ExecuteAsync(request);

            return JsonConvert.DeserializeObject<ApiResponse>(response.Content);
        }

        public async Task<ApiResponse<T>> ExcuteAsync<T>(BaseRequest baseRequest)
        {
            var request = new RestRequest() { Method = baseRequest.Method };
            request.AddHeader("Content-Tpye", baseRequest.ContentType);
            if (baseRequest.Parameter != null)
            {
                string body = JsonConvert.SerializeObject(baseRequest.Parameter);
                request.AddStringBody(body, ContentType.Json);
            }
            client = new RestClient(new RestClientOptions(new Uri(_apiUrl + baseRequest.Route)));
            var response = await client.ExecuteAsync(request);

            return JsonConvert.DeserializeObject<ApiResponse<T>>(response.Content);
        }
    }
}
