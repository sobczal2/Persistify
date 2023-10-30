using System.Collections.Generic;
using System.Globalization;
using Persistify.Dtos.Documents.Search;

namespace Persistify.Server.Indexes.Searches;

public class SearchMetadata
{
    private readonly Dictionary<string, SortedSet<string>> _metadata;

    public SearchMetadata(float score)
    {
        Score = score;
        _metadata = new Dictionary<string, SortedSet<string>>();
    }

    public float Score { get; set; }

    public void Add(string name, string value)
    {
        if (_metadata.TryGetValue(name, out var list))
        {
            list.Add(value);
        }
        else
        {
            _metadata.Add(name, new SortedSet<string> { value });
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
            searchMetadataList.Add(
                new SearchMetadataDto { Name = name, Value = string.Join(", ", value) }
            );
        }

        return searchMetadataList;
    }

    public SearchMetadata Merge(SearchMetadata other)
    {
        var mergedMetadata = new SearchMetadata(Score + other.Score);

        var metadataDictionary = new Dictionary<string, SortedSet<string>>(_metadata);

        foreach (var (name, value) in other._metadata)
        {
            if (metadataDictionary.TryGetValue(name, out var list))
            {
                list.UnionWith(value);
            }
            else
            {
                metadataDictionary.Add(name, value);
            }
        }

        foreach (var (name, value) in metadataDictionary)
        {
            foreach (var item in value)
            {
                mergedMetadata.Add(name, item);
            }
        }

        return mergedMetadata;
    }
}
