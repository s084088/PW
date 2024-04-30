using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Util.Libs.Sockets;

public abstract partial class SocketClient
{
    private Socket _socket;
    private bool _isRuning;

    public void Connect(string ip, int port)
    {
        if (_isRuning) return;
        _isRuning = true;

        IPEndPoint endPoint = new(IPAddress.Parse(ip), port);
        _socket = new(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        _socket.Connect(endPoint);

        Task.Run(StartRecv);
    }

    public void Disconnect()
    {
        if (!_isRuning) return;

        _socket.Shutdown(SocketShutdown.Both);
        _socket.Close();
        _isRuning = false;
    }

    protected void Send(byte[] bytes)
    {
        SocketEventArgs args = new(bytes);
        OnSending(args);
        if (!args.IsHandled)
        {
            _socket.Send(bytes);
            OnSended(args);
        }
    }

    protected abstract void Receive(byte[] bytes);


    private void StartRecv()
    {
        byte[] container = new byte[1024];
        while (_isRuning)
        {
            try
            {
                int length = _socket.Receive(new ArraySegment<byte>(container), SocketFlags.None);

                if (length == 0)
                {
                    Disconnect();
                    break;
                }

                byte[] recBytes = new byte[length];
                Array.Copy(container, 0, recBytes, 0, length);

                Task.Run(() =>
                {
                    SocketEventArgs args = new(recBytes);
                    OnReveivd(args);
                    if (!args.IsHandled)
                        Receive(recBytes);
                });
            }
            catch
            {
                Disconnect();
                break;
            }
        }
    }
}