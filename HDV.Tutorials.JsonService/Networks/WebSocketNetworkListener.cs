using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Fleck;


namespace HDV.Tutorials.JsonService.Networks
{
    public class WebSocketNetworkListener : NetworkListener
    {
        private class WebSocketClientInfo
        {
            private string m_Id;
            private IWebSocketConnection m_Socket;

            public WebSocketClientInfo(string id, IWebSocketConnection socket)
            {
                this.m_Id = id;
                this.m_Socket = socket;
            }

            public string Id
            {
                get
                {
                    return m_Id;
                }
            }

            public IWebSocketConnection Socket
            {
                get
                {
                    return m_Socket;
                }
            }
        }

        private string m_Location;
        private Dictionary<string, WebSocketClientInfo> m_Connections;

        public WebSocketNetworkListener(string name, string location)
            : base(name)
        {
            this.m_Location = location;
            this.m_Connections = new Dictionary<string, WebSocketClientInfo>();
        }

        public override void StartListen(int port)
        {
            WebSocketServer webSocketServer = new WebSocketServer(port, m_Location);
            webSocketServer.Start(client => 
            {
                client.OnOpen = () => OnConnectionOpened(client);
                client.OnClose = () => OnConnectionClosed(client);
                client.OnMessage = (message) => OnMessageReceived(client, message);
            });
        }

        private void OnConnectionOpened(IWebSocketConnection client)
        {
            //Lưu vào từ điển
            string connectionId = NetworkConnection.GenerateId();
            WebSocketClientInfo clientInfo = new WebSocketClientInfo(connectionId, client);
            m_Connections.Add(connectionId, clientInfo);

            //Ném ra sự kiện
            RaiseConnecionOpenedEvent(
                new ConnectionOpenedEventArgs 
                { 
                    Connection = new NetworkConnection 
                    { 
                        Id = connectionId, 
                        BelongListener = this 
                    } 
                });
        }

        private void OnConnectionClosed(IWebSocketConnection client)
        {
            //Xóa khỏi từ điển
            var clientInfo = FindClientBySocket(client);
            if (clientInfo == null)
                return;

            m_Connections.Remove(clientInfo.Id);

            //Ném ra sự kiện
            RaiseConnectionClosedEvent(
                new ConnectionClosedEventArgs
                {
                    Connection = new NetworkConnection
                    {
                        Id = clientInfo.Id,
                        BelongListener = this
                    }
                });
        }

        private void OnMessageReceived(IWebSocketConnection client, string message)
        {
            var clientInfo = FindClientBySocket(client);
            if (clientInfo == null)
                return;

            RaiseMessageReceivedEvent(
                new NetworkConnection
                {
                    Id = clientInfo.Id,
                    BelongListener = this
                },
                message);
        }

        private WebSocketClientInfo FindClientBySocket(IWebSocketConnection socket)
        {
            foreach (var item in m_Connections)
            {
                if (item.Value == socket)
                    return item.Value;
            }

            return null;
        }

        public override void SendMessage(NetworkConnection connection, string message)
        {
            if (m_Connections.ContainsKey(connection.Id))
                return;

            WebSocketClientInfo clientInfo = m_Connections[connection.Id];
            clientInfo.Socket.Send(message);
        }
    }
}
