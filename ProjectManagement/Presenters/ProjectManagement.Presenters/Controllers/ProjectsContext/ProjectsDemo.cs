using System.Net;
using Microsoft.AspNetCore.Mvc;
using ProjectManagement.Domain.ProjectContext;
using ProjectManagement.Infrastructure.ProjectContext;

namespace ProjectManagement.Presenters.Controllers.ProjectsContext;

[ApiController]
[Route("api/projects-demo")]
public sealed class ProjectsDemoController
{
    [HttpGet("{id:guid}")]
    public async Task<Envelope> GetProjectById(
        [FromRoute(Name = "id")] Guid id,
        [FromServices] ProjectsRepository repository,
        CancellationToken ct
        )
    {
        Project? project = await repository.GetProjectDemo(id);
        if (project is null) return new Envelope(HttpStatusCode.NotFound, null, null);
        return new Envelope(project.ToDto());
    }
}