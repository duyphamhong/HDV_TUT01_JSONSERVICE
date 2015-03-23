using HDV.Tutorials.JsonService.Client.Networks;
using HDV.Tutorials.JsonService.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HDV.Tutorials.JsonService.Client
{
    public partial class Form1 : Form
    {
        private class ComboBoxItem
        {
            public string ConnectionName { set; get; }
            public Networks.Client Client { set; get; }

            public override string ToString()
            {
                return ConnectionName;
            }
        }

        public Form1()
        {
            InitializeComponent();
            TcpClient.Current.ConnectionOpened += OnConnectionOpened;
            TcpClient.Current.ListProductCommandReceived += OnListProductCommandReceived;
        }

        private void OnListProductCommandReceived(object sender, ListProductCommandReceivedEventArgs e)
        {
            Invoke(new Action(() => { 
                foreach (var product in e.Products)
                {
                    lstProduct.Items.Add(product);

                }
            }));
        }

        
        private void btnTcpConnect_Click(object sender, EventArgs e)
        {
            TcpClient.Current.Connect();
        }

        private void OnConnectionOpened(object sender, ConnectionOpenedEventArgs e)
        {
            Invoke(new Action(() =>
            {
                if (e.IsSuccessful)
                {
                    Networks.Client client = sender as Networks.Client;

                    cbConnections.Items.Add(
                        new ComboBoxItem 
                        { 
                            ConnectionName = client.Name, 
                            Client = client 
                        });
                    
                    if (cbConnections.Items.Count == 1)
                    {
                        cbConnections.SelectedIndex = 0;
                    }

                    if (sender is TcpClient)
                    {
                        btnTcpConnect.Enabled = false;
                        MessageBox.Show("Connect to service through TCP successful!");
                    }
                }
            }));
            
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            string minPriceString = txtMinPrice.Text;
            if (string.IsNullOrEmpty(minPriceString))
                return;

            long minPrice;
            if (!long.TryParse(minPriceString, out minPrice))
                return;

            var item = cbConnections.SelectedItem as ComboBoxItem;
            if (item != null)
            {
                Command command = new Command();
                command.Name = CommandNames.ListProductCommand;
                command.AddParameter(CommandNames.MinPriceParameter, minPrice);

                item.Client.SendCommand(command);
            }
        }

    }
}
