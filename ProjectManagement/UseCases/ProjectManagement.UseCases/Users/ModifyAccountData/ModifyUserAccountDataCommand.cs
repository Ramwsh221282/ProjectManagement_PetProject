namespace ProjectManagement.UseCases.Users.ModifyAccountData;

public sealed record ModifyUserAccountDataCommand(
    Guid UserId,
    string? Email = null,
    string? Login = null)
{
    public bool HasChanges => Email is not null && Login is not null;
}