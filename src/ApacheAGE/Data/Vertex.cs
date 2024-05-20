using System.Collections.Generic;

namespace ApacheAGE.Data
{
    /// <summary>
    /// AGE vertex.
    /// </summary>
    public class Vertex
    {
        /// <summary>
        /// GraphId for the vertex.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Name of the label for this vertex.
        /// </summary>
        public string? Label { get; set; }

        /// <summary>
        /// Vertex's properties.
        /// </summary>
        public Dictionary<string, object?>? Properties { get; set; }
    }
}
