using BenchmarkDotNet.Attributes;
using Persistify.Management.Bool.Manager;
using Persistify.Management.Score;
using Persistify.Protos.Documents;
using Persistify.TestHelpers.Documents;
using BoolQuery = Persistify.Management.Bool.Search.BoolQuery;

namespace Persistify.Management.Tests.Bench.Bool;

[MemoryDiagnoser]
public class HashSetBoolManagerBenchmark
{
    private const uint TextFields = 3;
    private const uint NumberFields = 3;
    private const uint BoolFields = 3;
    private IBoolManager _100kBoolManager;
    private IBoolManager _10kBoolManager;
    private IBoolManager _1kBoolManager;
    private Document _benchmarkDocument;

    private IBoolManager _emptyBoolManager;
    private IScoreCalculator _scoreCalculator;


    [GlobalSetup]
    public void GlobalSetup()
    {
        _scoreCalculator = new LinearScoreCalculator();
        var documentGenerator = new DocumentGenerator();
        _emptyBoolManager = new HashSetBoolManager(_scoreCalculator);
        _1kBoolManager = GenerateBoolManagerWithItems(1000, _scoreCalculator, documentGenerator);
        _10kBoolManager = GenerateBoolManagerWithItems(10000, _scoreCalculator, documentGenerator);
        _100kBoolManager = GenerateBoolManagerWithItems(100000, _scoreCalculator, documentGenerator);
        _benchmarkDocument = documentGenerator.GenerateDocument(TextFields, NumberFields, BoolFields);
    }

    private static IBoolManager GenerateBoolManagerWithItems(int itemsCount, IScoreCalculator scoreCalculator,
        DocumentGenerator documentGenerator)
    {
        var boolManager = new HashSetBoolManager(scoreCalculator);
        for (var i = 0; i < itemsCount; i++)
        {
            boolManager.Add("template", documentGenerator.GenerateDocument(TextFields, NumberFields, BoolFields),
                (ulong)i);
        }

        return boolManager;
    }

    [Benchmark]
    public void Add()
    {
        _emptyBoolManager.Add("template", _benchmarkDocument, 1);
    }

    [Benchmark]
    public void SearchIn1K()
    {
        _1kBoolManager.Search("template", new BoolQuery { FieldName = "bool_field0", Value = true }, _scoreCalculator);
    }

    [Benchmark]
    public void SearchIn10K()
    {
        _10kBoolManager.Search("template", new BoolQuery { FieldName = "bool_field0", Value = true }, _scoreCalculator);
    }

    [Benchmark]
    public void SearchIn100K()
    {
        _100kBoolManager.Search("template", new BoolQuery { FieldName = "bool_field0", Value = true },
            _scoreCalculator);
    }
}
