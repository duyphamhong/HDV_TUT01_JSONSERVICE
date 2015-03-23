using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HDV.Tutorials.JsonService.Networks
{
    public class ConnectionOpenedEventArgs : EventArgs
    {
        public NetworkConnection Connection
        {
            set;
            get;
        }
    }

    public class ConnectionClosedEventArgs : EventArgs
    {
        public NetworkConnection Connection
        {
            set;
            get;
        }
    }

    public class MessageReceivedEventArgs : EventArgs
    {
        public NetworkConnection FromConnection
        { 
            set; 
            get; 
        }

        public string Message
        {
            set;
            get;
        }
    }
}
