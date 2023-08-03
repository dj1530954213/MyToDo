using Microsoft.EntityFrameworkCore;
using MyToDoWeb.Context.UnitOfWork;

namespace MyToDoWeb.Context.Repository
{
    public class UserRepository: Repository<User>, IRepository<User>
    {
        public UserRepository(DbContext dbContext) : base(dbContext)
        {

        }
    }
}
