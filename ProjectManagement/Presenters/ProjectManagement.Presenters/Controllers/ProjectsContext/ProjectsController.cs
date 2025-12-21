using Microsoft.AspNetCore.Mvc;
using ProjectManagement.Domain.ProjectContext;
using ProjectManagement.Presenters.Controllers.ProjectsContext.Requests;
using ProjectManagement.UseCases.Projects.CreateProjectByUser;

namespace ProjectManagement.Presenters.Controllers.ProjectsContext;

/// <summary>
/// Контроллер для работы с проектами
/// </summary>
[ApiController]
[Route("api/projects")]
public class ProjectsController
{
    /// <summary>
    /// Создание проекта
    /// </summary>
    /// <param name="request">Запрос на создание проекта (UserId, Name, Description)</param>
    /// <param name="handler">Обработчик создания проекта</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns>Созданный проект</returns>
    [HttpPost]
    public async Task<Envelope> Create(
        [FromBody] CreateProjectRequest request,
        [FromServices] CreateProjectByUserHandler handler,
        CancellationToken ct = default
        )
    {
        CreateProjectByUserCommand command = new(
            UserId: request.UserId, 
            ProjectName: request.Name, 
            ProjectDescription: request.Description);
        
        Project project = await handler.Handle(command, ct);
        return new Envelope(project.ToDto());        
    }
}