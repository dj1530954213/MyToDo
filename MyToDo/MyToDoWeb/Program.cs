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
            /*ע����������������ڱ�ʶMyToDoContect��DbContext�ķ��͵ľ������ͷ��򽫻ᱨ�����ο���������
             *https://www.cnblogs.com/xuejianxiyang/p/15131372.html
             */
            builder.Services.AddScoped<DbContext, MyToDoContect>();

            //�������������������ļ����д��������������
            builder.Services.AddDbContext<MyToDoContect>(options =>
            {
                var connectionString = builder.Configuration.GetConnectionString("ToDoConnection");
                options.UseSqlite(connectionString);
            }).AddUnitOfWork<MyToDoContect>()
            .AddCustomRepository<ToDo, ToDoRepository>()
            .AddCustomRepository<Memo, MemoRepository>()
            .AddCustomRepository<User, UserRepository>();
            //ע�����ݿ��������
            builder.Services.AddTransient<IToDoService, ToDoService>();
            builder.Services.AddTransient<IMemoService, MemoService>();
            builder.Services.AddTransient<ILoginService, LoginService>();
            #region ���AutoMapper
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