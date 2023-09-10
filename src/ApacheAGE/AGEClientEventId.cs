namespace ApacheAGE;

public static class AgeClientEventId
{
    #region Connection
    public const int OPENING_CONNECTION          = 1000;
    public const int CONNECTION_OPENED           = 1001;

    public const int CONNECTION_ERROR            = 1900;
    #endregion


    #region Load extension
    public const int CREATING_EXTENSION             = 2100;
    public const int EXTENSION_CREATED              = 2101;
    public const int LOADING_EXTENSION              = 2102;
    public const int EXTENSION_LOADED               = 2103;

    public const int EXTENSION_NOT_CREATED_ERROR  = 2900;
    public const int EXTENSION_NOT_LOADED_ERROR  = 2901;
    #endregion
}
