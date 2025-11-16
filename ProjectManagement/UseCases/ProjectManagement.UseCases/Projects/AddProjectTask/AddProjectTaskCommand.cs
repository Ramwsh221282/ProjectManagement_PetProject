using System.Net;

namespace ProjectManagement.UseCases.Projects.AddProjectTask;

public record AddProjectTaskCommand(
    string ProjectName, 
    short MembersLimit, 
    string Status, 
    DateTime? FinishedAt,
    string Title,
    string Description);