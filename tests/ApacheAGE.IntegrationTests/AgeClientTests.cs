using Npgsql;

namespace ApacheAGE.IntegrationTests;

internal class AgeClientTests: TestBase
{
    [Test]
    public async Task ConnectAsync_CreatesExtensionInDatabase()
    {
        await using var client = CreateAgeClient();
        await using var connection = GetConnection();
        await using var command = new NpgsqlCommand("DROP EXTENSION IF EXISTS age;", connection);

        //First remove the extension from the database if it exists.
        await command.ExecuteNonQueryAsync();
        await client.ConnectAsync();

        // Now, check if the extension exists in the database.
        command.CommandText = "SELECT extname FROM pg_extension WHERE extname = 'age';";
        var result = await command.ExecuteScalarAsync();

        Assert.That(result, Is.Not.Null);
    }
}