using ProjectManagement.Domain.Contracts;
using ProjectManagement.Domain.ProjectContext;
using ProjectManagement.Domain.ProjectContext.Entities.ProjectMembers;
using ProjectManagement.Domain.Utilities;

namespace ProjectManagement.UseCases.Projects.UpdateProjectInfo;

public sealed class UpdateProjectInfoHandler(
    IProjectsRepository repository,
    IUnitOfWork unitOfWork,
    ITransactionSource transactionSource)
{
    public async Task<Result<Project, Error>> Handle(UpdateProjectInfoCommand command, CancellationToken ct = default)
    {
        await using ITransactionScope scope = await transactionSource.BeginTransactionScope(ct);
        
        Result<Project, Nothing> project = await repository.GetProject(command.ProjectId, withLock: true, ct);
        if (project.IsFailure) return Failure<Project, Error>(Error.NotFound("Проект не найден."));
        
        Result<ProjectMember, Nothing> creator = project.OnSuccess.FindMember(command.CreatorId);
        if (creator.IsFailure) return Failure<Project, Error>(Error.NotFound("Создатель проекта не найден."));
        
        if (!creator.OnSuccess.IsOwning(project.OnSuccess)) 
            return Failure<Project, Error>(Error.Conflict("Участник не является владельцем проекта."));
        
        if (command.NothingToUpdate()) return Success<Project, Error>(project.OnSuccess);
        
        Result<Unit, Error> update = project.OnSuccess.Update(command.NewName, command.NewDescription);
        if (update.IsFailure) return Failure<Project, Error>(update.OnError);
        
        ProjectRegistrationApproval approval = await repository.GetApproval(project.OnSuccess.Name, ct);
        if (!approval.HasUniqueName) return Failure<Project, Error>(Error.Conflict("Проект с таким названием уже существует."));
        
        Result<Unit, Error> saving = await unitOfWork.SaveChangesAsync(ct);
        if (saving.IsFailure) 
            return Failure<Project, Error>(saving.OnError);
        
        Result<Unit, Error> commit = await scope.CommitAsync(ct);
        
        return commit.IsFailure 
            ? Failure<Project, Error>(commit.OnError) 
            : Success<Project, Error>(project.OnSuccess);
    }
}