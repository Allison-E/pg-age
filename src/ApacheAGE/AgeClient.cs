using Npgsql;

namespace ApacheAGE;
internal class AgeClient: IAgeClient, IDisposable, IAsyncDisposable
{
    private NpgsqlDataSource _dataSource;

    public string ConnectionString => _dataSource.ConnectionString;

    public AgeClient(string connectionString)
    {
        _dataSource = new NpgsqlDataSourceBuilder(connectionString)
            .Build();
    }

    public Task ConnectAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task CreateGraphAsync(string graphName, CancellationToken cancellationToken = default) => throw new NotImplementedException();
    public Task DisconnectAsync(CancellationToken cancellationToken = default) => throw new NotImplementedException();
    public Task DropGraphAsync(string graphName, bool cascade = false, CancellationToken cancellationToken = default) => throw new NotImplementedException();
    public Task<AgType> ExecuteCypherAsync(string graphName, string cypherQuery, CancellationToken cancellationToken = default) => throw new NotImplementedException();
    public Task<AgType<T>> ExecuteCypherAsync<T>(string graphName, string cypherQuery, CancellationToken cancellationToken = default) => throw new NotImplementedException();
    public void Dispose() => throw new NotImplementedException();
    public ValueTask DisposeAsync() => throw new NotImplementedException();
}
