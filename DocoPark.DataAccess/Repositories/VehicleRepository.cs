using DocoPark.BusinessLogic.Interfaces;
using DocoPark.Domain.Entities;

namespace DocoPark.DataAccess.Repositories;

public class VehicleRepository : Repository<Vehicle>, IVehicleRepository
{
    public VehicleRepository(DataContext context) : base(context) { }
}