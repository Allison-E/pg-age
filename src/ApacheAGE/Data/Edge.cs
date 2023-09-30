using System.Text.Json.Serialization;

namespace ApacheAGE.Data;

/// <summary>
/// AGE edge.
/// </summary>
public class Edge
{
    /// <summary>
    /// GraphId for the edge.
    /// </summary>
    public int Id { get; set; }

    [JsonPropertyName("start_id")]
    /// <summary>
    /// GraphId for the source vertex.
    /// </summary>
    public int StartId { get; set; }

    [JsonPropertyName("end_id")]
    /// <summary>
    /// GraphId for the target vertex.
    /// </summary>
    public int EndId { get; set; }

    /// <summary>
    /// Name of the label for the edge.
    /// </summary>
    public string? Label { get; set; }

    /// <summary>
    /// Edge's properties.
    /// </summary>
    public Dictionary<string, object?>? Properties { get; set; }
}
