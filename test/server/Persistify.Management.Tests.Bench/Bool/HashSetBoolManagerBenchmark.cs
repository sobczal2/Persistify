using BenchmarkDotNet.Attributes;
using Persistify.Management.Bool.Manager;
using Persistify.Management.Bool.Search;
using Persistify.Management.Score;
using Persistify.TestHelpers.Documents;

namespace Persistify.Management.Tests.Bench.Bool;

[MemoryDiagnoser]
public class HashSetBoolManagerBenchmark
{
    private const int TextFields = 3;
    private const int NumberFields = 3;
    private const int BoolFields = 3;
    private IBoolManager _100KBoolManager = null!;
    private IBoolManager _10KBoolManager = null!;
    private IBoolManager _1KBoolManager = null!;
    private Protos.Documents.Shared.Document _benchmarkDocument = null!;

    private IBoolManager _emptyBoolManager = null!;
    private IScoreCalculator _scoreCalculator = null!;


    [GlobalSetup]
    public void GlobalSetup()
    {
        _scoreCalculator = new LinearScoreCalculator();
        var documentGenerator = new DocumentGenerator();
        _emptyBoolManager = new HashSetBoolManager(_scoreCalculator);
        _1KBoolManager = GenerateBoolManagerWithItems(1000, _scoreCalculator, documentGenerator);
        _10KBoolManager = GenerateBoolManagerWithItems(10000, _scoreCalculator, documentGenerator);
        _100KBoolManager = GenerateBoolManagerWithItems(100000, _scoreCalculator, documentGenerator);
        _benchmarkDocument = documentGenerator.GenerateDocument(TextFields, NumberFields, BoolFields);
    }

    private static IBoolManager GenerateBoolManagerWithItems(int itemsCount, IScoreCalculator scoreCalculator,
        DocumentGenerator documentGenerator)
    {
        var boolManager = new HashSetBoolManager(scoreCalculator);
        for (var i = 0; i < itemsCount; i++)
        {
            boolManager.Add("template", documentGenerator.GenerateDocument(TextFields, NumberFields, BoolFields),
                i);
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
        _1KBoolManager.Search("template", new BoolQuery { FieldName = "bool_field0", Value = true }, _scoreCalculator);
    }

    [Benchmark]
    public void SearchIn10K()
    {
        _10KBoolManager.Search("template", new BoolQuery { FieldName = "bool_field0", Value = true }, _scoreCalculator);
    }

    [Benchmark]
    public void SearchIn100K()
    {
        _100KBoolManager.Search("template", new BoolQuery { FieldName = "bool_field0", Value = true },
            _scoreCalculator);
    }
}
