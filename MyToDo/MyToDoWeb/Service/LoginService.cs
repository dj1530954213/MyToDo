using AutoMapper;
using MyToDo.Shared.Dtos;
using MyToDo.Shared.Extensions;
using MyToDoWeb.Context;
using MyToDoWeb.Context.UnitOfWork;

namespace MyToDoWeb.Service
{
    public class LoginService : ILoginService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public LoginService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<ApiResponse> LoginAsync(string account, string password)
        {
            try
            {
                password = password.GetMD5();
                var model = await _unitOfWork.GetRepository<User>().GetFirstOrDefaultAsync(predicate:
                    x => (x.Account.Equals(account)) && (x.PassWord.Equals(password)));
                if (model == null)
                {
                    return new ApiResponse("登录失败，请检查用户名及密码是否正确");
                }

                return new ApiResponse(true, model);
            }
            catch (Exception e)
            {
                return new ApiResponse(false, e.Message.ToString());
            }
        }

        public async Task<ApiResponse> RegisterAsync(UserDto user)
        {
            try
            {
                var userinformation = _mapper.Map<User>(user);
                var userRepository = _unitOfWork.GetRepository<User>();
                var model = await userRepository.GetFirstOrDefaultAsync(predicate:
                    x => x.Account.Equals(userinformation.Account));
                if (model != null)
                {
                    return new ApiResponse(false, $"当前用户:{model.Account}已存在,切勿重复添加");
                }
                userinformation.CreateDate = DateTime.Now;
                userinformation.PassWord = userinformation.PassWord.GetMD5();
                await userRepository.InsertAsync(userinformation);
                if (await _unitOfWork.SaveChangesAsync() > 0)
                {
                    return new ApiResponse(true, userinformation);
                }
                return new ApiResponse(false, "添加失败");
            }
            catch (Exception e)
            {
                return new ApiResponse(false, e.Message.ToString());
            }
        }
    }
}
