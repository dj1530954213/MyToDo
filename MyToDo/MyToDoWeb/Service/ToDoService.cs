using System.Collections.ObjectModel;
using AutoMapper;
using MyToDo.Shared.Dtos;
using MyToDo.Shared.Parameters;
using MyToDoWeb.Context;
using MyToDoWeb.Context.UnitOfWork;

namespace MyToDoWeb.Service
{
    public class ToDoService:IToDoService
    {
        //数据库操作单元在ToDoService进行构建的时候获取
        private readonly IUnitOfWork _unitOfWork;
        //通过此对象将数据传输层的对象与数据库实体层进行了关系的映射
        private readonly IMapper _mapper;

        public ToDoService(IUnitOfWork unitOfWork,IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ApiResponse> GetAllAsync(QueryParameter parameter)
        {
            try
            {
                //首先获取ToDo表的操作对象
                var repository = _unitOfWork.GetRepository<ToDo>();
                //获取所有数据  使用分页查询
                //var todoResults = await repository.GetAllAsync();
                var todoResults = await repository.GetPagedListAsync(predicate:
                    x=>string.IsNullOrWhiteSpace(parameter.Search)?true:x.Title.Contains(parameter.Search) || x.Content.Contains(parameter.Search),
                    pageIndex:parameter.PageIndex,
                    pageSize:parameter.PageSize,
                    orderBy:source=>source.OrderByDescending(t =>t.CreateDate)
                    );

                /*第一行：首先判断查询条件是否为空如果不为空则之间返回查询条件
                 *第二行：设置Linq的页索引
                 *第三行：设置Linq的页大小
                 *第四行：设置Linq的排序字段
                 */
                return new ApiResponse(true, todoResults);
            }
            catch (Exception e)
            {
                return new ApiResponse(e.Message.ToString());
            }
        }

        public async Task<ApiResponse> GetSingleAsync(int id)
        {
            try
            {
                var respository = _unitOfWork.GetRepository<ToDo>();
                var toDoRespository = await respository.GetFirstOrDefaultAsync(predicate:x=> x.Id.Equals(id));
                return new ApiResponse(true, toDoRespository);
            }
            catch (Exception e)
            {
                return new ApiResponse(e.Message.ToString());
            }
        }

        public async Task<ApiResponse> AddAsync(ToDoDto model)
        {
            try
            {
                //这里将ToDoDto类型的model映射为了ToDo类型的对象，在InsertAsync中被使用
                var todo = _mapper.Map<ToDo>(model);
                todo.CreateDate = DateTime.Now;
                todo.UpdateDate = DateTime.Now;
                //使用异步的方式添加至数据库中并保存
                await _unitOfWork.GetRepository<ToDo>().InsertAsync(todo);
                if (await _unitOfWork.SaveChangesAsync() > 0)
                {
                    return new ApiResponse(true, todo);
                }
                return new ApiResponse("添加数据失败");
            }
            catch (Exception e)
            {
                return new ApiResponse(e.Message.ToString());
            }
        }

        public async Task<ApiResponse> UpdateAsync(ToDoDto model)
        {
            try
            {
                var todo = _mapper.Map<ToDo>(model);
                var respository = _unitOfWork.GetRepository<ToDo>();
                var toDoResult = await respository.GetFirstOrDefaultAsync(predicate:x=>x.Id.Equals(todo.Id));
                //更新内容复制
                toDoResult.Title = model.Title;
                toDoResult.Content = model.Content;
                toDoResult.Status = model.Status;
                toDoResult.UpdateDate = DateTime.Now;
                //进行工作单元内的更新
                respository.Update(toDoResult);
                if (await _unitOfWork.SaveChangesAsync()>0)
                {
                    return new ApiResponse(true, toDoResult);
                }

                return new ApiResponse("更新失败");
            }
            catch (Exception e)
            {
                return new ApiResponse(e.Message);
            }
        }

        public async Task<ApiResponse> DeleteAsync(int id)
        {
            try
            {
                //首先获取ToDo表的操作对象
                var repository = _unitOfWork.GetRepository<ToDo>();
                //使用异步的方式添加至数据库中并保存
                //predicate:x=>x.Id.Equals(id)中的predicate表示后面的语句为返回一个bool结果的判断类型的语句否则GetFirstOrDefaultAsync将会报错因为参数第一个要求传入一个func<ToDo,bool>的查询语句
                var todoResult = await _unitOfWork.GetRepository<ToDo>().GetFirstOrDefaultAsync(predicate:x=>x.Id.Equals(id));
                //将查询到的对应id的实例传入删除方法中进行删除
                repository.Delete(todoResult);
                if (await _unitOfWork.SaveChangesAsync() > 0)
                {
                    return new ApiResponse(true,"删除成功");
                }
                return new ApiResponse("删除数据失败");
            }
            catch (Exception e)
            {
                return new ApiResponse(e.Message.ToString());
            }
        }

        public async Task<ApiResponse> GetAllAsync(ToDoParameter toDoParameter)
        {
            try
            {
                //首先获取ToDo表的操作对象
                var repository = _unitOfWork.GetRepository<ToDo>();
                //获取所有数据  使用分页查询
                //var todoResults = await repository.GetAllAsync();
                var todoResults = await repository.GetPagedListAsync(predicate:
                    //首先检查搜索条件是否为空，如果为空则返回全部------否则在标题或者内容中搜索是否包含搜索的字段-----在判断下拉框状态
                    x => string.IsNullOrWhiteSpace(toDoParameter.Search) ? (true&&(toDoParameter.Status == null ? true : x.Status.Equals(toDoParameter.Status))) : ((toDoParameter.Status == null?true:x.Status.Equals(toDoParameter.Status))&& (x.Title.Contains(toDoParameter.Search) || x.Content.Contains(toDoParameter.Search))),
                    pageIndex: toDoParameter.PageIndex,
                    pageSize: toDoParameter.PageSize,
                    orderBy: source => source.OrderByDescending(t => t.CreateDate)
                );

                /*第一行：首先判断查询条件是否为空如果不为空则之间返回查询条件
                 *第二行：设置Linq的页索引
                 *第三行：设置Linq的页大小
                 *第四行：设置Linq的排序字段
                 */
                return new ApiResponse(true, todoResults);
            }
            catch (Exception e)
            {
                return new ApiResponse(e.Message.ToString());
            }
        }

        public async Task<ApiResponse> Summary()
        {
            try
            {
                //注意这里使用ToDo而不能使用ToDoDto,需要通过类型转换工具Mapper来进行类型的映射
                var todos = await _unitOfWork.GetRepository<ToDo>().GetAllAsync(orderBy: source => source.OrderByDescending(t => t.CreateDate));
                var memos = await _unitOfWork.GetRepository<Memo>().GetAllAsync(orderBy: source => source.OrderByDescending(t => t.CreateDate));
                SummaryDto summary = new SummaryDto();
                summary.Sum = todos.Count;//待办事项总数量
                summary.CompletedCount = todos.Count(t => t.Status == 1);//待办事项完成数量
                summary.CompletedRatio = (summary.CompletedCount / (double)todos.Count).ToString("0%");//待办事项完成百分比
                summary.MemoCount = memos.Count;//备忘录总数
                summary.ToDoList = new ObservableCollection<ToDoDto>(_mapper.Map<List<ToDoDto>>(todos.Where(t=>t.Status == 0)));
                summary.MemoList = new ObservableCollection<MemoDto>(_mapper.Map<List<MemoDto>>(memos));
                return new ApiResponse(true,summary);
            }
            catch (Exception e)
            {
                return new ApiResponse(false,$"获取统计数据失败,错误为{e.Message.ToString()}");
            }
        }
    }
}
