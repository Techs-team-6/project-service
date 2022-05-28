using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectService.WebApi.Entities;

public class Project
{
    public Project(Uri uri, string name, string githubToken)
    {
        Id = Guid.Empty;
        Uri = uri;
        Name = name;
        GithubToken = githubToken;
    }

    [Key]
    public Guid Id { get; private set; }
    public Uri Uri { get; private set; }
    public string Name { get; private set; }
    public string GithubToken { get; private set; }
}