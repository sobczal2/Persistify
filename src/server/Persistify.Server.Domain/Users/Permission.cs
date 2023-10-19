using System;

namespace Persistify.Server.Domain.Users;

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
    PresetAnalyzerRead = 64,
    PresetAnalyzerWrite = 128,
    Root = 256,
    All = DocumentRead | DocumentWrite | TemplateRead | TemplateWrite | UserRead | UserWrite | PresetAnalyzerRead | PresetAnalyzerWrite | Root
}
