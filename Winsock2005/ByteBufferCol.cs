using System;
using System.Collections;
using System.Diagnostics;

namespace Winsock2005DLL
{
    public class ByteBufferCol : CollectionBase
    {
        [DebuggerNonUserCode]
        public ByteBufferCol()
        {
        }

        public void Add(byte byt) => this.List.Add((object)byt);

        public void Add(byte[] byt)
        {
            int num = Information.UBound((Array)byt);
            int index = 0;
            while (index <= num)
            {
                this.List.Add((object)byt[index]);
                checked { ++index; }
            }
        }

        public byte[] Combine()
        {
            if (this.List.Count == 0)
                return (byte[])null;
            byte[] numArray = new byte[checked(this.List.Count - 1 + 1)];
            int num = checked(this.List.Count - 1);
            int index = 0;
            while (index <= num)
            {
                numArray[index] = Conversions.ToByte(this.List[index]);
                checked { ++index; }
            }
            return numArray;
        }
    }
}
