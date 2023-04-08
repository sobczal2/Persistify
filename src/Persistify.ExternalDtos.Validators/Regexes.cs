using System.Text.RegularExpressions;

namespace Persistify.ExternalDtos.Validators;

public static class Regexes
{
    private const string JsonPathRegex = @"^(\$|\$\.|)[a-zA-Z_][a-zA-Z0-9_]*(\.[a-zA-Z_][a-zA-Z0-9_]*|\[\d+\])*$";

    private const string TypeNameRegex =
        @"^(?:[a-zA-Z_][a-zA-Z0-9_]*\.)*[a-zA-Z_][a-zA-Z0-9_]*(?:\+[a-zA-Z_][a-zA-Z0-9_]*)*$";

    private const string QueryRegex = @"^[a-zA-Z0-9_ ]+$";


    public static readonly Regex JsonPath = new(JsonPathRegex, RegexOptions.Compiled);
    public static readonly Regex TypeName = new(TypeNameRegex, RegexOptions.Compiled);
    public static readonly Regex Query = new(QueryRegex, RegexOptions.Compiled);
}