using System.Buffers;
using System.Text;
using ApacheAGE.Types;
using Npgsql.Internal;

namespace ApacheAGE.Converters
{
#pragma warning disable NPG9001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
    internal class AgtypeConverter: PgBufferedConverter<Agtype>
    {
        public override bool CanConvert(DataFormat format, out BufferRequirements bufferRequirements)
        {
            bufferRequirements = BufferRequirements.None;
            return format is DataFormat.Text;
        }

        /// <summary>
        /// Read agtype from its binary representation.
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        protected override Agtype ReadCore(PgReader reader)
        {
            ReadOnlySequence<byte> textBytes = reader.ReadBytes(reader.CurrentRemaining);
            string text = Encoding.UTF8.GetString(textBytes);

            return new(text);
        }

        /// <summary>
        /// Write agtype to its binary representation.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        protected override void WriteCore(PgWriter writer, Agtype value)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(value.GetString());
            writer.WriteBytes(bytes);
        }
    }
#pragma warning restore NPG9001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
}
