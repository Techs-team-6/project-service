﻿using ProjectService.WebApi.Entities;

namespace ProjectService.WebApi.Interfaces;

public interface IProjectBuildService
{
    ProjectBuild CreateBuild(Project project);
    MemoryStream GetBuild(ProjectBuild build);
}