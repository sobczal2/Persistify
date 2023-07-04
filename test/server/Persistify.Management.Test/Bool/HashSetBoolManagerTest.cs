using System.Linq;
using System.Threading.Tasks;
using Persistify.Management.Bool.Manager;
using Persistify.Management.Common;
using Persistify.Management.Score;
using Persistify.Protos.Documents;
using Xunit;
using BoolQuery = Persistify.Management.Bool.Search.BoolQuery;

namespace Persistify.Management.Test.Bool;

public class HashSetBoolManagerTest
{
    private readonly IBoolManager _boolManager;
    
    public HashSetBoolManagerTest()
    {
        _boolManager = new HashSetBoolManager(new LinearScoreCalculator());
    }
    
    [Fact]
    public async Task AddAsync_SingleTrueFieldSingleDocument_DocumentAdded()
    {
        // Arrange
        const string templateName = "template";
        var document = new Document
        {
            BoolFields =
            {
                new BoolField()
                {
                    FieldName = "field",
                    Value = true
                }
            }
        };
        const ulong documentId = 1UL;
        
        // Act
        await _boolManager.AddAsync(templateName, document, documentId);
        
        // Assert
        var trueHashSets = _boolManager.GetTrueHashSets();
        Assert.True(trueHashSets.ContainsKey(new TemplateFieldIdentifier("template", "field")));
        Assert.Contains(documentId, trueHashSets[new TemplateFieldIdentifier("template", "field")]);
        
        var falseHashSets = _boolManager.GetFalseHashSets();
        Assert.False(falseHashSets.ContainsKey(new TemplateFieldIdentifier("template", "field")));
    }
    
    [Fact]
    public async Task AddAsync_SingleFalseFieldSingleDocument_DocumentAdded()
    {
        // Arrange
        const string templateName = "template";
        var document = new Document
        {
            BoolFields =
            {
                new BoolField()
                {
                    FieldName = "field",
                    Value = false
                }
            }
        };
        const ulong documentId = 1UL;
        
        // Act
        await _boolManager.AddAsync(templateName, document, documentId);
        
        // Assert
        var falseHashSets = _boolManager.GetFalseHashSets();
        Assert.True(falseHashSets.ContainsKey(new TemplateFieldIdentifier("template", "field")));
        Assert.Contains(documentId, falseHashSets[new TemplateFieldIdentifier("template", "field")]);
        
        var trueHashSets = _boolManager.GetTrueHashSets();
        Assert.False(trueHashSets.ContainsKey(new TemplateFieldIdentifier("template", "field")));
    }
    
    [Fact]
    public async Task AddAsync_SingleTrueFieldMultipleDocuments_DocumentsAdded()
    {
        // Arrange
        const string templateName = "template";
        var document1 = new Document
        {
            BoolFields =
            {
                new BoolField()
                {
                    FieldName = "field",
                    Value = true
                }
            }
        };
        var document2 = new Document
        {
            BoolFields =
            {
                new BoolField()
                {
                    FieldName = "field",
                    Value = true
                }
            }
        };
        const ulong documentId1 = 1UL;
        const ulong documentId2 = 2UL;
        
        // Act
        await _boolManager.AddAsync(templateName, document1, documentId1);
        await _boolManager.AddAsync(templateName, document2, documentId2);
        
        // Assert
        var trueHashSets = _boolManager.GetTrueHashSets();
        Assert.True(trueHashSets.ContainsKey(new TemplateFieldIdentifier("template", "field")));
        Assert.Contains(documentId1, trueHashSets[new TemplateFieldIdentifier("template", "field")]);
        Assert.Contains(documentId2, trueHashSets[new TemplateFieldIdentifier("template", "field")]);
        
        var falseHashSets = _boolManager.GetFalseHashSets();
        Assert.False(falseHashSets.ContainsKey(new TemplateFieldIdentifier("template", "field")));
    }
    
    [Fact]
    public async Task AddAsync_SingleFalseFieldMultipleDocuments_DocumentsAdded()
    {
        // Arrange
        const string templateName = "template";
        var document1 = new Document
        {
            BoolFields =
            {
                new BoolField()
                {
                    FieldName = "field",
                    Value = false
                }
            }
        };
        var document2 = new Document
        {
            BoolFields =
            {
                new BoolField()
                {
                    FieldName = "field",
                    Value = false
                }
            }
        };
        const ulong documentId1 = 1UL;
        const ulong documentId2 = 2UL;
        
        // Act
        await _boolManager.AddAsync(templateName, document1, documentId1);
        await _boolManager.AddAsync(templateName, document2, documentId2);
        
        // Assert
        var falseHashSets = _boolManager.GetFalseHashSets();
        Assert.True(falseHashSets.ContainsKey(new TemplateFieldIdentifier("template", "field")));
        Assert.Contains(documentId1, falseHashSets[new TemplateFieldIdentifier("template", "field")]);
        Assert.Contains(documentId2, falseHashSets[new TemplateFieldIdentifier("template", "field")]);
        
        var trueHashSets = _boolManager.GetTrueHashSets();
        Assert.False(trueHashSets.ContainsKey(new TemplateFieldIdentifier("template", "field")));
    }
    
