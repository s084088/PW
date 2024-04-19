namespace PwApi.Comm;

internal static class Extend
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
    /// <param name="length">读取的CUInt长度</param>
    /// <returns>读取到的值</returns>
    public static uint ReadCUInt(this byte[] bytes, int pos, out int length)
    {
        if (pos >= bytes.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(pos), "读取位置超出字节数组范围");
        }

        uint firstByte = bytes[pos];

        if (firstByte < 0b10000000u)
        {
            length = 1;
            return firstByte;
        }

        if (firstByte < 0b11000000u)
        {
            if (pos + 1 >= bytes.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(pos), "读取位置超出字节数组范围");
            }

            length = 2;
            byte[] temp = new byte[2];
            Array.Copy(bytes, pos, temp, 0, 2);
            Array.Reverse(temp);
            return BitConverter.ToUInt16(temp) - 0b1000000000000000u;
        }

        if (firstByte < 0b11100000u)
        {
            if (pos + 3 >= bytes.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(pos), "读取位置超出字节数组范围");
            }

            length = 4;
            byte[] temp = new byte[4];
            Array.Copy(bytes, pos, temp, 0, 4);
            Array.Reverse(temp);
            return BitConverter.ToUInt32(temp) - 0b11000000000000000000000000000000u;
        }

        else
        {
            if (pos + 4 >= bytes.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(pos), "读取位置超出字节数组范围");
            }

            length = 5;
            byte[] temp = new byte[4];
            Array.Copy(bytes, pos + 1, temp, 0, 4);
            Array.Reverse(temp);
            return BitConverter.ToUInt32(temp);
        }
    }

    /// <summary>
    /// 转为字符串
    /// </summary>
    /// <param name="bytes"></param>
    /// <returns></returns>
    public static string ToHexString(this byte[] bytes) => Convert.ToHexString(bytes);

    /// <summary>
    /// 给IEnumerable拓展ForEach方法
    /// </summary>
    /// <typeparam name="T">模型类</typeparam>
    /// <param name="iEnumberable">数据源</param>
    /// <param name="func">方法</param>
    public static void ForEach<T>(this IEnumerable<T> iEnumberable, Action<T> func)
    {
        foreach (T item in iEnumberable)
            func(item);
    }
}