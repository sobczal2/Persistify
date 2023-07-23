using System.Collections.Generic;
using Persistify.Persistence.DataStructures.Trees;

namespace Persistify.Persistence.DataStructures.Tests.Unit.Trees;

public class AvlAsyncTreeTest : AsyncTreeTestBase
{
    public class LongComparer : IComparer<long>
    {
        public int Compare(long x, long y)
        {
            return x.CompareTo(y);
        }
    }
    public AvlAsyncTreeTest() : base(new AvlAsyncTree<long>(new ArrayStorageProvider<AvlAsyncTree<long>.Node>(1000), new LongComparer()))
    {
    }
}
