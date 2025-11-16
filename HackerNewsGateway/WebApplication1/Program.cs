using HackerNewsGateway.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddHttpClient();
builder.Services.AddTransient<IHackerNewsService, HackerNewsService>();

var swaggerEnabled = builder.Configuration.GetValue<bool>("SwaggerEnabled");
if (swaggerEnabled)
{
    // Required to generate Swagger/OpenAPI docs for controller
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

}

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

if (swaggerEnabled)
{
    // Serve the generated OpenAPI/Swagger JSON and the interactive UI at the app root
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "HackerNews Gateway API V1");
        c.RoutePrefix = string.Empty; // serve UI at "/"
    });
}


app.Run();
