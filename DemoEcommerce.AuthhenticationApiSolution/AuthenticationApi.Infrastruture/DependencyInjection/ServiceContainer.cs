using AuthenticationApi.Infrastruture.Data;
using AuthenticationApi.Infrastruture.Repositories;
using eCommerce.SharedLibrary.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using IUser = AuthenticationApi.Application.Interfaces.IUser;

namespace AuthenticationApi.Infrastruture.DependencyInjection
{
    public static class ServiceContainer
    {
        public static IServiceCollection AddInfrastructureService(this IServiceCollection services, IConfiguration config)
        {
            //Add Database Connectivity
            // JWT Add Authentication Scheme

            SharedServiceContainer.AddSharedServices<AuthenticationDbContext>(services, config, config["MySerilog:FileName"]!);

            // Create Depenedency Injection
            services.AddScoped<IUser, UserRepository>();

            return services;
        }

        public static IApplicationBuilder UserInfrastructurePolicy(this IApplicationBuilder app) 
        {
            // Register middleware such as::
            // Global Exception :: Handle external Errors.
            // Listen only to API Gateway : block outsiders call..

            SharedServiceContainer.UseSharedPolicies(app);
            
            return app;

        }
    }
}
