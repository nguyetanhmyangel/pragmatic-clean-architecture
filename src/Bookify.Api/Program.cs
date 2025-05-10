using System.Diagnostics;
using Bookify.Api.Extensions;
using Bookify.API.Extensions;
using Bookify.Application;
using Bookify.Infrastructure;
using Carter;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true);

// Add services to the container.
// cấu hình carter 
builder.Services.AddCarter();
// REMARK: If you want to use Controllers, you'll need this.
builder.Services.AddControllers();
// Swashbuckle for Swagger/OpenAPI
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

//builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "My API Title",
        Version = "v1",
        Description = "My API description here"
    });
});

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

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
    app.ApplyMigrations();
    // REMARK: Uncomment if you want to seed initial data
    app.SeedData();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API Title v1");
        //c.RoutePrefix = string.Empty; // Đặt Swagger UI tại root (/)
    });
    // app.UseSwaggerUI(options =>
    // {
    //     var descriptions = app.DescribeApiVersions();
    //     foreach (var groupName in descriptions.Select(description => description.GroupName))
    //     {
    //         var url = $"/swagger/{groupName}/swagger.json";
    //         var name = groupName.ToUpperInvariant();
    //         options.SwaggerEndpoint(url, name);
    //     }
    // });
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
app.Run();
