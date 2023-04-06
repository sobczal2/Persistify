using Google.Protobuf.Collections;
using Persistify.Grpc.Protos;
using Persistify.Indexer.Types;

namespace Persistify.Grpc.Mappings;

public static class TypeFieldMapping
{
    public static TypeFieldProto MapToProto(this TypeField typeField)
    {
        return new TypeFieldProto
        {
            Path = typeField.Path,
        };
    }
    
    public static TypeField MapToNormal(this TypeFieldProto typeFieldProto)
    {
        return new TypeField(typeFieldProto.Path);
    }
    
    public static TypeFieldProto[] MapToProto(this TypeField[] typeFields)
    {
        var typeFieldProtos = new TypeFieldProto[typeFields.Length];
        for (var i = 0; i < typeFields.Length; i++)
        {
            typeFieldProtos[i] = typeFields[i].MapToProto();
        }
        
        return typeFieldProtos;
    }
    
    public static TypeField[] MapToNormal(this TypeFieldProto[] typeFieldProtos)
    {
        var typeFields = new TypeField[typeFieldProtos.Length];
        for (var i = 0; i < typeFieldProtos.Length; i++)
        {
            typeFields[i] = typeFieldProtos[i].MapToNormal();
        }
        
        return typeFields;
    }
    
    public static TypeField[] MapToNormal(this RepeatedField<TypeFieldProto> typeFieldProtos)
    {
        var typeFields = new TypeField[typeFieldProtos.Count];
        for (var i = 0; i < typeFieldProtos.Count; i++)
        {
            typeFields[i] = typeFieldProtos[i].MapToNormal();
        }
        
        return typeFields;
    }
}