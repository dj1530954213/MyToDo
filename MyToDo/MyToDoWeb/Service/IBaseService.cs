using MyToDo.Shared.Parameters;

namespace MyToDoWeb.Service
{
    /// <summary>
    /// 规定了通用的数据库接口
    /// </summary>
    /// <typeparam name="T">需要进行操作的数据库对象的类型</typeparam>
    /// 当下游的子类进行继承的时候这里的T就自适应为对应的数据操作对象的类型
    public interface IBaseService<T>
    {
        Task<ApiResponse> GetAllAsync(QueryParameter query);
        Task<ApiResponse> GetSingleAsync(int id);
        Task<ApiResponse> AddAsync(T model);
        Task<ApiResponse> UpdateAsync(T model);
        Task<ApiResponse> DeleteAsync(int id);
    }
}
