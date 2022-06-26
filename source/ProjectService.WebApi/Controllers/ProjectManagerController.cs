using Microsoft.AspNetCore.Mvc;
using ProjectService.Core.Interfaces;
using ProjectService.Shared.Entities;
using ProjectService.Shared.Exceptions;
using ProjectService.Shared.Models;
using Server.API.Client.Contracts;
using Project = ProjectService.Shared.Entities.Project;

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
    public async Task<ActionResult<Uri>> CreateProject([FromBody] ProjectCreateDto projectCreateDto, [FromQuery] Guid templateId = default)
    {
        try
        {
            return await _projectService.AddProjectAsync(projectCreateDto, templateId);
        }
        catch (EntityAlreadyExistsException<Project> e)
        {
            return Conflict("Project with such id already created");
        }
    }
    
    [HttpPost("project/{projectId}/buildString/update/{buildString}")]
    public ActionResult UpdateBuildString(Guid projectId, string buildString)
    {
        try
        {
            _projectService.UpdateBuildString(projectId, buildString);
        }
        catch (EntityNotFoundException<Project> e)
        {
            return NotFound(e.Message);
        }
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
        catch (EntityNotFoundException<ProjectBuild> e)
        {
            return NotFound(e.Message);
        }
    }
    
    [HttpGet("git/info")]
    public ActionResult<GitInfo> GetGitInfo()
    {
        return _projectService.GetGitInfo();
    }

    [HttpPost("projects/{projectId}/builds/create")]
    public async Task<ActionResult> CreateBuild(Guid projectId)
    {
        try
        {
            await _projectService.CreateVersionAsync(projectId);
        }
        catch (EntityNotFoundException<Project> e)
        {
            return NotFound(e.Message);
        }
        catch (SwaggerException e)
        {
            return Ok("Can not connect to central server. Build created, but server notification failed.");
        }

        return Ok();
    }
    
    [HttpGet]
    [Route("DownloadZipFile/{storageId}")]
    public IActionResult DownloadPdfFile(Guid storageId)
    {
        try
        {
            using Stream file = _projectService.GetProjectVersionArchive(storageId);
            FileStreamResult responce = File(file, "application/zip");
            responce.FileDownloadName = "file.zip";
            return responce;
        }
        catch (EntityNotFoundException<ProjectBuild> e)
        {
            return NotFound(e.Message);
        }
    }
}