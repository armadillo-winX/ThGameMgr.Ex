namespace ThGameMgr.Ex.Extensions
{
    internal static class BinaryReaderExtensions
    {
        public static byte[] ReadExactBytes(this BinaryReader reader, int count)
        {
            if (reader is null)
                throw new ArgumentNullException(nameof(reader));

            byte[] bytes = reader.ReadBytes(count);
            if (bytes.Length < count)
                throw new EndOfStreamException();

            return bytes;
        }
    }
}
