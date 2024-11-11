using Microsoft.AspNetCore.Mvc;
using Netpack.GatewayApi.AspireResources;
using Netpack.GatewayApi.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped<ClusterStorage>();
builder.Services.AddSingleton<DashboardServiceHost>();
builder.Services.AddHostedService(sp => sp.GetRequiredService<DashboardServiceHost>());

var app = builder.Build();
app.MapGet("/", () => "Hello World!");
app.MapPost("/cluster", async ([FromQuery] string name, HttpContext context, [FromServices] ClusterStorage clusterStorage) =>
{
    context.Request.EnableBuffering();

    string body;
    using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true))
    {
        body = await reader.ReadToEndAsync();
    }
    clusterStorage.AddExternalCluster(name, body);
});

app.Run();
