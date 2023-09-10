using Npgsql;

namespace ApacheAGE;

/// <summary>
/// Client for use with the Apache AGE extension for PostgreSQL.
/// </summary>
public class AgeClient: IAgeClient, IDisposable, IAsyncDisposable
{
    private NpgsqlDataSource? _dataSource;
    private AgeConfiguration? _configuration;

    private bool _isLoaded = false;
    private bool _isDisposed = false;     

    public string ConnectionString => _dataSource!.ConnectionString;

    internal AgeClient(string connectionString, AgeConfiguration configuration)
    {
        _dataSource = new NpgsqlDataSourceBuilder(connectionString)
            .Build();
        _configuration = configuration;
    }

    ~AgeClient() => Dispose();

    public async Task LoadAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await using var connection = await OpenConnectionAsync(cancellationToken);
            await CreateExtensionAsync(connection, cancellationToken);
            await LoadExtensionAsync(connection, cancellationToken);

            _isLoaded = true;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public Task CreateGraphAsync(string graphName, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task DropGraphAsync(string graphName, bool cascade = false, CancellationToken cancellationToken = default) => throw new NotImplementedException();
    public Task<AgType> ExecuteCypherAsync(string graphName, string cypherQuery, CancellationToken cancellationToken = default) => throw new NotImplementedException();
    public Task<AgType<T>> ExecuteCypherAsync<T>(string graphName, string cypherQuery, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public void Dispose()
    {
        if (_isDisposed)
            return;

        _isDisposed = true;
        _dataSource!.Dispose();
        _isLoaded = false;
        _dataSource = null;
        _configuration = null;
    }

    public async ValueTask DisposeAsync()
    {
        if (_isDisposed)
            return;

        _isDisposed = true;
        await _dataSource!.DisposeAsync();
        _isLoaded = false;
        _dataSource =null;
        _configuration = null;
        GC.SuppressFinalize(this);
    }

    private async Task<NpgsqlConnection> OpenConnectionAsync(
        CancellationToken cancellationToken = default)
    {
        try
        {
            _configuration!.Logger.ConnectionLogger.OpeningConnection(ConnectionString);

            var connection = await _dataSource!.OpenConnectionAsync(cancellationToken)
                .ConfigureAwait(false);

            _configuration.Logger.ConnectionLogger.ConnectionOpened(ConnectionString);

            return connection;
        }
        catch (Exception e)
        {
            _configuration!.Logger.ConnectionLogger.ConnectionError(e.Message, e);
            throw new AgeException("Could not connect to to database.", e);
        }
    }

    private async Task CreateExtensionAsync(
        NpgsqlConnection connection,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _configuration!.Logger.ConnectionLogger.CreatingExtension(ConnectionString);

            await using var command = new NpgsqlCommand(
                "CREATE EXTENSION IF NOT EXISTS age;",
                connection);
            await command.ExecuteNonQueryAsync(cancellationToken)
                .ConfigureAwait(false);

            _configuration!.Logger.ConnectionLogger.ExtensionCreated(ConnectionString);
        }
        catch (PostgresException e)
        {
            _configuration!.Logger.ConnectionLogger.ExtensionNotCreatedError(
                ConnectionString,
                e.MessageText);

            throw new AgeException("Could not create AGE extension in database.", e);
        }
        catch (Exception e)
        {
            _configuration!.Logger.ConnectionLogger.ExtensionNotCreatedError(
                ConnectionString,
                e.Message);

            throw new AgeException("Could not create AGE extension in database.", e);
        }
    }

    private async Task LoadExtensionAsync(
        NpgsqlConnection connection,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _configuration!.Logger.ConnectionLogger.LoadingExtension(ConnectionString);

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

            _configuration!.Logger.ConnectionLogger.ExtensionLoaded(ConnectionString);
        }
        catch (PostgresException e)
        {
            _configuration!.Logger.ConnectionLogger.ExtensionNotLoadedError(
                ConnectionString,
                e.MessageText);

            throw new AgeException("Could not create AGE extension in database.", e);
        }
        catch (Exception e)
        {
            _configuration!.Logger.ConnectionLogger.ExtensionNotLoadedError(
                ConnectionString,
                e.Message);

            throw new AgeException("Could not create AGE extension in database.", e);
        }
    }
}
