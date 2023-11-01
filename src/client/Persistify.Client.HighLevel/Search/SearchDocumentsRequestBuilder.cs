using System;
using System.Reflection;
using Persistify.Client.HighLevel.Attributes;
using Persistify.Client.HighLevel.Core;
using Persistify.Client.HighLevel.Exceptions;
using Persistify.Client.HighLevel.Search.Queries;
using Persistify.Dtos.Common;
using Persistify.Requests.Documents;

namespace Persistify.Client.HighLevel.Search;

public class SearchDocumentsRequestBuilder<TDocument>
    where TDocument : class
{
    private readonly IPersistifyHighLevelClient _persistifyHighLevelClient;
    private readonly string _templateName;
    private PaginationDto _paginationDto;
    private SearchQueryDtoBuilder<TDocument>? _searchQueryDtoBuilder;

    public SearchDocumentsRequestBuilder(IPersistifyHighLevelClient persistifyHighLevelClient)
    {
        _persistifyHighLevelClient = persistifyHighLevelClient;
        var documentType = typeof(TDocument);
        var documentAttribute = documentType.GetCustomAttribute<PersistifyDocumentAttribute>();
        if (documentAttribute == null)
        {
            throw new PersistifyHighLevelClientException(
                $"Document type {documentType.FullName} does not have {nameof(PersistifyDocumentAttribute)}"
            );
        }

        _templateName =
            documentAttribute.Name
            ?? documentType.FullName
            ?? throw new InvalidOperationException();

        _paginationDto = new PaginationDto { PageNumber = 0, PageSize = 10 };
    }

    public SearchDocumentsRequestBuilder<TDocument> WithPagination(int pageNumber, int pageSize)
    {
        _paginationDto = new PaginationDto { PageNumber = pageNumber, PageSize = pageSize };
        return this;
    }

    public SearchDocumentsRequestBuilder<TDocument> WithSearchQuery(
        Func<SearchQueryDtoBuilder<TDocument>, SearchQueryDtoBuilder<TDocument>> searchQueryAction
    )
    {
        var searchQueryBuilder = new SearchQueryDtoBuilder<TDocument>(_persistifyHighLevelClient);
        _searchQueryDtoBuilder = searchQueryAction(searchQueryBuilder);
        return this;
    }

    internal SearchDocumentsRequest Build()
    {
        var searchQueryDto = _searchQueryDtoBuilder?.Build();
        return new SearchDocumentsRequest
        {
            TemplateName = _templateName,
            PaginationDto = _paginationDto,
            SearchQueryDto =
                searchQueryDto
                ?? throw new PersistifyHighLevelClientException("Search query is not set")
        };
    }
}
