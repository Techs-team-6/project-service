﻿namespace ProjectService.Core.Entities;

public class Project
{
    public Project(Uri uri, string name, string githubToken)
    {
        Id = Guid.Empty;
        Uri = uri;
        Name = name;
        GithubToken = githubToken;
    }

    public Guid Id { get; private set; }
    public Uri Uri { get; private set; }
    public string Name { get; private set; }
    public string GithubToken { get; private set; }
}