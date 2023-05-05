namespace Persistify.Indexes.Common;

public interface ISearchPredicate
{
    string TypeName { get; }
    string Path { get; }
}