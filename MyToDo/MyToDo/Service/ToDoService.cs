using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyToDo.Shared.Contact;
using MyToDo.Shared.Dtos;
using MyToDo.Shared.Parameters;

namespace MyToDo.Service
{
    public class ToDoService:BaseService<ToDoDto>,IToDoService
    {
        private readonly HttpRestClient _client;
        //public ToDoService(HttpRestClient client, string serviceName) : base(client,"ToDo")
        //{

        //}
        public ToDoService(HttpRestClient client) : base(client,"ToDo")
        {
            _client = client;
        }

        public async Task<ApiResponse<PagedList<ToDoDto>>> GetAllFilterAsync(ToDoParameter toDoParameter)
        {
            BaseRequest request = new BaseRequest();
            request.Method = RestSharp.Method.Get;
            request.Route = $"api/ToDo/GetAll?pageIndex={toDoParameter.PageIndex}" +
                            $"&pageSize={toDoParameter.PageSize}" +
                            $"&search={toDoParameter.Search}"+
                            $"&status={toDoParameter.Status}";
            return await _client.ExcuteAsync<PagedList<ToDoDto>>(request);
        }

        public async Task<ApiResponse<SummaryDto>> SummaryAsync()
        {
            BaseRequest requset = new BaseRequest();
            requset.Method = RestSharp.Method.Get;
            requset.Route = "api/ToDo/Summary";
            return await _client.ExcuteAsync<SummaryDto>(requset);
        }
    }
}
