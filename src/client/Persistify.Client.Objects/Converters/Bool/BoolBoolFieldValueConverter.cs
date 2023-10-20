// using System;
// using Persistify.Domain.Documents;
//
// namespace Persistify.Client.Objects.Converters.Bool;
//
// public class BoolBoolFieldValueConverter : IFieldValueConverter<bool, BoolFieldValue>
// {
//     public BoolFieldValue Convert(bool from, string fieldName)
//     {
//         return new BoolFieldValue { FieldName = fieldName, Value = from };
//     }
//
//     public BoolFieldValue Convert(object? from, string fieldName)
//     {
//         if (from is bool value)
//         {
//             return Convert(value, fieldName);
//         }
//
//         throw new ArgumentException($"Cannot convert {from?.GetType().Name ?? "null"} to {nameof(BoolFieldValue)}");
//     }
// }


