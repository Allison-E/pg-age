using ApacheAGE.Data;

namespace ApacheAGE.UnitTests;
public class AgTypeTests
{
    #region GetBoolean()

    [Test]
    public void GetBoolean_Should_ReturnTrue_For_EquivalentTrueValues()
    {
        var agtype = new AgType("true");
        var agtype2 = new AgType("True");
        var agtype3 = new AgType("TRUE");

        Assert.Multiple(() =>
        {
            Assert.That(agtype.GetBoolean(), Is.True);
            Assert.That(agtype2.GetBoolean(), Is.True);
            Assert.That(agtype3.GetBoolean(), Is.True);
        });
    }

    [Test]
    public void GetBoolean_Should_ReturnFalse_For_EquivalentFalseValues()
    {
        var agtype = new AgType("false");
        var agtype2 = new AgType("False");
        var agtype3 = new AgType("FALSE");

        Assert.Multiple(() =>
        {
            Assert.That(agtype.GetBoolean(), Is.False);
            Assert.That(agtype2.GetBoolean(), Is.False);
            Assert.That(agtype3.GetBoolean(), Is.False);
        });
    }

    [Test]
    public void GetBoolean_Should_ThrowException_When_AgtypeValueIsNull()
    {
        var agtype = new AgType(null);

        Assert.That(() => agtype.GetBoolean(), Throws.InstanceOf<NullReferenceException>());
    }

    [Test]
    public void GetBoolean_Should_ThrowException_When_AgtypeValueIsInTheWrongFormat()
    {
        var agtype = new AgType("23");

        Assert.That(() => agtype.GetBoolean(), Throws.InstanceOf<FormatException>());
    }

    #endregion

    #region GetDouble()

    [Test]
    public void GetDouble_Should_ReturnEquivalentDouble()
    {
        var numString = "1.0023e3";
        var agtype = new AgType(numString);
        var doubleEquivalent = double.Parse(numString);

        Assert.That(agtype.GetDouble(), Is.EqualTo(doubleEquivalent));
    }

    [Test]
    public void GetDouble_Should_ReturnDoubleEquivalent_For_NegativeInfinity()
    {
        var agtype = new AgType("-Infinity");

        Assert.That(agtype.GetDouble(), Is.EqualTo(double.NegativeInfinity));
    }

    [Test]
    public void GetDouble_Should_ReturnDoubleEquivalent_For_PositiveInfinity()
    {
        var agtype = new AgType("Infinity");

        Assert.That(agtype.GetDouble(), Is.EqualTo(double.PositiveInfinity));
    }

    [Test]
    public void GetDouble_Should_ReturnDoubleEquivalent_For_NaN()
    {
        var agtype = new AgType("NaN");

        Assert.That(agtype.GetDouble(), Is.EqualTo(double.NaN));
    }

    [Test]
    public void GetDouble_Should_ThrowException_When_AgtypeValueIsNull()
    {
        var agtype = new AgType(null);

        Assert.That(() => agtype.GetDouble(), Throws.InstanceOf<NullReferenceException>());
    }

    [Test]
    public void GetDouble_Should_ThrowException_When_AgtypeValueIsInTheWrongFormat()
    {
        var agtype = new AgType("true");

        Assert.That(() => agtype.GetDouble(), Throws.InstanceOf<FormatException>());
    }

    #endregion

    #region GetInteger()

    [Test]
    public void GetInteger_Should_ReturnEquivalentDouble()
    {
        var numString = "1";
        var agtype = new AgType(numString);
        var doubleEquivalent = int.Parse(numString);

        Assert.That(agtype.GetInteger(), Is.EqualTo(doubleEquivalent));
    }

    [Test]
    public void GetInteger_Should_ThrowException_When_AgtypeValueIsNull()
    {
        var agtype = new AgType(null);

        Assert.That(() => agtype.GetInteger(), Throws.InstanceOf<NullReferenceException>());
    }

