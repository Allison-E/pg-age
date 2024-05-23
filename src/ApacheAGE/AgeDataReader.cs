using System;
using System.Threading.Tasks;
using Npgsql;

namespace ApacheAGE
{
    /// <summary>
    /// Data reader for accessing the results from an AGE query execution.
    /// </summary>
    public class AgeDataReader: IAgeDataReader, IDisposable, IAsyncDisposable
    {
        private readonly NpgsqlDataReader _reader;
        private bool _isDisposed = false;

        /// <summary>
        /// Initialises a new instance of <see cref="AgeDataReader"/>.
        /// </summary>
        /// <param name="reader">
        /// </param>
        internal AgeDataReader(NpgsqlDataReader reader)
        {
            _reader = reader;
        }

        public int FieldCount => _reader.FieldCount;
        public bool IsOnRow => _reader.IsOnRow;
        public bool HasRows => _reader.HasRows;
        public bool IsClosed => _reader.IsClosed;

        public void Close() => _reader.Close();

        public void CloseAsync() => _reader.CloseAsync();

        public bool Read() => _reader.Read();

        public Task<bool> ReadAsync() => _reader.ReadAsync();

        public int GetValues(object[] values) => _reader.GetValues(values);

        public T? GetValue<T>(int ordinal) => _reader.GetFieldValueAsync<T?>(ordinal).GetAwaiter().GetResult();

        public async Task<T?> GetValueAsync<T>(int ordinal)
        {
            var value = await _reader.GetFieldValueAsync<T?>(ordinal)
                .ConfigureAwait(false);
            return value;
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
            _isDisposed = true;
        }

        public async ValueTask DisposeAsync()
        {
            if (!_isDisposed)
            {
                await _reader.DisposeAsync();
                GC.SuppressFinalize(this);
            }
            _isDisposed = true;
        }

        #endregion
    }
}
