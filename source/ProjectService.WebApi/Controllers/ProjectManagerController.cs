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
    
    [HttpGet("GetInfo")]
    public ActionResult<GitInfo> GetInfo()
    {
        return _gitInfo;
    }
    
    [HttpPost("projects/{projectId}/builds/create")]
    public ActionResult CreateBuild(Guid projectId)
    {
        throw new NotImplementedException();
    }
}