    [Test]
    public void GetInteger_Should_ThrowException_When_AgtypeValueIsInTheWrongFormat()
    {
        var agtype = new AgType("true");

        Assert.That(() => agtype.GetInteger(), Throws.InstanceOf<FormatException>());
    }

    #endregion

    #region GetLong()

    [Test]
    public void GetLong_Should_ReturnEquivalentDouble()
    {
        var numString = "1";
        var agtype = new AgType(numString);
        var doubleEquivalent = long.Parse(numString);

        Assert.That(agtype.GetLong(), Is.EqualTo(doubleEquivalent));
    }

    [Test]
    public void GetLong_Should_ThrowException_When_AgtypeValueIsNull()
    {
        var agtype = new AgType(null);

        Assert.That(() => agtype.GetLong(), Throws.InstanceOf<NullReferenceException>());
    }

    [Test]
    public void GetLong_Should_ThrowException_When_AgtypeValueIsInTheWrongFormat()
    {
        var agtype = new AgType("true");

        Assert.That(() => agtype.GetLong(), Throws.InstanceOf<FormatException>());
    }

    #endregion

    #region GetDecimal()

    [Test]
    public void GetDecimal_Should_ReturnEquivalentDouble()
    {
        var numString = "1";
        var agtype = new AgType(numString);
        var doubleEquivalent = decimal.Parse(numString);

        Assert.That(agtype.GetDecimal(), Is.EqualTo(doubleEquivalent));
    }

    [Test]
    public void GetDecimal_Should_ThrowException_When_AgtypeValueIsNull()
    {
        var agtype = new AgType(null);

        Assert.That(() => agtype.GetDecimal(), Throws.InstanceOf<NullReferenceException>());
    }

    [Test]
    public void GetDecimal_Should_ThrowException_When_AgtypeValueIsInTheWrongFormat()
    {
        var agtype = new AgType("true");

        Assert.That(() => agtype.GetDecimal(), Throws.InstanceOf<FormatException>());
    }

    #endregion

    #region GetList()

    [Test]
    public void GetList_Should_ReturnEquivalentList()
    {
        var list = new List<object?> { 1, 2, "string", null, };
        var agtype = new AgType("[1, 2, \"string\", null]");

        var agtypeList = agtype.GetList();

        Assert.Multiple(() =>
        {
            Assert.That(agtypeList, Has.Count.EqualTo(list.Count));
            Assert.That(agtype.GetList(), Is.EquivalentTo(list));
        });
    }

    [Test]
    public void GetList_Should_ReturnEquivalentList_When_ItIsANestedList()
    {
        var list = new List<object?>
        {
            1,
            2,
            "string",
            null,
            new List<object?> { 1, 2, "string", null },
        };
        var agtype = new AgType("[1, 2, \"string\", null, [1, 2, \"string\", null]]");

        var agtypeList = agtype.GetList();

        Assert.Multiple(() =>
        {
            Assert.That(agtypeList, Has.Count.EqualTo(list.Count));
            Assert.That(agtype.GetList(), Is.EquivalentTo(list));
        });
    }

    [Test]
    public void GetList_Should_ReturnNegativeInfinity_When_Supplied_NegativeInfinity()
    {
        var list = new List<object?> { 1, 2, double.NegativeInfinity, };
        var agtype = new AgType("[1, 2, \"-Infinity\"]");

        var agtypeList = agtype.GetList(true);

        Assert.Multiple(() =>
        {
            Assert.That(agtypeList, Has.Count.EqualTo(list.Count));
            Assert.That(agtypeList, Is.EquivalentTo(list));
        });
    }

    [Test]
    public void GetList_Should_ThrowException_When_AgtypeValueIsNull()
    {
        var agtype = new AgType(null);

        Assert.That(() => agtype.GetList(), Throws.InstanceOf<NullReferenceException>());
    }

    #endregion

