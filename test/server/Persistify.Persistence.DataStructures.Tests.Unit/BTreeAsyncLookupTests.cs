using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Persistify.Persistence.Core.Tests.Unit;
using Persistify.Server.Persistence.DataStructures.Trees;
using ProtoBuf;
using Xunit;

namespace Persistify.Persistence.DataStructures.Tests.Unit;

public class BTreeAsyncLookupTests
{
    [ProtoContract]
    private class TestItem
    {
        [ProtoMember(1)]
        public double Value { get; set; }
    }

    private readonly BTreeAsyncLookup<double, TestItem> _sut;

    public BTreeAsyncLookupTests()
    {
        _sut = new BTreeAsyncLookup<double, TestItem>(new DictionaryRepository<BTreeInternalNode<double>>(),
            new DictionaryRepository<BTreeLeafNode<double, TestItem>>(), new DictionaryLinearRepository(), 100,
            Comparer<double>.Default);
    }

    [Fact]
    public async Task AddAsync_ShouldAddItem_WhenItemIsNotPresent()
    {
        // Arrange
        var random = new Random(1);
        var item = new TestItem { Value = 1.0 };

        for (var i = 0; i < 100; i++)
        {
            var value = random.NextDouble();
            await _sut.AddAsync(value, item);
        }
    }
}
