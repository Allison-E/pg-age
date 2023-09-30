namespace ApacheAGE.IntegrationTests;
internal class AgeTypeTests: TestBase
{
    [Test]
    public async Task Value_Should_BeNull_When_AGEOutputsNull()
    {
        await using var client = CreateAgeClient();
        var graphname = await CreateTempGraphAsync();

        await client.OpenConnectionAsync();
        await using var dataReader = await client.ExecuteQueryAsync(
$@"SELECT * FROM cypher('{graphname}', $$
    RETURN NULL
$$) as (value agtype);");
        await dataReader.ReadAsync();
        var agResult = dataReader.GetValue(0);

        Assert.That(agResult.Value, Is.Null);
    }

    [Test]
    public async Task ToDouble_Should_ReturnPositiveInfinity_When_AGEOutputsInfinity()
    {
        await using var client = CreateAgeClient();
        var graphname = await CreateTempGraphAsync();
    
        await client.OpenConnectionAsync();
        await using var dataReader = await client.ExecuteQueryAsync(
$@"SELECT * FROM cypher('{graphname}', $$
    RETURN 'Infinity'::float
$$) as (value agtype);");
        await dataReader.ReadAsync();
        var agResult = dataReader.GetValue(0);

        Assert.That(agResult.GetDouble(), Is.EqualTo(double.PositiveInfinity));
    }
    
    [Test]
    public async Task ToDouble_Should_ReturnNegativeInfinity_When_AGEOutputsNegativeInfinity()
    {
        await using var client = CreateAgeClient();
        var graphname = await CreateTempGraphAsync();
    
        await client.OpenConnectionAsync();
        await using var dataReader = await client.ExecuteQueryAsync(
$@"SELECT * FROM cypher('{graphname}', $$
    RETURN '-Infinity'::float
$$) as (value agtype);");
        await dataReader.ReadAsync();
        var agResult = dataReader.GetValue(0);

        Assert.That(agResult.GetDouble(), Is.EqualTo(double.NegativeInfinity));
    }
    
    [Test]
    public async Task ToDouble_Should_ReturnNaN_When_AGEOutputsNaN()
    {
        await using var client = CreateAgeClient();
        var graphname = await CreateTempGraphAsync();
    
        await client.OpenConnectionAsync();
        await using var dataReader = await client.ExecuteQueryAsync(
$@"SELECT * FROM cypher('{graphname}', $$
    RETURN 'NaN'::float
$$) as (value agtype);");
        await dataReader.ReadAsync();
        var agResult = dataReader.GetValue(0);

        Assert.That(agResult.GetDouble(), Is.EqualTo(double.NaN));
    }

    // TODO: Test for NULL in lists, since AGE prints out the NULL value in lists only.
}
