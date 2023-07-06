using BenchmarkDotNet.Attributes;
using Persistify.Management.Number.Manager;
using Persistify.Management.Score;
using Persistify.Protos.Documents;
using Persistify.TestHelpers.Documents;
using NumberQuery = Persistify.Management.Number.Search.NumberQuery;

namespace Persistify.Management.Tests.Bench.Number;

[MemoryDiagnoser]
public class IntervalTreeNumberManagerBenchmark
{
    private const uint TextFields = 3;
    private const uint NumberFields = 3;
    private const uint BoolFields = 3;

    private INumberManager _emptyBoolManager;
    private INumberManager _1kNumberManager;
    private INumberManager _10kNumberManager;
    private INumberManager _100kNumberManager;
    private Document _benchmarkDocument;
    private IScoreCalculator _scoreCalculator;


    [GlobalSetup]
    public void GlobalSetup()
    {
        _scoreCalculator = new LinearScoreCalculator();
        var documentGenerator = new DocumentGenerator();
        _emptyBoolManager = new IntervalTreeNumberManager(_scoreCalculator);
        _1kNumberManager = GenerateNumberManagerWithItems(1000, _scoreCalculator, documentGenerator);
        _10kNumberManager = GenerateNumberManagerWithItems(10000, _scoreCalculator, documentGenerator);
        _100kNumberManager = GenerateNumberManagerWithItems(100000, _scoreCalculator, documentGenerator);
        _benchmarkDocument = documentGenerator.GenerateDocument(TextFields, NumberFields, BoolFields);
    }

    private static INumberManager GenerateNumberManagerWithItems(int itemsCount, IScoreCalculator scoreCalculator,
        DocumentGenerator documentGenerator)
    {
        var numberManager = new IntervalTreeNumberManager(scoreCalculator);
        for (var i = 0; i < itemsCount; i++)
        {
            numberManager.Add("template", documentGenerator.GenerateDocument(TextFields, NumberFields, BoolFields),
                (ulong)i);
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
        _1kNumberManager.Search("template", new NumberQuery()
        {
            FieldName = "number_field0",
            MinValue = -10,
            MaxValue = 10
        }, _scoreCalculator);
    }
    
    [Benchmark]
    public void SearchIn10K()
    {
        _10kNumberManager.Search("template", new NumberQuery()
        {
            FieldName = "number_field0",
            MinValue = -10,
            MaxValue = 10
        }, _scoreCalculator);
    }
    
    [Benchmark]
    public void SearchIn100K()
    {
        _100kNumberManager.Search("template", new NumberQuery()
        {
            FieldName = "number_field0",
            MinValue = -10,
            MaxValue = 10
        }, _scoreCalculator);
    }
}
