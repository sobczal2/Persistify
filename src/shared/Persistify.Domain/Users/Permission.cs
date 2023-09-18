﻿using System;

namespace Persistify.Domain.Users;

[Flags]
public enum Permission
{
    None = 0,
    DocumentRead = 1,
    DocumentWrite = 2,
    TemplateRead = 4,
    TemplateWrite = 8,
    UserRead = 16,
    UserWrite = 32,
    Root = 64,
    All = DocumentRead | DocumentWrite | TemplateRead | TemplateWrite | UserRead | UserWrite | Root
}
