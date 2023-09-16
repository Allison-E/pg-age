using Npgsql;

namespace ApacheAGE;

/// <summary>
/// Client for use with the Apache AGE extension for PostgreSQL.
/// </summary>
public class AgeClient: IAgeClient, IDisposable, IAsyncDisposable
{
    private NpgsqlDataSource? _dataSource;
    private AgeConfiguration? _configuration;

    private bool _agCatalogLoaded = false;
    private bool _isDisposed = false;

    public string ConnectionString => _dataSource!.ConnectionString;

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

            LogMessages.CreatingGraph(
                _configuration!.Logger.CommandLogger,
                graphName);

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

    public Task DisconnectAsync(CancellationToken cancellationToken = default) => throw new NotImplementedException();

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

            LogMessages.DroppingGraph(
                _configuration!.Logger.CommandLogger,
                graphName,
                cascade);

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

    public async Task<AgType> ExecuteCypherAsync(
        string graphName,
        string cypherQuery,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await using var connection = await OpenConnectionAsync(cancellationToken);
            await LoadExtensionAsync(connection, cancellationToken);

            await using var command = new NpgsqlCommand(
                "SELECT * FROM cypher($1, $2);",
                connection)
            {
                Parameters =
                {
                    new() { Value = graphName },
                    new() { Value = cypherQuery },
                }
            };

            //_configuration!.Logger.CommandLogger.DroppingGraph(graphName, cascade);
            //await command.ExecuteNonQueryAsync(cancellationToken)
            //    .ConfigureAwait(false);
            //_configuration!.Logger.CommandLogger.GraphDropped(graphName, cascade);

            throw new NotImplementedException();
        }
        catch (PostgresException e)
        {
            LogMessages.GraphNotDroppedError(
                _configuration!.Logger.CommandLogger,
                graphName,
                e.MessageText,
                e);

            throw new AgeException($"Could not create graph '{graphName}'.", e);
        }
        catch (Exception e)
        {
            LogMessages.GraphNotDroppedError(
                _configuration!.Logger.CommandLogger,
                graphName,
                e.Message,
                e);

            throw new AgeException($"Could not create graph '{graphName}'.", e);
        }
    }

    public Task<AgType<T>> ExecuteCypherAsync<T>(string graphName, string cypherQuery, CancellationToken cancellationToken = default) => throw new NotImplementedException();

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
            LogMessages.OpeningConnection(
                _configuration!.Logger.ConnectionLogger,
                ConnectionString);

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
            LogMessages.CreatingExtension(
                _configuration!.Logger.CommandLogger,
                ConnectionString);

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

            _agCatalogLoaded = true;
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
            LogMessages.LoadingExtension(
                _configuration!.Logger.ConnectionLogger,
                ConnectionString);

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
}
