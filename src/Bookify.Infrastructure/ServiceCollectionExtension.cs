using Bookify.Application.Abstractions.Data;
using Bookify.Application.Abstractions.Email;
using Bookify.Domain.Entities.Apartments;
using Bookify.Domain.Entities.Bookings;
using Bookify.Domain.Entities.Users;
using Bookify.Infrastructure.Data;
using Bookify.Infrastructure.Email;
using Bookify.Infrastructure.Repositories;
using Bookify.Infrastructure.Time;
using Bookify.ShareKernel.BaseRepository;
using Bookify.ShareKernel.Time;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Bookify.Infrastructure;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddTransient<IDateTimeProvider, DateTimeProvider>();
        services.AddTransient<IEmailService, EmailService>();

        AddPersistence(services, configuration);
        //
        // AddAuthentication(services, configuration);
        //
        // AddBackgroundJobs(services, configuration);
        //
        // AddAuthorization(services);
        //
        // AddCaching(services, configuration);
        //
        // AddHealthChecks(services, configuration);
        //
        // AddApiVersioning(services);

        return services;
    }

    // private static void AddBackgroundJobs(IServiceCollection services, IConfiguration configuration)
    // {
    //     services.Configure<OutboxOptions>(configuration.GetSection("Outbox"));
    //
    //     services.AddQuartz(configurator =>
    //     {
    //         var schedulerId = Guid.NewGuid();
    //         configurator.SchedulerId = $"default-id-{schedulerId}";
    //         configurator.SchedulerName = $"default-name-{schedulerId}";
    //     });
    //
    //     services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);
    //
    //     services.ConfigureOptions<ProcessOutboxMessageJobSetup>();
    // }
    //
    // private static void AddAuthentication(IServiceCollection services, IConfiguration configuration)
    // {
    //     services
    //         .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    //         .AddJwtBearer();
    //
    //     services.Configure<AuthenticationOptions>(configuration.GetSection("Authentication"));
    //
    //     services.ConfigureOptions<JwtBearerOptionsSetup>();
    //
    //     services.Configure<KeycloakOptions>(configuration.GetSection("Keycloak"));
    //
    //     services.AddTransient<AdminAuthorizationDelegatingHandler>();
    //
    //     services.AddHttpClient<IAuthenticationService, AuthenticationService>((serviceProvider, httpclient) =>
    //     {
    //         var keycloakOptions = serviceProvider.GetRequiredService<IOptions<KeycloakOptions>>().Value;
    //
    //         httpclient.BaseAddress = new Uri(keycloakOptions.AdminUrl);
    //     }).AddHttpMessageHandler<AdminAuthorizationDelegatingHandler>();
    //
    //     services.AddHttpClient<IJwtService, JwtService>((serviceProvider, httpclient) =>
    //     {
    //         var keycloakOptions = serviceProvider.GetRequiredService<IOptions<KeycloakOptions>>().Value;
    //
    //         httpclient.BaseAddress = new Uri(keycloakOptions.TokenUrl);
    //     });
    //
    //     services.AddHttpContextAccessor();
    //
    //     services.AddScoped<IUserContext, UserContext>();
    // }
    //
    private static void AddPersistence(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Database") ??
            throw new ArgumentNullException(nameof(configuration));
    
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(connectionString).UseSnakeCaseNamingConvention();
        });
    
        #region Repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IApartmentRepository, ApartmentRepository>();
        services.AddScoped<IBookingRepository, BookingRepository>();
    
        #endregion
    
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<ApplicationDbContext>());
    
        services.AddSingleton<ISqlConnectionFactory>(_ => new SqlConnectionFactory(connectionString));
        //SqlMapper là class trong thư viện Dapper.
        //AddTypeHandler nghĩa là đăng ký một TypeHandler tùy chỉnh DateOnlyTypeHandler, 
        //được sử dụng để ánh xạ kiểu dữ liệu DateOnly trong ứng dụng của bạn với kiểu dữ liệu DateTime trong cơ sở dữ liệu.
        SqlMapper.AddTypeHandler(new DateOnlyTypeHandler());
    }
    
    // private static void AddAuthorization(IServiceCollection services)
    // {
    //     services.AddScoped<AuthorizationService>();
    //
    //     services.AddTransient<IClaimsTransformation, CustomClaimsTransformation>();
    //
    //     services.AddTransient<IAuthorizationHandler, PermissionAuthorizationHandler>();
    //
    //     services.AddTransient<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();
    // }
    //
    // private static void AddCaching(IServiceCollection services, IConfiguration configuration)
    // {
    //     var connectionString = configuration.GetConnectionString("Cache") ??
    //                             throw new ArgumentNullException(nameof(configuration));
    //
    //     services.AddStackExchangeRedisCache(options => options.Configuration = connectionString);
    //
    //     services.AddSingleton<ICacheService, CacheService>();
    // }
    //
    // private static void AddHealthChecks(IServiceCollection services, IConfiguration configuration)
    // {
    //     services.AddHealthChecks()
    //         .AddNpgSql(configuration.GetConnectionString("Database"))
    //         .AddRedis(configuration.GetConnectionString("Cache"))
    //         .AddUrlGroup(new Uri(configuration["KeyCloak:BaseUrl"]), HttpMethod.Get, "keycloak");
    // }


    ///<summary>
    /// Add API Versioning when using Controllers
    /// If using Minimal APIs, consider the Nuget Package "Asp.Versioning.Http"
    /// </summary>
    /// <param name="services"></param>
    // private static void AddApiVersioning(IServiceCollection services)
    // {
    //     services
    //         .AddApiVersioning(options =>
    //         {
    //             options.DefaultApiVersion = new ApiVersion(1);
    //             options.ReportApiVersions = true;
    //             options.ApiVersionReader = new UrlSegmentApiVersionReader();
    //         })
    //         .AddMvc()
    //         .AddApiExplorer(options =>
    //         {
    //             options.GroupNameFormat = "'v'V";
    //             options.SubstituteApiVersionInUrl = true;
    //         });
    // }
}