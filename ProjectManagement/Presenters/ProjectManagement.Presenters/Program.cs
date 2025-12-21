using ProjectManagement.Presenters.Extensions;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.RegisterInfrastructureServices();
builder.Services.RegisterProjectsUseCases();
builder.Services.RegisterUsersUseCases();

WebApplication app = builder.Build();
await app.ApplyMigrations();

app.MapControllers();
app.UseSwagger();
app.UseSwaggerUI();

app.Run();
