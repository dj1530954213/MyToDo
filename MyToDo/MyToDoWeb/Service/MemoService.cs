using AutoMapper;
using MyToDo.Shared.Dtos;
using MyToDo.Shared.Parameters;
using MyToDoWeb.Context;
using MyToDoWeb.Context.UnitOfWork;

namespace MyToDoWeb.Service
{
    public class MemoService:IMemoService
    {
        //数据库操作单元在ToDoService进行构建的时候获取
        private readonly IUnitOfWork _unitOfWork;
        //通过此对象将数据传输层的对象与数据库实体层进行了关系的映射
        private readonly IMapper _mapper;

        public MemoService(IUnitOfWork unitOfWork,IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ApiResponse> GetAllAsync(QueryParameter parameter)
        {
            try
            {
                //首先获取ToDo表的操作对象
                var repository = _unitOfWork.GetRepository<Memo>();
                //获取所有数据
                var memoResults = await repository.GetPagedListAsync(predicate:
                    x=>string.IsNullOrWhiteSpace(parameter.Search)?true:x.Title.Equals(parameter.Search),
                    pageIndex:parameter.PageIndex,
                    pageSize:parameter.PageSize,
                    orderBy:source=>source.OrderByDescending(t=>t.CreateDate)
                    );
                return new ApiResponse(true, memoResults);
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
                var respository = _unitOfWork.GetRepository<Memo>();
                var memoRespository = await respository.GetFirstOrDefaultAsync(predicate:x=> x.Id.Equals(id));
                return new ApiResponse(true, memoRespository);
            }
            catch (Exception e)
            {
                return new ApiResponse(e.Message.ToString());
            }
        }

        public async Task<ApiResponse> AddAsync(MemoDto model)
        {
            try
            {
                //这里将ToDoDto类型的model映射为了ToDo类型的对象，在InsertAsync中被使用
                var memo = _mapper.Map<Memo>(model);
                memo.CreateDate = DateTime.Now;
                memo.UpdateDate = DateTime.Now;
                //使用异步的方式添加至数据库中并保存
                await _unitOfWork.GetRepository<Memo>().InsertAsync(memo);
                if (await _unitOfWork.SaveChangesAsync() > 0)
                {
                    return new ApiResponse(true, memo);
                }
                return new ApiResponse("添加数据失败");
            }
            catch (Exception e)
            {
                return new ApiResponse(e.Message.ToString());
            }
        }

        public async Task<ApiResponse> UpdateAsync(MemoDto model)
        {
            try
            {
                var memo = _mapper.Map<Memo>(model);
                var respository = _unitOfWork.GetRepository<Memo>();
                var toDoResult = await respository.GetFirstOrDefaultAsync(predicate:x=>x.Id.Equals(memo.Id));
                //更新内容复制
                toDoResult.Title = model.Title;
                toDoResult.Content = model.Content;
                //toDoResult.Status = model.Status;
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
                var repository = _unitOfWork.GetRepository<Memo>();
                //使用异步的方式添加至数据库中并保存
                //predicate:x=>x.Id.Equals(id)中的predicate表示后面的语句为返回一个bool结果的判断类型的语句否则GetFirstOrDefaultAsync将会报错因为参数第一个要求传入一个func<ToDo,bool>的查询语句
                var memoResult = await _unitOfWork.GetRepository<Memo>().GetFirstOrDefaultAsync(predicate:x=>x.Id.Equals(id));
                //将查询到的对应id的实例传入删除方法中进行删除
                repository.Delete(memoResult);
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
    }
}
