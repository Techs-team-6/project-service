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
    public async Task<ActionResult<string>> CreateProject([FromBody] ProjectCreateDto projectCreateDto)
    {
        return (await _projectService.AddProjectAsync(projectCreateDto)).ToString()!;
    }
    
    [HttpPost("project/{projectId}/buildString/update/{buildString}")]
    public ActionResult UpdateBuildString(Guid projectId, string buildString)
    {
        if (_projectService.UpdateBuildString(projectId, buildString) is null)
            return NotFound();

        return Ok();
    }

    [HttpGet("project/{repositoryId:guid}/builds/{id:int}")]
    public ActionResult<byte[]> GetBuild([FromRoute] Guid repositoryId, [FromRoute] int id)
    {

        Span<byte> bytes = default;
        try
        {
            using Stream buildStream = _projectService.GetProjectVersionArchive(repositoryId, GetHashCode());
            buildStream.Read(bytes);
        }
        catch (EntityNotFoundException<ProjectBuild>)
        {
            return NotFound();
        }
        
        return bytes.ToArray();
    }
    
    [HttpGet("/git/info")]
    public ActionResult<GitInfo> GetGiInfo()
    {
        return _projectService.GetGitInfo();
    }

    [HttpPost("projects/{projectId}/builds/create")]
    public ActionResult CreateBuild(Guid projectId)
    {
        _projectService.CreateVersionAsync(projectId);
        return Ok();
    }
}