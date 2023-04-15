namespace Persistify.Pipeline.Contexts.Abstractions;

public enum PipelineStep
{
    None,
    Validation,
    TypeStore,
    DocumentStore,
    IndexStore
}