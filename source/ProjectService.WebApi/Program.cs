using Microsoft.EntityFrameworkCore;
using ProjectService.Core.Interfaces;
using ProjectService.Core.Notifiers;
using ProjectService.Core.Repositories;
using ProjectService.Core.Services;
using ProjectService.Core.Wrappers;
using ProjectService.Database;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
string? connectionString = builder.Configuration.GetSection("ConnectionString").Value;
builder.Services.AddDbContext<ProjectDbContext>(options  => options.UseSqlServer(connectionString));
builder.Services.AddScoped<IProjectService, ProjectService.Core.Services.ProjectService>();
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

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
