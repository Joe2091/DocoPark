using DocoPark.BusinessLogic.Interfaces;
using DocoPark.Domain.Entities;

namespace DocoPark.DataAccess.Repositories;

public class ParkingSpotRepository : Repository<ParkingSpot>, IParkingSpotRepository
{
    public ParkingSpotRepository(DataContext context) : base(context) { }
}