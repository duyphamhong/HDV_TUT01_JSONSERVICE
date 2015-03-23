using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HDV.Tutorials.JsonService.Client.Networks
{
    public class WebSocketClient : Client
    {
        private static WebSocketClient m_Current;
        public static WebSocketClient Current
        {
            get
            {
                if (m_Current == null)
                    m_Current = new WebSocketClient();

                return m_Current;
            }
        }

        private WebSocketClient()
        {
        }

        public override void Connect()
        {
        }


        public override void SendMessage(string message)
        {
        }
    }
}