    [Fact]
    public async Task AddAsync_MultipleFieldsMultipleDocuments_DocumentsAdded()
    {
        // Arrange
        const string templateName = "template";
        var document1 = new Document
        {
            BoolFields =
            {
                new BoolField()
                {
                    FieldName = "field1",
                    Value = true
                },
                new BoolField()
                {
                    FieldName = "field2",
                    Value = false
                }
            }
        };
        var document2 = new Document
        {
            BoolFields =
            {
                new BoolField()
                {
                    FieldName = "field1",
                    Value = true
                },
                new BoolField()
                {
                    FieldName = "field2",
                    Value = false
                }
            }
        };
        const ulong documentId1 = 1UL;
        const ulong documentId2 = 2UL;
        
        // Act
        await _boolManager.AddAsync(templateName, document1, documentId1);
        await _boolManager.AddAsync(templateName, document2, documentId2);
        
        // Assert
        var trueHashSets = _boolManager.GetTrueHashSets();
        Assert.True(trueHashSets.ContainsKey(new TemplateFieldIdentifier("template", "field1")));
        Assert.Contains(documentId1, trueHashSets[new TemplateFieldIdentifier("template", "field1")]);
        Assert.Contains(documentId2, trueHashSets[new TemplateFieldIdentifier("template", "field1")]);
        
        var falseHashSets = _boolManager.GetFalseHashSets();
        Assert.True(falseHashSets.ContainsKey(new TemplateFieldIdentifier("template", "field2")));
        Assert.Contains(documentId1, falseHashSets[new TemplateFieldIdentifier("template", "field2")]);
        Assert.Contains(documentId2, falseHashSets[new TemplateFieldIdentifier("template", "field2")]);
    }
    
    [Fact]
    public async Task SearchAsync_SingleTrueFieldSingleDocument_DocumentFound()
    {
        // Arrange
        const string templateName = "template";
        var document = new Document
        {
            BoolFields =
            {
                new BoolField()
                {
                    FieldName = "field",
                    Value = true
                }
            }
        };
        const ulong documentId = 1UL;
        await _boolManager.AddAsync(templateName, document, documentId);
        
        // Act
        var result = await _boolManager.SearchAsync(templateName, new BoolQuery(){ FieldName = "field", Value = true }); 
        
        // Assert
        Assert.Single(result);
        Assert.Equal(documentId, result.Single().Id);
    }
    
    [Fact]
    public async Task SearchAsync_SingleFalseFieldSingleDocument_DocumentFound()
    {
        // Arrange
        const string templateName = "template";
        var document = new Document
        {
            BoolFields =
            {
                new BoolField()
                {
                    FieldName = "field",
                    Value = false
                }
            }
        };
        const ulong documentId = 1UL;
        await _boolManager.AddAsync(templateName, document, documentId);
        
        // Act
        var result = await _boolManager.SearchAsync(templateName, new BoolQuery(){ FieldName = "field", Value = false }); 
        
        // Assert
        Assert.Single(result);
        Assert.Equal(documentId, result.Single().Id);
    }
    
    [Fact]
    public async Task SearchAsync_SingleTrueFieldMultipleDocuments_DocumentsFound()
    {
        // Arrange
        const string templateName = "template";
        var document1 = new Document
        {
            BoolFields =
            {
                new BoolField()
                {
                    FieldName = "field",
                    Value = true
                }
            }
        };
        var document2 = new Document
        {
            BoolFields =
            {
                new BoolField()
                {
                    FieldName = "field",
                    Value = true
                }
            }
        };
        const ulong documentId1 = 1UL;
        const ulong documentId2 = 2UL;
        await _boolManager.AddAsync(templateName, document1, documentId1);
        await _boolManager.AddAsync(templateName, document2, documentId2);
        
        // Act
        var result = await _boolManager.SearchAsync(templateName, new BoolQuery(){ FieldName = "field", Value = true }); 
        
        // Assert
        Assert.Equal(2, result.Count());
        Assert.Contains(result, x => x.Id == documentId1);
        Assert.Contains(result, x => x.Id == documentId2);
    }
    
    [Fact]
    public async Task SearchAsync_SingleFalseFieldMultipleDocuments_DocumentsFound()
    {
        // Arrange
        const string templateName = "template";
        var document1 = new Document
        {
            BoolFields =
            {
                new BoolField()
                {
                    FieldName = "field",
                    Value = false
                }
            }
        };
        var document2 = new Document
        {
            BoolFields =
            {
                new BoolField()
                {
                    FieldName = "field",
                    Value = false
                }
            }
        };
        const ulong documentId1 = 1UL;
        const ulong documentId2 = 2UL;
        await _boolManager.AddAsync(templateName, document1, documentId1);
        await _boolManager.AddAsync(templateName, document2, documentId2);
        
        // Act
        var result = await _boolManager.SearchAsync(templateName, new BoolQuery(){ FieldName = "field", Value = false }); 
        
        // Assert
        Assert.Equal(2, result.Count());
        Assert.Contains(result, x => x.Id == documentId1);
        Assert.Contains(result, x => x.Id == documentId2);
    }
    
