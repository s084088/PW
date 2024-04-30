namespace PW.Protocol.Comm;

public static class ConvertExtend
{
    /// <summary>
    /// 转为压缩整型
    /// </summary>
    /// <param name="i"></param>
    /// <returns></returns>
    public static byte[] ToCUInt(this uint i)
    {
        if (i < 0b01000000u)
        {
            return [(byte)i];
        }
        if (i < 0b0100000000000000u)
        {
            byte[] b = BitConverter.GetBytes((ushort)(i + 0b1000000000000000u));
            Array.Reverse(b);
            return b;
        }
        if (i < 0b00100000000000000000000000000000u)
        {
            byte[] b = BitConverter.GetBytes(i + 0b11000000000000000000000000000000u);
            Array.Reverse(b);
            return b;
        }

        byte[] b1 = BitConverter.GetBytes(i);
        Array.Reverse(b1);

        return [0b11100000, .. b1];
    }

    /// <summary>
    /// 从字节组中读取CUInt
    /// </summary>
    /// <param name="bytes">完整字节组</param>
    /// <param name="pos">开始读取的位置</param>
    /// <returns>读取到的值</returns>
    public static uint ReadCUInt(this byte[] bytes, ref int pos)
    {
        if (pos >= bytes.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(pos), "读取位置超出字节数组范围");
        }

        uint firstByte = bytes[pos];

        if (firstByte < 0b10000000u)
        {
            pos += 1;
            return firstByte;
        }

        if (firstByte < 0b11000000u)
        {
            if (pos + 1 >= bytes.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(pos), "读取位置超出字节数组范围");
            }

            byte[] temp = new byte[2];
            Array.Copy(bytes, pos, temp, 0, 2);
            Array.Reverse(temp);
            pos += 2;
            return BitConverter.ToUInt16(temp) - 0b1000000000000000u;
        }

        if (firstByte < 0b11100000u)
        {
            if (pos + 3 >= bytes.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(pos), "读取位置超出字节数组范围");
            }

            byte[] temp = new byte[4];
            Array.Copy(bytes, pos, temp, 0, 4);
            Array.Reverse(temp);
            pos += 4;
            return BitConverter.ToUInt32(temp) - 0b11000000000000000000000000000000u;
        }

        else
        {
            if (pos + 4 >= bytes.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(pos), "读取位置超出字节数组范围");
            }

            byte[] temp = new byte[4];
            Array.Copy(bytes, pos + 1, temp, 0, 4);
            Array.Reverse(temp);
            pos += 5;
            return BitConverter.ToUInt32(temp);
        }
    }


    public static string ToHexString(this byte[] bytes) => Convert.ToHexString(bytes);
}
