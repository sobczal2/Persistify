using System;
using Newtonsoft.Json;

namespace Persistify.Indexer.Types;

public static class TypeDefinitionExtensions
{
    public static string Serialize(this TypeDefinition typeDefinition)
    {
        return JsonConvert.SerializeObject(typeDefinition);
    }
    
    public static TypeDefinition Deserialize(this string typeDefinition)
    {
        return JsonConvert.DeserializeObject<TypeDefinition>(typeDefinition) ?? throw new Exception("Failed to deserialize type definition");
    }
}