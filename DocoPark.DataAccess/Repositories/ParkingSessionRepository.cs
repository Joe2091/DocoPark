using DocoPark.BusinessLogic.Interfaces;
using DocoPark.Domain.Entities;

namespace DocoPark.DataAccess.Repositories;

public class ParkingSessionRepository : Repository<ParkingSession>, IParkingSessionRepository
{
    public ParkingSessionRepository(DataContext context) : base(context) { }
}