using Persistify.Grpc.Protos;
using Persistify.Indexer.Core;

namespace Persistify.Grpc.Mappings;

public static class DocumentMapping
{
    public static DocumentProto MapToProto(this Document document)
    {
        return new DocumentProto
        {
            Id = document.Id,
            Type = document.Type,
            Data = document.Data
        };
    }
    
    public static Document MapToNormal(this DocumentProto documentProto)
    {
        return new Document
        {
            Id = documentProto.Id,
            Type = documentProto.Type,
            Data = documentProto.Data
        };
    }
    
    public static DocumentProto[] MapToProto(this Document[] documents)
    {
        var documentProtos = new DocumentProto[documents.Length];
        for (var i = 0; i < documents.Length; i++)
        {
            documentProtos[i] = documents[i].MapToProto();
        }
        
        return documentProtos;
    }
}