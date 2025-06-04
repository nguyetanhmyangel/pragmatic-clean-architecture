using Bookify.Application.Abstractions.Authentication;
using Bookify.Application.Abstractions.Data;
using Bookify.Application.Abstractions.Email;
using Bookify.Domain.Entities.Apartments;
using Bookify.Domain.Entities.Bookings;
using Bookify.Domain.Entities.Users;
using Bookify.Infrastructure.Authentication;
using Bookify.Infrastructure.Authorization;
using Bookify.Infrastructure.Database;
using Bookify.Infrastructure.Repositories;
using Bookify.Infrastructure.Repositories.Generic;
using Bookify.Infrastructure.Services;
using Bookify.Infrastructure.Utilities;
using Bookify.ShareKernel.Repositories;
using Bookify.ShareKernel.Utilities;
using Dapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using AuthenticationOptions = Bookify.Infrastructure.Authentication.AuthenticationOptions;
using AuthenticationService = Bookify.Infrastructure.Authentication.AuthenticationService;
using IAuthenticationService = Bookify.Application.Abstractions.Authentication.IAuthenticationService;

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
        AddAuthentication(services, configuration);
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
    private static void AddAuthentication(IServiceCollection services, IConfiguration configuration)
    {
        //khi sử dụng [Authorize] hoặc .RequireAuthorization() để bảo vệ các endpoint,
        //framework cần biết scheme xác thực nào sẽ được sử dụng để xác thực người dùng
        //dòng cấu hình này chỉ định rằng JWT Bearer là scheme xác thực mặc định. Sau đó, AddJwtBearer() đăng ký
        //handler cho scheme này, cho phép ứng dụng xác thực các token JWT được gửi từ một Identity Provider như Keycloak..
        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer();
    
        //sử dụng Options Pattern để ánh xạ (bind) các thiết lập từ phần cấu hình "Authentication"
        //trong appsettings.json vào lớp AuthenticationOptions.
        services.Configure<AuthenticationOptions>(configuration.GetSection("Authentication"));
        
        //đăng ký một lớp cấu hình tùy chỉnh (JwtBearerOptionsSetup) để thiết lập các tùy chọn
        //cho xác thực JWT Bearer.
        services.ConfigureOptions<JwtBearerOptionsSetup>();
        
        //sử dụng Options Pattern để ánh xạ (bind) các thiết lập từ phần cấu hình "Keycloak"
        //trong appsettings.json vào lớp KeycloakOptions.
        services.Configure<KeycloakOptions>(configuration.GetSection("Keycloak"));
        
        //Admin API client. đăng ký handler với lifetime Transient, nghĩa là một instance mới được tạo mỗi khi cần.
        services.AddTransient<AdminAuthorizationDelegatingHandler>();
        
        // Mỗi khi inject IAuthenticationService sẽ:
        // Tạo HttpClient với BaseAddress từ cấu hình Keycloak. Đăng ký IAuthenticationService với HttpClient vừa tạo.
        // gắn AdminAuthorizationDelegatingHandler để tự động thêm token.
        // Gửi HttpClient này vào constructor của AuthenticationService
        services.AddHttpClient<IAuthenticationService, AuthenticationService>((serviceProvider, httpclient) =>
        {
            var keycloakOptions = serviceProvider.GetRequiredService<IOptions<KeycloakOptions>>().Value;
            httpclient.BaseAddress = new Uri(keycloakOptions.BaseUrl);
        })
        .AddHttpMessageHandler<AdminAuthorizationDelegatingHandler>();
        
        // Token client (login, token exchange, etc.)
        services.AddHttpClient<IJwtService, JwtService>((serviceProvider, httpclient) =>
        {
            var keycloakOptions = serviceProvider.GetRequiredService<IOptions<KeycloakOptions>>().Value;
        
            httpclient.BaseAddress = new Uri(keycloakOptions.BaseUrl);
        });
        services.AddHttpContextAccessor();
        services.AddScoped<IUserContext, UserContext>();
    }
    
    private static void AddPersistence(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Database") ??
            throw new ArgumentNullException("Connection string 'Database' is not found.");
    
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(connectionString).UseSnakeCaseNamingConvention();
        });
    
        #region Repositories
        services.AddScoped(typeof(IRepository<,>), typeof(GenericRepository<,>));
        // services.AddScoped<IUserRepository, UserRepository>();
        // services.AddScoped<IApartmentRepository, ApartmentRepository>();
        // services.AddScoped<IBookingRepository, BookingRepository>();
        
        // sử dụng thư viện Scrutor để tự động đăng ký các repository 
        services.Scan(scan => scan
            .FromAssemblyOf<IUserRepository>() // hoặc typeof(UserRepository)
            .AddClasses(classes => classes.Where(type => type.Name.EndsWith("Repository")))
            .AsImplementedInterfaces()
            .WithScopedLifetime()
        );
        #endregion
    
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<ApplicationDbContext>());
    
        //services.AddSingleton<ISqlConnectionFactory>(_ => new SqlConnectionFactory(connectionString));
        services.AddSingleton<ISqlConnectionFactory>(new SqlConnectionFactory(connectionString));
        //SqlMapper là class trong thư viện Dapper.
        //AddTypeHandler nghĩa là đăng ký một TypeHandler tùy chỉnh DateOnlyTypeHandler, 
        //được sử dụng để ánh xạ kiểu dữ liệu DateOnly trong ứng dụng của bạn với kiểu dữ liệu DateTime trong cơ sở dữ liệu.
        SqlMapper.AddTypeHandler(new DateOnlyTypeHandler());
    }
    
    private static void AddAuthorization(IServiceCollection services)
    {
        services.AddScoped<AuthorizationService>();
    
        services.AddTransient<IClaimsTransformation, CustomClaimsTransformation>();
    
        services.AddTransient<IAuthorizationHandler, PermissionAuthorizationHandler>();
    
        services.AddTransient<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();
    }
    
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