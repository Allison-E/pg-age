namespace ApacheAGE;

public static class AGEClientEventId
{
    public const int OpeningConnection = 1000;
    public const int ConnectionOpened = 1001;

    public const int CreatingExtension = 1100;
    public const int ExtensionCreated = 1101;
    public const int LoadingExtension = 1102;
    public const int ExtensionLoaded = 1103;
    public const int AddingCatalogToSearchPath = 1104;
    public const int CatalogAddedToSearchPath = 1105;

    public const int ExtensionNotInstalledError = 1900;
}
