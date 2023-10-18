using System.Collections.Generic;
using System.Globalization;
using Persistify.Dtos.Documents.Search;

namespace Persistify.Server.Indexes.Searches;

public class SearchMetadata
{
    private readonly Dictionary<string, string> _metadata;

    public SearchMetadata(float score)
    {
        Score = score;
        _metadata = new Dictionary<string, string>();
    }

    public float Score { get; set; }

    public void Add(string name, string value)
    {
        if (_metadata.ContainsKey(name))
        {
            _metadata[name] = $"{_metadata[name]};{value}";
        }
        else
        {
            _metadata.Add(name, value);
        }
    }

    public List<SearchMetadataDto> ToSearchMetadataList()
    {
        var searchMetadataList = new List<SearchMetadataDto>
        {
            new() { Name = "score", Value = Score.ToString(CultureInfo.InvariantCulture) }
        };
        foreach (var (name, value) in _metadata)
        {
            searchMetadataList.Add(new SearchMetadataDto { Name = name, Value = value });
        }

        return searchMetadataList;
    }
}
