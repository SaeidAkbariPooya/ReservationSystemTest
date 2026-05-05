using FluentValidation;
using Microsoft.EntityFrameworkCore;
using ReservationSystem.API.Middlewares;
using ReservationSystem.Core.DTOs;
using ReservationSystem.Core.IService;
using ReservationSystem.Core.Mappings;
using ReservationSystem.Core.Service;
using ReservationSystem.Core.Validators;
using ReservationSystem.Core.RepositoryInterfaces;
using ReservationSystem.Infra.Context;
using ReservationSystem.Infra.UnitOfWork;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// DbContext
builder.Services.AddDbContext<ReservationSystemDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// UnitOfWork & Repositories
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// Services
builder.Services.AddScoped<IReservationService, ReservationService>();

// FluentValidation
builder.Services.AddScoped<IValidator<CreateReservationDto>, CreateReservationValidator>();

// CORS (for Angular)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Middlewares
app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseCors("AllowAngular");
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseCors("AllowAngular");
app.UseHttpsRedirection();
app.MapControllers();

// Seed data (optional)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ReservationSystemDbContext>();
    db.Database.Migrate();
    if (!db.Users.Any())
    {
        db.Users.Add(new ReservationSystem.Core.Entities.User { Name = "کاربر نمونه", Email = "sample@example.com" });
        db.Resources.AddRange(
            new ReservationSystem.Core.Entities.Resource { Name = "اتاق جلسه A", Type = "MeetingRoom", MaxConcurrentUsage = 10 },
            new ReservationSystem.Core.Entities.Resource { Name = "پروژکتور Full HD", Type = "Projector", MaxConcurrentUsage = 1 },
            new ReservationSystem.Core.Entities.Resource { Name = "خودروی شرکتی", Type = "Car", MaxConcurrentUsage = 1 }
        );
        db.SaveChanges();
    }
}


app.Run();