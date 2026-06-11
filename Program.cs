using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using VisitBookingSystem.Data;
using VisitBookingSystem.Middleware;
using VisitBookingSystem.Services;

var builder = WebApplication.CreateBuilder(args);

// ETAP III: Rejestracja DbContext (EF Core + SQLite)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// ETAP II: Rejestracja serwisów biznesowych w kontenerze DI
builder.Services.AddScoped<IPatientService, PatientService>();
builder.Services.AddScoped<IAppointmentService, AppointmentService>();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

// ETAP III: Swagger z dokumentacją XML
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Visit Booking System API",
        Version = "v1",
        Description = "System rezerwacji wizyt – REST API (.NET 8)"
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
});

var app = builder.Build();

// ETAP III: Automatyczne zastosowanie migracji przy starcie
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

// ETAP II: Globalna obsługa wyjątków (Middleware)
app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
