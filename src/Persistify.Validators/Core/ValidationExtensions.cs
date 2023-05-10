using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace Persistify.Validators.Core;

public static class ValidationExtensions
{
    public static void ValidateAndThrow<T>(this IValidator<T> validator, T instance)
    {
        var failures = validator.Validate(instance);
        if (failures.Length > 0) throw new ValidationException(failures);
    }

    public static IServiceCollection AddValidators(this IServiceCollection services)
    {
        var validatorTypes = typeof(ValidationExtensions).Assembly
            .GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && t.GetInterfaces()
                .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IValidator<>)))
            .ToArray();

        foreach (var validatorType in validatorTypes)
        {
            var validatorInterface = validatorType.GetInterfaces()
                .Single(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IValidator<>));
            services.AddSingleton(validatorInterface, validatorType);
        }

        return services;
    }
}