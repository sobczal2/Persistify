namespace Persistify.Server.Serialization.Tests.Unit;

public class JsonSerializerTests : SerializerTests<JsonSerializer>
{
    protected override JsonSerializer CreateSut()
    {
        return new JsonSerializer();
    }
}
