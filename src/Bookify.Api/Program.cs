using System.Diagnostics;
using System.Reflection;
using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Bookify.Api.Extensions;
using Bookify.API.Extensions;
using Bookify.Api.OpenAiOptions;
using Bookify.Application;
using Bookify.Infrastructure;
using Carter;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true);

//// Add services to the container.
// cấu hình carter 
builder.Services.AddCarter();

// REMARK: If you want to use Controllers, you'll need this.
builder.Services.AddControllers();

// Đăng ký API Versioning
builder.Services.AddApiVersioning(options =>
{
    //Đặt phiên bản API mặc định là 1.0
    options.DefaultApiVersion = new ApiVersion(1, 0);
    //yêu cầu không chỉ định phiên bản API, hệ thống sẽ sử dụng phiên bản mặc định đã được cấu hình
    options.AssumeDefaultVersionWhenUnspecified = true;
    //thêm các header như api-supported-versions và api-deprecated-versions vào phản hồi, giúp client biết được các phiên bản API hiện có và những phiên bản đã bị loại bỏ.
    options.ReportApiVersions = true;
    options.ApiVersionReader = ApiVersionReader.Combine(
        //Cấu hình cách đọc phiên bản API từ URL. Với UrlSegmentApiVersionReader, phiên bản API được chỉ định trong đường dẫn URL, ví dụ: /api/v1/products.
        new UrlSegmentApiVersionReader(),
        //Đọc phiên bản API từ header HTTP có tên X-Api-Version.
        new HeaderApiVersionReader("X-Api-Version")
    );
})
.AddApiExplorer(options =>
{
    //Định dạng tên nhóm cho các phiên bản API trong tài liệu Swagger. Với cấu hình này, các nhóm sẽ được đặt tên như v1, v2,...
    options.GroupNameFormat = "'v'VVV"; 
    //hệ thống sẽ tự động thay thế tham số {version} trong các route URL bằng giá trị phiên bản cụ thể, ví dụ: từ /api/v{version}/products thành /api/v1/products.
    options.SubstituteApiVersionInUrl = true;
});


// Đăng ký Swagger (tuỳ chọn, để kiểm tra API)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

//in order to configure swagger with different versions 
builder.Services.ConfigureOptions<ConfigureSwaggerOptions>();

// Đăng ký ErrorHandlingOptions (tuỳ cấu hình để ẩn/hiện lỗi)
// phải đảm bảo appsettings.json có cấu hình đúng:"ErrorHandling": {"ExposeExceptionDetails": true}
builder.Services.Configure<ErrorHandlingOptions>(
    builder.Configuration.GetSection("ErrorHandling"));

// Đăng ký GlobalErrorHandler
builder.Services.AddExceptionHandler<GlobalErrorHandler>();
// (Tuỳ chọn) Đăng ký ActivitySource để theo dõi lỗi
builder.Services.AddSingleton(new ActivitySource("ErrorHandling"));
builder.Services.AddProblemDetails();
if (builder.Environment.IsDevelopment())
{
    // Ghi đè để luôn hiện chi tiết exception trong môi trường dev
    builder.Services.PostConfigure<ErrorHandlingOptions>(options =>
    {
        options.ExposeExceptionDetails = true;
    });
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // Migration database
    // app.ApplyMigrations();
    // REMARK: Uncomment if you want to seed initial data
    // app.SeedData();
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        foreach (var desc in app.Services.GetRequiredService<IApiVersionDescriptionProvider>().ApiVersionDescriptions)
        {
            options.SwaggerEndpoint($"/swagger/{desc.GroupName}/swagger.json", $"My API {desc.ApiVersion}");
        }
    });
}
app.UseHttpsRedirection();
// app.UseRequestContextLogging();
// app.UseSerilogRequestLogging();
// Cấu hình ExceptionHandler, bắt buộc để sử dụng GlobalErrorHandler
app.UseExceptionHandler();
app.UseAuthentication();
app.UseAuthorization();
// Sử dụng Carter
app.MapCarter();
// REMARK: If you want to use Controllers, you'll need this.
app.MapControllers();
foreach (var endpoint in app.Services.GetRequiredService<EndpointDataSource>().Endpoints)
{
    Console.WriteLine($"Endpoint: {endpoint.DisplayName}");
}
app.Run();
