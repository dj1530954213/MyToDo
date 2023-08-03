using MyToDo.Shared.Contact;
using MyToDo.Shared.Dtos;
using MyToDo.Shared.Parameters;
using MyToDoWeb.Context;

namespace MyToDoWeb.Service
{
    public interface IToDoService:IBaseService<ToDoDto>
    {
        Task<ApiResponse> GetAllAsync(ToDoParameter toDoParameter);
        Task<ApiResponse> Summary();
    }
}