    [Fact]
    public async Task SearchAsync_SingleMixedFieldMultipleDocuments_DocumentsFound()
    {
        // Arrange
        const string templateName = "template";
        var document1 = new Document
        {
            BoolFields =
            {
                new BoolField()
                {
                    FieldName = "field",
                    Value = true
                }
            }
        };
        var document2 = new Document
        {
            BoolFields =
            {
                new BoolField()
                {
                    FieldName = "field",
                    Value = false
                }
            }
        };
        const ulong documentId1 = 1UL;
        const ulong documentId2 = 2UL;
        await _boolManager.AddAsync(templateName, document1, documentId1);
        await _boolManager.AddAsync(templateName, document2, documentId2);
        
        // Act
        var trueResult = await _boolManager.SearchAsync(templateName, new BoolQuery(){ FieldName = "field", Value = true });
        var falseResult = await _boolManager.SearchAsync(templateName, new BoolQuery(){ FieldName = "field", Value = false });
        
        // Assert
        Assert.Single(trueResult);
        Assert.Single(falseResult);
        Assert.Equal(documentId1, trueResult.Single().Id);
        Assert.Equal(documentId2, falseResult.Single().Id);
    }
    
    [Fact]
    public async Task SearchAsync_MultipleMixedFieldsMultipleDocuments_DocumentsFound()
    {
        // Arrange
        const string templateName = "template";
        var document1 = new Document
        {
            BoolFields =
            {
                new BoolField()
                {
                    FieldName = "field1",
                    Value = true
                },
                new BoolField()
                {
                    FieldName = "field2",
                    Value = false
                }
            }
        };
        var document2 = new Document
        {
            BoolFields =
            {
                new BoolField()
                {
                    FieldName = "field1",
                    Value = false
                },
                new BoolField()
                {
                    FieldName = "field2",
                    Value = true
                }
            }
        };
        const ulong documentId1 = 1UL;
        const ulong documentId2 = 2UL;
        await _boolManager.AddAsync(templateName, document1, documentId1);
        await _boolManager.AddAsync(templateName, document2, documentId2);
        
        // Act
        var trueResult = await _boolManager.SearchAsync(templateName, new BoolQuery(){ FieldName = "field1", Value = true });
        var falseResult = await _boolManager.SearchAsync(templateName, new BoolQuery(){ FieldName = "field2", Value = false });
        
        // Assert
        Assert.Single(trueResult);
        Assert.Single(falseResult);
        Assert.Equal(documentId1, trueResult.Single().Id);
        Assert.Equal(documentId1, falseResult.Single().Id);
    }
    
    [Fact]
    public async Task DeleteAsync_SingleTrueFieldSingleDocument_DocumentDeleted()
    {
        // Arrange
        const string templateName = "template";
        var document = new Document
        {
            BoolFields =
            {
                new BoolField()
                {
                    FieldName = "field",
                    Value = true
                }
            }
        };
        const ulong documentId = 1UL;
        await _boolManager.AddAsync(templateName, document, documentId);
        
        // Act
        await _boolManager.DeleteAsync(templateName, documentId);
        var result = await _boolManager.SearchAsync(templateName, new BoolQuery(){ FieldName = "field", Value = true }); 
        
        // Assert
        Assert.Empty(result);
    }
    
    [Fact]
    public async Task DeleteAsync_SingleFalseFieldSingleDocument_DocumentAndHashSetDeleted()
    {
        // Arrange
        const string templateName = "template";
        var document = new Document
        {
            BoolFields =
            {
                new BoolField()
                {
                    FieldName = "field",
                    Value = false
                }
            }
        };
        const ulong documentId = 1UL;
        await _boolManager.AddAsync(templateName, document, documentId);
        
        // Act
        await _boolManager.DeleteAsync(templateName, documentId);
        var hashSet = _boolManager.GetHashSet(templateName, "field", false);
        
        // Assert
        Assert.Null(hashSet);
    }
    
    [Fact]
    public async Task DeleteAsync_SingleTrueFieldMultipleDocuments_DocumentDeleted()
    {
        // Arrange
        const string templateName = "template";
        var document1 = new Document
        {
            BoolFields =
            {
                new BoolField()
                {
                    FieldName = "field",
                    Value = true
                }
            }
        };
        var document2 = new Document
        {
            BoolFields =
            {
                new BoolField()
                {
                    FieldName = "field",
                    Value = true
                }
            }
        };
        const ulong documentId1 = 1UL;
        const ulong documentId2 = 2UL;
        await _boolManager.AddAsync(templateName, document1, documentId1);
        await _boolManager.AddAsync(templateName, document2, documentId2);
        
        // Act
        await _boolManager.DeleteAsync(templateName, documentId1);
        var result = await _boolManager.SearchAsync(templateName, new BoolQuery(){ FieldName = "field", Value = true }); 
        
        // Assert
        Assert.Single(result);
        Assert.Equal(documentId2, result.Single().Id);
    }
}
