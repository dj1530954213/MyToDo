using Microsoft.AspNetCore.Mvc;
using MyToDo.Shared.Dtos;
using MyToDo.Shared.Parameters;
using MyToDoWeb.Context;
using MyToDoWeb.Context.UnitOfWork;
using MyToDoWeb.Service;

namespace MyToDoWeb.Controllers
{
    //标注这个类为API控制类且其路由如下
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class MemoController : ControllerBase
    {
        private readonly IMemoService _toDoService;

        public MemoController(IMemoService toDoService)
        {
            _toDoService = toDoService;
        }
        //标注这为一个get请求
        /*第一步构建一个IBaseService来约定数据库操作的接口
         *第二步ITodoService继承IBaseService
         *第三部实现ToDoService，在对象实例化的时候就需要传入继承对应接口的对象（在最顶层调用的时候）
         *第四部直接调用ToDoService内部标准的异步方法并返回结果
         */
        [HttpGet]
        public async Task<ApiResponse> Get(int id) => await _toDoService.GetSingleAsync(id);
        [HttpGet]
        public async Task<ApiResponse> GetAll([FromQuery] QueryParameter parameter) => await _toDoService.GetAllAsync(parameter);
        [HttpPost]
        public async Task<ApiResponse> Add([FromBody]MemoDto model) => await _toDoService.AddAsync(model);
        [HttpPost]
        public async Task<ApiResponse> Update([FromBody] MemoDto model) => await _toDoService.UpdateAsync(model);
        [HttpDelete]
        public async Task<ApiResponse> Delete(int id) => await _toDoService.DeleteAsync(id);
    }
}
