using Redis.OM;
using WebApiRedisOM.Models;
using WebApiRedisOM.Services;

var builder = WebApplication.CreateBuilder(args);

var redisConnectionString = builder.Configuration.GetConnectionString("REDIS_CONNECTION_STRING");
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHostedService<IndexCreationService>();
builder.Services.AddSingleton(new RedisConnectionProvider(redisConnectionString));
// builder.Services.AddScoped((provider => provider.GetRequiredService<RedisConnectionProvider>().RedisCollection<Person>()));
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();