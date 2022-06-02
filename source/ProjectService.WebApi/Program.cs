using ProjectService.WebApi.Interfaces;
using ProjectService.WebApi.Services;
using ProjectService.WebApi.Wrappers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IProjectService, ProjectService.WebApi.Services.ProjectService>();
builder.Services.AddScoped<IProjectBuildService, ProjectBuildService>();
builder.Services.AddScoped<IGithubService, GithubService>();
builder.Services.AddScoped<IConfigurationWrapper, ConfigurationWrapper>();

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
