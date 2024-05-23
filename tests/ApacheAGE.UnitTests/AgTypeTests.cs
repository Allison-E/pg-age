using ApacheAGE.Types;

namespace ApacheAGE.UnitTests;
public class AgTypeTests
{
    #region GetBoolean()

    [Test]
    public void GetBoolean_Should_ReturnTrue_For_EquivalentTrueValues()
    {
        var agtype = new Agtype("true");
        var agtype2 = new Agtype("True");
        var agtype3 = new Agtype("TRUE");

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
        var agtype = new Agtype("false");
        var agtype2 = new Agtype("False");
        var agtype3 = new Agtype("FALSE");

        Assert.Multiple(() =>
        {
            Assert.That(agtype.GetBoolean(), Is.False);
            Assert.That(agtype2.GetBoolean(), Is.False);
            Assert.That(agtype3.GetBoolean(), Is.False);
        });
    }

    [Test]
    public void GetBoolean_Should_ThrowException_When_AgtypeValueIsInTheWrongFormat()
    {
        var agtype = new Agtype("23");

        Assert.That(() => agtype.GetBoolean(), Throws.InstanceOf<FormatException>());
    }

    #endregion

    #region GetDouble()

    [Test]
    public void GetDouble_Should_ReturnEquivalentDouble()
    {
        var numString = "1.0023e3";
        var agtype = new Agtype(numString);
        var doubleEquivalent = double.Parse(numString);

        Assert.That(agtype.GetDouble(), Is.EqualTo(doubleEquivalent));
    }

    [Test]
    public void GetDouble_Should_ReturnDoubleEquivalent_For_NegativeInfinity()
    {
        var agtype = new Agtype("-Infinity");

        Assert.That(agtype.GetDouble(), Is.EqualTo(double.NegativeInfinity));
    }

    [Test]
    public void GetDouble_Should_ReturnDoubleEquivalent_For_PositiveInfinity()
    {
        var agtype = new Agtype("Infinity");

        Assert.That(agtype.GetDouble(), Is.EqualTo(double.PositiveInfinity));
    }

    [Test]
    public void GetDouble_Should_ReturnDoubleEquivalent_For_NaN()
    {
        var agtype = new Agtype("NaN");

        Assert.That(agtype.GetDouble(), Is.EqualTo(double.NaN));
    }

    [Test]
    public void GetDouble_Should_ThrowException_When_AgtypeValueIsNull()
    {
        var agtype = new Agtype(null);

        Assert.That(() => agtype.GetDouble(), Throws.InstanceOf<NullReferenceException>());
    }

    [Test]
    public void GetDouble_Should_ThrowException_When_AgtypeValueIsInTheWrongFormat()
    {
        var agtype = new Agtype("true");

        Assert.That(() => agtype.GetDouble(), Throws.InstanceOf<FormatException>());
    }

    #endregion

    #region GetInteger()

    [Test]
    public void GetInteger_Should_ReturnEquivalentDouble()
    {
        var numString = "1";
        var agtype = new Agtype(numString);
        var doubleEquivalent = int.Parse(numString);

        Assert.That(agtype.GetInt32(), Is.EqualTo(doubleEquivalent));
    }

    [Test]
    public void GetInteger_Should_ThrowException_When_AgtypeValueIsInTheWrongFormat()
    {
        var agtype = new Agtype("true");

        Assert.That(() => agtype.GetInt32(), Throws.InstanceOf<FormatException>());
    }

    #endregion

    #region GetLong()

    [Test]
    public void GetLong_Should_ReturnEquivalentDouble()
    {
        var numString = "1";
        var agtype = new Agtype(numString);
        var doubleEquivalent = long.Parse(numString);

        Assert.That(agtype.GetInt64(), Is.EqualTo(doubleEquivalent));
    }

    [Test]
    public void GetLong_Should_ThrowException_When_AgtypeValueIsInTheWrongFormat()
    {
        var agtype = new Agtype("true");

        Assert.That(() => agtype.GetInt64(), Throws.InstanceOf<FormatException>());
    }

    #endregion

    #region GetDecimal()

    [Test]
    public void GetDecimal_Should_ReturnEquivalentDouble()
    {
        var numString = "1";
        var agtype = new Agtype(numString);
        var doubleEquivalent = decimal.Parse(numString);

        Assert.That(agtype.GetDecimal(), Is.EqualTo(doubleEquivalent));
    }

    [Test]
    public void GetDecimal_Should_ThrowException_When_AgtypeValueIsInTheWrongFormat()
    {
        var agtype = new Agtype("true");

        Assert.That(() => agtype.GetDecimal(), Throws.InstanceOf<FormatException>());
    }

