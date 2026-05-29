using VisitBookingSystem.Data;
using VisitBookingSystem.Middleware;
using VisitBookingSystem.Services;

var builder = WebApplication.CreateBuilder(args);

// Rejestracja bazy danych w pamięci (Singleton dla zachowania danych między żądaniami)
builder.Services.AddSingleton<IInMemoryDatabase, InMemoryDatabase>();

// ETAP II: Rejestracja serwisów biznesowych w kontenerze DI
builder.Services.AddScoped<IPatientService, PatientService>();
builder.Services.AddScoped<IAppointmentService, AppointmentService>();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

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
