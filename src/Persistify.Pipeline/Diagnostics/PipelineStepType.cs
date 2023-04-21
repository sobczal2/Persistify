namespace Persistify.Pipeline.Diagnostics;

public enum PipelineStepType
{
    StaticValidation,
    DynamicValidation,
    TypeStore,
    DocumentStore,
    Indexer,
    Mutation,
    Tokenizer
}