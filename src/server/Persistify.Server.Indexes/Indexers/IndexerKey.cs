namespace Persistify.Server.Indexes.Indexers;

public record IndexerKey(IndexType IndexType, int TemplateId, string FieldName);
