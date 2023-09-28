using Npgsql;

namespace ApacheAGE;

/// <summary>
/// Data reader for accessing the results from an AGE query execution.
/// </summary>
public class AgeDataReader: IAgeDataReader, IDisposable, IAsyncDisposable
{
    private NpgsqlDataReader _reader;
    private bool _isDisposed = false;

    public int FieldCount => _reader.FieldCount;
    public bool IsOnRow => _reader.IsOnRow;
    public bool HasRows => _reader.HasRows;
    public bool IsClosed => _reader.IsClosed;

    /// <summary>
    /// Initialises a new instance of <see cref="AgeDataReader"/>.
    /// </summary>
    /// <param name="reader">
    /// </param>
    internal AgeDataReader(NpgsqlDataReader reader)
    {
        _reader = reader;
    }

    public void Close() => _reader.Close();

    public void CloseAsync() => _reader.CloseAsync();

    public Task<bool> ReadAsync() => _reader.ReadAsync();

    public AgeRowSet GetValues()
    {
        object[] values = new object[FieldCount];

        _ = _reader.GetValues(values);

        return new(values);
    }

    public AgType GetValue(int ordinal)
    {
        var value = _reader.GetValue(ordinal);

        return new(value);
    }

    public string GetName(int ordinal) => _reader.GetName(ordinal);

    public int GetOrdinal(string name) => _reader.GetOrdinal(name);

    #region Dispose

    ~AgeDataReader() => _reader.Dispose();

    public void Dispose()
    {
        if (!_isDisposed)
        {
            _reader.Dispose();
            GC.SuppressFinalize(this); 
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (!_isDisposed)
        {
            await _reader.DisposeAsync();
            GC.SuppressFinalize(this); 
        }
    }

    #endregion
}
