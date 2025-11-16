using HackerNewsGateway.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddHttpClient();
builder.Services.AddTransient<IHackerNewsService, HackerNewsService>();

var swaggerEnabled = builder.Configuration.GetValue<bool>("SwaggerEnabled");
if (swaggerEnabled)
{
    builder.Services.AddOpenApi();
}

var app = builder.Build();

if (swaggerEnabled)
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
