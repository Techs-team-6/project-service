using Microsoft.AspNetCore.Mvc;
using ProjectService.WebApi.Enums;
using ProjectService.WebApi.Interfaces;
using ProjectService.WebApi.Models;

namespace ProjectService.WebApi.Controllers;

[ApiController]
[Route("api/v1")]
public class ProjectManagerController : ControllerBase
{
    private readonly GitInfo _gitInfo;

    public ProjectManagerController(GitInfo gitInfo)
    { 
        _gitInfo = gitInfo;
    }

    [HttpPost("Create")]
    public ActionResult<string> CreateProject([FromBody] ProjectCreateDto projectCreateDto)
    {
        throw new NotImplementedException();
    }
    
    [HttpPost("UpdateBuildString/{projectId}/{buildString}")]
    public ActionResult UpdateBuildString(Guid projectId, string buildString)
    {
        throw new NotImplementedException();
    }

    [HttpGet("Get/{repositoryId:guid}/{id:int}")]
    public ActionResult<MemoryStream> GetBuild([FromRoute] Guid repositoryId, [FromRoute] int id)
    {
        throw new NotImplementedException();
    }
    
    [HttpGet("GetInfo")]
    public ActionResult<GitInfo> GetInfo()
    {
        return _gitInfo;
    }
}