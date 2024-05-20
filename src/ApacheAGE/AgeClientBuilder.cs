using System;
using Microsoft.Extensions.Logging;

namespace ApacheAGE
{
    public class AgeClientBuilder
    {
        private ILoggerFactory? _logger;
        private readonly string _connectionString;

        /// <summary>
        /// Create a client builder.
        /// </summary>
        /// <param name="connectionString">
        /// Connection string to the database.
        /// </param>
        /// <exception cref="ArgumentException">
        /// A required argument is null or empty or default.
        /// </exception>
        public AgeClientBuilder(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentException("Connection string cannot be null or empty.", nameof(connectionString));

            _connectionString = connectionString;
        }

        /// <summary>
        /// Make use of a given configured logger.
        /// </summary>
        /// <param name="logger">
        /// Logger factory that will be used for logging.
        /// </param>
        /// <returns>
        /// The same builder instance so multiple calls could be chained.
        /// </returns>
        public AgeClientBuilder UseLogger(ILoggerFactory logger)
        {
            _logger = logger;
            return this;
        }

        /// <summary>
        /// Generate a new <see cref="AgeClient"/> using the configurations
        /// set in the builder.
        /// </summary>
        /// <returns></returns>
        public AgeClient Build() => new(_connectionString, BuildConfigurations());

        private AgeConfiguration BuildConfigurations() => 
            new(_logger is null
                    ? AgeLoggerConfiguration.NullLoggerConfiguration
                    : new AgeLoggerConfiguration(_logger));
    }
}
