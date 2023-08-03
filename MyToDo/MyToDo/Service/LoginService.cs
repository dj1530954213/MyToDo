using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyToDo.Shared.Contact;
using MyToDo.Shared.Dtos;
using RestSharp;

namespace MyToDo.Service
{
    public class LoginService:ILoginService
    {
        private readonly HttpRestClient _client;
        private readonly string _serviceName = "Login";
        public LoginService(HttpRestClient client)
        {
            _client = client;
        }

        public async Task<ApiResponse<UserDto>> LoginAsync(UserDto userDto)
        {
            BaseRequest request = new BaseRequest();
            request.Method = Method.Post;
            request.Route = $"api/{_serviceName}/Login";
            request.Parameter = userDto;
            return await _client.ExcuteAsync<UserDto>(request);
        }

        public async Task<ApiResponse> RegisterAsync(UserDto userDto)
        {
            BaseRequest request = new BaseRequest();
            request.Method = Method.Post;
            request.Route = $"api/{_serviceName}/Register";
            request.Parameter = userDto;
            return await _client.ExcuteAsync(request);
        }
    }
}
