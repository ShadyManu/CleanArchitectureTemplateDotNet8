using Application.Common.Interfaces.Repositories;
using Infrastructure.Data;
using Infrastructure.Data.Interceptors;
using Infrastructure.Data.Repositories.ToDo;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Infrastructure;

public static class InfrastructureDependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        #region Database

        var databaseProvider = configuration["Database:DatabaseProvider"] ?? "MSSQL";
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        if (connectionString is null)
            throw new NullReferenceException("connection string is null");

        services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
        if (databaseProvider.Equals("MSSQL", StringComparison.OrdinalIgnoreCase))
        {
            services.AddDbContext<ApplicationDbContext>((sp, options) =>
            {
                options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
                options.UseSqlServer(connectionString);
            });
        }
        else if (databaseProvider.Equals("InMemory", StringComparison.OrdinalIgnoreCase))
        {
            services.AddDbContext<ApplicationDbContext>(
                (sp, options) =>
                {
                    options.UseInMemoryDatabase("InMemoryDb");
                    options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
                    options.LogTo(Console.WriteLine);
                });
        }
        
        #endregion

        #region Repositories
        
        services.TryAddScoped<IToDoRepository, ToDoRepository>();

        #endregion

        services.AddSingleton(TimeProvider.System);

        return services;
    }
}
