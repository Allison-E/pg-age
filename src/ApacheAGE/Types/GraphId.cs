using System;
using System.Diagnostics.CodeAnalysis;

namespace ApacheAGE.Types
{
    /// <summary>
    /// Represents the <c>ag_catalog.graphid</c> PostgreSQL type.
    /// </summary>
    public readonly struct GraphId: IComparable, IComparable<GraphId>
    {
        /// <summary>
        /// Creates a new instance of <see cref="GraphId"/>.
        /// </summary>
        /// <param name="value"></param>
        public GraphId(ulong value) => Value = value;

        /// <summary>
        /// Internal value of the graphid.
        /// </summary>
        public ulong Value { get; }

        public int CompareTo(GraphId other)
        {
            if (this < other)
                return -1;

            if (this > other)
                return 1;

            return 0;
        }

        public int CompareTo(object? obj)
        {
            if (obj is null || obj is not GraphId)
                throw new ArgumentException("obj is not a GraphId", nameof(obj));

            return CompareTo((GraphId)obj);
        }

        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            if (obj is null || obj is not GraphId)
                return false;

            var input = (GraphId)obj;

            return this == input;
        }

        public override int GetHashCode() => Value.GetHashCode();

        public override string ToString() => Value.ToString();

        #region Operators
        public static bool operator <(GraphId left, GraphId right)
        {
            return left.Value < right.Value;
        }

        public static bool operator >(GraphId left, GraphId right)
        {
            return left.Value > right.Value;
        }

        public static bool operator <=(GraphId left, GraphId right)
        {
            return left.Value <= right.Value;
        }

        public static bool operator >=(GraphId left, GraphId right)
        {
            return left.Value >= right.Value;
        }

        public static bool operator ==(GraphId left, GraphId right)
        {
            return left.Value == right.Value;
        }

        public static bool operator !=(GraphId left, GraphId right)
        {
            return left.Value != right.Value;
        } 
        #endregion
    }
}
