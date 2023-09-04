using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Persistify.Server.Persistence.Object;
using Persistify.Server.Serialization;
using Xunit;

namespace Persistify.Server.Persistence.Tests.Unit.Object;

public class ObjectStreamRepositoryTests
{
    public class TestClass
    {
        public int Id { get; set; }
        public string? Name { get; set; }
    }

    private Stream _mainStream;
    private Stream _offsetLengthStream;
    private int _sectorSize;

    private ObjectStreamRepository<TestClass> _sut;

    public ObjectStreamRepositoryTests()
    {
        _mainStream = new MemoryStream();
        _offsetLengthStream = new MemoryStream();
        _sectorSize = 100;
        _sut = new ObjectStreamRepository<TestClass>(
            _mainStream,
            _offsetLengthStream,
            new JsonSerializer(),
            _sectorSize
        );
    }

    [Fact]
    public void Ctor_WhenMainStreamIsNull_ThrowsArgumentNullException()
    {
        // Arrange
        Stream mainStream = null!;
        Stream offsetLengthStream = new MemoryStream();
        ISerializer serializer = new JsonSerializer();
        var sectorSize = 100;

        // Act
        var action = new Action(() =>
        {
            var unused = new ObjectStreamRepository<TestClass>(
                mainStream,
                offsetLengthStream,
                serializer,
                sectorSize
            );
        });

        // Assert
        action.Should().ThrowExactly<ArgumentNullException>();
    }

    [Fact]
    public void Ctor_WhenOffsetLengthStreamIsNull_ThrowsArgumentNullException()
    {
        // Arrange
        Stream mainStream = new MemoryStream();
        Stream offsetLengthStream = null!;
        ISerializer serializer = new JsonSerializer();
        var sectorSize = 100;

        // Act
        var action = new Action(() =>
        {
            var unused = new ObjectStreamRepository<TestClass>(
                mainStream,
                offsetLengthStream,
                serializer,
                sectorSize
            );
        });

        // Assert
        action.Should().ThrowExactly<ArgumentNullException>();
    }

    [Fact]
    public void Ctor_WhenSerializerIsNull_ThrowsArgumentNullException()
    {
        // Arrange
        Stream mainStream = new MemoryStream();
        Stream offsetLengthStream = new MemoryStream();
        ISerializer serializer = null!;
        var sectorSize = 100;

        // Act
        var action = new Action(() =>
        {
            var unused = new ObjectStreamRepository<TestClass>(
                mainStream,
                offsetLengthStream,
                serializer,
                sectorSize
            );
        });

        // Assert
        action.Should().ThrowExactly<ArgumentNullException>();
    }

    [Fact]
    public void Ctor_WhenSectorSizeIsLessThanOne_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        Stream mainStream = new MemoryStream();
        Stream offsetLengthStream = new MemoryStream();
        ISerializer serializer = new JsonSerializer();
        var sectorSize = 0;

        // Act
        var action = new Action(() =>
        {
            var unused = new ObjectStreamRepository<TestClass>(
                mainStream,
                offsetLengthStream,
                serializer,
                sectorSize
            );
        });

