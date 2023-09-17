using Npgsql;

namespace ApacheAGE;

/// <summary>
/// Client for use with the Apache AGE extension for PostgreSQL.
/// </summary>
public class AgeClient: IAgeClient, IDisposable, IAsyncDisposable
{
    private NpgsqlDataSource? _dataSource;
    private AgeConfiguration? _configuration;

    private bool _isDisposed = false;

    public string ConnectionString => _dataSource!.ConnectionString;
    public bool AgCatalogLoaded { get; private set; } = false;

    internal AgeClient(string connectionString, AgeConfiguration configuration)
    {
        _dataSource = new NpgsqlDataSourceBuilder(connectionString)
            .Build();
        _configuration = configuration;
    }

    ~AgeClient() => Dispose(false);

    public async Task ConnectAsync(CancellationToken cancellationToken = default)
    {
        await using var connection = await OpenConnectionAsync(cancellationToken);
        await CreateExtensionAsync(connection, cancellationToken);
        await AddAgCatalogToSearchPath(connection, cancellationToken);
    }

    public async Task CreateGraphAsync(
        string graphName,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await using var connection = await OpenConnectionAsync(cancellationToken);
            await LoadExtensionAsync(connection, cancellationToken);

            await using var command = new NpgsqlCommand(
                "SELECT * FROM create_graph($1);",
                connection)
            {
                Parameters =
                {
                    new() { Value = graphName },
                }
            };

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
        try
        {
            await using var connection = await OpenConnectionAsync(cancellationToken);
            await LoadExtensionAsync(connection, cancellationToken);

            await using var command = new NpgsqlCommand(
                "SELECT * FROM drop_graph($1, $2);",
                connection)
            {
                Parameters =
                {
                    new() { Value = graphName },
                    new() { Value = cascade },
                }
            };

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
        try
        {
            await using var connection = await OpenConnectionAsync(cancellationToken);
            await LoadExtensionAsync(connection, cancellationToken);

            await using var command = new NpgsqlCommand(
                $"SELECT * FROM cypher('{graph}', $$ {cypher} $$) as (result agtype);",
                connection);

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

    public async Task<AgType> ExecuteQueryAsync(
        string query,
        CancellationToken cancellationToken = default,
        params object?[] parameters)
    {
        try
        {
            await using var connection = await OpenConnectionAsync(cancellationToken);
            await LoadExtensionAsync(connection, cancellationToken);

            await using var command = new NpgsqlCommand(query, connection)
            {
                Parameters = { BuildParameters(parameters), },
            };

            var reader = await command.ExecuteReaderAsync(cancellationToken)
                .ConfigureAwait(false);

            LogMessages.QueryExecuted(
                _configuration!.Logger.CommandLogger,
                query);

            throw new NotImplementedException();
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

            await _dataSource!.DisposeAsync();
            _dataSource = null;
            _isDisposed = true;
        }
    }
    #endregion

    private async Task<NpgsqlConnection> OpenConnectionAsync(
        CancellationToken cancellationToken = default)
    {
        try
        {
            var connection = await _dataSource!.OpenConnectionAsync(cancellationToken)
                .ConfigureAwait(false);

            LogMessages.ConnectionOpened(
                _configuration!.Logger.ConnectionLogger,
                ConnectionString);

            return connection;
        }
        catch (Exception e)
        {
            LogMessages.ConnectionError(
                _configuration!.Logger.ConnectionLogger,
                e.Message,
                e);

            throw new AgeException("Could not connect to to database.", e);
        }
    }

    private async Task CreateExtensionAsync(
        NpgsqlConnection connection,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await using var command = new NpgsqlCommand(
                "CREATE EXTENSION IF NOT EXISTS age;",
                connection);
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
        NpgsqlConnection connection,
        CancellationToken cancellationToken = default)
    {
        try
        {

            await using var command = new NpgsqlCommand(
                "SHOW search_path;",
                connection);

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

            AgCatalogLoaded = true;
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
        NpgsqlConnection connection,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await using var batch = new NpgsqlBatch(connection)
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

    private string BuildQueryForCypherExecution()
    {
        if (AgCatalogLoaded)
            return "SELECT * FROM cypher($1, $$ $2 $$);";
        else
            return "SELECT * FROM ag_catalog.cypher($1, $$ $2 $$);";
    }
}
