using System;

namespace IDMAX_FrameWork
{
    public class CRC
    {
        //const ushort polynomial = 0xA001;       // IEEE 802.3 Standard (32bit = 0x04C11DB7)
        private const ushort polynomial = 0x1021;

        // CRC16 연산 후 Source뒤에 붙여 return
        public static byte[] SetCrc16(byte[] src)
        {
            byte[] ret = new byte[src.Length + 2];
            byte[] sum = new byte[2];

            sum = BitConverter.GetBytes(ComputeChecksumBytes(src, src.Length));
            src.CopyTo(ret, 0);
            ret[src.Length] = sum[1];           // MSB
            ret[src.Length + 1] = sum[0];       // LSB

            return ret;
        }

        private static ushort ComputeChecksumBytes(byte[] bytes, int Length)
        {
            ushort value, temp;
            ushort crc = 0;
            ushort[] table = new ushort[256];

            // Generation CRC16 table
            for (int i = 0; i < table.Length; ++i)
            {
                temp = 0;
                value = (ushort)(i << 8);
                for (int j = 0; j < 8; ++j)
                {
                    if (((value ^ temp) & 0x8000) != 0)
                    {
                        temp = (ushort)((temp << 1) ^ polynomial);
                    }
                    else
                    {
                        temp <<= 1;
                    }
                    value <<= 1;
                }
                table[i] = temp;
            }

            for (int i = 0; i < Length; ++i)
            {
                byte index = (byte)((crc >> 8) ^ bytes[i]);
                crc = (ushort)((crc << 8) ^ table[index]);
            }
            return crc;
        }
    }
}
