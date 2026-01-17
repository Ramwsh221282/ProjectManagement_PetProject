using FluentValidation;
using FluentValidation.Results;
using ProjectManagement.Domain;
using ProjectManagement.Domain.ProjectContext.ValueObjects;
using ProjectManagement.Domain.Utilities;
using ProjectManagement.UseCases.Common;

namespace ProjectManagement.UseCases;

public sealed class CreateSomeFancyClassCommandValidator
    : AbstractValidator<CreateSomeFancyClassCommand>
{
    public CreateSomeFancyClassCommandValidator()
    {
        RuleFor(x => x.FancyPropertyA).MustBeValid(str => ProjectName.Create(str));

        RuleFor(x => x.DtoData)
            .ChildRules(dto =>
            {
                dto.RuleFor(d => d.Property).MustBeValid(ProjectName.Create);
            });

        RuleForEach(x => x.FancyStringCollection)
            .ChildRules(str =>
            {
                str.RuleFor(s => s).MustBeValid(ProjectName.Create);
            });
    }
}

public sealed record CreateSomeFancyClassCommand(
    string FancyPropertyA,
    int FancyPropertyB,
    IEnumerable<string> FancyStringCollection,
    SomeDtoData DtoData
);

public sealed record SomeDtoData(string Property);

public sealed class CreateSomeFancyClass(IValidator<CreateSomeFancyClassCommand> validator)
{
    public async Task<Result<SomeFancyClass>> Execute(
        CreateSomeFancyClassCommand command,
        CancellationToken ct = default
    )
    {
        ValidationResult validationResult = await validator.ValidateAsync(command, ct);
        if (validationResult.IsValid == false)
        {
            return validationResult.ToError<SomeFancyClass>();
        }

        return new SomeFancyClass
        {
            FancyPropertyA = command.FancyPropertyA,
            FancyPropertyB = command.FancyPropertyB,
            FancyStringCollection = command.FancyStringCollection.ToList(),
        };
    }
}
