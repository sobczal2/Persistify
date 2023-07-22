using System;
using System.Collections.Generic;
using Persistify.Management.Types.Abstractions;
using Persistify.Management.Types.Bool;
using Persistify.Management.Types.Fts;
using Persistify.Management.Types.Number;
using Persistify.Management.Types.Text;

namespace Persistify.Management.Types.Factories;

public class TypeManagerFactory : ITypeManagerFactory
{
    private readonly BoolManager _boolManager;
    private readonly FtsManager _ftsManager;
    private readonly NumberManager _numberManager;
    private readonly TextManager _textManager;

    public TypeManagerFactory()
    {
        _textManager = new TextManager();
        _ftsManager = new FtsManager();
        _numberManager = new NumberManager();
        _boolManager = new BoolManager();
    }

    public IEnumerable<ITypeManager> GetAll()
    {
        return new List<ITypeManager> { _textManager, _ftsManager, _numberManager, _boolManager };
    }

    public ITypeManager<TQuery, THit> Get<TQuery, THit>()
        where TQuery : ITypeManagerQuery
        where THit : ITypeManagerHit
    {
        switch ((typeof(TQuery), typeof(THit)))
        {
            case var (query, hit) when query == typeof(TextManagerQuery) && hit == typeof(TextManagerHit):
                return (ITypeManager<TQuery, THit>)_textManager;
            case var (query, hit) when query == typeof(FtsManagerQuery) && hit == typeof(FtsManagerHit):
                return (ITypeManager<TQuery, THit>)_ftsManager;
            case var (query, hit) when query == typeof(NumberManagerQuery) && hit == typeof(NumberManagerHit):
                return (ITypeManager<TQuery, THit>)_numberManager;
            case var (query, hit) when query == typeof(BoolManagerQuery) && hit == typeof(BoolManagerHit):
                return (ITypeManager<TQuery, THit>)_boolManager;
            default:
                throw new NotSupportedException();
        }
    }
}
