namespace ApacheAGE;
internal class AgeConfiguration
{
    public AgeLoggerConfiguration Logger { get; set; }

    public AgeConfiguration(AgeLoggerConfiguration logger) => Logger = logger;
}
