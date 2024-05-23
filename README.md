# ApacheAGE

[![Nuget](https://img.shields.io/nuget/v/ApacheAGE?color=blue)](https://www.nuget.org/packages/ApacheAGE/)

## What is Apache AGE?

Apache AGE is an open-source extension for PostgreSQL which provides it with the capabilities of a graph database.

## Quickstart

Here's a simple example to get you started:

```csharp
using ApacheAGE;
using ApacheAGE.Types;

var connectionString = "Host=server;Port=5432;Username=user;Password=pass;Database=sample1";

// Create a client.
var clientBuilder = new AgeClientBuilder(connectionString);
await using var client = clientBuilder.Build();
await client.OpenConnectionAsync();

// Create a graph and add vertices.
await client.CreateGraphAsync("graph1");
await client.ExecuteCypherAsync("graph1", "CREATE (:Person {age: 23}), (:Person {age: 78})");
await using var reader = await client.ExecuteQueryAsync(
@"SELECT * FROM cypher('graph1', $$
    MATCH (n:Person)
    RETURN n
$$) AS (persons agtype);");

// Read the result row by row.
while(await reader.ReadAsync())
{
    var agtypeResult = reader.GetValue<Agtype>(0);
    Vertex person = agtypeResult.GetVertex();
    Console.WriteLine(person);
}
```
