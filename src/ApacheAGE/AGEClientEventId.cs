namespace ApacheAGE;

public static class AgeClientEventId
{
    #region Connection
    public const int OPENING_CONNECTION          = 1000;
    public const int CONNECTION_OPENED           = 1001;

    public const int CONNECTION_ERROR            = 1900;
    #endregion


    #region Internals
    public const int CREATING_EXTENSION                         = 2100;
    public const int EXTENSION_CREATED                          = 2101;
    public const int LOADING_EXTENSION                          = 2102;
    public const int EXTENSION_LOADED                           = 2103;
    public const int DROPPING_EXTENSION                         = 2104;
    public const int EXTENSION_DROPPED                          = 2105;
    public const int RETRIEVED_CURRENT_SEARCH_PATH             = 2106;
    public const int AG_CATALOG_ADDED_TO_SEARCH_PATH            = 2107;

    public const int EXTENSION_NOT_CREATED_ERROR                = 2900;
    public const int EXTENSION_NOT_LOADED_ERROR                 = 2901;
    public const int EXTENSION_NOT_DROPPED_ERROR                = 2902;
    public const int AG_CATALOG_NOT_ADDED_TO_SEARCH_PATH_ERROR  = 2903;
    #endregion

    #region Commands
    public const int CREATING_GRAPH             = 3000;
    public const int GRAPH_CREATED              = 3001;
    public const int DROPPING_GRAPH             = 3002;
    public const int GRAPH_DROPPED              = 3003;

    public const int EXECUTING_CYPHER           = 3100;
    public const int CYPHER_EXECUTED            = 3101;

    public const int GRAPH_NOT_CREATED_ERROR    = 3900;
    public const int GRAPH_NOT_DROPPED_ERROR    = 3901;
    #endregion

    #region Command
    #endregion
}
