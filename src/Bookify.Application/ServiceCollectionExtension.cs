using Bookify.Application.Abstractions.Behaviors;
using Bookify.Domain.Entities.Bookings;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Bookify.Application;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(configuration =>
        {
            configuration.RegisterServicesFromAssembly(typeof(ServiceCollectionExtension).Assembly);

            //When sending a command, it's going to first enter the logging behavior,
            //run the logging statement and then execute the command handler
            //before returning the response
            //Ghi log cho các request.
             configuration.AddOpenBehavior(typeof(LoggingBehavior<,>));
            //Thực hiện xác thực (validation) cho các request.
             configuration.AddOpenBehavior(typeof(ValidationBehavior<,>));
            //Lưu trữ kết quả của các truy vấn để cải thiện hiệu suất.
             configuration.AddOpenBehavior(typeof(QueryCachingBehavior<,>));
        });

        services.AddValidatorsFromAssembly(typeof(ServiceCollectionExtension).Assembly);
        services.AddTransient<PricingService>();

        return services;
    }
}