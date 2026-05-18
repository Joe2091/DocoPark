using DocoPark.BusinessLogic.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocoPark.BusinessLogic.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository Users { get; }
        IVehicleRepository Vehicles { get; }
        IParkingSpotRepository ParkingSpots { get; }
        IParkingSessionRepository ParkingSessions { get; }

        IReservationRepository Reservations { get; }
        ISubscriptionRepository Subscriptions { get; }
        ITransactionRepository Transactions { get; }

        Task<int> SaveChangesAsync();
    }
}