        // Assert
        action.Should().ThrowExactly<ArgumentOutOfRangeException>();
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task ReadAsync_WhenKeyIsLessThanZero_ThrowsArgumentOutOfRangeException(bool useLock)
    {
        // Arrange
        var key = -1;

        // Act
        var action = new Func<Task>(async () => await _sut.ReadAsync(key, useLock));

        // Assert
        await action.Should().ThrowExactlyAsync<ArgumentOutOfRangeException>();
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task ReadAsync_ObjectDoesNotExist_ReturnsNull(bool useLock)
    {
        // Arrange
        var key = 0;

        // Act
        var result = await _sut.ReadAsync(key, useLock);

        // Assert
        result.Should().BeNull();
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task ReadAsync_ObjectExists_ReturnsObject(bool useLock)
    {
        // Arrange
        var key = 0;
        var testClass = new TestClass { Id = 1, Name = "Test" };
        await _sut.WriteAsync(key, testClass, false);

        // Act
        var result = await _sut.ReadAsync(key, useLock);

        // Assert
        result.Should().BeEquivalentTo(testClass);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task ReadAsync_ObjectExistsAndMultipleObjectsAreWritten_ReturnsObject(bool useLock)
    {
        // Arrange
        await _sut.WriteAsync(0, new TestClass { Id = 0, Name = "Test0" }, false);
        var key = 1;
        var testClass = new TestClass { Id = 1, Name = "Test1" };
        await _sut.WriteAsync(key, testClass, false);
        await _sut.WriteAsync(2, new TestClass { Id = 2, Name = "Test2" }, false);

        // Act
        var result = await _sut.ReadAsync(key, useLock);

        // Assert
        result.Should().BeEquivalentTo(testClass);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task ReadRangeAsync_WhenSkipIsLessThanZero_ThrowsArgumentOutOfRangeException(bool useLock)
    {
        // Arrange
        var skip = -1;

        // Act
        var action = new Func<Task>(async () => await _sut.ReadRangeAsync(1000, skip, useLock));

        // Assert
        await action.Should().ThrowAsync<ArgumentOutOfRangeException>();
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task ReadRangeAsync_WhenTakeIsEqualToZero_ThrowsArgumentOutOfRangeException(bool useLock)
    {
        // Arrange
        var take = 0;

        // Act
        var action = new Func<Task>(async () => await _sut.ReadRangeAsync(take, 0, useLock));

        // Assert
        await action.Should().ThrowAsync<ArgumentOutOfRangeException>();
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task ReadRangeAsync_WhenSkipIsEqualToLength_ReturnsEmptyList(bool useLock)
    {
        // Arrange
        var skip = 1;
        await _sut.WriteAsync(0, new TestClass { Id = 1 }, false);

        // Act
        var result = await _sut.ReadRangeAsync(1000, skip, useLock);

        // Assert
        result.Should().BeEmpty();
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task ReadRangeAsync_WhenSkipIsGreaterThanLength_ReturnsEmptyList(bool useLock)
    {
        // Arrange
        var skip = 2;
        await _sut.WriteAsync(0, new TestClass { Id = 1 }, false);

        // Act
        var result = await _sut.ReadRangeAsync(1000, skip, useLock);

        // Assert
        result.Should().BeEmpty();
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task ReadRangeAsync_WhenTakeIsGreaterThanLength_ReturnsList(bool useLock)
    {
        // Arrange
        var take = 2;
        await _sut.WriteAsync(0, new TestClass { Id = 1 }, false);

        // Act
        var result = await _sut.ReadRangeAsync(take, 0, useLock);

        // Assert
        result.Should().HaveCount(1);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task ReadRangeAsync_WhenTakeIsLessThanLength_ReturnsList(bool useLock)
    {
        // Arrange
        var take = 1;
        await _sut.WriteAsync(0, new TestClass { Id = 1 }, false);
        await _sut.WriteAsync(1, new TestClass { Id = 2 }, false);

        // Act
        var result = await _sut.ReadRangeAsync(take, 0, useLock);

        // Assert
        result.Should().HaveCount(1);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task ReadRangeAsync_WhenSkipSkipsOverDeletedValues_ReturnsList(bool useLock)
    {
        // Arrange
        var skip = 1;
        await _sut.WriteAsync(0, new TestClass { Id = 1 }, false);
        await _sut.DeleteAsync(0, false);
        await _sut.WriteAsync(1, new TestClass { Id = 2 }, false);
        await _sut.WriteAsync(2, new TestClass { Id = 3 }, false);


        // Act
        var result = await _sut.ReadRangeAsync(1000, skip, useLock);

        // Assert
        result.Should().HaveCount(1);
        result.FirstOrDefault(x => x.Key == 2).Value.Should().BeEquivalentTo(new TestClass { Id = 3 });
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task CountAsync_WhenRepositoryIsEmpty_ReturnsZero(bool useLock)
    {
        // Arrange

        // Act
        var result = await _sut.CountAsync(useLock);

        // Assert
        result.Should().Be(0);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task CountAsync_WhenRepositoryIsNotEmpty_ReturnsCount(bool useLock)
    {
        // Arrange
        await _sut.WriteAsync(0, new TestClass { Id = 1 }, false);
        await _sut.WriteAsync(1, new TestClass { Id = 2 }, false);

        // Act
        var result = await _sut.CountAsync(useLock);

        // Assert
        result.Should().Be(2);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task CountAsync_WhenValueIsDeleted_ReturnsCount(bool useLock)
    {
        // Arrange
        await _sut.WriteAsync(0, new TestClass { Id = 1 }, false);
        await _sut.DeleteAsync(0, false);
        await _sut.WriteAsync(1, new TestClass { Id = 2 }, false);

        // Act
        var result = await _sut.CountAsync(useLock);

        // Assert
        result.Should().Be(1);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task WriteAsync_WhenKeyIsLessThanZero_ThrowsArgumentOutOfRangeException(bool useLock)
    {
        // Arrange
        var key = -1;
        var testClass = new TestClass { Id = 1, Name = "Test" };

        // Act
        var action = new Func<Task>(async () => await _sut.WriteAsync(key, testClass, useLock));

        // Assert
        await action.Should().ThrowExactlyAsync<ArgumentOutOfRangeException>();
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task WriteAsync_WhenObjectIsNull_ThrowsArgumentNullException(bool useLock)
    {
        // Arrange
        var key = 0;
        TestClass testClass = null!;

        // Act
        var action = new Func<Task>(async () => await _sut.WriteAsync(key, testClass, useLock));

        // Assert
        await action.Should().ThrowExactlyAsync<ArgumentNullException>();
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task WriteAsync_WhenObjectIsNotNull_WritesObject(bool useLock)
    {
        // Arrange
        var key = 0;
        var testClass = new TestClass { Id = 1, Name = "Test" };

        // Act
        await _sut.WriteAsync(key, testClass, useLock);

        // Assert
        var result = await _sut.ReadAsync(key, false);
        result.Should().BeEquivalentTo(testClass);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task WriteAsync_WhenOverwritingObject_WritesObject(bool useLock)
    {
        // Arrange
        var key = 0;
        var testClass = new TestClass { Id = 1, Name = "Test" };
        await _sut.WriteAsync(key, testClass, false);
        var testClass2 = new TestClass { Id = 2, Name = "Test2" };

        // Act
        await _sut.WriteAsync(key, testClass2, useLock);

        // Assert
        var result = await _sut.ReadAsync(key, false);
        result.Should().BeEquivalentTo(testClass2);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task WriteAsync_WhenMultipleObjectsAreWritten_ExtendsStreamToLengthDivisibleBySectorSize(bool useLock)
    {
        // Arrange
        var key0 = 0;
        var testClass0 = new TestClass { Id = 0, Name = "Test0" };
        await _sut.WriteAsync(key0, testClass0, false);
        var key1 = 1;
        var testClass1 = new TestClass { Id = 1, Name = "Test1" };
        await _sut.WriteAsync(key1, testClass1, false);
        var key2 = 2;
        var testClass2 = new TestClass { Id = 2, Name = "Test2" };

        // Act
        await _sut.WriteAsync(key2, testClass2, useLock);

        // Assert
        _mainStream.Length.Should().Be(3 * _sectorSize);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task
        WriteAsync_WhenOverridingNotLastValueWithLongerValue_WritesValueAndExtendsStreamToLengthDivisibleBySectorSize(bool useLock)
    {
        // Arrange
        var key0 = 0;
        var testClass0 = new TestClass { Id = 0, Name = "Test0" };
        await _sut.WriteAsync(key0, testClass0, false);
        var key1 = 1;
        var testClass1 = new TestClass { Id = 1, Name = "Test1" };
        await _sut.WriteAsync(key1, testClass1, false);
        var key2 = 2;
        var testClass2 = new TestClass { Id = 2, Name = "Test2" };
        await _sut.WriteAsync(key2, testClass2, false);
        var key1New = 1;
        var testClass1New = new TestClass { Id = 1, Name = new string('a', 100) };

        // Act
        await _sut.WriteAsync(key1New, testClass1New, useLock);

        // Assert
        _mainStream.Length.Should().Be(5 * _sectorSize);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task
        WriteAsync_WhenOverridingLastValueWithLongerValue_WritesValueAndExtendsStreamToLengthDivisibleBySectorSize(bool useLock)
    {
        // Arrange
        var key0 = 0;
        var testClass0 = new TestClass { Id = 0, Name = "Test0" };
        await _sut.WriteAsync(key0, testClass0, false);
        var key1 = 1;
        var testClass1 = new TestClass { Id = 1, Name = "Test1" };
        await _sut.WriteAsync(key1, testClass1, false);
        var key2 = 2;
        var testClass2 = new TestClass { Id = 2, Name = "Test2" };
        await _sut.WriteAsync(key2, testClass2, false);
        var key2New = 2;
        var testClass2New = new TestClass { Id = 2, Name = new string('a', 100) };

        // Act
        await _sut.WriteAsync(key2New, testClass2New, useLock);

        // Assert
        _mainStream.Length.Should().Be(4 * _sectorSize);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task WriteAsync_WhenOverridingLastValueWithShorterValue_WritesValueAndDoesNotExtendStream(bool useLock)
    {
        // Arrange
        var key0 = 0;
        var testClass0 = new TestClass { Id = 0, Name = "Test0" };
        await _sut.WriteAsync(key0, testClass0, false);
        var key1 = 1;
        var testClass1 = new TestClass { Id = 1, Name = "Test1" };
        await _sut.WriteAsync(key1, testClass1, false);
        var key2 = 2;
        var testClass2 = new TestClass { Id = 2, Name = "Test2" };
        await _sut.WriteAsync(key2, testClass2, false);
        var key2New = 2;
        var testClass2New = new TestClass { Id = 2, Name = "a" };

        // Act
        await _sut.WriteAsync(key2New, testClass2New, useLock);

        // Assert
        _mainStream.Length.Should().Be(3 * _sectorSize);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task WriteAsync_WhenOverridingNotLastValueWithShorterValue_WritesValueAndDoesNotExtendStream(bool useLock)
    {
        // Arrange
        var key0 = 0;
        var testClass0 = new TestClass { Id = 0, Name = "Test0" };
        await _sut.WriteAsync(key0, testClass0, false);
        var key1 = 1;
        var testClass1 = new TestClass { Id = 1, Name = "Test1" };
        await _sut.WriteAsync(key1, testClass1, false);
        var key2 = 2;
        var testClass2 = new TestClass { Id = 2, Name = "Test2" };
        await _sut.WriteAsync(key2, testClass2, false);
        var key1New = 1;
        var testClass1New = new TestClass { Id = 1, Name = "a" };

        // Act
        await _sut.WriteAsync(key1New, testClass1New, useLock);

        // Assert
        _mainStream.Length.Should().Be(3 * _sectorSize);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task WriteAsync_WhenOverridingLastValueWithShorterValue_ShrinksStreamToLengthDivisibleBySectorSize(bool useLock)
    {
        // Arrange
        var key = 0;
        var value = new TestClass { Id = 1, Name = new string('a', 100) };
        await _sut.WriteAsync(key, value, false);
        var value2 = new TestClass { Id = 2 };

        // Act
        await _sut.WriteAsync(key, value2, useLock);

        // Assert
        var result = _mainStream.Length;
        result.Should().Be(_sectorSize);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task DeleteAsync_WhenKeyIsLessThanZero_ThrowsArgumentOutOfRangeException(bool useLock)
    {
        // Arrange
        var key = -1;

        // Act
        Func<Task> act = async () => await _sut.DeleteAsync(key, useLock);

        // Assert
        await act.Should().ThrowAsync<ArgumentOutOfRangeException>();
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task DeleteAsync_WhenKeyIsGreaterThanMaxKey_ReturnsFalse(bool useLock)
    {
        // Arrange
        var key = 100;

        // Act
        var result = await _sut.DeleteAsync(key, useLock);

        // Assert
        result.Should().BeFalse();
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task DeleteAsync_WhenKeyIsCorrectAndObjectExists_ReturnsTrue(bool useLock)
    {
        // Arrange
        var key = 0;
        var value = new TestClass { Id = 1 };
        await _sut.WriteAsync(key, value, false);

        // Act
        var result = await _sut.DeleteAsync(key, useLock);

        // Assert
        result.Should().BeTrue();
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task DeleteAsync_WhenKeyIsCorrectAndObjectDoesNotExist_ReturnsFalse(bool useLock)
    {
        // Arrange
        var key = 0;

        // Act
        var result = await _sut.DeleteAsync(key, useLock);

        // Assert
        result.Should().BeFalse();
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task DeleteAsync_WhenKeyIsCorrectAndObjectExists_RemovesValue(bool useLock)
    {
        // Arrange
        var key = 0;
        var value = new TestClass { Id = 1 };
        await _sut.WriteAsync(key, value, false);

        // Act
        await _sut.DeleteAsync(key, useLock);

        // Assert
        var result = await _sut.ReadAsync(key, false);
        result.Should().BeNull();
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task DeleteAsync_WhenKeyIsCorrectAndObjectIsLast_RemovesValueAndTruncatesStream(bool useLock)
    {
        // Arrange
        var key = 0;
        var value = new TestClass { Id = 1 };
        await _sut.WriteAsync(key, value, false);

        // Act
        await _sut.DeleteAsync(key, useLock);

        // Assert
        var result = _mainStream.Length;
        result.Should().Be(0);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task DeleteAsync_WhenKeyIsCorrectAndMultipleObjectsExist_ShrinksStreamToLengthDivisibleBySectorSize(bool useLock)
    {
        // Arrange
        var key = 0;
        var value = new TestClass { Id = 1 };
        await _sut.WriteAsync(key, value, false);
        var key2 = 1;
        var value2 = new TestClass { Id = 2 };
        await _sut.WriteAsync(key2, value2, false);

        // Act
        await _sut.DeleteAsync(key2, useLock);

        // Assert
        var streamLength = _mainStream.Length;
        streamLength.Should().Be(_sectorSize);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task DeleteAsync_WhenKeyIsCorrect_DoesNotRemoveOtherObjects(bool useLock)
    {
        // Arrange
        var key = 0;
        var value = new TestClass { Id = 1 };
        await _sut.WriteAsync(key, value, false);
        var key2 = 1;
        var value2 = new TestClass { Id = 2 };
        await _sut.WriteAsync(key2, value2, false);

        // Act
        await _sut.DeleteAsync(key, useLock);

        // Assert
        var result = await _sut.ReadAsync(key2, false);
        result.Should().BeEquivalentTo(value2);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Clear_WhenCalled_DeletesAllObjects(bool useLock)
    {
        // Arrange
        var id = 0;
        var value = new TestClass { Id = 1 };
        await _sut.WriteAsync(id, value, false);
        var id2 = 1;
        var value2 = new TestClass { Id = 2 };
        await _sut.WriteAsync(id2, value2, false);

        // Act
        _sut.Clear(useLock);

        // Assert
        var result = await _sut.ReadAsync(id, false);
        result.Should().BeNull();
        var result2 = await _sut.ReadAsync(id2, false);
        result2.Should().BeNull();
    }

    [Fact]
    public void Dispose_WhenCalled_DisposesStream()
    {
        // Arrange

        // Act
        _sut.Dispose();

        // Assert
        _mainStream.CanRead.Should().BeFalse();
    }
}
