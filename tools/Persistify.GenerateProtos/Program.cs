using System;
using System.IO;
using Persistify.Services;
using ProtoBuf.Grpc.Reflection;
using ProtoBuf.Meta;

var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../../../proto");

var generator = new SchemaGenerator() { ProtoSyntax = ProtoSyntax.Proto3 };

File.WriteAllText(Path.Combine(path, "templates.proto"), generator.GetSchema<ITemplateService>());
File.WriteAllText(Path.Combine(path, "documents.proto"), generator.GetSchema<IDocumentService>());