    #endregion

    #region GetList()

    [Test]
    public void GetList_Should_ReturnEquivalentList()
    {
        var list = new List<object?> { 1, 2, "string", null, };
        var agtype = new Agtype("[1, 2, \"string\", null]");

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
        var agtype = new Agtype("[1, 2, \"string\", null, [1, 2, \"string\", null]]");

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
        var agtype = new Agtype("[1, 2, \"-Infinity\"]");

        var agtypeList = agtype.GetList(true);

        Assert.Multiple(() =>
        {
            Assert.That(agtypeList, Has.Count.EqualTo(list.Count));
            Assert.That(agtypeList, Is.EquivalentTo(list));
        });
    }

    #endregion

    #region GetVertex()

    [Test]
    public void GetVertex_Should_ReturnEquivalentVertex()
    {
        var vertex = new Vertex
        {
            Id = new(2343953235),
            Label = "Person",
            Properties = new()
            { 
                { "name", "Emmanuel" },
                { "age", 22 },
            },
        };
        var agtype = new Agtype(vertex.ToString());
        var generatedVertex = agtype.GetVertex();

        Assert.Multiple(() =>
        {
            Assert.That(generatedVertex.Id, Is.EqualTo(vertex.Id));
            Assert.That(generatedVertex.Label, Is.EqualTo(vertex.Label));
            Assert.That(generatedVertex.Properties, Is.EquivalentTo(vertex.Properties));
        });
    }
    #endregion

    #region GetEdge()

    [Test]
    public void GetEdge_Should_ReturnEquivalentEdge()
    {
        var edge = new Edge
        {
            Id = new(2),
            StartId = new(0),
            EndId = new(1),
            Label = "Edge_label",
            Properties = new() { { "colour", "red" }, },
        };
        var agtype = new Agtype(edge.ToString());
        var generatedEdge = agtype.GetEdge();

        Assert.Multiple(() =>
        {
            Assert.That(generatedEdge.Id, Is.EqualTo(edge.Id));
            Assert.That(generatedEdge.Label, Is.EqualTo(edge.Label));
            Assert.That(generatedEdge.StartId, Is.EqualTo(edge.StartId));
            Assert.That(generatedEdge.EndId, Is.EqualTo(edge.EndId));
            Assert.That(generatedEdge.Properties, Is.EquivalentTo(edge.Properties));
        });
    }
    #endregion

    #region GetPath()

    [Test]
    public void GetPath_Should_ReturnEquivalentPath()
    {
        Vertex[] vertices =
        [
            new Vertex
            {
                Id = new(0),
                Label = "Label_name_1",
                Properties = new() { { "i", 0 }, },
            },
            new Vertex
            {
                Id = new(2),
                Label = "Label_name_1",
                Properties = [],
            }
        ];
        var edge = new Edge
        {
            Id = new(2),
            StartId = vertices[0].Id,
            EndId = vertices[1].Id,
            Label = "Edge_label",
            Properties = [],
        };
        var agtype = new Agtype($"[{vertices[0]}, {edge}, {vertices[1]}]{Types.Path.FOOTER}");
        var path = agtype.GetPath();

        Assert.Multiple(() =>
        {
            Assert.That(path.Length, Is.EqualTo(1));
            Assert.That(path.Vertices, Has.Length.EqualTo(2));
            Assert.That(path.Edges, Has.Length.EqualTo(1));
            Assert.That(path.Vertices, Is.EquivalentTo(vertices));
            Assert.That(path.Vertices[1].Properties, Is.EquivalentTo(vertices[1].Properties));
            Assert.That(path.Edges[0], Is.EqualTo(edge));
        });
    }

    [Test]
    public void GetPath_Should_ThrowException_When_AgtypeValueIsInWrongFormat()
    {
        Vertex[] vertices =
        [
            new Vertex
            {
                Id = new(0),
                Label = "Label_name_1",
                Properties = new()
                {
                    { "i", 0 },
                },
            },
            new Vertex
            {
                Id = new(2),
                Label = "Label_name_1",
                Properties = [],
            }
        ];
        var edge = new Edge
        {
            Id = new(2),
            StartId = vertices[0].Id,
            EndId = vertices[1].Id,
            Label = "Edge_label",
            Properties = [],
        };
        // Omit the path footer.
        var agtype = new Agtype($"[{vertices[0]}, {edge}, {vertices[1]}]");

        Assert.That(() => agtype.GetPath(), Throws.InstanceOf<FormatException>());
    }

    #endregion
}
