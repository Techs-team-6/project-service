using Microsoft.EntityFrameworkCore.ChangeTracking;
using ProjectService.Core.Interfaces;
using ProjectService.Database;
using ProjectService.Shared.Entities;

namespace ProjectService.Core.Services;

public class TemplateService : ITemplateService
{
    private IRepository _repository;
    private ProjectDbContext _context;

    public TemplateService(IRepository repository, ProjectDbContext context)
    {
        _repository = repository;
        _context = context;
    }

    public ProjectTemplate CreateProjectTemplate(Stream data, string templateName, string buildString)
    {
        Guid id = _repository.SaveStream(data);
        EntityEntry<ProjectTemplate> template = _context.Templates.Add(new ProjectTemplate(id, templateName, buildString));
        _context.SaveChanges();
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