using System;
using System.Collections;

namespace Winsock2005DLL
{
    public class ByteCollection : CollectionBase
    {
        public ByteCollection()
        {
            int num = 1;
            do
            {
                this.List.Add((object)checked((byte)num));
                checked { ++num; }
            }
            while (num <= (int)byte.MaxValue);
        }

        public int FreeByt(byte[] byt)
        {
            int num = checked(this.List.Count - 1);
            int index = 0;
            while (index <= num)
            {
                if (Array.IndexOf<byte>(byt, Conversions.ToByte(this.List[index])) == -1)
                    return (int)Conversions.ToByte(this.List[index]);
                checked { ++index; }
            }
            return 0;
        }

        public bool Contains(byte value) => this.List.Contains((object)value);

        public void Remove(byte value)
        {
            if (!this.Contains(value))
                return;
            this.List.Remove((object)value);
        }

        public byte LowestValue()
        {
            if (this.List.Count == 0)
                return 0;
            byte maxValue = byte.MaxValue;
            int num = checked(this.List.Count - 1);
            int index = 0;
            while (index <= num)
            {
                if ((uint)Conversions.ToByte(this.List[index]) < (uint)maxValue)
                    maxValue = Conversions.ToByte(this.List[index]);
                checked { ++index; }
            }
            return maxValue;
        }
    }
}
