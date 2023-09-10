using Microsoft.Extensions.Logging;

namespace ApacheAGE;
public static partial class LogMessages
{
    [LoggerMessage(
        EventId = AGEClientEventId.CreatingExtension,
        Level = LogLevel.Debug,
        Message = "Creating AGE extension in {connectionString}.")]
    public static partial void CreatingExtension(
        this ILogger logger,
        string connectionString);

    [LoggerMessage(
        EventId = AGEClientEventId.ExtensionCreated,
        Level = LogLevel.Debug,
        Message = "Created AGE extension in {connectionString}.")]
    public static partial void ExtensionCreated(
        this ILogger logger,
        string connectionString);

    [LoggerMessage(
        EventId = AGEClientEventId.LoadingExtension,
        Level = LogLevel.Debug,
        Message = "Loading AGE extension in {connectionString}.")]
    public static partial void LoadingExtension(
        this ILogger logger,
        string connectionString);

    [LoggerMessage(
        EventId = AGEClientEventId.ExtensionLoaded,
        Level = LogLevel.Debug,
        Message = "AGE extension loaded in {connectionString}.")]
    public static partial void ExtensionLoaded(
        this ILogger logger,
        string connectionString);

    [LoggerMessage(
        EventId = AGEClientEventId.AddingCatalogToSearchPath,
        Level = LogLevel.Debug,
        Message = "Adding 'ag_catalog' to 'search_path' in {connectionString}.")]
    public static partial void AddingCatalogToSearchPath(
        this ILogger logger,
        string connectionString);

    [LoggerMessage(
        EventId = AGEClientEventId.CatalogAddedToSearchPath,
        Level = LogLevel.Debug,
        Message = "'ag_catalog' added to 'search_path' in {connectionString}.")]
    public static partial void CatalogAddedToSearchPath(
        this ILogger logger,
        string connectionString);

    [LoggerMessage(
        EventId = AGEClientEventId.ExtensionNotInstalledError,
        Level = LogLevel.Debug,
        Message = "AGE extension not loaded in {connectionString}. Reason: {reason}",
        SkipEnabledCheck = true)]
    public static partial void ExtensionNotInstalledError(
        this ILogger logger,
        string connectionString,
        string reason);
}
