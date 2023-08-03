using MyToDo.Shared.Dtos;

namespace MyToDoWeb.Service
{
    public interface ILoginService
    {
        Task<ApiResponse> LoginAsync(string account, string password);

        Task<ApiResponse> RegisterAsync(UserDto user);
    }
}
