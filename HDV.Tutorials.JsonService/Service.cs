using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HDV.Tutorials.JsonService.Networks;
using HDV.Tutorials.JsonService.Constants;
using HDV.Tutorials.JsonService.Models;
using Newtonsoft.Json;

namespace HDV.Tutorials.JsonService
{
    public class Service
    {
        private static Service m_Current;
        public static Service Current
        {
            get
            {
                if (m_Current == null)
                    m_Current = new Service();

                return m_Current;
            }
        }

        private Service()
        {
        }

        public void Start()
        {
            //Tcp Network Listener 
            try
            {
                TcpSocketNetworkListener tcpListener = new TcpSocketNetworkListener(Configuration.TcpSocketListenerName);
                tcpListener.ConnectionOpened += OnConnectionOpened;
                tcpListener.ConnectionClosed += OnConnectionClosed;
                tcpListener.MessageReceived += OnMessageReceived;
                tcpListener.StartListen(Configuration.TcpSocketListenerPort);

                Console.WriteLine("TCP Socket Listener was started!");
            } 
            catch
            {
                Console.WriteLine("Can't start TCP Socket Listener");
            }

            //Tcp Network Listener 
            try
            {
                WebSocketNetworkListener webListener = new WebSocketNetworkListener(Configuration.WebSocketListenerName, "ws://0.0.0.0");
                webListener.ConnectionOpened += OnConnectionOpened;
                webListener.ConnectionClosed += OnConnectionClosed;
                webListener.StartListen(Configuration.WebSocketListenerPort);

                Console.WriteLine("Web Socket Listener was started!");
            }
            catch
            {
                Console.WriteLine("Can't start Web Socket Listener");
            }
        }

        private void OnMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            NetworkConnection connection = e.FromConnection;
            Console.WriteLine("{0}: {1} - \"{2}\"", connection.BelongListener.Name, connection.Id, e.Message);

            Command command = JsonConvert.DeserializeObject<Command>(e.Message);
            if (command.Name.Equals(CommandNames.ListProductCommand))
            {
                OnProcessCommandListProductCommand(connection, command);
            }
        }

        private void OnConnectionOpened(object sender, ConnectionOpenedEventArgs e)
        {
            NetworkConnection connection = e.Connection;
            Console.WriteLine("{0}: {1} connected to service", connection.BelongListener.Name, connection.Id);
        }

        private void OnConnectionClosed(object sender, ConnectionClosedEventArgs e)
        {
            NetworkConnection connection = e.Connection;
            Console.WriteLine("#{0}: {1} disconnected to service", connection.BelongListener.Name, connection.Id);
        }


        private void OnProcessCommandListProductCommand(NetworkConnection connection, Command command)
        {
            List<Product> inStoreProducts = new List<Product>();

            inStoreProducts.Add(new Product
            {
                Id = "1",
                Price = 30000,
                Name = "Omo"
            });
            inStoreProducts.Add(new Product
            {
                Id = "2",
                Price = 50000,
                Name = "Lifebouy"
            });
            inStoreProducts.Add(new Product
            {
                Id = "3",
                Price = 5000,
                Name = "O'Star"
            });
            inStoreProducts.Add(new Product
            {
                Id = "4",
                Price = 1000,
                Name = "Big Babol"
            });
            inStoreProducts.Add(new Product
            {
                Id = "5",
                Price = 500,
                Name = "Cool Air"
            });
            inStoreProducts.Add(new Product
            {
                Id = "6",
                Price = 100000,
                Name = "X-Men"
            });
            inStoreProducts.Add(new Product
            {
                Id = "7",
                Price = 1000000,
                Name = "Rolex"
            });
            inStoreProducts.Add(new Product
            {
                Id = "8",
                Price = 106000,
                Name = "Camay"
            });
            inStoreProducts.Add(new Product
            {
                Id = "9",
                Price = 10000,
                Name = "Big"
            });

            List<Product> resultProducts = new List<Product>();

            long minPrice = command.GetParameter<long>(CommandNames.MinPriceParameter, 0);
            foreach (var product in inStoreProducts)
            {
                if (product.Price >= minPrice)
                {
                    resultProducts.Add(product);
                }
            }

            //Gửi phản hồi trả lại
            Command resultCommand = new Command();
            resultCommand.Name = CommandNames.ListProductCommand;
            if (resultProducts.Count > 0)
            {
                resultCommand.SetData(resultProducts);
                resultCommand.Code = 0;
            } else
            {
                resultCommand.Code = 1;
                resultCommand.Message = string.Format("Không có sản phẩm nào giá trên {0} VNĐ", minPrice);
            }

            //Gửi
            connection.BelongListener.SendMessage(connection, resultCommand.ToString());
        }
    }
}
