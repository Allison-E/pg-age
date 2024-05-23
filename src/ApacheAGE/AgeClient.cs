using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Npgsql;

namespace ApacheAGE
{
    /// <summary>
    /// Client for use with the Apache AGE extension for PostgreSQL.
    /// </summary>
    public class AgeClient: IAgeClient, IDisposable, IAsyncDisposable
    {
        private NpgsqlDataSource? _dataSource;
        private AgeConfiguration? _configuration;
        private NpgsqlConnection? _connection;
        private bool _isDisposed = false;

        public string ConnectionString => _dataSource!.ConnectionString;
        public bool IsConnected => _connection?.FullState == System.Data.ConnectionState.Open;

        internal AgeClient(string connectionString, AgeConfiguration configuration)
        {
            var builder = new NpgsqlDataSourceBuilder(connectionString);

            builder.UseAge();

            _dataSource = builder.Build();
            _configuration = configuration;
        }

        ~AgeClient() => Dispose(false);

        #region Connection

        public async Task OpenConnectionAsync(CancellationToken cancellationToken = default)
        {
            _connection = await OpenConnectionInternalAsync(cancellationToken);
            await CreateExtensionAsync(_connection, cancellationToken);
            await AddAgCatalogToSearchPath(_connection, cancellationToken);
            await LoadExtensionAsync(_connection, cancellationToken);
        }

        public async Task CloseConnectionAsync(CancellationToken cancellationToken = default)
        {
            CheckForExistingConnection("There is no existing open connection.");

            try
            {
                await _connection!.CloseAsync();
                await _connection!.DisposeAsync();
                _connection = null;

                LogMessages.ConnectionClosed(
                    _configuration!.Logger.ConnectionLogger,
                    ConnectionString);
            }
            catch (Exception e)
            {
                LogMessages.CloseConnectionError(
                    _configuration!.Logger.ConnectionLogger,
                    e.Message,
                    e);

                throw new AgeException("Could not close the existing connection to the database.", e);
            }
        }

        #endregion

        #region Commands

        public async Task CreateGraphAsync(
            string graphName,
            CancellationToken cancellationToken = default)
        {
            CheckForExistingConnection();

            if (await GraphExistsAsync(graphName, cancellationToken))
                return;

            await using var command = new NpgsqlCommand(
                "SELECT * FROM create_graph($1);",
                _connection)
            {
                Parameters =
                    {
                        new() { Value = graphName },
                    }
            };

            try
            {
                await command.ExecuteNonQueryAsync(cancellationToken)
                    .ConfigureAwait(false);

                LogMessages.GraphCreated(
                    _configuration!.Logger.CommandLogger,
                    graphName);
            }
            catch (PostgresException e)
            {
                LogMessages.GraphNotCreatedError(
                    _configuration!.Logger.CommandLogger,
                    graphName,
                    e.MessageText,
                    e);

                throw new AgeException($"Could not create graph '{graphName}'.", e);
            }
            catch (Exception e)
            {
                LogMessages.GraphNotCreatedError(
                    _configuration!.Logger.CommandLogger,
                    graphName,
                    e.Message,
                    e);

                throw new AgeException($"Could not create graph '{graphName}'.", e);
            }
        }

        public async Task DropGraphAsync(
            string graphName,
            bool cascade = false,
            CancellationToken cancellationToken = default)
        {
            CheckForExistingConnection();

            await using var command = new NpgsqlCommand(
                "SELECT * FROM drop_graph($1, $2);",
                _connection)
            {
                Parameters =
                    {
                        new() { Value = graphName },
                        new() { Value = cascade },
                    }
            };

            try
            {
                await command.ExecuteNonQueryAsync(cancellationToken)
                    .ConfigureAwait(false);

                LogMessages.GraphDropped(
                    _configuration!.Logger.CommandLogger,
                    graphName,
                    cascade);
            }
            catch (PostgresException e)
            {
                LogMessages.GraphNotDroppedError(
                    _configuration!.Logger.CommandLogger,
                    graphName,
                    e.MessageText,
                    e);

                throw new AgeException($"Could not drop graph '{graphName}'.", e);
            }
            catch (Exception e)
            {
                LogMessages.GraphNotDroppedError(
                    _configuration!.Logger.CommandLogger,
                    graphName,
                    e.Message,
                    e);

                throw new AgeException($"Could not drop graph '{graphName}'.", e);
            }
        }

