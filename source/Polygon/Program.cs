// See https://aka.ms/new-console-template for more information

using Microsoft.Extensions.Configuration;
using ProjectService.WebApi.Entities;
using ProjectService.WebApi.Enums;
using ProjectService.WebApi.Interfaces;
using ProjectService.WebApi.Models;
using ProjectService.WebApi.Repositories;
using ProjectService.WebApi.Services;

var configurationManager = new ConfigurationManager();
configurationManager.AddJsonFile("appsettings.json");

IGithubService githubService =
    new GithubService(new TempRepository(@"C:\Users\eid20\OneDrive\Рабочий стол\New folder (2)"), new ProjectCreator(),
        new GitInfo("eid20021@gmail.com", "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", "TestOrgOktokit"), configurationManager); 
        
Project res = await githubService.CreateProject(new ProjectCreateDto(Guid.NewGuid(), "aboba", true));

Console.WriteLine(res.Uri);