using System;

namespace Persistify.Stores.Types;

public class TypeExistsException : Exception
{
    public TypeExistsException() : base("Type with that name already exists")
    {
    }
}