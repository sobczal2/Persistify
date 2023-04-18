namespace Persistify.RequestValidators.Types;

public static class TypeErrorCodes
{
    public const string TypeDefinitionEmpty = "type-definition-empty";

    public const string PathEmpty = "path-empty";
    public const string PathInvalid = "path-invalid";

    public const string NameEmpty = "name-empty";
    public const string NameInvalid = "name-invalid";
    public const string NameDuplicate = "name-duplicate";

    public const string FieldsEmpty = "fields-empty";

    public const string IdFieldPathEmpty = "id-field-path-empty";
    public const string IdFieldPathInvalid = "id-field-path-invalid";
    public const string IdFieldPathSameAsOtherFieldPath = "id-field-path-same-as-other-field-path";
}