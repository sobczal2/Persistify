using System;

namespace Persistify.Stores.Types;

public class TypeNotFoundException : Exception
{
    public TypeNotFoundException() : base("Type with that name was not found")
    {
    }
}