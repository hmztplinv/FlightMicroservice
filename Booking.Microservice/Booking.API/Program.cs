using Booking.API.Repositories;
using Booking.API.Settings;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Redis ayarlarını yapılandırma
var redisSettings = builder.Configuration.GetSection("RedisSettings").Get<RedisSettings>();
builder.Services.AddSingleton(redisSettings);
builder.Services.AddScoped<IBookingRepository, BookingRepository>();

// MassTransit ve RabbitMQ yapılandırması
builder.Services.AddMassTransit(config =>
{
    config.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host("localhost", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });
    });
});

// Swagger/OpenAPI yapılandırması
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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