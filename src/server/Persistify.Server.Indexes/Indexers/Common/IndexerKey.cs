namespace Persistify.Server.Indexes.Indexers.Common;

public record IndexerKey(IndexType IndexType, int TemplateId, string FieldName);
