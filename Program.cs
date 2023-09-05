using System.Text;
using WeirdTool;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

// Add services to the container.

WebApplication app = builder.Build();

// 注入Configuration
AppSettings.SetConfiguration(app.Configuration);
Scheduler.Initialize();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.MapGet("/weatherforecast", () =>
{
    return 0;
});

app.Run();
