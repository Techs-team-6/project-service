using Microsoft.EntityFrameworkCore.ChangeTracking;
using NLog;
using ProjectService.Core.Interfaces;
using ProjectService.Database;
using ProjectService.Shared.Entities;

namespace ProjectService.Core.Services;

public class TemplateService : ITemplateService
{
    private IRepository _repository;
    private ProjectDbContext _context;
    private readonly Logger _logger;

    public TemplateService(IRepository repository, ProjectDbContext context, Logger logger)
    {
        _repository = repository;
        _context = context;
        _logger = logger;
    }

    public ProjectTemplate CreateProjectTemplate(Stream data, string templateName, string buildString)
    {
        Guid id = _repository.SaveStream(data);
        EntityEntry<ProjectTemplate> template = _context.Templates.Add(new ProjectTemplate(id, templateName, buildString));
        _context.SaveChanges();
        
        _logger.Log(LogLevel.Info, $"new template {0} was added!", templateName);
        return template.Entity;
    }

    public IEnumerable<ProjectTemplate> GetAllTemplates()
    {
        return _context.Templates;
    }

    public string GetTemplateBuildString(Guid templateId)
    {
        return GetTemplate(templateId).BuildString;
    }

    public Stream GetTemplateZip(Guid templateId)
    {
        ProjectTemplate template = GetTemplate(templateId);
        return _repository.GetStream(template.StorageId);
    }

    private ProjectTemplate GetTemplate(Guid templateId)
    {
        if (_context.Templates.All(x => x.Id != templateId))
        {
            throw new ArgumentException("Template with such id not found");
        }

        return _context.Templates.First(x => x.Id == templateId);
    }
}