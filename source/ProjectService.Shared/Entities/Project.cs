namespace ProjectService.Shared.Entities;

public class Project
{
    public Project(Guid id, Uri uri, string name, string buildString)
    {
        Id = id;
        Uri = uri;
        Name = name;
        BuildString = buildString;
    }
    
    public Guid Id { get; private set; }
    public Uri Uri { get; private set; }
    public string Name { get; private set; }
    public string BuildString { get; set; }
}
