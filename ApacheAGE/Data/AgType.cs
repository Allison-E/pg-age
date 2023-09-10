namespace ApacheAGE.Data;

/// <summary>
/// Represents the data type returned by AGE.
/// </summary>
/// <remarks>
/// Given the type parameter <typeparamref name="T"/>, an implicit conversion
/// will be tried from <see cref="object"/> to <typeparamref name="T"/> for
/// <see cref="AgType{T}.Value"/>.
/// </remarks>
/// <typeparam name="T">
/// </typeparam>
internal class AgType<T>
{
    /// <summary>
    /// Data value.
    /// </summary>
    public T Value { get; }

    /// <summary>
    /// Creates a new instance of <see cref="AgType{}"/>.
    /// </summary>
    /// <param name="value">
    /// Value to store in the instance.
    /// </param>
    /// <remarks>
    /// An implicit conversion will be tried to convert the input value
    /// to type <typeparamref name="T"/> before storing it.
    /// </remarks>
    public AgType(object value)
    {
        Value = (T)value;
    }
}

/// <summary>
/// Represents the data type returned by AGE.
/// </summary>
internal class AgType: AgType<object>
{
    /// <summary>
    /// Creates a new instance of <see cref="AgType"/>.
    /// </summary>
    /// <param name="value">
    /// Value to store in the instance.
    /// </param>
    public AgType(object value) : base(value)
    {
    }
}