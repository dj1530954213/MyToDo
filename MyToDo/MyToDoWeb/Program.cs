using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MyToDoWeb.Context;
using MyToDoWeb.Context.Repository;
using MyToDoWeb.Context.UnitOfWork;
using MyToDoWeb.Extensions;
using MyToDoWeb.Service;

namespace MyToDoWeb
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            /*注意下面的这个语句用于标识MyToDoContect是DbContext的泛型的具体类型否则将会报错具体参考如下连接
             *https://www.cnblogs.com/xuejianxiyang/p/15131372.html
             */
            builder.Services.AddScoped<DbContext, MyToDoContect>();

            //在主程序中引用配置文件进行处理并将其关联起来
            builder.Services.AddDbContext<MyToDoContect>(options =>
            {
                var connectionString = builder.Configuration.GetConnectionString("ToDoConnection");
                options.UseSqlite(connectionString);
            }).AddUnitOfWork<MyToDoContect>()
            .AddCustomRepository<ToDo, ToDoRepository>()
            .AddCustomRepository<Memo, MemoRepository>()
            .AddCustomRepository<User, UserRepository>();
            //注册数据库操作服务
            builder.Services.AddTransient<IToDoService, ToDoService>();
            builder.Services.AddTransient<IMemoService, MemoService>();
            builder.Services.AddTransient<ILoginService, LoginService>();
            #region 添加AutoMapper
            var autoMapperConfig = new MapperConfiguration(config =>
            {
                config.AddProfile(new AutoMapperProfile());
            });
            builder.Services.AddSingleton(autoMapperConfig.CreateMapper());
            #endregion

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();





            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}