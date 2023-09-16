namespace ApacheAGE.UnitTests;

public class AgeClientBuilderTests
{
    [Test]
    public void Constructor_Throws_ArgumentException_When_ConnectionStringIsNull()
    {
        Assert.Throws<ArgumentException>(() => new AgeClientBuilder(null));
    }

    [Test]
    public void Constructor_Throws_ArgumentException_When_ConnectionStringIsEmpty()
    {
        Assert.Throws<ArgumentException>(() => new AgeClientBuilder(string.Empty));
    }
}