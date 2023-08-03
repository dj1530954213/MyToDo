using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyToDo.Shared.Contact;
using MyToDo.Shared.Parameters;
using ApiResponse = MyToDo.Shared.Contact.ApiResponse;

namespace MyToDo.Service
{
    public class BaseService<TEntity>:IBaseService<TEntity> where TEntity : class
    {
        private readonly HttpRestClient _client;
        private readonly string _serviceName;

        public BaseService(HttpRestClient client,string serviceName)
        {
            _client = client;
            _serviceName = serviceName;
        }

        public async Task<ApiResponse<TEntity>> AddAsync(TEntity entity)
        {
            BaseRequest request = new BaseRequest();
            request.Method = RestSharp.Method.Post;
            request.Route = $"api/{_serviceName}/Add";
            request.Parameter = entity;
            return await _client.ExcuteAsync<TEntity>(request);
        }

        public async Task<ApiResponse<TEntity>> UpdateAsync(TEntity entity)
        {
            BaseRequest request = new BaseRequest();
            request.Method = RestSharp.Method.Post;
            request.Route = $"api/{_serviceName}/Update";
            request.Parameter = entity;
            return await _client.ExcuteAsync<TEntity>(request);
        }

        public async Task<ApiResponse> DeleteAsync(int id)
        {
            BaseRequest request = new BaseRequest();
            request.Method = RestSharp.Method.Delete;
            request.Route = $"api/{_serviceName}/Delete?id={id}";
            return await _client.ExcuteAsync(request);
        }

        public async Task<ApiResponse<TEntity>> GetFirstOfDefaultAsync(int id)
        {
            BaseRequest request = new BaseRequest();
            request.Method = RestSharp.Method.Get;
            request.Route = $"api/{_serviceName}/Get?id={id}";
            return await _client.ExcuteAsync<TEntity>(request);
        }

        public async Task<ApiResponse<PagedList<TEntity>>> GetAllAsync(QueryParameter parameter)
        {
            BaseRequest request = new BaseRequest();
            request.Method = RestSharp.Method.Get;
            request.Route = $"api/{_serviceName}/GetAll?pageIndex={parameter.PageIndex}" +
                            $"&pageSize={parameter.PageSize}" +
                            $"&search={parameter.Search}";
            return await _client.ExcuteAsync<PagedList<TEntity>>(request);
        }
    }
}
