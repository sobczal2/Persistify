using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Persistify.Core.Handlers;
using Persistify.Core.Indexes;
using Persistify.Core.Tokens;
using Persistify.Storage.Sqlite;
using Persistify.Web.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();

builder.Services.AddSqliteStorage("Data Source=storage.db");
builder.Services.AddSingleton<IndexesStore>();
builder.Services.AddSingleton<ITokenizer, PlainTextTokenizer>();
builder.Services.AddScoped<IndexHandler>();
builder.Services.AddScoped<SearchHandler>();


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var indexesStore = scope.ServiceProvider.GetRequiredService<IndexesStore>();
    await indexesStore.Initialize();
}

app.MapGrpcService<PersistifyService>();

app.Run();