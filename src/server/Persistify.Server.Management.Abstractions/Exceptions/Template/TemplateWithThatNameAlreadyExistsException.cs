﻿using System;

namespace Persistify.Server.Management.Abstractions.Exceptions.Template;

public class TemplateWithThatNameAlreadyExistsException : Exception
{
    public TemplateWithThatNameAlreadyExistsException() : base("Template with that name already exists")
    {

    }
}