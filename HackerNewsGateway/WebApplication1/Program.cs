using HackerNewsGateway.Services;
using HackerNewsGateway.Services.Cache;
using System.Net;
using Polly;
using Polly.Registry;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<ICache, MemoryCacheService>();

var policyRegistry = new PolicyRegistry();

var retryPolicy = Policy<HttpResponseMessage>
    .Handle<HttpRequestException>()
    .OrResult(msg => (int)msg.StatusCode >= 500 || msg.StatusCode == HttpStatusCode.TooManyRequests)
    .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromMilliseconds(200 * Math.Pow(2, retryAttempt)) + TimeSpan.FromMilliseconds(Random.Shared.Next(0, 100)));

var circuitBreakerPolicy = Policy<HttpResponseMessage>
    .Handle<HttpRequestException>()
    .OrResult(msg => (int)msg.StatusCode >= 500 || msg.StatusCode == HttpStatusCode.TooManyRequests)
    .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30));

policyRegistry.Add("HackerNewsRetryPolicy", retryPolicy);
policyRegistry.Add("HackerNewsCircuitBreakerPolicy", circuitBreakerPolicy);

builder.Services.AddSingleton<IReadOnlyPolicyRegistry<string>>(policyRegistry);

builder.Services.AddHttpClient("HackerNewsClient", client =>
{
    client.BaseAddress = new Uri("https://hacker-news.firebaseio.com/v0/");
    client.DefaultRequestHeaders.UserAgent.ParseAdd("HackerNewsGateway/1.0 (+https://example.com/contact)");
    client.Timeout = TimeSpan.FromSeconds(6);
})
.ConfigurePrimaryHttpMessageHandler(() =>
{
    return new SocketsHttpHandler
    {
        PooledConnectionLifetime = TimeSpan.FromMinutes(2),
        MaxConnectionsPerServer = 8,
        AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
    };
})
.AddPolicyHandlerFromRegistry("HackerNewsRetryPolicy")
.AddPolicyHandlerFromRegistry("HackerNewsCircuitBreakerPolicy");

builder.Services.AddScoped<IHackerNewsService>(sp =>
{
    var factory = sp.GetRequiredService<IHttpClientFactory>();
    var client = factory.CreateClient("HackerNewsClient");
    var cache = sp.GetRequiredService<ICache>();
    return new HackerNewsService(client, cache);
});

var swaggerEnabled = builder.Configuration.GetValue<bool>("SwaggerEnabled");
if (swaggerEnabled)
{
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
}

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

if (swaggerEnabled)
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "HackerNews Gateway API V1");
        c.RoutePrefix = string.Empty;
    });
}

app.Run();
