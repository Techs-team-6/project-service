using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectService.WebApi.Entities;

public class Project
{
    public Project(Guid id, Uri uri, string name, string githubToken)
    {
        Id = id;
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
