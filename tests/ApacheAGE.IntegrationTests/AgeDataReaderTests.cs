using ApacheAGE.Types;

namespace ApacheAGE.IntegrationTests;
internal class AgeDataReaderTests: TestBase
{
    [Test]
    public async Task GetValue_Should_ReturnExactColumn()
    {
        await using var client = CreateAgeClient();
        var graphName = await CreateTempGraphAsync();

        await client.OpenConnectionAsync();
        await using var dataReader = await client.ExecuteQueryAsync(
$@"SELECT * FROM cypher('{graphName}', $$
    RETURN 1
$$) AS (num agtype);");
        await dataReader.ReadAsync();
        var result = dataReader.GetValue<Agtype?>(0);

        Assert.That(result, Is.Not.Null, "'result' returns null.");
        Assert.That(result?.GetInt32(), Is.EqualTo(1), "Number returned from AGE is not 1.");

        await DropTempGraphAsync(graphName);
    }
    
    [Test]
    public async Task GetValue_Should_ThrowException_When_TheOrdinalIsOutOfRange()
    {
        await using var client = CreateAgeClient();
        var graphName = await CreateTempGraphAsync();

        await client.OpenConnectionAsync();
        await using var dataReader = await client.ExecuteQueryAsync(
$@"SELECT * FROM cypher('{graphName}', $$
    RETURN 1
$$) AS (num agtype);");
        await dataReader.ReadAsync();

        Assert.That(() => dataReader.GetValue<Agtype?>(3), Throws.TypeOf<IndexOutOfRangeException>());

        await DropTempGraphAsync(graphName);
    }

    [Test]
    public async Task GetValues_Should_ReturnTheExactOutputtedColumns()
    {
        await using var client = CreateAgeClient();
        var graphName = await CreateTempGraphAsync();

        await client.OpenConnectionAsync();
        await using var dataReader = await client.ExecuteQueryAsync(
$@"SELECT * FROM cypher('{graphName}', $$
    RETURN 1, 2, 3, 4   /* These values are sequential on purpose. */
$$) AS (num1 agtype, num2 agtype, num3 agtype, num4 agtype);");
        await dataReader.ReadAsync();
        var fields = new object[4];
        int loaded = dataReader.GetValues(fields);

        Assert.That(loaded, Is.EqualTo(4));
        Assert.Multiple(() =>
        {
            for (int i = 0; i < fields.Length; i++)
            {
                // The value is sequential on purpose.
                int value = i + 1;
                Agtype fieldValue = (Agtype)fields[i];
                
                Assert.That((int)fieldValue, Is.EqualTo(value), $"resultSet[{i}] is not {i + 1}.");
            }
        });

        await DropTempGraphAsync(graphName);
    }

    [Test]
    public async Task GetOrdinal_Should_ReturnTheOrdinalOfTheColumn()
    {
        await using var client = CreateAgeClient();
        var graphName = await CreateTempGraphAsync();

        await client.OpenConnectionAsync();
        await using var dataReader = await client.ExecuteQueryAsync(
$@"SELECT * FROM cypher('{graphName}', $$
    RETURN 1, 2
$$) AS (num1 agtype, num2 agtype);");
        await dataReader.ReadAsync();

        Assert.Multiple(() =>
        {
            Assert.That(dataReader.GetOrdinal("num1"), Is.EqualTo(0), "Wrong column ordinal for num1");
            Assert.That(dataReader.GetOrdinal("num2"), Is.EqualTo(1), "Wrong column ordinal for num2");
        });

        await DropTempGraphAsync(graphName);
    }

    [Test]
    public async Task GetName_Should_ReturnTheNameOfTheColumn()
    {
        await using var client = CreateAgeClient();
        var graphName = await CreateTempGraphAsync();

        await client.OpenConnectionAsync();
        await using var dataReader = await client.ExecuteQueryAsync(
$@"SELECT * FROM cypher('{graphName}', $$
    RETURN 1, 2
$$) AS (num1 agtype, num2 agtype);");
        await dataReader.ReadAsync();

        Assert.Multiple(() =>
        {
            Assert.That(dataReader.GetName(0), Is.EqualTo("num1"), "Wrong column name for 0");
            Assert.That(dataReader.GetName(1), Is.EqualTo("num2"), "Wrong column name for 1");
        });

        await DropTempGraphAsync(graphName);
    }
}
