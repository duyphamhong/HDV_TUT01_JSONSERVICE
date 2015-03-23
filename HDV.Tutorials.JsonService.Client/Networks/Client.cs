using HDV.Tutorials.JsonService.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HDV.Tutorials.JsonService.Client.Networks
{
    public class ConnectionOpenedEventArgs : EventArgs
    {
        public bool IsSuccessful
        {
            set;
            get;
        }
    }

    public class ListProductCommandReceivedEventArgs : EventArgs
    {
        public List<Product> Products
        {
            set;
            get;
        }
    }

    public abstract class Client
    {
        protected bool m_IsConnected;
        protected string m_Name;

        public abstract void Connect();

        public bool IsConnected
        {
            get
            {
                return m_IsConnected;
            }
        }

        public string Name
        {
            get
            {
                return m_Name;
            }
        }


        public abstract void SendMessage(string message);
        
        public void SendCommand(Command command)
        {
            SendMessage(command.ToString());
        }

        protected void OnReceiveMessage(string message)
        {
            Command command = JsonConvert.DeserializeObject<Command>(message);
            if (command.Code != 0)
            {
                return;
            }

            if (command.Name.Equals(CommandNames.ListProductCommand))
            {
                OnRecievedListProductCommand(command);
            }
        }

        private void OnRecievedListProductCommand(Command command)
        {
            var productList = command.GetDataAs<List<Product>>();
            if (ListProductCommandReceived != null)
            {
                ListProductCommandReceived(this, new ListProductCommandReceivedEventArgs { Products = productList });
            }
        }

        protected void RaiseConnectionOpenedEvent(ConnectionOpenedEventArgs eventArgs)
        {
            if (ConnectionOpened != null)
                ConnectionOpened(this, eventArgs);
        }

        public event EventHandler<ConnectionOpenedEventArgs> ConnectionOpened;
        public event EventHandler<ListProductCommandReceivedEventArgs> ListProductCommandReceived;
    }
}
