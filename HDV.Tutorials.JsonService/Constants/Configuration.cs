using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HDV.Tutorials.JsonService.Constants
{
    public static class Configuration
    {
        public const int WebSocketListenerPort = 6969;
        public const int TcpSocketListenerPort = 7890;
        public const string WebSocketListenerName = "WebSocketListener";
        public const string TcpSocketListenerName = "TcpSocketListener";
        public const string WebSocketConnectionName = "WebSocket Connection";
        public const string TcpSocketConnectionName = "Tcp Connection";

    }
}
