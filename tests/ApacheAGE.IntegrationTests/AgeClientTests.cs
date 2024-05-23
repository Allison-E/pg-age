using Npgsql;

namespace ApacheAGE.IntegrationTests;

internal class AgeClientTests: TestBase
{
    [Test]
    public async Task OpenConnectionAsync_Should_CreateExtensionInDatabase()
    {
        await using var client = CreateAgeClient();
        await using var connection = GetConnection();
        await using var command = new NpgsqlCommand("DROP EXTENSION IF EXISTS age CASCADE;", connection);

        //First remove the extension from the database if it exists.
        await command.ExecuteNonQueryAsync();
        await client.OpenConnectionAsync();

        // Now, check if the extension exists in the database.
        command.CommandText = "SELECT extname FROM pg_extension WHERE extname = 'age';";
        var result = await command.ExecuteScalarAsync();

        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public async Task ExecuteQueryAsync_With_NoParameters_Should_ReturnDataReader()
    {
        await using var client = CreateAgeClient();
        var graphName = await CreateTempGraphAsync();

        await client.OpenConnectionAsync();
        await using var dataReader = await client.ExecuteQueryAsync(
$@"SELECT * FROM cypher('{graphName}', $$
    RETURN 1
$$) AS (num agtype);");

        Assert.That(dataReader, Is.Not.Null);
        Assert.That(dataReader.HasRows, Is.True);

        await DropTempGraphAsync(graphName);
    }

    [Test]
    public async Task GraphExistsAsync_Should_ReturnTrueIfGraphExists()
    {
        await using var client = CreateAgeClient();
        var graphName = await CreateTempGraphAsync();

        await client.OpenConnectionAsync();
        var graphExists = await client.GraphExistsAsync(graphName);
        Assert.That(graphExists, Is.True);
    }

    [Test]
    public async Task GraphExistsAsync_Should_ReturnFalseIfGraphExists()
    {
        await using var client = CreateAgeClient();

        await client.OpenConnectionAsync();
        var graphExists = await client.GraphExistsAsync("sidjfa23knlsd9a8dfndfhjbnzxeunjakssdf3sdmvns_asdjfk");
        Assert.That(graphExists, Is.False);
    }
}