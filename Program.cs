using Microsoft.EntityFrameworkCore;
using milktea_server.Data;
using milktea_server.Interfaces.Services;
using milktea_server.Services;

var builder = WebApplication.CreateBuilder(args);

builder
    .Services.AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(
            new System.Text.Json.Serialization.JsonStringEnumConverter()
        )
    );
;
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ApplicationDBContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DbConnectionString"));
});

// builder.Services.AddScoped<IRoomRepository, RoomRepository>();
builder.Services.AddScoped<IJwtService, JwtService>();

var app = builder.Build();

app.UsePathBase("/api/v1");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();
