using AutoMapper;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using MyToDo.Shared.Dtos;
using MyToDoWeb.Context;

namespace MyToDoWeb.Extensions
{
    /// <summary>
    /// 引入AutoMapper的作用是完成从数据层的ToDo向更上层的ToDoDto的映射关系
    /// </summary>
    public class AutoMapperProfile:MapperConfigurationExpression
    {
        public AutoMapperProfile()
        {
            CreateMap<ToDo, ToDoDto>().ReverseMap();
            CreateMap<Memo, MemoDto>().ReverseMap();
            CreateMap<User, UserDto>().ReverseMap();
        }
    }
}
