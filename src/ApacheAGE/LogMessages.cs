using Microsoft.Extensions.Logging;

namespace ApacheAGE;

public static partial class LogMessages
{
    #region Connection
    [LoggerMessage(
    EventId = AgeClientEventId.CONNECTION_OPENED,
    Level = LogLevel.Debug,
    Message = "Connection opened to {connectionString}")]
    public static partial void ConnectionOpened(
        ILogger logger,
        string connectionString);

    [LoggerMessage(
    EventId = AgeClientEventId.CONNECTION_ERROR,
    Level = LogLevel.Error,
    Message = "{message}",
    SkipEnabledCheck = true)]
    public static partial void ConnectionError(
        ILogger logger,
        string message,
        Exception exception);
    #endregion

    #region Internals

    [LoggerMessage(
        EventId = AgeClientEventId.EXTENSION_CREATED,
        Level = LogLevel.Debug,
        Message = "Created 'age' extension in {connectionString}")]
    public static partial void ExtensionCreated(
        ILogger logger,
        string connectionString);

    [LoggerMessage(
        EventId = AgeClientEventId.EXTENSION_LOADED,
        Level = LogLevel.Debug,
        Message = "Loaded 'age' in {connectionString}")]
    public static partial void ExtensionLoaded(
        ILogger logger,
        string connectionString);

    [LoggerMessage(
        EventId = AgeClientEventId.RETRIEVED_CURRENT_SEARCH_PATH,
        Level = LogLevel.Debug,
        Message = "Retrieved current search_path. search_path = {searchPath}")]
    public static partial void RetrievedCurrentSearchPath(
        ILogger logger,
        string? searchPath);
    
    [LoggerMessage(
        EventId = AgeClientEventId.AG_CATALOG_ADDED_TO_SEARCH_PATH,
        Level = LogLevel.Debug,
        Message = "'ag_catalog' added to search_path")]
    public static partial void AgCatalogAddedToSearchPath(
        ILogger logger);

    #region Error Logs

    [LoggerMessage(
    EventId = AgeClientEventId.EXTENSION_NOT_CREATED_ERROR,
    Level = LogLevel.Warning,
    Message = "AGE extension not created in {connectionString}. Reason: {reason}",
    SkipEnabledCheck = true)]
    public static partial void ExtensionNotCreatedError(
    ILogger logger,
    string connectionString,
    string reason);

    [LoggerMessage(
        EventId = AgeClientEventId.EXTENSION_NOT_LOADED_ERROR,
        Level = LogLevel.Warning,
        Message = "AGE extension not loaded in {connectionString}. Reason: {reason}",
        SkipEnabledCheck = true)]
    public static partial void ExtensionNotLoadedError(
        ILogger logger,
        string connectionString,
        string reason);

    [LoggerMessage(
        EventId = AgeClientEventId.AG_CATALOG_NOT_ADDED_TO_SEARCH_PATH_ERROR,
        Level = LogLevel.Warning,
        Message = "'ag_catalog' could not be added to search_path. Reason: {reason}. Will use the full qualified name instead")]
    public static partial void AgCatalogNotAddedToSearchPathError(
        ILogger logger,
        string reason);

    #endregion
    #endregion

    #region Commands

    [LoggerMessage(
        EventId = AgeClientEventId.GRAPH_CREATED,
        Level = LogLevel.Information,
        Message = "Created graph '{graphName}'")]
    public static partial void GraphCreated(
        ILogger logger,
        string graphName);

    [LoggerMessage(
        EventId = AgeClientEventId.GRAPH_NOT_CREATED_ERROR,
        Level = LogLevel.Error,
        Message = "Could not droppe graph '{graphName}'. Reason: {reason}")]
    public static partial void GraphNotCreatedError(
        ILogger logger,
        string graphName,
        string reason,
        Exception exception);

    [LoggerMessage(
        EventId = AgeClientEventId.GRAPH_DROPPED,
        Level = LogLevel.Information,
        Message = "Dropped graph '{graphName}'. Cascade: {cascade}")]
    public static partial void GraphDropped(
        ILogger logger,
        string graphName,
        bool cascade);

    [LoggerMessage(
        EventId = AgeClientEventId.CYPHER_QUERY_EXECUTED,
        Level = LogLevel.Information,
        Message = "Executed query\n{query}")]
    public static partial void CypherQueryExecuted(
        ILogger logger,
        string query);

    #region Error logs

    [LoggerMessage(
    EventId = AgeClientEventId.GRAPH_NOT_DROPPED_ERROR,
    Level = LogLevel.Error,
    Message = "Could not drop graph '{graphName}'. Reason: {reason}")]
    public static partial void GraphNotDroppedError(
    ILogger logger,
    string graphName,
    string reason,
    Exception exception);

    #endregion
    #endregion
}
