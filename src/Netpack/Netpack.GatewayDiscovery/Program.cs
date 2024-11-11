var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapPost("/{id}", (string id, HttpContext context) =>
{
    var result = Environment.GetEnvironmentVariable($"GATEWAY_{id.ToUpperInvariant()}");
    if (result == null)
    {
        context.Response.StatusCode = 404;
        return string.Empty;
    }
    else
    {
        return result;
    }
});

app.Run();
