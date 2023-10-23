// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Reflection;
// using Persistify.Domain.Documents;
// using Persistify.Domain.Templates;
//
// namespace Persistify.Client.Objects.Converters;
//
// public class DefaultFieldValueConverterStore : IFieldValueConverterStore
// {
//     private readonly Dictionary<(Type, Type), object> _fromToConverterMap;
//
//     public DefaultFieldValueConverterStore(
//         params Assembly[] assemblies
//     )
//     {
//         var converterTypes = Assembly
//             .GetExecutingAssembly()
//             .GetTypes()
//             .Concat(assemblies.SelectMany(assembly => assembly.GetTypes()))
//             .Where(type => type is { IsClass: true, IsAbstract: false }
//                            && type.GetInterfaces()
//                                .Any(i => i.IsGenericType
//                                          && i.GetGenericTypeDefinition() == typeof(IFieldValueConverter<,>)))
//             .ToList();
//
//         _fromToConverterMap = new Dictionary<(Type, Type), object>();
//
//         foreach (var converterType in converterTypes)
//         {
//             var converterInterface = converterType.GetInterfaces().First(type =>
//                 type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IFieldValueConverter<,>));
//             var converterInterfaceArguments = converterInterface.GetGenericArguments();
//             var fromType = converterInterfaceArguments[0];
//             var toType = converterInterfaceArguments[1];
//             var converterInstance = Activator.CreateInstance(converterType) ?? throw new InvalidOperationException();
//
//             _fromToConverterMap.Add((fromType, toType), converterInstance);
//         }
//     }
//
//     public object GetConverter(Type fromType, FieldType toType)
//     {
//         return toType switch
//         {
//             FieldType.Bool => _fromToConverterMap[(fromType, typeof(BoolFieldValue))],
//             FieldType.Number => _fromToConverterMap[(fromType, typeof(NumberFieldValue))],
//             FieldType.Text => _fromToConverterMap[(fromType, typeof(TextFieldValue))],
//             _ => throw new ArgumentOutOfRangeException(nameof(toType), toType, null)
//         };
//     }
// }



