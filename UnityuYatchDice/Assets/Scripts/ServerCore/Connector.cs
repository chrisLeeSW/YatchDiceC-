using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore
{
    public class Connector
    {
        Socket socket;
        IPEndPoint endPoint;

        Session sessionConnectorFuncs;

        public bool isEntery = false;

        public void Connect(IPEndPoint endPoint, Session sessionFactory)
        {
            socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            sessionConnectorFuncs = sessionFactory;
            this.endPoint = endPoint;

            SocketAsyncEventArgs args = new SocketAsyncEventArgs();

            args.Completed += OnConnectCompleted;
            args.RemoteEndPoint = this.endPoint;
            args.UserToken = socket;

            RegisterConnect(args);
        }

       
        void RegisterConnect(SocketAsyncEventArgs args)
        {
            Socket socket = args.UserToken as Socket;
            if (socket == null)
                return;

            bool pending = socket.ConnectAsync(args);
            if (pending == false)
            {
                OnConnectCompleted(null, args);
            }
               
        }

        void OnConnectCompleted(object sender, SocketAsyncEventArgs args)
        {
            if (args.SocketError == SocketError.Success)
            {
                
                sessionConnectorFuncs.Start(args.ConnectSocket);
                sessionConnectorFuncs.OnConnected(args.RemoteEndPoint);
                if (UIManager.Instance != null)
                {
                    UIManager.Instance.funcQueue.Enqueue(UIManager.Instance.OnConnectedServerSucced);
                }
            }
            else
            {
                Console.WriteLine($"OnConnectCompleted Fail: {args.SocketError}");
                if (UIManager.Instance != null)
                {
                    UIManager.Instance.funcQueue.Enqueue(UIManager.Instance.OnConnectedServerFail);
                }
            }
        }
    }
}
