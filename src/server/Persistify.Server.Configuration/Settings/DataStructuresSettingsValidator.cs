using FluentValidation;

namespace Persistify.Server.Configuration.Settings;

public class DataStructuresSettingsValidator : AbstractValidator<DataStructuresSettings>
{
    public DataStructuresSettingsValidator()
    {
        RuleFor(x => x.BTreeDegree).GreaterThan(1);
    }
}
