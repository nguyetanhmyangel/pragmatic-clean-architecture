using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Bookify.Api.OpenAiOptions;

public class ConfigureSwaggerOptions : IConfigureNamedOptions<SwaggerGenOptions>
{
    private readonly IApiVersionDescriptionProvider _provider;

    public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider) => _provider = provider;

    public void Configure(string name, SwaggerGenOptions options) => Configure(options);

    public void Configure(SwaggerGenOptions options)
    {
        foreach (var desc in _provider.ApiVersionDescriptions)
        {
            options.SwaggerDoc(desc.GroupName, new OpenApiInfo
            {
                Title = $"My API {desc.ApiVersion}",
                Version = desc.ApiVersion.ToString(),
                Description = desc.IsDeprecated ? "This API version has been deprecated." : null
            });
        }

        //Đảm bảo rằng Swagger chỉ hiển thị các endpoint tương ứng với đúng version.
        options.DocInclusionPredicate((docName, apiDesc) =>
            string.Equals(apiDesc.GroupName, docName, StringComparison.OrdinalIgnoreCase));

        // Khai báo cơ chế auth Bearer JWT cho Swagger
        // options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        // {
        //     Name = "Authorization",
        //     Type = SecuritySchemeType.ApiKey,
        //     Scheme = "Bearer",
        //     BearerFormat = "JWT",
        //     In = ParameterLocation.Header
        // });

        // Bắt buộc các endpoint phải có Bearer token trong Swagger UI
        // options.AddSecurityRequirement(new OpenApiSecurityRequirement {
        //     {
        //         new OpenApiSecurityScheme {
        //             Reference = new OpenApiReference {
        //                 Type = ReferenceType.SecurityScheme,
        //                 Id = "Bearer"
        //             }
        //         },
        //         Array.Empty<string>()
        //     }
        // });
    }
}
