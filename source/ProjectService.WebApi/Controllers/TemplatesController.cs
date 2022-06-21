using Microsoft.AspNetCore.Mvc;
using ProjectService.Core.Interfaces;
using ProjectService.Shared.Entities;
using ProjectService.Shared.Models;

namespace ProjectService.WebApi.Controllers;

[ApiController]
[Route("api/v1/template")]
public class TemplatesController : Controller
{
    private readonly ITemplateService _templateService;

    public TemplatesController(ITemplateService templateService)
    {
        _templateService = templateService;
    }

    [HttpPost("create")]
    public ActionResult<Guid> CreateTemplate(
        [FromForm] IFormFile zip,
        [FromQuery] string name,
        [FromQuery] string buildString)
    {
        using Stream stream = zip.OpenReadStream();
        ProjectTemplate createdTemplate = 
            _templateService.CreateProjectTemplate(stream, name, buildString);
        return Ok(createdTemplate.Id);
    }

    [HttpGet("all")]
    public ActionResult<IEnumerable<TemplateDto>> GetAllTemplates()
    {
        IEnumerable<TemplateDto> templates = _templateService.GetAllTemplates()
            .Select(t => new TemplateDto(t.Id, t.Name));
        return Ok(templates);
    }
}