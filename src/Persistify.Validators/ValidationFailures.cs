using Persistify.Validators.Core;

namespace Persistify.Validators;

public static class ValidationFailures
{
    public static readonly ValidationFailure TypeNameEmpty = new("type-name-empty");
    public static readonly ValidationFailure FieldPathEmpty = new("field-path-empty");
    public static readonly ValidationFailure TypeNotFound = new("type-not-found");
    public static readonly ValidationFailure TypeFieldsEmpty = new("type-fields-empty");

    public static readonly ValidationFailure PageNumberInvalid = new("page-number-invalid");
    public static readonly ValidationFailure PageSizeInvalid = new("page-size-invalid");
    public static readonly ValidationFailure DataInvalid = new("data-invalid");
    public static readonly ValidationFailure RequiredFieldMissing = new("required-field-missing");
    public static readonly ValidationFailure FieldTypeInvalid = new("field-type-invalid");
    public static readonly ValidationFailure IdInvalid = new("id-invalid");
    public static readonly ValidationFailure NumberRangeInvalid = new("number-range-invalid");
    public static readonly ValidationFailure DocumentNotFound = new("document-not-found");
    public static readonly ValidationFailure TypeAlreadyExists = new("type-already-exists");
    public static readonly ValidationFailure QueryEmpty = new("query-empty");
    public static readonly ValidationFailure PaginationEmpty = new("pagination-empty");
}
