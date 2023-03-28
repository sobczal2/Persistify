using System.Linq;
using Persistify.Grpc.Protos;
using Persistify.Validators.Common;
using Persistify.Validators.Helpers;

namespace Persistify.Grpc.Validators.Types;

public class InitTypeRequestValidator : IValidator<InitTypeRequest>
{
    public ValidationResult Validate(InitTypeRequest obj)
    {
        return obj.TypeDefinition == null
            ? new ValidationResult(false, "Type definition is required")
            : !TypeValidationHelpers.TypeNameRegex.IsMatch(obj.TypeDefinition.Name)
                ? new ValidationResult(false, "Invalid type name")
                : obj.TypeDefinition.TypeFields.Count == 0
                    ? new ValidationResult(false, "Type definition must have at least one type field")
                    : obj.TypeDefinition.TypeFields.Count > 100
                        ? new ValidationResult(
                            false,
                            "Type definition must have at most 100 type fields"
                        )
                        : obj.TypeDefinition.TypeFields.Any(
                            typeField =>
                                !TypeValidationHelpers.TypeFieldNameRegex.IsMatch(typeField.Path)
                        )
                            ? new ValidationResult(false, "Invalid type field name")
                            : new ValidationResult(true);
    }
}
