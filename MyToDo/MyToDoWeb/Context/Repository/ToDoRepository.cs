using Microsoft.EntityFrameworkCore;
using MyToDoWeb.Context.UnitOfWork;

namespace MyToDoWeb.Context.Repository
{
    public class ToDoRepository : Repository<ToDo>, IRepository<ToDo>
    {
        public ToDoRepository(MyToDoContect dbContext) : base(dbContext)
        {
        }
    }
}
