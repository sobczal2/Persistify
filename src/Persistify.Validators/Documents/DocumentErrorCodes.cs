namespace Persistify.Validators.Documents;

public static class DocumentErrorCodes
{
    public const string TypeNotFound = "type-not-found";
    public const string RequiredFieldMissing = "required-field-missing";
    public const string FieldTypeMismatch = "field-type-mismatch";
    public const string InvalidJson = "invalid-json";
    public const string QueryMissing = "query-missing";
}