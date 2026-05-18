using DocoPark.BusinessLogic.Interfaces.Repositories;
using DocoPark.Domain.Entities;

namespace DocoPark.DataAccess.Repositories;

public class ReservationRepository : Repository<Reservation>, IReservationRepository
{
    public ReservationRepository(DataContext context) : base(context) { }
}