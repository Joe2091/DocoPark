using DocoPark.BusinessLogic.Interfaces;
using DocoPark.Domain.Entities;

namespace DocoPark.DataAccess.Repositories;

public class ReservationRepository : Repository<Reservation>, IReservationRepository
{
    public ReservationRepository(DataContext context) : base(context) { }
}