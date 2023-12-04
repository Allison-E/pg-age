using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using ApacheAGE.JsonConverters;

namespace ApacheAGE.Data
{
    /// <summary>
    /// Represents the agtype data type for Apache AGE.
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

        /// <summary>
        /// Return the agtype value as a string.
        /// </summary>
        /// <returns>
        /// String value.
        /// </returns>
        /// <exception cref="NullReferenceException">
        /// Thrown when the value of the agtype is null.
        /// </exception>
        public string? GetString() => Value?.ToString();

        /// <summary>
        /// Return the agtype value as a boolean.
        /// </summary>
        /// <returns>
        /// Boolean value.
        /// </returns>
        /// <exception cref="NullReferenceException">
        /// Thrown when the value of the agtype is null.
        /// </exception>
        public bool GetBoolean()
        {
            if (Value is null)
                throw new NullReferenceException("Cannot convert agtype to boolean, because its value is null.");

            var stringValue = GetString();

            //if (bool.TrueString.Equals(stringValue!, StringComparison.OrdinalIgnoreCase))
            //    return true;

            //if (bool.FalseString.Equals(stringValue!, StringComparison.OrdinalIgnoreCase))
            //    return false;

            return bool.Parse(stringValue!);
        }

        /// <summary>
        /// Return the agtype value as a double.
        /// </summary>
        /// <returns>
        /// Double value.
        /// </returns>
        /// <exception cref="NullReferenceException">
        /// Thrown when the value of the agtype is null.
        /// </exception>
        public double GetDouble()
        {
            if (Value is null)
                throw new NullReferenceException("Cannot convert agtype to double, because its value is null.");

            var stringValue = GetString();

            if (stringValue!.Equals("-Infinity", StringComparison.OrdinalIgnoreCase))
                return double.NegativeInfinity;

            if (stringValue!.Equals("Infinity", StringComparison.OrdinalIgnoreCase))
                return double.PositiveInfinity;

            if (stringValue!.Equals("NaN", StringComparison.OrdinalIgnoreCase))
                return double.NaN;

            return double.Parse(stringValue!);
        }

        /// <summary>
        /// Return the agtype value as an integer.
        /// </summary>
        /// <returns>
        /// Integer value.
        /// </returns>
        /// <exception cref="NullReferenceException">
        /// Thrown when the value of the agtype is null.
        /// </exception>
        public int GetInteger()
        {
            if (Value is null)
                throw new NullReferenceException("Cannot convert agtype to integer, because its value is null.");

            return int.Parse(GetString()!);
        }

        /// <summary>
        /// Return the agtype value as a long.
        /// </summary>
        /// <returns>
        /// Long value.
        /// </returns>
        /// <exception cref="NullReferenceException">
        /// Thrown when the value of the agtype is null.
        /// </exception>
        public long GetLong()
        {
            if (Value is null)
                throw new NullReferenceException("Cannot convert agtype to long, because its value is null.");

            return long.Parse(GetString()!);
        }

        /// <summary>
        /// Return the agtype value as a decimal.
        /// </summary>
        /// <returns>
        /// Decimal value.
        /// </returns>
        /// <exception cref="NullReferenceException">
        /// Thrown when the value of the agtype is null.
        /// </exception>
        public decimal GetDecimal()
        {
            if (Value is null)
                throw new NullReferenceException("Cannot convert agtype to decimal, because its value is null.");

            return decimal.Parse(GetString()!);
        }

        /// <summary>
        /// Return the agtype value as a list.
        /// </summary>
        /// <remarks>
        /// The list may contain mixed data types.
        /// Example: [1, 2, "string", null].
        /// </remarks>
        /// <param name="readFloatingPointLiterals">
        /// Indicates if the reserved floating values "-Infinity", "Infinity",
        /// and "NaN" should be parsed to <see cref="double.NegativeInfinity"/>,
        /// <see cref="double.PositiveInfinity"/>, and <see cref="double.NaN"/>
        /// respectively.
        /// 
        /// <para>
        /// If <see langword="false"/>, the reserved floating values are parsed as
        /// strings. The default value is <see langword="false"/>.
        /// </para>
        /// 
        /// </param>
        /// <returns>
        /// List.
        /// </returns>
        /// <exception cref="NullReferenceException">
        /// Thrown when the value of the agtype is null.
        /// </exception>
        public List<object?> GetList(bool readFloatingPointLiterals = false)
        {
            if (Value is null)
                throw new NullReferenceException("Cannot convert agtype to list, because its value is null.");

            var json = GetString();
            var serializerOptions = new JsonSerializerOptions
            {
                AllowTrailingCommas = true,
                Converters = { new InferredObjectConverter(), },
                PropertyNameCaseInsensitive = true,
                NumberHandling = readFloatingPointLiterals ? JsonNumberHandling.AllowNamedFloatingPointLiterals : JsonNumberHandling.Strict,
            };
            var result = JsonSerializer.Deserialize<List<object?>>(json!, serializerOptions);

            return result!;
        }

        /// <summary>
        /// Return the agtype value as a <see cref="Vertex"/>.
        /// </summary>
        /// <returns>
        /// Vertex.
        /// </returns>
        /// <exception cref="NullReferenceException">
        /// Thrown when the value of the agtype is null.
        /// </exception>
        public Vertex GetVertex()
        {
            if (Value is null)
                throw new NullReferenceException("Cannot convert agtype to vertex, because its value is null.");

            var json = GetString()!.Replace("::vertex", "");
            var vertex = JsonSerializer.Deserialize<Vertex>(json, new JsonSerializerOptions
            {
                AllowTrailingCommas = true,
                Converters = { new InferredObjectConverter() },
                PropertyNameCaseInsensitive = true,
                NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals,
            });

            return vertex!;
        }

        /// <summary>
        /// Return the agtype value as a <see cref="Edge"/>.
        /// </summary>
        /// <returns>
        /// Edge.
        /// </returns>
        /// <exception cref="NullReferenceException">
        /// Thrown when the value of the agtype is null.
        /// </exception>
        public Edge GetEdge()
        {
            if (Value is null)
                throw new NullReferenceException("Cannot convert agtype to path, because its value is null.");

            var json = GetString()!.Replace("::path", "");
            var edge = JsonSerializer.Deserialize<Edge>(json, new JsonSerializerOptions
            {
                AllowTrailingCommas = true,
                Converters = { new InferredObjectConverter() },
                PropertyNameCaseInsensitive = true,
                NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals,
            });

            return edge!;
        }

        /// <summary>
        /// Return the agtype value as a path containing vertices and edges.
        /// </summary>
        /// <returns>
        /// An array of <see cref="object"/> which can be type casted to either
        /// vertices or edges.
        /// </returns>
        /// <exception cref="NullReferenceException">
        /// Thrown when the value of the agtype is null.
        /// </exception>
        public object[] GetPath()
        {
            try
            {
                if (Value is null)
                    throw new NullReferenceException("Cannot convert agtype to path, because its value is null.");

                var json = GetString()!
                    .Replace("::path", "")
                    .Replace("::vertex", "")
                    .Replace("::edge", "");
                var path = JsonSerializer.Deserialize<List<object>>(json, new JsonSerializerOptions
                {
                    AllowTrailingCommas = true,
                    Converters = { new PathConverter() },
                    PropertyNameCaseInsensitive = true,
                });

                return path!.ToArray();
            }
            catch (JsonException e)
            {
                throw new FormatException("Path may be in the wrong format and cannot be parsed correctly.", e);
                throw;
            }
        }
    }
}