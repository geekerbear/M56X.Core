using System.Text;

namespace M56X.Core.Common
{
    public class BinaryWriterEx : BinaryWriter
    {
        public BinaryWriterEx(Stream output) : base(output)
        {
        }

        public BinaryWriterEx(Stream output, Encoding encoding) : base(output, encoding)
        {
        }

        public BinaryWriterEx(Stream output, Encoding encoding, bool leaveOpen) : base(output, encoding, leaveOpen)
        {
        }

        protected BinaryWriterEx()
        {
        }

        private void WriteReverse(byte[] data)
        {
            Array.Reverse(data);
            base.Write(data);
        }

        public void WriteReverse(short value)
        {
            WriteReverse(BitConverter.GetBytes(value));
        }

        public void WriteReverse(ushort value)
        {
            WriteReverse(BitConverter.GetBytes(value));
        }

        public void WriteReverse(int value)
        {
            WriteReverse(BitConverter.GetBytes(value));
        }

        public void WriteReverse(uint value)
        {
            WriteReverse(BitConverter.GetBytes(value));
        }

        public void WriteReverse(long value)
        {
            WriteReverse(BitConverter.GetBytes(value));
        }

        public void WriteReverse(ulong value)
        {
            WriteReverse(BitConverter.GetBytes(value));
        }

        public void WriteReverse(float value)
        {
            WriteReverse(BitConverter.GetBytes(value));
        }

        public void WriteReverse(double value)
        {
            WriteReverse(BitConverter.GetBytes(value));
        }

    }
}
