using Microsoft.EntityFrameworkCore;

namespace MyToDoWeb.Context
{
    public class MyToDoContect:DbContext
    {
        public MyToDoContect(DbContextOptions<MyToDoContect> options):base(options)
        {
            
        }

        public DbSet<ToDo> ToDoTable { get; set; }
        public DbSet<User> UserTable { get; set; }
        public DbSet<Memo> MemoTable { get; set; }
    }
}
