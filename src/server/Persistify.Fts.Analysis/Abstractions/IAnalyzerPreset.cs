namespace Persistify.Fts.Analysis.Abstractions;

public interface IAnalyzerPreset
{
    IAnalyzer GetAnalyzer(IAnalyzerFactory analyzerFactory);
}
