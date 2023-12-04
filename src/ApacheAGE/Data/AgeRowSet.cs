using System;
using System.ComponentModel.DataAnnotations;

namespace ApacheAGE.Data
{
    /// <summary>
    /// Contains a set of columns for a given row. Each field is an instance of 
    /// <see cref="AgType"/>.
    /// </summary>
    public class AgeRowSet
    {
        private readonly AgType[] _results;

        /// <summary>
        /// The number of columns in the row set.
        /// </summary>
        public int Columns => _results.Length;

        /// <summary>
        /// Initialises a new instance of <see cref="AgeRowSet"/>.
        /// </summary>
        /// <param name="values"></param>
        /// <exception cref="ArgumentException"></exception>
        internal AgeRowSet(object[] values)
        {
            if (values is null)
                throw new ArgumentNullException(nameof(values));

            if (values.Length == 0)
                throw new ArgumentException($"'{nameof(values)}' cannot be empty.", nameof(values));

            _results = new AgType[values.Length];

            for (int i = 0; i < values.Length; i++)
            {
                _results[i] = new(values[i]);
            }
        }

        /// <summary>
        /// Get the <see cref="AgType"/> data in the given column.
        /// </summary>
        /// <param name="ordinal">
        /// Zero-based column ordinal.
        /// </param>
        /// <returns>
        /// The <see cref="AgType"/> field value.
        /// </returns>
        public AgType this[int ordinal]
        {
            get => _results[ordinal];
        }
    }
}
