﻿using System.Net;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using ProjectService.WebApi.Entities;
using ProjectService.WebApi.Exceptions;
using ProjectService.WebApi.Interfaces;
using ProjectService.WebApi.Models;

namespace ProjectService.WebApi.Controllers;

[ApiController]
[Route("api/v1")]
public class ProjectManagerController : ControllerBase
{
    private readonly IProjectService _projectService;

    public ProjectManagerController(IProjectService projectService)
    {
        _projectService = projectService;
    }

    [HttpPost("project/create")]
    public async Task<ActionResult<Uri>> CreateProject([FromBody] ProjectCreateDto projectCreateDto)
    {
        return (await _projectService.AddProjectAsync(projectCreateDto));
    }
    
    [HttpPost("project/{projectId}/buildString/update/{buildString}")]
    public ActionResult UpdateBuildString(Guid projectId, string buildString)
    {
        if (_projectService.UpdateBuildString(projectId, buildString) is null)
            return NotFound();

        return Ok();
    }

    [HttpGet("project/{repositoryId:guid}/builds/{id:int}")]
    public async Task<ActionResult<byte[]>> GetBuild([FromRoute] Guid repositoryId, [FromRoute] int id)
    {
        try
        {
            await using Stream buildStream = _projectService.GetProjectVersionArchive(repositoryId, id);
            byte[] bytes = new byte[buildStream.Length];
            await buildStream.ReadAsync(bytes, 0, (int) buildStream.Length);
            return bytes.ToArray();
        }
        catch (EntityNotFoundException<ProjectBuild>)
        {
            return NotFound();
        }
    }
    
    [HttpGet("git/info")]
    public ActionResult<GitInfo> GetGiInfo()
    {
        return _projectService.GetGitInfo();
    }

    [HttpPost("projects/{projectId}/builds/create")]
    public async Task<ActionResult> CreateBuild(Guid projectId)
    {
        await _projectService.CreateVersionAsync(projectId);
        return Ok();
    }
    
    [HttpGet]
    [Route("DownloadZipFile/{storageId}")]
    public IActionResult DownloadPdfFile(Guid storageId)
    {
        Stream file = _projectService.GetProjectVersionArchive(storageId);
        FileStreamResult responce = File(file, "application/zip");
        responce.FileDownloadName = "file.zip";
        return responce;
    }
}