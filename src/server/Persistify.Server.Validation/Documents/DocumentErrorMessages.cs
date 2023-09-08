namespace Persistify.Server.Validation.Documents;

public static class DocumentErrorMessages
{
    public const string NoFieldValues = "No field values";
    public const string FieldNameNotUnique = "Field name not unique";
    public const string InvalidDocumentId = "Invalid document id";
    public const string SearchNodeNull = "Search node null";
    public const string NameEmpty = "Name empty";
    public const string NameTooLong = "Name too long";
    public const string AndSearchNodeMustHaveAtLeastTwoNodes = "And search node must have at least two nodes";
    public const string OrSearchNodeMustHaveAtLeastTwoNodes = "Or search node must have at least two nodes";
    public const string NotSearchNodeMustHaveOneNode = "Not search node must have one node";
    public const string ValueEmpty = "Value empty";
}
