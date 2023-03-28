using System;

namespace Persistify.Validators.Common;

public class MicrosoftDIValidatorFactory : IValidatorFactory
{
    private readonly IServiceProvider _serviceProvider;

    public MicrosoftDIValidatorFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IValidator<T>? GetValidator<T>()
    {
        return (IValidator<T>?)_serviceProvider.GetService(typeof(IValidator<T>));
    }
}