    #region GetVertex()

    [Test]
    public void GetVertex_Should_ReturnEquivalentVertex()
    {
        var agtype = new AgType("{\"id\": 0, \"label\": \"Person\", \"properties\": {\"name\": \"Emmanuel\", \"age\": 21}}::vertex");

        var vertex = agtype.GetVertex();

        Assert.Multiple(() =>
        {
            Assert.That(vertex, Is.Not.Null);
            Assert.That(vertex.Id, Is.EqualTo(0));
            Assert.That(vertex.Label, Is.EqualTo("Person"));
            Assert.That(vertex.Properties!["name"], Is.EqualTo("Emmanuel"));
            Assert.That(vertex.Properties!["age"], Is.EqualTo(21));
        });
    }

    [Test]
    public void GetVertex_Should_ThrowException_When_AgtypeValueIsNull()
    {
        var agtype = new AgType(null);

        Assert.That(() => agtype.GetVertex(), Throws.InstanceOf<NullReferenceException>());
    }

    #endregion

    #region GetEdge()

    [Test]
    public void GetEdge_Should_ReturnEquivalentVertex()
    {
        var agtype = new AgType("{\"id\": 2, \"label\": \"KNOWS\", \"end_id\": 1, \"start_id\": 0, \"properties\": {\"colour\": \"red\"}}::path");

        var edge = agtype.GetEdge();

        Assert.Multiple(() =>
        {
            Assert.That(edge, Is.Not.Null);
            Assert.That(edge.Id, Is.EqualTo(2));
            Assert.That(edge.Label, Is.EqualTo("KNOWS"));
            Assert.That(edge.StartId, Is.EqualTo(0));
            Assert.That(edge.EndId, Is.EqualTo(1));
            Assert.That(edge.Properties!["colour"], Is.EqualTo("red"));
        });
    }

    [Test]
    public void GetEdge_Should_ThrowException_When_AgtypeValueIsNull()
    {
        var agtype = new AgType(null);

        Assert.That(() => agtype.GetEdge(), Throws.InstanceOf<NullReferenceException>());
    }

    #endregion

    #region GetPath()

    [Test]
    public void GetPath_Should_ReturnEquivalentVertex()
    {
        var agtype = new AgType("[{\"id\": 0, \"label\": \"label_name_1\", \"properties\": {\"i\": 0}}::vertex, {\"id\": 2, \"label\": \"edge_label\", \"end_id\": 1, \"start_id\": 0, \"properties\": {\"i\": 0}}::path,\r\n{\"id\": 1, \"label\": \"label_name_2\", \"properties\": {}}::vertex]::path");

        var path = agtype.GetPath();

        Assert.Multiple(() =>
        {
            Assert.That(path, Is.Not.Null);
            Assert.That(path, Has.Length.EqualTo(3));
            for (int i = 0; i < 2; i++)
            {
                if (i % 2 == 0)
                    Assert.That(path[i], Is.InstanceOf<Vertex>());
                else
                    Assert.That(path[i], Is.InstanceOf<Edge>());
            }
            Assert.That(((Vertex)path[0]).Label, Is.EqualTo("label_name_1"));
        });
    }

    [Test]
    public void GetPath_Should_ThrowException_When_AgtypeValueIsNull()
    {
        var agtype = new AgType(null);

        Assert.That(() => agtype.GetPath(), Throws.InstanceOf<NullReferenceException>());
    }
    
    [Test]
    public void GetPath_Should_ThrowException_When_AgtypeValueIsInWrongFormat()
    {
        var agtype = new AgType("[{\"id\": 0, \"label\": \"label_name_1\", \"properties\": {\"i\": 0}}, 4,\r\n{\"id\": 1, \"label\": \"label_name_2\", \"properties\": {}}::vertex]");

        Assert.That(() => agtype.GetPath(), Throws.InstanceOf<FormatException>());
    }

    #endregion
}
