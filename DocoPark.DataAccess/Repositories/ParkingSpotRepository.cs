using DocoPark.BusinessLogic.Interfaces.Repositories;
using DocoPark.Domain.Entities;

namespace DocoPark.DataAccess.Repositories;

public class ParkingSpotRepository : Repository<ParkingSpot>, IParkingSpotRepository
{
    public ParkingSpotRepository(DataContext context) : base(context) { }
}