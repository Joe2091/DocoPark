using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocoPark.BusinessLogic.Interfaces;
using DocoPark.DataAccess.Repositories;

namespace DocoPark.DataAccess
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DataContext _context;

        public UnitOfWork(DataContext context)
        {
            _context = context;
            Users = new UserRepository(_context);
            Vehicles = new VehicleRepository(_context);
            ParkingSpots = new ParkingSpotRepository(_context);
            ParkingSessions = new ParkingSessionRepository(_context);
            Reservations = new ReservationRepository(_context);
            Subscriptions = new SubscriptionRepository(_context);
            Transactions = new TransactionRepository(_context);
        }

        public IUserRepository Users { get; }
        public IVehicleRepository Vehicles { get; }
        public IParkingSpotRepository ParkingSpots { get; }
        public IParkingSessionRepository ParkingSessions { get; }
        public IReservationRepository Reservations { get; }
        public ISubscriptionRepository Subscriptions { get; }
        public ITransactionRepository Transactions { get; }

        public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();

        public void Dispose() => _context.Dispose();
    }
}
