using Microsoft.Extensions.Configuration;
using Npgsql;

namespace ApacheAGE.IntegrationTests;

internal class TestBase
{
    private static string _defaultConnectionString =
        "Server=localhost;Port=5432;Username=agedotnet;Password=agedotnet;Database=agedotnet_tests;";

    protected string ConnectionString => 
        Environment.GetEnvironmentVariable("AGE_TEST_DB") ?? _defaultConnectionString;

    protected AgeClientBuilder CreateAgeClientBuilder() => new(ConnectionString);

    protected AgeClient CreateAgeClient() => CreateAgeClientBuilder().Build();

    protected NpgsqlConnection GetConnection() => 
        new NpgsqlDataSourceBuilder(ConnectionString)
             .Build()
             .OpenConnection();

    protected NpgsqlCommand CreateCommand(string command) => 
        new NpgsqlCommand(command, GetConnection());
}
