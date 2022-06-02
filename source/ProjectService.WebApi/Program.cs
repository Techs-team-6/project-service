using ProjectService.WebApi.Interfaces;
using ProjectService.WebApi.Notifiers;
using ProjectService.WebApi.Repositories;
using ProjectService.WebApi.Services;
using ProjectService.WebApi.Wrappers;
using Server.API.Client;

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
builder.Services.AddScoped<IBuildNotifier, BuildNotifier>();
builder.Services.AddScoped<ITempRepository, TempRepository>(conf => new TempRepository(conf.GetService<IConfiguration>()!["TempPath"]!));
builder.Services.AddScoped<IRepository, Repository>(conf => new Repository(conf.GetService<IConfiguration>()!["RepositoryPath"]!));
builder.Services.AddScoped<IBuildNotifier, BuildNotifier>();
builder.Services.AddScoped<IConfigurationWrapper, ConfigurationWrapper>();
builder.Services.AddScoped<IBuilder, ProjectBuilder>();
builder.Services.AddScoped<IProjectCreator, ProjectCreator>();

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
