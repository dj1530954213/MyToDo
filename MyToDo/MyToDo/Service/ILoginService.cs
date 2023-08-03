using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyToDo.Shared.Contact;
using MyToDo.Shared.Dtos;

namespace MyToDo.Service
{
    public interface ILoginService
    {
        Task<ApiResponse<UserDto>> LoginAsync(UserDto userDto);
        Task<ApiResponse> RegisterAsync(UserDto userDto);
    }
}
