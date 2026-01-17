namespace ProjectManagement.Domain.UserContext;

public sealed record UserRegistrationApproval(
    bool HasUniqueLogin,
    bool HasUniqueEmail,
    bool HasUniquePhone
    );