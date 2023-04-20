namespace Persistify.Pipeline.Diagnostics;

public enum PipelineStepType
{
    StaticValidation,
    DynamicValidation,
    TypeStore,
    DocumentStore,
    IndexStore,
    Mutation,
    Tokenizer
}