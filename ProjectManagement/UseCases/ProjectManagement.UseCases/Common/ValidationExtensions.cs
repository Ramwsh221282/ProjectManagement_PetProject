using FluentValidation;
using FluentValidation.Results;
using ProjectManagement.Domain.Utilities;

namespace ProjectManagement.UseCases.Common;

public static class ValidationExtensions
{
    public static IRuleBuilderOptionsConditions<T, TProperty> MustBeValid<T, TProperty>(
        this IRuleBuilderInitial<T, TProperty> builder,
        Func<TProperty, Result> resultFactory
    )
    {
        IRuleBuilderOptionsConditions<T, TProperty> validator = builder.Custom(
            (property, context) =>
            {
                Result result = resultFactory(property);
                if (result.IsFailure)
                {
                    context.AddFailure(CreateFailure(result.OnError));
                }
            }
        );
        return validator;
    }

    public static IRuleBuilderOptionsConditions<T, TProperty?> MustBeValidIfProvided<T, TProperty>(
        this IRuleBuilderInitial<T, TProperty?> builder,
        Func<TProperty, Result> resultFactory
    )
    {
        IRuleBuilderOptionsConditions<T, TProperty?> validator = builder.Custom(
            (property, context) =>
            {
                if (property != null)
                {
                    Result result = resultFactory(property);
                    if (result.IsFailure)
                    {
                        context.AddFailure(CreateFailure(result.OnError));
                    }
                }
            }
        );
        return validator;
    }

    public static IRuleBuilderOptionsConditions<T, IEnumerable<TProperty>> EachMustBeValid<
        T,
        TProperty
    >(
        this IRuleBuilderInitial<T, IEnumerable<TProperty>> builder,
        Func<TProperty, Result>[] resultFactories
    )
    {
        IRuleBuilderOptionsConditions<T, IEnumerable<TProperty>> validator = builder.Custom(
            (property, context) =>
            {
                foreach (TProperty entry in property)
                {
                    foreach (Func<TProperty, Result> @delegate in resultFactories)
                    {
                        Result result = @delegate(entry);
                        if (result.IsFailure)
                        {
                            context.AddFailure(CreateFailure(result.OnError));
                        }
                    }
                }
            }
        );
        return validator;
    }

    public static Result<T> ToError<T>(this ValidationResult result)
        where T : notnull
    {
        if (result.IsValid)
            throw new InvalidOperationException(
                "Валидация была успешной. Нельзя преобразовать в Result Error"
            );

        List<ValidationFailure> errors = result.Errors;
        string message = string.Join(", ", errors.Select(e => e.ErrorMessage));
        return Error.Validation(message);
    }

    private static ValidationFailure CreateFailure(Error error)
    {
        return new ValidationFailure()
        {
            ErrorCode = error.Type.ToString(),
            ErrorMessage = error.Message,
        };
    }
}
