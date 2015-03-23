using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace HDV.Tutorials.JsonService.Networks
{
    public class TcpSocketNetworkListener : NetworkListener
    {
        #region TCP Client Info
        private class TcpClientInfo
        {
            public const int BufferSize = 1024;

            private Socket m_ClientSocket;
            private byte[] m_Buffer;
            private string m_Id;
            private TcpHeaderFactory m_HeaderFactory;

            public TcpClientInfo(string id, Socket clientSocket)
            {
                this.m_ClientSocket = clientSocket;
                this.m_Buffer = new byte[BufferSize];
                this.m_Id = id;
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

            public string Id
            {
                get
                {
                    return m_Id;
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
        #endregion

        private Dictionary<string, TcpClientInfo> m_ClientConnections;

        public TcpSocketNetworkListener(string name) 
            : base(name)
        {
            this.m_ClientConnections = new Dictionary<string, TcpClientInfo>();
        }

        public override void StartListen(int port)
        {
            m_ClientConnections.Clear();

            Socket listenerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            listenerSocket.Bind(new IPEndPoint(IPAddress.Any, port));
            listenerSocket.Listen(100);

            listenerSocket.BeginAccept(OnEndAccept, listenerSocket);
        }

        private void OnEndAccept(IAsyncResult iar)
        {
            Socket listenerSocket = iar.AsyncState as Socket;

            //Lưu client vào từ điển theo id
            string connectionId = NetworkConnection.GenerateId();

            var clientSocket = listenerSocket.EndAccept(iar);
            var tcpClientInfo = new TcpClientInfo(connectionId, clientSocket);
            m_ClientConnections.Add(connectionId, tcpClientInfo);

            //Khởi tạo vòng nhận dữ liệu cho client
            clientSocket.BeginReceive(
                tcpClientInfo.Buffer, 
                0, 
                TcpClientInfo.BufferSize,
                SocketFlags.None,
                OnEndReceiveCallback, 
                tcpClientInfo);


            //Tiếp tục lắng nghe kết nối tiếp
            listenerSocket.BeginAccept(OnEndAccept, listenerSocket);

            //Raise Event 
            NetworkConnection connection = new NetworkConnection
            {
                Id = connectionId,
                BelongListener = this,
            };

            RaiseConnecionOpenedEvent(new ConnectionOpenedEventArgs { Connection = connection });
        }

        private void OnEndReceiveCallback(IAsyncResult iar)
        {
            TcpClientInfo tcpClientInfo = iar.AsyncState as TcpClientInfo;
            var tcpClientSocket = tcpClientInfo.ClientSocket;

            try
            {
                int receivedCount = tcpClientSocket.EndReceive(iar);
                if (receivedCount <= 0)
                    throw new SocketException();

                var headerFactory = tcpClientInfo.HeaderFactory;
                headerFactory.PumpData(tcpClientInfo.Buffer, 0, receivedCount);
                string message;
                while (headerFactory.TryProcess(out message))
                {
                    RaiseMessageReceivedEvent(
                        new NetworkConnection
                        {
                            Id = tcpClientInfo.Id,
                            BelongListener = this
                        },
                        message);
                }

                //Tiếp tục nhận dữ liệu
                tcpClientSocket.BeginReceive(
                    tcpClientInfo.Buffer,
                    0,
                    TcpClientInfo.BufferSize,
                    SocketFlags.None,
                    OnEndReceiveCallback,
                    tcpClientInfo);
            }
            catch (SocketException ex)
            {
                //Khi có lỗi xảy ra rong quá trình nhận dữ liệu thì đóng kết nối
                string id = tcpClientInfo.Id;
                m_ClientConnections.Remove(id);
                tcpClientSocket.Close();

                //Ném ra sự kiện đóng kết nối
                RaiseConnectionClosedEvent(
                    new ConnectionClosedEventArgs 
                    { 
                        Connection = new NetworkConnection 
                        { 
                            Id = tcpClientInfo.Id, 
                            BelongListener = this 
                        } 
                    });
            } 
            catch (Exception ex)
            {
                Console.WriteLine("{0} Exception: \r\n {1}", Name, ex);
            }
        }

        public override void SendMessage(NetworkConnection connection, string message)
        {
            if (!m_ClientConnections.ContainsKey(connection.Id))
                return;

            TcpClientInfo clientInfo = m_ClientConnections[connection.Id];
            clientInfo.ClientSocket.Send(
                TcpHeaderFactory.ToPacket(
                    Encoding.UTF8.GetBytes(message)
                ));
        }
    }
}
