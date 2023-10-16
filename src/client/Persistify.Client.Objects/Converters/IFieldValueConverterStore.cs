
using Persistify.Domain.Templates;

namespace Persistify.Client.Objects.Converters;

public interface IFieldValueConverterStore
{
    object GetConverter(Type fromType, FieldType toType);
}
