// using System;
// using System.Reflection;
// using Persistify.Domain.Templates;
//
// namespace Persistify.Client.Objects.Attributes;
//
// public static class PersistifyAttributeExtensions
// {
//     public static string GetFieldName(this MemberInfo memberInfo)
//     {
//         var persistifyFieldAttribute = memberInfo.GetCustomAttribute<PersistifyFieldAttribute>();
//         return persistifyFieldAttribute?.FieldName ?? memberInfo.Name;
//     }
//
//     public static FieldType? GetFieldType(this MemberInfo memberInfo)
//     {
//         var persistifyFieldAttribute = memberInfo.GetCustomAttribute<PersistifyFieldAttribute>();
//         return persistifyFieldAttribute?.FieldType;
//     }
//
//     public static bool IsRequired(this MemberInfo memberInfo)
//     {
//         var persistifyFieldAttribute = memberInfo.GetCustomAttribute<PersistifyFieldAttribute>();
//         return persistifyFieldAttribute?.Required ?? false;
//     }
//
//     public static string? GetAnalyzerDescriptorName(this MemberInfo memberInfo)
//     {
//         var persistifyTextFieldAttribute = memberInfo.GetCustomAttribute<PersistifyTextFieldAttribute>();
//         return persistifyTextFieldAttribute?.AnalyzerDescriptorName;
//     }
//
//     public static bool IsPersistifyField(this MemberInfo memberInfo)
//     {
//         return memberInfo.GetCustomAttribute<PersistifyFieldAttribute>() != null;
//     }
//
//     public static bool IsPersistifyTemplate(this MemberInfo memberInfo)
//     {
//         return memberInfo.GetCustomAttribute<PersistifyTemplateAttribute>() != null;
//     }
//
//     public static string GetTemplateName(this Type type)
//     {
//         var persistifyTemplateAttribute = type.GetCustomAttribute<PersistifyTemplateAttribute>();
//         return persistifyTemplateAttribute?.TemplateName ?? type.FullName ?? type.Name;
//     }
// }


