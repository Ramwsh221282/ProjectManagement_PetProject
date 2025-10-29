using ProjectManagement.Domain.ProjectContext;
using ProjectManagement.Domain.ProjectContext.Entities.ProjectMembers;
using ProjectManagement.Domain.ProjectContext.Entities.ProjectMembers.ValueObjects;
using ProjectManagement.Domain.ProjectContext.Entities.ProjectMembers.ValueObjects.Enumerations;
using ProjectManagement.Domain.ProjectContext.Entities.ProjectTasks;
using ProjectManagement.Domain.ProjectContext.Entities.ProjectTasks.ValueObjects;
using ProjectManagement.Domain.ProjectContext.Entities.ProjectTasks.ValueObjects.Enumerations;
using ProjectManagement.Domain.ProjectContext.ValueObjects;

namespace ProjectManagement.Infrastructure;

public static class ProjectsStorage
{
    public static readonly Dictionary<Guid, Project> Projects = [];

    static ProjectsStorage()
    {
        if (Projects.Count > 0) return;
        // Создание тестовых проектов для демонстрации


        var project1 = new Project(
            ProjectDescription.Create(
                "Это первый тестовый проект для демонстрации системы управления проектами."
            ), ProjectName.Create("Тестовый проект 1"));
        ProjectTask[] project1Tasks = [new(
            new ProjectTaskId(),
            ProjectTaskMembersLimit.Create(3),
            new ProjectTaskStatusInfo(
                new ProjectTaskStatusOpened(),
                ProjectTaskSchedule.Create(
                    DateTime.UtcNow,
                    DateTime.UtcNow.AddDays(7)
                )
            ),
            ProjectTaskInfo.Create(
                "Основная задача",
                "Разработать основную функциональность"
            ),
            project1, // Проект будет установлен позже
            []
        ), new(
            new ProjectTaskId(),
            ProjectTaskMembersLimit.Create(2),
            new ProjectTaskStatusInfo(
                new ProjectTaskStatusClosed(),
                ProjectTaskSchedule.Create(
                    DateTime.UtcNow,
                    DateTime.UtcNow.AddDays(5)
                )
            ),
            ProjectTaskInfo.Create("Завершенная задача", "Завершить предыдущую работу"),
            project1, // Проект будет установлен позже
            []
        ),];
        
        // Проект 1: Активный проект с задачами
        var project2 = new Project(
            ProjectDescription.Create(
                "Второй проект для тестирования различных сценариев работы системы."
            ),
            ProjectName.Create("Проект тестирования"));
            
            ProjectTask[] project2Tasks = [
                    new ProjectTask(
                        new ProjectTaskId(),
                        ProjectTaskMembersLimit.Create(5),
                        new ProjectTaskStatusInfo(
                            new ProjectTaskStatusOpened(),
                            ProjectTaskSchedule.Create(
                                DateTime.UtcNow.AddDays(-10),
                                DateTime.UtcNow.AddDays(5)
                            )
                        ),
                        ProjectTaskInfo.Create(
                            "Тестирование",
                            "Провести комплексное тестирование системы"
                        ),
                        project2, // Проект будет установлен позже
                        []
                    )];

            var project3 = new Project(
                ProjectDescription.Create("Архивный проект, завершенный в прошлом месяце."),
                ProjectName.Create("Архивный проект"));
            
            ProjectTask[] project3Tasks = [
                new ProjectTask(
                    new ProjectTaskId(),
                    ProjectTaskMembersLimit.Create(5),
                    new ProjectTaskStatusInfo(
                        new ProjectTaskStatusOpened(),
                        ProjectTaskSchedule.Create(
                            DateTime.UtcNow.AddDays(-10),
                            DateTime.UtcNow.AddDays(5)
                        )
                    ),
                    ProjectTaskInfo.Create(
                        "Тестирование",
                        "Провести комплексное тестирование системы"
                    ),
                    project3, // Проект будет установлен позже
                    []
                ), new(
                    new ProjectTaskId(),
                    ProjectTaskMembersLimit.Create(3),
                    new ProjectTaskStatusInfo(
                        new ProjectTaskStatusOpened(),
                        ProjectTaskSchedule.Create(
                            DateTime.UtcNow,
                            DateTime.UtcNow.AddDays(7)
                        )
                    ),
                    ProjectTaskInfo.Create(
                        "Основная задача",
                        "Разработать основную функциональность"
                    ),
                    project3, // Проект будет установлен позже
                    []
                ), new(
                    new ProjectTaskId(),
                    ProjectTaskMembersLimit.Create(2),
                    new ProjectTaskStatusInfo(
                        new ProjectTaskStatusClosed(),
                        ProjectTaskSchedule.Create(
                            DateTime.UtcNow,
                            DateTime.UtcNow.AddDays(5)
                        )
                    ),
                    ProjectTaskInfo.Create("Завершенная задача", "Завершить предыдущую работу"),
                    project3, // Проект будет установлен позже
                    []
                ),];
        
        
        Projects.Add(project1.Id.Value, project1);
        Projects.Add(project2.Id.Value, project2);
        Projects.Add(project3.Id.Value, project3);
    }
}
