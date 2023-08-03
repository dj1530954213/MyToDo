using Microsoft.EntityFrameworkCore;
using MyToDoWeb.Context.UnitOfWork;

namespace MyToDoWeb.Context.Repository
{
    public class MemoRepository : Repository<Memo>, IRepository<Memo>
    {
        public MemoRepository(MyToDoContect dbContext) : base(dbContext)
        {

        }
    }
}
