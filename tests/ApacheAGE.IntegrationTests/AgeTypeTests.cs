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

        await DropTempGraphAsync(graphname);
    }

    [Test]
    public async Task GetDouble_Should_ReturnPositiveInfinity_When_AGEOutputsInfinity()
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

        await DropTempGraphAsync(graphname);
    }
    
    [Test]
    public async Task GetDouble_Should_ReturnNegativeInfinity_When_AGEOutputsNegativeInfinity()
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

        await DropTempGraphAsync(graphname);
    }
    
    [Test]
    public async Task GetDouble_Should_ReturnNaN_When_AGEOutputsNaN()
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

        await DropTempGraphAsync(graphname);
    }

    [Test]
    public async Task GetList_Should_CorrectlyParseNullValues()
    {
        await using var client = CreateAgeClient();
        var graphname = await CreateTempGraphAsync();
        var list = new List<object?> { 1, 2, 3, 2, null, };

        await client.OpenConnectionAsync();
        await using var dataReader = await client.ExecuteQueryAsync(
$@"SELECT * FROM cypher('{graphname}', $$
    WITH [1, 2, 3, 2, NULL] AS list
    RETURN list
$$) as (value agtype);");
        await dataReader.ReadAsync();
        var agResult = dataReader.GetValue(0);

        Assert.That(agResult.GetList(), Is.EquivalentTo(list));

        await DropTempGraphAsync(graphname);
    }
}
