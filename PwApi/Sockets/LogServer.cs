namespace PwApi.Sockets;

public class LogServer
{
    private bool isHandled = false;
    internal LogConfig _config;


    public void LogSendPackets<T>(T data) where T : ISendPackage
    {
        if (isHandled) return;
        _config.dics.TryGetValue(typeof(T), out List<Action<LogArgs>> list);
        if (list != null && list.Count > 0)
        {
            LogArgs<T> args = new(data) { IsHandled = isHandled };

            foreach (Action<LogArgs> action in list)
            {
                action(args);
                isHandled = args.IsHandled;
                if (isHandled) return;
            }
        }

        if (isHandled) return;
        if (_config.LogSendPackage != null)
        {
            LogSendPackageArgs args = new(data) { IsHandled = isHandled };
            _config.LogSendPackage.Invoke(args);

            isHandled = args.IsHandled;
        }
    }

    public void LogRecvPackets<T>(T data) where T : IRecvPackage
    {
        if (isHandled) return;
        _config.dics.TryGetValue(typeof(T), out List<Action<LogArgs>> list);
        if (list != null && list.Count > 0)
        {
            LogArgs<T> args = new(data) { IsHandled = isHandled };

            foreach (Action<LogArgs> action in list)
            {
                action(args);
                isHandled = args.IsHandled;
                if (isHandled) return;
            }
        }

        if (isHandled) return;
        if (_config.LogSendPackage != null)
        {
            LogRecvPackageArgs args = new(data) { IsHandled = isHandled };
            _config.LogRecvPackage.Invoke(args);

            isHandled = args.IsHandled;
        }
    }

    public void LogCallSend<TSend, TRecv>(ICallPackage<TSend, TRecv> data)
        where TSend : ISend
        where TRecv : IRecv
    {
        if (isHandled) return;
        _config.dics.TryGetValue(typeof(TSend), out List<Action<LogArgs>> list);
        if (list != null && list.Count > 0)
        {
            LogArgs<TSend> args = new(data.Send) { IsHandled = isHandled };

            foreach (Action<LogArgs> action in list)
            {
                action(args);
                isHandled = args.IsHandled;
                if (isHandled) return;
            }
        }

        if (isHandled) return;
        if (_config.LogCallSendPacakge != null)
        {
            LogCallSendPacakgeArgs args = new(data.Type, data.Send) { IsHandled = isHandled };
            _config.LogCallSendPacakge.Invoke(args);

            isHandled = args.IsHandled;
        }
    }



    public void LogCallRecv<TSend, TRecv>(ICallPackage<TSend, TRecv> data)
        where TSend : ISend
        where TRecv : IRecv
    {
        if (isHandled) return;
        _config.dics.TryGetValue(typeof(TRecv), out List<Action<LogArgs>> list);
        if (list != null && list.Count > 0)
        {
            LogArgs<TRecv> args = new(data.Recv) { IsHandled = isHandled };

            foreach (Action<LogArgs> action in list)
            {
                action(args);
                isHandled = args.IsHandled;
                if (isHandled) return;
            }
        }

        if (isHandled) return;
        if (_config.LogCallRecvPacakge != null)
        {
            LogCallRecvPacakgeArgs args = new(data.Type, data.Send, data.Recv) { IsHandled = isHandled };
            _config.LogCallRecvPacakge.Invoke(args);

            isHandled = args.IsHandled;
        }
    }

    public void LogSendBytes(byte[] data)
    {
        if (isHandled) return;

        if (_config.LogSendBytes != null)
        {
            LogBytesArgs args = new(data) { IsHandled = isHandled };
            _config.LogSendBytes.Invoke(args);

            isHandled = args.IsHandled;
        }
    }

    public void LogRecvBytes(byte[] data)
    {
        if (isHandled) return;

        if (_config.LogRecvBytes != null)
        {
            LogBytesArgs args = new(data) { IsHandled = isHandled };
            _config.LogRecvBytes.Invoke(args);

            isHandled = args.IsHandled;
        }
    }

}

public class LogServerpProvider
{
    internal LogConfig _config = new();

    public LogServer CreateServer() => new() { _config = _config };
}

public class LogConfig
{
    internal Dictionary<Type, List<Action<LogArgs>>> dics = [];

    /// <summary>
    /// 记录发送的字节
    /// </summary>
    public Action<LogBytesArgs> LogSendBytes { get; set; }

    /// <summary>
    /// 记录收到的字节
    /// </summary>
    public Action<LogBytesArgs> LogRecvBytes { get; set; }


    public Action<LogSendPackageArgs> LogSendPackage { get; set; }


    public Action<LogRecvPackageArgs> LogRecvPackage { get; set; }


    public Action<LogCallSendPacakgeArgs> LogCallSendPacakge { get; set; }

    public Action<LogCallRecvPacakgeArgs> LogCallRecvPacakge { get; set; }


    public LogConfig AddPackageLog<T>(Action<LogArgs<T>> action) where T : class
    {
        if (action == null) return this;

        if (!dics.TryGetValue(typeof(T), out List<Action<LogArgs>> list))
        {
            list = [];
            dics.Add(typeof(T), list);
        }

        list.Add(x => action.Invoke(x as LogArgs<T>));
        return this;
    }
}


public abstract record LogArgs
{
    public bool IsHandled { get; set; }
}

public record LogArgs<T>(T Data) : LogArgs;

public record LogBytesArgs(byte[] Data) : LogArgs;

public record LogSendPackageArgs(ISendPackage Package) : LogArgs;

public record LogRecvPackageArgs(IRecvPackage Package) : LogArgs;

public record LogCallSendPacakgeArgs(uint type, ISend Send) : LogArgs;
public record LogCallRecvPacakgeArgs(uint type, ISend Send, IRecv Recv) : LogArgs;
