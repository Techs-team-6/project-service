// See https://aka.ms/new-console-template for more information

using System.Collections.Immutable;
using Microsoft.Extensions.Configuration;
using Octokit;
using ProjectService.WebApi.Interfaces;
using ProjectService.WebApi.Models;
using ProjectService.WebApi.Repositories;
using ProjectService.WebApi.Services;
using ProjectService.WebApi.Wrappers;

var configurationManager = new ConfigurationManager();
configurationManager.AddJsonFile("appsettings.json");
var repo = new TempRepository(@"C:\test");

IGithubService githubService =
    new GithubService(repo, new ProjectCreator(), new ConfigurationWrapper(configurationManager)); 
        
var res = await githubService.CreateProject(new ProjectCreateDto(Guid.NewGuid(), Guid.NewGuid().ToString(), true));
Console.WriteLine(res.Uri);

var folder = repo.GetTempFolder(res);
IBuilder builder = new ProjectBuilder();

var buildedPath = await builder.Build(folder, res.BuildString);
Console.WriteLine(buildedPath);