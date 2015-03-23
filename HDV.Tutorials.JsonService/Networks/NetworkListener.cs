using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HDV.Tutorials.JsonService.Networks
{
    public abstract class NetworkListener
    {
        private string m_Name;
        public NetworkListener(string listenerName)
        {
            this.m_Name = listenerName;
        }

        public string Name
        {
            get
            {
                return m_Name;
            }
        }

        /// <summary>
        /// Bắt đầu lắng nghe tại cổng
        /// </summary>
        /// <param name="port">Cổng</param>
        public abstract void StartListen(int port);

        public abstract void SendMessage(NetworkConnection connection, string message);

        //Sự kiện cho kết nối thiết lập
        protected void RaiseConnecionOpenedEvent(ConnectionOpenedEventArgs eventArgs)
        {
            if (ConnectionOpened != null)
                ConnectionOpened(this, eventArgs);
        }

        /// <summary>
        /// Sự kiện khi có một client kết nối tới service
        /// </summary>
        public event EventHandler<ConnectionOpenedEventArgs> ConnectionOpened;


        //Sự kiện cho kết nối bị đóng
        protected void RaiseConnectionClosedEvent(ConnectionClosedEventArgs eventArgs)
        {
            if (ConnectionClosed != null)
                ConnectionClosed(this, eventArgs);
        }

        /// <summary>
        /// Sự kiện khi một kết nối của Client bị đóng
        /// </summary>
        public event EventHandler<ConnectionClosedEventArgs> ConnectionClosed;


        //Sự kiện cho nhận thông điệp
        protected void RaiseMessageReceivedEvent(NetworkConnection connection, string message)
        {
            if (MessageReceived != null)
                MessageReceived(
                    this, 
                    new MessageReceivedEventArgs 
                    { 
                        FromConnection = connection, 
                        Message = message 
                    });
        }

        /// <summary>
        /// Sự kiện khi nhận được một Mesage từ client
        /// </summary>
        public event EventHandler<MessageReceivedEventArgs> MessageReceived;
    }
}
