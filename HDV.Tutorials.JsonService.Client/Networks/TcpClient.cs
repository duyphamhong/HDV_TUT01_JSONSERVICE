using HDV.Tutorials.JsonService.Constants;
using HDV.Tutorials.JsonService.Networks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace HDV.Tutorials.JsonService.Client.Networks
{
    public class TcpClient : Client
    {
        const int BufferSize = 1024;

        private class ClientAsyncState
        {
            private Socket m_ClientSocket;
            private byte[] m_Buffer;
            private TcpHeaderFactory m_HeaderFactory;
            public ClientAsyncState(Socket clientSocket)
            {
                this.m_ClientSocket = clientSocket;
                this.m_Buffer = new byte[BufferSize];
                this.m_HeaderFactory = new TcpHeaderFactory();
            }

            public Socket ClientSocket
            {
                get
                {
                    return m_ClientSocket;
                }
            }
            public byte[] Buffer
            {
                get
                {
                    return m_Buffer;
                }
            }

            public TcpHeaderFactory HeaderFactory
            {
                get
                {
                    return m_HeaderFactory;
                }
            }
        }

        private static TcpClient m_Current;
        public static TcpClient Current
        {
            get
            {
                if (m_Current == null)
                    m_Current = new TcpClient();

                return m_Current;
            }
        }
        
        private Socket m_Socket;

        private TcpClient()
        {
            m_Name = Configuration.TcpSocketConnectionName;
        }

        public override void Connect()
        {
            Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            clientSocket.BeginConnect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), Configuration.TcpSocketListenerPort),
                OnConnectCallback,
                clientSocket);
        }

        private void OnConnectCallback(IAsyncResult iar)
        {
            try
            {
                Socket clientSocket = iar.AsyncState as Socket;
                clientSocket.EndConnect(iar);
                this.m_Socket = clientSocket;
                m_IsConnected = true;

                //Bắt đầu nhận dữ liệu
                ClientAsyncState asyncState = new ClientAsyncState(clientSocket);
                clientSocket.BeginReceive(asyncState.Buffer, 0, BufferSize, SocketFlags.None, OnReceiveCallback, asyncState);

                //Nem su kien
                RaiseConnectionOpenedEvent(new ConnectionOpenedEventArgs { IsSuccessful = true });
            }
            catch
            {
                RaiseConnectionOpenedEvent(new ConnectionOpenedEventArgs { IsSuccessful = false });
            }
        }

        private void OnReceiveCallback(IAsyncResult iar)
        {
            ClientAsyncState asyncState = iar.AsyncState as ClientAsyncState;
            Socket clientSocket = asyncState.ClientSocket;

            int readCount = clientSocket.EndReceive(iar);
            TcpHeaderFactory headerFactory = asyncState.HeaderFactory;
            headerFactory.PumpData(asyncState.Buffer, 0, readCount);

            string message;
            while (headerFactory.TryProcess(out message))
            {
                OnReceiveMessage(message);
            }

            clientSocket.BeginReceive(asyncState.Buffer, 0, BufferSize, SocketFlags.None, OnReceiveCallback, asyncState);
        }

        public override void SendMessage(string message)
        {
            if (!m_IsConnected)
                return;

            var encoding = Encoding.UTF8;
            var buffer = encoding.GetBytes(message);
            m_Socket.Send(HDV.Tutorials.JsonService.Networks.TcpHeaderFactory.ToPacket(buffer));
        }
    }
}
