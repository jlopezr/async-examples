using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Kernel.HttpServer
{
    public class Server: IDisposable
    {
        public readonly int Port;
        private readonly TcpListener _tcpListener;
        private readonly Task _listenTask;
        public string ServerName = "Tedd.Demo.HttpServer";

        public Server(int port)
            : this(IPAddress.Any, port)
        {
        }

        public Server(IPAddress listeningAddress, int port)
        {
            Port = port;

            // Start listening
            _tcpListener = new TcpListener(listeningAddress, Port);
            _tcpListener.Start();

            // Start a background thread to listen for incoming
            _listenTask = Task.Factory.StartNew(ListenLoop);
        }

        private async void ListenLoop()
        {
            for (; ; )
            {
                // Wait for connection
                var socket = await _tcpListener.AcceptSocketAsync();
                if (socket == null)
                    break;

                // Got new connection, create a client handler for it
                var client = new ServerClient(this, socket);
                // Create a task to handle new connection
                Task.Factory.StartNew(client.Do);
            }
        }

        public void Dispose()
        {
            if (_listenTask != null)
            {
                _tcpListener.Stop();
                _listenTask.Wait(200);
                _listenTask.Dispose();
            }
        }
    }
}
