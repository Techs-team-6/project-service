using Microsoft.AspNetCore.Mvc;
using ProjectService.WebApi.Entities;
using ProjectService.WebApi.Enums;
using ProjectService.WebApi.Exceptions;
using ProjectService.WebApi.Interfaces;
using ProjectService.WebApi.Models;

namespace ProjectService.WebApi.Controllers;

[ApiController]
[Route("api/v1")]
public class ProjectManagerController : ControllerBase
{
    private readonly GitInfo _gitInfo;
    private readonly IProjectService _projectService;

    public ProjectManagerController(GitInfo gitInfo, IProjectService projectService)
    {
        _gitInfo = gitInfo;
        _projectService = projectService;
    }

    [HttpPost("Create")]
    public ActionResult<string> CreateProject([FromBody] ProjectCreateDto projectCreateDto)
    {
        return _projectService.AddProject(projectCreateDto).ToString()!;
    }
    
    [HttpPost("UpdateBuildString/{projectId}/{buildString}")]
    public ActionResult UpdateBuildString(Guid projectId, string buildString)
    {
        if (_projectService.UpdateBuildString(projectId, buildString) is null)
            return NotFound();

        return Ok();
    }

    [HttpGet("Get/{repositoryId:guid}/{id:int}")]
    public ActionResult<byte[]> GetBuild([FromRoute] Guid repositoryId, [FromRoute] int id)
    {
        Stream buildStream;
        try
        {
            buildStream = _projectService.GetProjectVersionArchive(repositoryId, GetHashCode());
        }
        catch (EntityNotFoundException<ProjectBuild>)
        {
            return NotFound();
        }
        
        Span<byte> bytes = default;
        buildStream.Read(bytes);
        
        return bytes.ToArray();
    }
    
    [HttpGet("GetInfo")]
    public ActionResult<GitInfo> GetInfo()
    {
        return _gitInfo;
    }
}