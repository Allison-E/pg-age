using ApacheAGE.Types;

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
        var agResult = dataReader.GetValue<Agtype?>(0);

        Assert.That(agResult, Is.Null);

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
        var agResult = dataReader.GetValue<Agtype?>(0);

        Assert.That(agResult?.GetDouble(), Is.EqualTo(double.PositiveInfinity));

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
        var agResult = dataReader.GetValue<Agtype?>(0);

        Assert.That(agResult?.GetDouble(), Is.EqualTo(double.NegativeInfinity));

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
        var agResult = dataReader.GetValue<Agtype?>(0);

        Assert.That(agResult?.GetDouble(), Is.EqualTo(double.NaN));

        await DropTempGraphAsync(graphname);
    }

    [Test]
    public async Task GetVertex_Should_ReturnCorrectVertex()
    {
        await using var client = CreateAgeClient();
        var graphname = await CreateTempGraphAsync();
        ulong id = 234323;
        var label = "Person";
        var i = 3;

        await client.OpenConnectionAsync();
        await using AgeDataReader dataReader = await client.ExecuteQueryAsync(
$@"SELECT * FROM cypher('{graphname}', $$
    WITH {{id: {id}, label: ""{label}"", properties: {{i: {i}}}}}::vertex as v
	RETURN v
$$) as (value agtype);");
        await dataReader.ReadAsync();
        var agResult = dataReader.GetValue<Agtype?>(0);
        var vertex = agResult?.GetVertex();

        Assert.That(vertex, Is.Not.Null);
        Assert.That(vertex?.Id.Value, Is.EqualTo(id));
        Assert.That(vertex?.Label, Is.EqualTo(label));
        Assert.That(vertex?.Properties["i"], Is.EqualTo(i));

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
        var agResult = dataReader.GetValue<Agtype?>(0);

        Assert.That(agResult?.GetList(), Is.EquivalentTo(list));

        await DropTempGraphAsync(graphname);
    }
}
