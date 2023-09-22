using System;
using System.IO;
using FluentAssertions;
using ProtoBuf;
using Xunit;

namespace Persistify.Server.Serialization.Tests.Unit;

public abstract class SerializerTests<TSerializer> where TSerializer : ISerializer
{
    private readonly TSerializer _sut;

    public SerializerTests()
    {
        // ReSharper disable once VirtualMemberCallInConstructor
        _sut = CreateSut();
    }

    protected abstract TSerializer CreateSut();

    [Fact]
    public void Serialize_WhenObjectIsNull_ThrowsArgumentNullException()
    {
        // Arrange

        // Act
        Action act = () => _sut.Serialize<object>(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Serialize_WhenObjectIsNotNull_ReturnsBytes()
    {
        // Arrange
        var obj = new TestClass { Property = "test" };

        // Act
        var bytes = _sut.Serialize(obj);

        // Assert
        bytes.ToArray().Should().NotBeEmpty();
    }

    [Fact]
    public void Deserialize_WhenBytesIsEmpty_ThrowsInvalidDataException()
    {
        // Arrange

        // Act
        Action act = () => _sut.Deserialize<object>(ReadOnlyMemory<byte>.Empty);

        // Assert
        act.Should().Throw<InvalidDataException>();
    }

    [Fact]
    public void Deserialize_WhenBytesIsNotEmpty_ReturnsObject()
    {
        // Arrange
        var obj = new TestClass { Property = "test" };
        var bytes = _sut.Serialize(obj);

        // Act
        var result = _sut.Deserialize<TestClass>(bytes);

        // Assert
        result.Should().BeEquivalentTo(obj);
    }

    [Fact]
    public void Deserialize_WhenBytesIsNotValid_ThrowsInvalidOperationException()
    {
        // Arrange
        var bytes = new ReadOnlyMemory<byte>(new byte[] { 1, 2, 3 });

        // Act
        Action act = () => _sut.Deserialize<TestClass>(bytes);

        // Assert
        act.Should().Throw<Exception>();
    }

    [ProtoContract]
    private class TestClass
    {
        [ProtoMember(1)] public string? Property { get; set; }
    }
}
