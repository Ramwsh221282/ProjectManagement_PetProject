using ProjectManagement.Infrastructure;
using ProjectManagement.Presenters.Controllers;

var builder = WebApplication.CreateBuilder(args);

int count = ProjectsStorage.Projects.Count;

builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

app.MapControllers();
app.UseSwagger();
app.UseSwaggerUI();

app.Run();
