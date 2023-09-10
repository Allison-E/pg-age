using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace ApacheAGE;
internal class AgeLoggerConfiguration
{
    public static AgeLoggerConfiguration NullLoggerConfiguration 
        => new(new NullLoggerFactory());

    public ILogger ConnectionLogger { get; set; }

    public AgeLoggerConfiguration(ILoggerFactory loggerFactory) 
        => ConnectionLogger = loggerFactory.CreateLogger("Apache.AGE.Connection");
}
