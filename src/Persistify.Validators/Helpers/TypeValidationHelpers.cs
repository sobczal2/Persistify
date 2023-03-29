using System.Text.RegularExpressions;

namespace Persistify.Validators.Helpers;

public static class TypeValidationHelpers
{
    public static Regex TypeNameRegex = new(
        @"^(?:[a-zA-Z_][\w]*\.)*[a-zA-Z_][\w]*$",
        RegexOptions.Compiled
    );

    public static Regex TypeFieldNameRegex = new(
        @"^(?:\w+|\[\d+\])(?:\.(?:\w+|\[\d+\]))*$",
        RegexOptions.Compiled
    );
}