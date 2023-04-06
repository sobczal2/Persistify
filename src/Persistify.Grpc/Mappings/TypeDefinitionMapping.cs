using Google.Protobuf.Collections;
using Persistify.Grpc.Protos;
using Persistify.Indexer.Types;

namespace Persistify.Grpc.Mappings;

public static class TypeDefinitionMapping
{
    public static TypeDefinitionProto MapToProto(this TypeDefinition typeDefinition)
    {
        return new TypeDefinitionProto
        {
            Name = typeDefinition.Name,
            TypeFields =
            {
                typeDefinition.TypeFields.MapToProto()
            }
        };
    }
    
    public static TypeDefinition MapToNormal(this TypeDefinitionProto typeDefinitionProto)
    {
        return new TypeDefinition(typeDefinitionProto.Name, typeDefinitionProto.TypeFields.MapToNormal());
    }
    
    public static TypeDefinitionProto[] MapToProto(this TypeDefinition[] typeDefinitions)
    {
        var typeDefinitionProtos = new TypeDefinitionProto[typeDefinitions.Length];
        for (var i = 0; i < typeDefinitions.Length; i++)
        {
            typeDefinitionProtos[i] = typeDefinitions[i].MapToProto();
        }
        
        return typeDefinitionProtos;
    }
    
    public static TypeDefinition[] MapToNormal(this TypeDefinitionProto[] typeDefinitionProtos)
    {
        var typeDefinitions = new TypeDefinition[typeDefinitionProtos.Length];
        for (var i = 0; i < typeDefinitionProtos.Length; i++)
        {
            typeDefinitions[i] = typeDefinitionProtos[i].MapToNormal();
        }
        
        return typeDefinitions;
    }
    
    public static TypeDefinition[] MapToNormal(this RepeatedField<TypeDefinitionProto> typeDefinitionProtos)
    {
        var typeDefinitions = new TypeDefinition[typeDefinitionProtos.Count];
        for (var i = 0; i < typeDefinitionProtos.Count; i++)
        {
            typeDefinitions[i] = typeDefinitionProtos[i].MapToNormal();
        }
        
        return typeDefinitions;
    }
}