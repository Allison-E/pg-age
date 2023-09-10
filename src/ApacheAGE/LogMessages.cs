using Microsoft.Extensions.Logging;

namespace ApacheAGE;
public static partial class LogMessages
{
    #region Connection
    [LoggerMessage(
    EventId = AgeClientEventId.OPENING_CONNECTION,
    Level = LogLevel.Debug,
    Message = "Opening connection to {connectionString}")]
    public static partial void OpeningConnection(
        this ILogger logger,
        string connectionString);
    
    [LoggerMessage(
    EventId = AgeClientEventId.CONNECTION_OPENED,
    Level = LogLevel.Debug,
    Message = "Connection opened to {connectionString}")]
    public static partial void ConnectionOpened(
        this ILogger logger,
        string connectionString);
    
    [LoggerMessage(
    EventId = AgeClientEventId.CONNECTION_ERROR,
    Level = LogLevel.Error,
    Message = "{message}",
    SkipEnabledCheck = true)]
    public static partial void ConnectionError(
        this ILogger logger,
        string message,
        Exception exception);
    #endregion

    #region Load extension
    [LoggerMessage(
    EventId = AgeClientEventId.CREATING_EXTENSION,
    Level = LogLevel.Debug,
    Message = "Creating AGE extension in {connectionString}.")]
    public static partial void CreatingExtension(
    this ILogger logger,
    string connectionString);

    [LoggerMessage(
        EventId = AgeClientEventId.EXTENSION_CREATED,
        Level = LogLevel.Debug,
        Message = "Created AGE extension in {connectionString}.")]
    public static partial void ExtensionCreated(
        this ILogger logger,
        string connectionString);

    [LoggerMessage(
        EventId = AgeClientEventId.LOADING_EXTENSION,
        Level = LogLevel.Debug,
        Message = "Loading AGE extension in {connectionString}.")]
    public static partial void LoadingExtension(
        this ILogger logger,
        string connectionString);

    [LoggerMessage(
        EventId = AgeClientEventId.EXTENSION_LOADED,
        Level = LogLevel.Debug,
        Message = "AGE extension loaded in {connectionString}.")]
    public static partial void ExtensionLoaded(
        this ILogger logger,
        string connectionString);

    [LoggerMessage(
        EventId = AgeClientEventId.EXTENSION_NOT_CREATED_ERROR,
        Level = LogLevel.Warning,
        Message = "AGE extension not created in {connectionString}. Reason: {reason}",
        SkipEnabledCheck = true)]
    public static partial void ExtensionNotCreatedError(
        this ILogger logger,
        string connectionString,
        string reason);
    
    [LoggerMessage(
        EventId = AgeClientEventId.EXTENSION_NOT_LOADED_ERROR,
        Level = LogLevel.Warning,
        Message = "AGE extension not loaded in {connectionString}. Reason: {reason}",
        SkipEnabledCheck = true)]
    public static partial void ExtensionNotLoadedError(
        this ILogger logger,
        string connectionString,
        string reason);
    #endregion
}
