namespace ApacheAGE;

/// <summary>
/// Defines clients for use with the Apache AGE extension for PostgreSQL.
/// </summary>
public interface IAgeClient
{
    /// <summary>
    /// Connect to the database.
    /// </summary>
    /// <param name="cancellationToken">
    /// Token for propagating a notification to stop the running operation.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> for monitoring the progress of the operation.
    /// </returns>
    Task ConnectAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Create a graph.
    /// </summary>
    /// <param name="graphName">
    /// Graph name.
    /// </param>
    /// <param name="cancellationToken">
    /// Token for propagating a notification to stop the running operation.
    /// </param>
    /// <returns></returns>
    Task CreateGraphAsync(
        string graphName,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Disconnect from the database.
    /// </summary>
    /// <param name="cancellationToken">
    /// Token for propagating a notification to stop the running operation.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> for monitoring the progress of the operation.
    /// </returns>
    Task DisconnectAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Drop the given graph.
    /// </summary>
    /// <param name="graphName">
    /// Graph name.
    /// </param>
    /// <param name="cascade">
    /// Indicates that labels and data that depend on the graph should
    /// be deleted. Default is <see langword="false"/>.
    /// </param>
    /// <param name="cancellationToken">
    /// Token for propagating a notification to stop the running operation.
    /// </param>
    /// <returns></returns>
    Task DropGraphAsync(
        string graphName,
        bool cascade = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Execute the given cypher query.
    /// </summary>
    /// <remarks>
    /// Only cypher queries currently supported by Apache AGE will run
    /// successfully; others will throw an error.
    /// </remarks>
    /// <param name="graphName">
    /// Graph name.
    /// </param>
    /// <param name="cypherQuery">
    /// Cypher quer.
    /// </param>
    /// <param name="cancellationToken">
    /// Token for propagating a notification to stop the running operation.
    /// </param>
    /// <returns>
    /// The result as <see cref="AgType"/>.
    /// </returns>
    Task<AgType> ExecuteCypherAsync(
        string graphName,
        string cypherQuery,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Execute the given cypher query.
    /// </summary>
    /// <remarks>
    /// Only cypher queries currently supported by Apache AGE will run
    /// successfully; others will throw an error.
    /// </remarks>
    /// <param name="graphName">
    /// Graph name.
    /// </param>
    /// <param name="cypherQuery">
    /// Cypher quer.
    /// </param>
    /// <param name="cancellationToken">
    /// Token for propagating a notification to stop the running operation.
    /// </param>
    /// <returns>
    /// The result converted to <see cref="AgType{T}"/>.
    /// </returns>
    Task<AgType<T>> ExecuteCypherAsync<T>(
        string graphName,
        string cypherQuery,
        CancellationToken cancellationToken = default);
}
