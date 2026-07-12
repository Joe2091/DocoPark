using DocoPark.BusinessLogic.Interfaces;
using DocoPark.BusinessLogic.Interfaces.Services;
using DocoPark.BusinessLogic.Mappings;
using DocoPark.BusinessLogic.Services;
using DocoPark.DataAccess;

namespace DocoParkWebApp.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices (this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IVehicleService, VehicleService>();
            services.AddScoped<IPricingService, PricingService>();
            services.AddScoped<IParkingSessionService, ParkingSessionService>();
            services.AddScoped<IReservationService, ReservationService>();
            services.AddAutoMapper(typeof(UserMappingProfile).Assembly);

            return services;
        }
}}
