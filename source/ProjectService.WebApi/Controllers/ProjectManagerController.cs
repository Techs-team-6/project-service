using Microsoft.AspNetCore.Mvc;
using ProjectService.WebApi.Models;

namespace ProjectService.WebApi.Controllers;

[ApiController]
[Route("api/v1")]
public class ProjectManagerController : ControllerBase
{
    [HttpPost("Create")]
    public ActionResult<string> CreateProject([FromBody] ProjectCreateDto projectCreateDto)
    {
        throw new NotImplementedException();
    }

    [HttpGet("Get/{repositoryId:guid}/{id:int}")]
    public ActionResult<MemoryStream> GetBuild([FromRoute] Guid repositoryId, [FromRoute] int id)
    {
        throw new NotImplementedException();
    }
}