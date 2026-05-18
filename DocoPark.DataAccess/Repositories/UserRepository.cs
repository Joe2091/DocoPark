using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocoPark.BusinessLogic.Interfaces.Repositories;
using DocoPark.Domain.Entities;

namespace DocoPark.DataAccess.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(DataContext context) : base(context) { }
    }
}
