namespace PW.Protocol.Comm;

/// <summary>
/// 字节流与压缩整型（CUInt）相关的扩展方法集合。
/// 协议层使用变长整数压缩字节占用：值越小占用越少（1~5 字节），
/// 通过首字节高位的连续 1 标记后续字节数。
/// </summary>
public static class ConvertExtend
{
    /// <summary>
    /// 将 <see cref="uint"/> 编码为压缩整型（CUInt）字节序列，按大端序输出。
    /// 编码规则按值域分四档：
    /// <list type="bullet">
    /// <item>&lt; 2^6（64）：1 字节，最高位 0</item>
    /// <item>&lt; 2^14：2 字节，最高位 10</item>
    /// <item>&lt; 2^29：4 字节，最高位 110</item>
    /// <item>其它：5 字节，首字节固定 0b11100000，后 4 字节为原值大端序</item>
    /// </list>
    /// </summary>
    /// <param name="i">待编码的无符号整型值。</param>
    /// <returns>压缩后的字节数组（长度 1/2/4/5）。</returns>
    public static byte[] ToCUInt(this uint i)
    {
        // 值小于 2^6 时，单字节即可表示，最高位天然为 0
        if (i < 0b01000000u)
        {
            return [(byte)i];
        }
        // 值小于 2^14 时，2 字节表示，加上 0x8000 让首字节高位变成 10xxxxxx
        if (i < 0b0100000000000000u)
        {
            byte[] b = BitConverter.GetBytes((ushort)(i + 0b1000000000000000u));
            // BitConverter 默认小端，反转得到大端字节序
            Array.Reverse(b);
            return b;
        }
        // 值小于 2^29 时，4 字节表示，加上 0xC0000000 让首字节高位变成 110xxxxx
        if (i < 0b00100000000000000000000000000000u)
        {
            byte[] b = BitConverter.GetBytes(i + 0b11000000000000000000000000000000u);
            Array.Reverse(b);
            return b;
        }

        // 超过 2^29 的大值用「标记字节 0b11100000 + 4 字节大端原值」共 5 字节
        byte[] b1 = BitConverter.GetBytes(i);
        Array.Reverse(b1);

        return [0b11100000, .. b1];
    }

    /// <summary>
    /// 从字节数组指定位置读取一个压缩整型（CUInt），并按读取长度推进 <paramref name="pos"/>。
    /// 解码逻辑按首字节高位标记反推编码档位：
    /// <list type="bullet">
    /// <item>0xxxxxxx：单字节直读</item>
    /// <item>10xxxxxx：双字节，去除偏移 0x8000</item>
    /// <item>110xxxxx：四字节，去除偏移 0xC0000000</item>
    /// <item>1110xxxx：跳过标记字节，读后续 4 字节大端整型</item>
    /// </list>
    /// </summary>
    /// <param name="bytes">完整字节数组（包含至少一个 CUInt）。</param>
    /// <param name="pos">起始读取位置；方法返回时已推进到 CUInt 之后的下一个字节。</param>
    /// <returns>解码得到的无符号整型值。</returns>
    /// <exception cref="ArgumentOutOfRangeException">读取位置或所需字节数超出数组范围时抛出。</exception>
    public static uint ReadCUInt(this byte[] bytes, ref int pos)
    {
        // 越界保护：起始位置不能超出数组
        if (pos >= bytes.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(pos), "读取位置超出字节数组范围");
        }

        uint firstByte = bytes[pos];

        // 首字节最高位为 0：1 字节直读
        if (firstByte < 0b10000000u)
        {
            pos += 1;
            return firstByte;
        }

        // 首字节高位为 10：2 字节编码
        if (firstByte < 0b11000000u)
        {
            if (pos + 1 >= bytes.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(pos), "读取位置超出字节数组范围");
            }

            // 拷贝 2 字节并反转为本机小端，再去除编码偏移 0x8000
            byte[] temp = new byte[2];
            Array.Copy(bytes, pos, temp, 0, 2);
            Array.Reverse(temp);
            pos += 2;
            return BitConverter.ToUInt16(temp) - 0b1000000000000000u;
        }

        // 首字节高位为 110：4 字节编码
        if (firstByte < 0b11100000u)
        {
            if (pos + 3 >= bytes.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(pos), "读取位置超出字节数组范围");
            }

            // 拷贝 4 字节并反转为本机小端，再去除编码偏移 0xC0000000
            byte[] temp = new byte[4];
            Array.Copy(bytes, pos, temp, 0, 4);
            Array.Reverse(temp);
            pos += 4;
            return BitConverter.ToUInt32(temp) - 0b11000000000000000000000000000000u;
        }

        else
        {
            // 首字节高位为 1110：标记字节占 1 位，后续 4 字节为完整大端整型
            if (pos + 4 >= bytes.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(pos), "读取位置超出字节数组范围");
            }

            // 跳过标记字节，读取随后 4 字节大端整型；共消耗 1 + 4 = 5 字节
            byte[] temp = new byte[4];
            Array.Copy(bytes, pos + 1, temp, 0, 4);
            Array.Reverse(temp);
            pos += 5;
            return BitConverter.ToUInt32(temp);
        }
    }


    /// <summary>
    /// 将字节数组转换为大写十六进制字符串（无分隔符），便于打印协议原始报文。
    /// </summary>
    /// <param name="bytes">待转换的字节数组。</param>
    /// <returns>对应的十六进制字符串。</returns>
    public static string ToHexString(this byte[] bytes) => Convert.ToHexString(bytes);
}
