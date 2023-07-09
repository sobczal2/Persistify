using BenchmarkDotNet.Attributes;
using Persistify.Management.Number.Manager;
using Persistify.Management.Score;
using Persistify.Protos.Documents.Shared;
using Persistify.TestHelpers.Documents;
using NumberQuery = Persistify.Management.Number.Search.NumberQuery;

namespace Persistify.Management.Tests.Bench.Number;

[MemoryDiagnoser]
public class IntervalTreeNumberManagerBenchmark
{
    private const int TextFields = 3;
    private const int NumberFields = 3;
    private const int BoolFields = 3;
    private INumberManager _100KNumberManager = null!;
    private INumberManager _10KNumberManager = null!;
    private INumberManager _1KNumberManager = null!;
    private Document _benchmarkDocument = null!;

    private INumberManager _emptyBoolManager = null!;
    private IScoreCalculator _scoreCalculator = null!;


    [GlobalSetup]
    public void GlobalSetup()
    {
        _scoreCalculator = new LinearScoreCalculator();
        var documentGenerator = new DocumentGenerator();
        _emptyBoolManager = new IntervalTreeNumberManager(_scoreCalculator);
        _1KNumberManager = GenerateNumberManagerWithItems(1000, _scoreCalculator, documentGenerator);
        _10KNumberManager = GenerateNumberManagerWithItems(10000, _scoreCalculator, documentGenerator);
        _100KNumberManager = GenerateNumberManagerWithItems(100000, _scoreCalculator, documentGenerator);
        _benchmarkDocument = documentGenerator.GenerateDocument(TextFields, NumberFields, BoolFields);
    }

    private static INumberManager GenerateNumberManagerWithItems(int itemsCount, IScoreCalculator scoreCalculator,
        DocumentGenerator documentGenerator)
    {
        var numberManager = new IntervalTreeNumberManager(scoreCalculator);
        for (var i = 0; i < itemsCount; i++)
        {
            numberManager.Add("template", documentGenerator.GenerateDocument(TextFields, NumberFields, BoolFields),
                i);
        }

        return numberManager;
    }

    [Benchmark]
    public void Add()
    {
        _emptyBoolManager.Add("template", _benchmarkDocument, 1);
    }

    [Benchmark]
    public void SearchIn1K()
    {
        _1KNumberManager.Search("template",
            new NumberQuery { FieldName = "number_field0", MinValue = -10, MaxValue = 10 }, _scoreCalculator);
    }

    [Benchmark]
    public void SearchIn10K()
    {
        _10KNumberManager.Search("template",
            new NumberQuery { FieldName = "number_field0", MinValue = -10, MaxValue = 10 }, _scoreCalculator);
    }

    [Benchmark]
    public void SearchIn100K()
    {
        _100KNumberManager.Search("template",
            new NumberQuery { FieldName = "number_field0", MinValue = -10, MaxValue = 10 }, _scoreCalculator);
    }
}
