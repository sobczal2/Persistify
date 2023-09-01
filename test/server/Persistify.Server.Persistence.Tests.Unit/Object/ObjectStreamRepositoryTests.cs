using System;
using System.IO;
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

    [Fact]
    public async Task ReadAsync_WhenKeyIsLessThanZero_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var key = -1;

        // Act
        var action = new Func<Task>(async () => await _sut.ReadAsync(key));

        // Assert
        await action.Should().ThrowExactlyAsync<ArgumentOutOfRangeException>();
    }

    [Fact]
    public async Task ReadAsync_ObjectDoesNotExist_ReturnsNull()
    {
        // Arrange
        var key = 0;

        // Act
        var result = await _sut.ReadAsync(key);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task ReadAsync_ObjectExists_ReturnsObject()
    {
        // Arrange
        var key = 0;
        var testClass = new TestClass { Id = 1, Name = "Test" };
        await _sut.WriteAsync(key, testClass);

        // Act
        var result = await _sut.ReadAsync(key);

        // Assert
        result.Should().BeEquivalentTo(testClass);
    }

    [Fact]
    public async Task ReadAsync_ObjectExistsAndMultipleObjectsAreWritten_ReturnsObject()
    {
        // Arrange
        await _sut.WriteAsync(0, new TestClass { Id = 0, Name = "Test0" });
        var key = 1;
        var testClass = new TestClass { Id = 1, Name = "Test1" };
        await _sut.WriteAsync(key, testClass);
        await _sut.WriteAsync(2, new TestClass { Id = 2, Name = "Test2" });

        // Act
        var result = await _sut.ReadAsync(key);

        // Assert
        result.Should().BeEquivalentTo(testClass);
    }

    [Fact]
    public async Task ReadAllAsync_WhenRepositoryIsEmpty_ReturnsEmptyList()
    {
        // Arrange

        // Act
        var result = await _sut.ReadAllAsync();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task ReadAllAsync_WhenRepositoryIsNotEmpty_ReturnsAllObjects()
    {
        // Arrange
        var key0 = 0;
        var testClass0 = new TestClass { Id = 0, Name = "Test0" };
        await _sut.WriteAsync(key0, testClass0);
        var key1 = 1;
        var testClass1 = new TestClass { Id = 1, Name = "Test1" };
        await _sut.WriteAsync(key1, testClass1);
        var key2 = 2;
        var testClass2 = new TestClass { Id = 2, Name = "Test2" };
        await _sut.WriteAsync(key2, testClass2);

        // Act
        var result = await _sut.ReadAllAsync();

        // Assert
        result[key0].Should().BeEquivalentTo(testClass0);
        result[key1].Should().BeEquivalentTo(testClass1);
        result[key2].Should().BeEquivalentTo(testClass2);
    }

    [Fact]
    public async Task WriteAsync_WhenKeyIsLessThanZero_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var key = -1;
        var testClass = new TestClass { Id = 1, Name = "Test" };

        // Act
        var action = new Func<Task>(async () => await _sut.WriteAsync(key, testClass));

        // Assert
        await action.Should().ThrowExactlyAsync<ArgumentOutOfRangeException>();
    }

    [Fact]
    public async Task WriteAsync_WhenObjectIsNull_ThrowsArgumentNullException()
    {
        // Arrange
        var key = 0;
        TestClass testClass = null!;

        // Act
        var action = new Func<Task>(async () => await _sut.WriteAsync(key, testClass));

        // Assert
        await action.Should().ThrowExactlyAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task WriteAsync_WhenObjectIsNotNull_WritesObject()
    {
        // Arrange
        var key = 0;
        var testClass = new TestClass { Id = 1, Name = "Test" };

        // Act
        await _sut.WriteAsync(key, testClass);

        // Assert
        var result = await _sut.ReadAsync(key);
        result.Should().BeEquivalentTo(testClass);
    }

    [Fact]
    public async Task WriteAsync_WhenOverwritingObject_WritesObject()
    {
        // Arrange
        var key = 0;
        var testClass = new TestClass { Id = 1, Name = "Test" };
        await _sut.WriteAsync(key, testClass);
        var testClass2 = new TestClass { Id = 2, Name = "Test2" };

        // Act
        await _sut.WriteAsync(key, testClass2);

        // Assert
        var result = await _sut.ReadAsync(key);
        result.Should().BeEquivalentTo(testClass2);
    }

    [Fact]
    public async Task WriteAsync_WhenMultipleObjectsAreWritten_ExtendsStreamToLengthDivisibleBySectorSize()
    {
        // Arrange
        var key0 = 0;
        var testClass0 = new TestClass { Id = 0, Name = "Test0" };
        await _sut.WriteAsync(key0, testClass0);
        var key1 = 1;
        var testClass1 = new TestClass { Id = 1, Name = "Test1" };
        await _sut.WriteAsync(key1, testClass1);
        var key2 = 2;
        var testClass2 = new TestClass { Id = 2, Name = "Test2" };

        // Act
        await _sut.WriteAsync(key2, testClass2);

        // Assert
        _mainStream.Length.Should().Be(3 * _sectorSize);
    }

    [Fact]
    public async Task
        WriteAsync_WhenOverridingNotLastValueWithLongerValue_WritesValueAndExtendsStreamToLengthDivisibleBySectorSize()
    {
        // Arrange
        var key0 = 0;
        var testClass0 = new TestClass { Id = 0, Name = "Test0" };
        await _sut.WriteAsync(key0, testClass0);
        var key1 = 1;
        var testClass1 = new TestClass { Id = 1, Name = "Test1" };
        await _sut.WriteAsync(key1, testClass1);
        var key2 = 2;
        var testClass2 = new TestClass { Id = 2, Name = "Test2" };
        await _sut.WriteAsync(key2, testClass2);
        var key1New = 1;
        var testClass1New = new TestClass { Id = 1, Name = new string('a', 100) };

        // Act
        await _sut.WriteAsync(key1New, testClass1New);

        // Assert
        _mainStream.Length.Should().Be(5 * _sectorSize);
    }

    [Fact]
    public async Task
        WriteAsync_WhenOverridingLastValueWithLongerValue_WritesValueAndExtendsStreamToLengthDivisibleBySectorSize()
    {
        // Arrange
        var key0 = 0;
        var testClass0 = new TestClass { Id = 0, Name = "Test0" };
        await _sut.WriteAsync(key0, testClass0);
        var key1 = 1;
        var testClass1 = new TestClass { Id = 1, Name = "Test1" };
        await _sut.WriteAsync(key1, testClass1);
        var key2 = 2;
        var testClass2 = new TestClass { Id = 2, Name = "Test2" };
        await _sut.WriteAsync(key2, testClass2);
        var key2New = 2;
        var testClass2New = new TestClass { Id = 2, Name = new string('a', 100) };

        // Act
        await _sut.WriteAsync(key2New, testClass2New);

        // Assert
        _mainStream.Length.Should().Be(4 * _sectorSize);
    }

    [Fact]
    public async Task WriteAsync_WhenOverridingLastValueWithShorterValue_WritesValueAndDoesNotExtendStream()
    {
        // Arrange
        var key0 = 0;
        var testClass0 = new TestClass { Id = 0, Name = "Test0" };
        await _sut.WriteAsync(key0, testClass0);
        var key1 = 1;
        var testClass1 = new TestClass { Id = 1, Name = "Test1" };
        await _sut.WriteAsync(key1, testClass1);
        var key2 = 2;
        var testClass2 = new TestClass { Id = 2, Name = "Test2" };
        await _sut.WriteAsync(key2, testClass2);
        var key2New = 2;
        var testClass2New = new TestClass { Id = 2, Name = "a" };

        // Act
        await _sut.WriteAsync(key2New, testClass2New);

        // Assert
        _mainStream.Length.Should().Be(3 * _sectorSize);
    }

    [Fact]
    public async Task WriteAsync_WhenOverridingNotLastValueWithShorterValue_WritesValueAndDoesNotExtendStream()
    {
        // Arrange
        var key0 = 0;
        var testClass0 = new TestClass { Id = 0, Name = "Test0" };
        await _sut.WriteAsync(key0, testClass0);
        var key1 = 1;
        var testClass1 = new TestClass { Id = 1, Name = "Test1" };
        await _sut.WriteAsync(key1, testClass1);
        var key2 = 2;
        var testClass2 = new TestClass { Id = 2, Name = "Test2" };
        await _sut.WriteAsync(key2, testClass2);
        var key1New = 1;
        var testClass1New = new TestClass { Id = 1, Name = "a" };

        // Act
        await _sut.WriteAsync(key1New, testClass1New);

        // Assert
        _mainStream.Length.Should().Be(3 * _sectorSize);
    }

    [Fact]
    public async Task WriteAsync_WhenOverridingLastValueWithShorterValue_ShrinksStreamToLengthDivisibleBySectorSize()
    {
        // Arrange
        var key = 0;
        var value = new TestClass { Id = 1, Name = new string('a', 100) };
        await _sut.WriteAsync(key, value);
        var value2 = new TestClass { Id = 2 };

        // Act
        await _sut.WriteAsync(key, value2);

        // Assert
        var result = _mainStream.Length;
        result.Should().Be(_sectorSize);
    }

    [Fact]
    public async Task DeleteAsync_WhenKeyIsLessThanZero_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var key = -1;

        // Act
        Func<Task> act = async () => await _sut.DeleteAsync(key);

        // Assert
        await act.Should().ThrowAsync<ArgumentOutOfRangeException>();
    }

    [Fact]
    public async Task DeleteAsync_WhenKeyIsGreaterThanMaxKey_ReturnsFalse()
    {
        // Arrange
        var key = 100;

        // Act
        var result = await _sut.DeleteAsync(key);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteAsync_WhenKeyIsCorrectAndObjectExists_ReturnsTrue()
    {
        // Arrange
        var key = 0;
        var value = new TestClass { Id = 1 };
        await _sut.WriteAsync(key, value);

        // Act
        var result = await _sut.DeleteAsync(key);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteAsync_WhenKeyIsCorrectAndObjectDoesNotExist_ReturnsFalse()
    {
        // Arrange
        var key = 0;

        // Act
        var result = await _sut.DeleteAsync(key);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteAsync_WhenKeyIsCorrectAndObjectExists_RemovesValue()
    {
        // Arrange
        var key = 0;
        var value = new TestClass { Id = 1 };
        await _sut.WriteAsync(key, value);

        // Act
        await _sut.DeleteAsync(key);

        // Assert
        var result = await _sut.ReadAsync(key);
        result.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_WhenKeyIsCorrectAndObjectIsLast_RemovesValueAndTruncatesStream()
    {
        // Arrange
        var key = 0;
        var value = new TestClass { Id = 1 };
        await _sut.WriteAsync(key, value);

        // Act
        await _sut.DeleteAsync(key);

        // Assert
        var result = _mainStream.Length;
        result.Should().Be(0);
    }

    [Fact]
    public async Task DeleteAsync_WhenKeyIsCorrectAndMultipleObjectsExist_ShrinksStreamToLengthDivisibleBySectorSize()
    {
        // Arrange
        var key = 0;
        var value = new TestClass { Id = 1 };
        await _sut.WriteAsync(key, value);
        var key2 = 1;
        var value2 = new TestClass { Id = 2 };
        await _sut.WriteAsync(key2, value2);

        // Act
        await _sut.DeleteAsync(key2);

        // Assert
        var streamLength = _mainStream.Length;
        streamLength.Should().Be(_sectorSize);
    }

    [Fact]
    public async Task DeleteAsync_WhenKeyIsCorrect_DoesNotRemoveOtherObjects()
    {
        // Arrange
        var key = 0;
        var value = new TestClass { Id = 1 };
        await _sut.WriteAsync(key, value);
        var key2 = 1;
        var value2 = new TestClass { Id = 2 };
        await _sut.WriteAsync(key2, value2);

        // Act
        await _sut.DeleteAsync(key);

        // Assert
        var result = await _sut.ReadAsync(key2);
        result.Should().BeEquivalentTo(value2);
    }

    [Fact]
    public async Task Clear_WhenCalled_DeletesAllObjects()
    {
        // Arrange
        var id = 0;
        var value = new TestClass { Id = 1 };
        await _sut.WriteAsync(id, value);
        var id2 = 1;
        var value2 = new TestClass { Id = 2 };
        await _sut.WriteAsync(id2, value2);

        // Act
        _sut.Clear();

        // Assert
        var result = await _sut.ReadAsync(id);
        result.Should().BeNull();
        var result2 = await _sut.ReadAsync(id2);
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
