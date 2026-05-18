using DocoPark.BusinessLogic.Interfaces.Repositories;
using DocoPark.Domain.Entities;

namespace DocoPark.DataAccess.Repositories;

public class TransactionRepository : Repository<Transaction>, ITransactionRepository
{
    public TransactionRepository(DataContext context) : base(context) { }
}