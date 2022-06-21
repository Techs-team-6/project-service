using ProjectService.Shared.Entities;

namespace ProjectService.Core.Interfaces;

public interface ITemplateService
{
    ProjectTemplate CreateProjectTemplate(
        Stream data, string templateName, string buildString);
    IEnumerable<ProjectTemplate> GetAllTemplates();
    string GetTemplateBuildString(Guid templateId);
    Stream GetTemplateZip(Guid templateId);
}