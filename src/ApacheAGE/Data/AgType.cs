namespace ApacheAGE.Data;

/// <summary>
/// Represents the data type returned by AGE.
/// </summary>
public class AgType
{
    /// <summary>
    /// Data value.
    /// </summary>
    public object? Value { get; }

    /// <summary>
    /// Creates a new instance of <see cref="AgType"/>.
    /// </summary>
    /// <param name="value">
    /// Value to store in the instance.
    /// </param>
    public AgType(object? value) => Value = value;
}