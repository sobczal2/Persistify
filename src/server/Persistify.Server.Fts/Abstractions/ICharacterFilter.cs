namespace Persistify.Server.Fts.Abstractions;

public interface ICharacterFilter
{
    string Code { get; }
    string Filter(string value);
}