        public async Task ExecuteCypherAsync(
            string graph,
            string cypher,
            CancellationToken cancellationToken = default)
        {
            CheckForExistingConnection();

            await using var command = new NpgsqlCommand(
                $"SELECT * FROM cypher('{graph}', $$ {cypher} $$) as (result agtype);",
                _connection);

            try
            {
                var result = await command.ExecuteNonQueryAsync(cancellationToken)
                    .ConfigureAwait(false);

                LogMessages.CypherExecuted(
                    _configuration!.Logger.CommandLogger,
                    graph,
                    cypher);
            }
            catch (PostgresException e)
            {
                LogMessages.CypherExecutionError(
                    _configuration!.Logger.CommandLogger,
                    graph,
                    e.MessageText,
                    e);

                throw new AgeException($"Could not execute Cypher command.", e);
            }
            catch (Exception e)
            {
                LogMessages.CypherExecutionError(
                    _configuration!.Logger.CommandLogger,
                    graph,
                    e.Message,
                    e);

                throw new AgeException($"Could not execute Cypher command.", e);
            }
        }

        public async Task<AgeDataReader> ExecuteQueryAsync(
            string query,
            CancellationToken cancellationToken = default,
            params object?[] parameters)
        {
            CheckForExistingConnection();

            await using var command = new NpgsqlCommand(query, _connection)
            {
                AllResultTypesAreUnknown = true
            };
            var @params = BuildParameters(parameters);
            foreach (var param in @params)
            {
                command.Parameters.Add(param);
            }

            try
            {
                var reader = await command.ExecuteReaderAsync(cancellationToken)
                    .ConfigureAwait(false);

                LogMessages.QueryExecuted(
                    _configuration!.Logger.CommandLogger,
                    query);

                return new(reader);
            }
            catch (PostgresException e)
            {
                LogMessages.QueryExecutionError(
                    _configuration!.Logger.CommandLogger,
                    query,
                    e.MessageText,
                    e);

                throw new AgeException($"Could not execute query.", e);
            }
            catch (Exception e)
            {
                LogMessages.GraphNotDroppedError(
                    _configuration!.Logger.CommandLogger,
                    query,
                    e.Message,
                    e);

                throw new AgeException($"Could not execute query.", e);
            }
        }

        public async Task<bool> GraphExistsAsync(
            string graphName,
            CancellationToken cancellationToken = default)
        {
            CheckForExistingConnection();

            await using var command = new NpgsqlCommand(
                "SELECT * FROM ag_catalog.ag_graph WHERE name = $1;",
                _connection)
            {
                Parameters =
                    {
                        new() { Value = graphName },
                    }
            };

            try
            {
                object? result = await command.ExecuteScalarAsync(cancellationToken)
                    .ConfigureAwait(false);

                if (result is null)
                {
                    LogMessages.GraphExists(
                        _configuration!.Logger.CommandLogger,
                        graphName);

                    return false;
                }

                LogMessages.GraphDoesNotExist(
                        _configuration!.Logger.CommandLogger,
                        graphName);

                return true;
            }
            catch (Exception e)
            {
                LogMessages.UnknownError(_configuration!.Logger.CommandLogger, e);
                throw new AgeException($"An error occurred.", e);
            }
        }

        #endregion

        #region Dispose
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public async ValueTask DisposeAsync()
        {
            await DisposeAsync(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    _configuration = null;
                }

                if (_connection is not null)
                {
                    _connection.Dispose();
                    _connection = null;
                }

                _dataSource!.Dispose();
                _dataSource = null;
                _isDisposed = true;
            }
        }

        protected virtual async ValueTask DisposeAsync(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    _configuration = null;
                }

                if (_connection is not null)
                {
                    await _connection.DisposeAsync();
                    _connection = null;
                }

                await _dataSource!.DisposeAsync();
                _dataSource = null;

