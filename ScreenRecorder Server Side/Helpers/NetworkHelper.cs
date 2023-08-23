using ScreenRecorder_Server_Side.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ScreenRecorder_Server_Side.Helpers
{
    public class NetworkHelper
    {
        static TcpListener listener = null;
        public static List<Client> Clients { get; set; }
        [Obsolete]
        public static void Connect()
        {
            var port = 27001;
            string hostName = Dns.GetHostName();
            string myIP = Dns.GetHostByName(hostName).AddressList[0].ToString();
            var ipAddress = IPAddress.Parse(myIP);
            Clients = new List<Client>();
            var ep = new IPEndPoint(ipAddress, port);
            listener = new TcpListener(ep);
            listener.Start();
            MessageBox.Show($"Listening on {listener.LocalEndpoint}");
            while (true)
            {
                var client = listener.AcceptTcpClient();
                var newClient = new Client
                {
                    TcpClient = client,
                };
                Clients.Add(newClient);
                Task.Delay(1);
            }
        }
    }
}
