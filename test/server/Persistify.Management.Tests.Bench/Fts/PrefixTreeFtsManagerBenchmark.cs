using BenchmarkDotNet.Attributes;
using Persistify.Management.Fts.Manager;
using Persistify.Management.Fts.Token;
using Persistify.Management.Score;
using Persistify.Protos.Documents;
using Persistify.TestHelpers.Documents;
using FtsQuery = Persistify.Management.Fts.Search.FtsQuery;

namespace Persistify.Management.Tests.Bench.Fts;

[MemoryDiagnoser]
public class PrefixTreeFtsManagerBenchmark
{
    private const uint TextFields = 3;
    private const uint NumberFields = 3;
    private const uint BoolFields = 3;
    private IFtsManager _100kBoolManager;
    private IFtsManager _10kBoolManager;
    private IFtsManager _1kBoolManager;
    private Document _benchmarkDocument;

    private IFtsManager _emptyBoolManager;
    private IScoreCalculator _scoreCalculator;
    private ITokenizer _tokenizer;


    [GlobalSetup]
    public void GlobalSetup()
    {
        _tokenizer = new DefaultTokenizer();
        _scoreCalculator = new LinearScoreCalculator();
        var documentGenerator = new DocumentGenerator();
        _emptyBoolManager = new PrefixTreeFtsManager(_tokenizer, _scoreCalculator);
        _1kBoolManager = GenerateFtsManagerWithItems(1000, _tokenizer, _scoreCalculator, documentGenerator);
        _10kBoolManager = GenerateFtsManagerWithItems(10000, _tokenizer, _scoreCalculator, documentGenerator);
        _100kBoolManager = GenerateFtsManagerWithItems(100000, _tokenizer, _scoreCalculator, documentGenerator);
        _benchmarkDocument = documentGenerator.GenerateDocument(TextFields, NumberFields, BoolFields);
    }

    private static IFtsManager GenerateFtsManagerWithItems(int itemsCount, ITokenizer tokenizer,
        IScoreCalculator scoreCalculator,
        DocumentGenerator documentGenerator)
    {
        var ftsManager = new PrefixTreeFtsManager(tokenizer, scoreCalculator);
        for (var i = 0; i < itemsCount; i++)
        {
            ftsManager.Add("template", documentGenerator.GenerateDocument(TextFields, NumberFields, BoolFields),
                (ulong)i);
        }

        return ftsManager;
    }

    [Benchmark]
    public void Add()
    {
        _emptyBoolManager.Add("template", _benchmarkDocument, 1);
    }

    [Benchmark]
    public void SearchIn1K_CaseSensitive()
    {
        _1kBoolManager.Search("template",
            new FtsQuery { FieldName = "text_field0", Value = "template", Exact = false, CaseSensitive = true },
            _scoreCalculator);
    }

    [Benchmark]
    public void SearchIn1K_CaseInsensitive()
    {
        _1kBoolManager.Search("template",
            new FtsQuery { FieldName = "text_field0", Value = "template", Exact = false, CaseSensitive = false },
            _scoreCalculator);
    }

    [Benchmark]
    public void SearchIn10K_CaseSensitive()
    {
        _10kBoolManager.Search("template",
            new FtsQuery { FieldName = "text_field0", Value = "template", Exact = false, CaseSensitive = true },
            _scoreCalculator);
    }

    [Benchmark]
    public void SearchIn10K_CaseInsensitive()
    {
        _10kBoolManager.Search("template",
            new FtsQuery { FieldName = "text_field0", Value = "template", Exact = false, CaseSensitive = false },
            _scoreCalculator);
    }

    [Benchmark]
    public void SearchIn100K_CaseSensitive()
    {
        _100kBoolManager.Search("template",
            new FtsQuery { FieldName = "text_field0", Value = "template", Exact = false, CaseSensitive = true },
            _scoreCalculator);
    }

    [Benchmark]
    public void SearchIn100K_CaseInsensitive()
    {
        _100kBoolManager.Search("template",
            new FtsQuery { FieldName = "text_field0", Value = "template", Exact = false, CaseSensitive = false },
            _scoreCalculator);
    }
}