                _isDisposed = true;
            }
        }
        #endregion

        #region Internals

        private async Task<NpgsqlConnection> OpenConnectionInternalAsync(
            CancellationToken cancellationToken = default)
        {
            try
            {
                var _connection = await _dataSource!.OpenConnectionAsync(cancellationToken)
                    .ConfigureAwait(false);

                LogMessages.ConnectionOpened(
                    _configuration!.Logger.ConnectionLogger,
                    ConnectionString);

                return _connection;
            }
            catch (Exception e)
            {
                LogMessages.OpenConnectionError(
                    _configuration!.Logger.ConnectionLogger,
                    e.Message,
                    e);

                throw new AgeException("Could not connect to the database.", e);
            }
        }

        private async Task CreateExtensionAsync(
            NpgsqlConnection _connection,
            CancellationToken cancellationToken = default)
        {
            try
            {
                await using var command = new NpgsqlCommand(
                    "CREATE EXTENSION IF NOT EXISTS age;",
                    _connection);
                await command.ExecuteNonQueryAsync(cancellationToken)
                    .ConfigureAwait(false);

                LogMessages.ExtensionCreated(
                    _configuration!.Logger.CommandLogger,
                    ConnectionString);
            }
            catch (PostgresException e)
            {
                LogMessages.ExtensionNotCreatedError(
                    _configuration!.Logger.CommandLogger,
                    ConnectionString,
                    e.MessageText);

                throw new AgeException("Could not create AGE extension in database.", e);
            }
            catch (Exception e)
            {
                LogMessages.ExtensionNotCreatedError(
                    _configuration!.Logger.CommandLogger,
                    ConnectionString,
                    e.Message);

                throw new AgeException("Could not create AGE extension in database.", e);
            }
        }

        private async Task AddAgCatalogToSearchPath(
            NpgsqlConnection _connection,
            CancellationToken cancellationToken = default)
        {
            try
            {

                await using var command = new NpgsqlCommand(
                    "SHOW search_path;",
                    _connection);

                var searchPath = (string?)await command.ExecuteScalarAsync(cancellationToken)
                    .ConfigureAwait(false);

                LogMessages.RetrievedCurrentSearchPath(
                    _configuration!.Logger.CommandLogger,
                    searchPath);

                var query = searchPath is not null
                    ? $"SET search_path = ag_catalog, {searchPath};"
                    : "SET search_path = ag_catalog;";

                command.CommandText = query;

                await command.ExecuteNonQueryAsync(cancellationToken)
                    .ConfigureAwait(false);

                LogMessages.AgCatalogAddedToSearchPath(
                    _configuration!.Logger.CommandLogger);
            }
            catch (PostgresException e)
            {
                LogMessages.AgCatalogNotAddedToSearchPathError(
                    _configuration!.Logger.CommandLogger,
                    e.MessageText);
            }
            catch (Exception e)
            {
                LogMessages.AgCatalogNotAddedToSearchPathError(
                    _configuration!.Logger.CommandLogger,
                    e.Message);
            }
        }

        private async Task LoadExtensionAsync(
            NpgsqlConnection _connection,
            CancellationToken cancellationToken = default)
        {
            try
            {
                await using var batch = new NpgsqlBatch(_connection)
                {
                    BatchCommands =
                    {
                        new("LOAD 'age';"),
                        new("SET search_path = ag_catalog, \"$user\", public;")
                    }
                };

                await batch.ExecuteNonQueryAsync(cancellationToken)
                    .ConfigureAwait(false);

                LogMessages.ExtensionLoaded(
                    _configuration!.Logger.ConnectionLogger,
                    ConnectionString);
            }
            catch (PostgresException e)
            {
                LogMessages.ExtensionNotLoadedError(
                    _configuration!.Logger.ConnectionLogger,
                    ConnectionString,
                    e.MessageText);

                throw new AgeException("Could not create AGE extension in database.", e);
            }
            catch (Exception e)
            {
                LogMessages.ExtensionNotLoadedError(
                    _configuration!.Logger.ConnectionLogger,
                    ConnectionString,
                    e.Message);

                throw new AgeException("Could not create AGE extension in database.", e);
            }
        }

        private static List<NpgsqlParameter> BuildParameters(object?[] parameters)
        {
            var result = new List<NpgsqlParameter>();

            if (parameters == null || parameters.Length == 0)
                return result;

            foreach (var parameter in parameters)
            {
                result.Add(new NpgsqlParameter { Value = parameter });
            }

            return result;
        }

        #endregion

        #region Checks

        private void CheckForExistingConnection(string? message = null)
        {
            if (_connection is not null)
                return;

            LogMessages.NoExistingConnectionWarning(
                _configuration!.Logger.ConnectionLogger,
                "An attempt to perform certain action was made when there is no existing connection to the database");
            message ??= "There is no existing connection to the database. Call OpenConnectionAsync() to open a connection.";
            throw new AgeException(message);
        }

        #endregion
    }
}
