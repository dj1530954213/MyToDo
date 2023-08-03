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
    public class LoginController : ControllerBase
    {
        private readonly ILoginService _loginService;

        public LoginController(ILoginService loginService)
        {
            _loginService = loginService;
        }

        //标注这为一个get请求
        /*第一步构建一个IBaseService来约定数据库操作的接口
         *第二步ITodoService继承IBaseService
         *第三部实现ToDoService，在对象实例化的时候就需要传入继承对应接口的对象（在最顶层调用的时候）
         *第四部直接调用ToDoService内部标准的异步方法并返回结果
         */
        [HttpPost]
        public async Task<ApiResponse> Login([FromBody] UserDto param) =>await _loginService.LoginAsync(param.Account, param.PassWord);

        [HttpPost]
        public async Task<ApiResponse> Register([FromBody] UserDto user) => await _loginService.RegisterAsync(user);
    }
}
