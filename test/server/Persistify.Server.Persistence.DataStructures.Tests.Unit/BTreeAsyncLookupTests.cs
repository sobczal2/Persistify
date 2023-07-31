using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Persistify.Server.Persistence.Core.Tests.Unit;
using Persistify.Server.Persistence.DataStructures.Trees;
using ProtoBuf;
using Xunit;

namespace Persistify.Server.Persistence.DataStructures.Tests.Unit;

public class BTreeAsyncLookupTests
{
    private readonly BTreeAsyncLookup<double, long> _sut;

    public BTreeAsyncLookupTests()
    {
        _sut = new BTreeAsyncLookup<double, long>(new RepositoryFake<BTreeInternalNode<double>>(),
            new RepositoryFake<BTreeLeafNode<double, long>>(), new IntLinearRepositoryFake(), 100,
            Comparer<double>.Default);
    }

    [Fact]
    public async Task AddAsync_ShouldAddItem_WhenItemIsNotPresent()
    {
        // // Arrange
        // var items = new List<(double, long)>();
        // for (var i = 0; i < 10000; i++)
        // {
        //     items.Add((Random.Shared.NextDouble(), Random.Shared.Next()));
        // }
        //
        // // Act
        // foreach (var (key, value) in items)
        // {
        //     await _sut.AddAsync(key, value);
        // }
        //
        // foreach (var (key, value) in items)
        // {
        //     await _sut.RemoveAsync(key, value);
        // }
        //
        // Console.WriteLine("Done");
    }
}
