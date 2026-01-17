using ProjectManagement.Presenters.Extensions;
using ProjectManagement.Presenters.Middlewares;
using ProjectManagement.UseCases;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.RegisterInfrastructureServices();
builder.Services.RegisterProjectsUseCases();
builder.Services.RegisterUsersUseCases();

builder.Services.RegisterValidators();

builder.Services.AddScoped<CreateSomeFancyClass>();

WebApplication app = builder.Build();
await app.ApplyMigrations();

app.MapControllers();
app.UseSwagger();
app.UseSwaggerUI();
app.UseMiddleware<ExceptionMiddleware>();

app.Run();
