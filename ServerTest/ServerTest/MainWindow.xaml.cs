using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net;
using System.Net.Sockets;
namespace ServerTest
{


    public partial class MainWindow : Window
    {
        private const int ServerPort = 12345;
        private bool isLampOn = false;
        private UdpClient server;
        private List<IPEndPoint> clients;
        private SolidColorBrush onColor = new SolidColorBrush(Colors.Green);
        private SolidColorBrush offColor = new SolidColorBrush(Colors.DarkGreen);

        public MainWindow()
        {
            InitializeComponent();

            server = new UdpClient(ServerPort);
            clients = new List<IPEndPoint>();

            Task.Run(() => StartListening());
        }

        private void StartListening()
        {
            Console.WriteLine("Server is running. Waiting for messages...");

            while (true)
            {
                IPEndPoint clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
                byte[] receivedData = server.Receive(ref clientEndPoint);
                string receivedMessage = Encoding.ASCII.GetString(receivedData);

                Console.WriteLine($"Received message from {clientEndPoint.Address}:{clientEndPoint.Port}: {receivedMessage}");

            


                // İstemciyi takip etmek için ekle
                if (!clients.Contains(clientEndPoint))
                    clients.Add(clientEndPoint);

                // İstemcilere mesajı gönder
                SendToClients(receivedMessage, clientEndPoint);
            }
        }

        private void SendToClients(string message, IPEndPoint sender)
        {
            byte[] responseData = Encoding.ASCII.GetBytes(message);

            foreach (var client in clients)
            {
                // Gönderen hariç tüm istemcilere mesajı gönder
                if (client.Equals(sender))
                    continue;

                server.Send(responseData, responseData.Length, client);
            }
        }

        private void LampButton_Click(object sender, RoutedEventArgs e)
        {
            isLampOn = !isLampOn;

            if (isLampOn==true)
                lampButton.Background = Brushes.LightGreen;
            else
                lampButton.Background = Brushes.Gray;

            string lampState = isLampOn ? "Acik" : "Kapalı";
            SendToClients($"Lamba durumu: {lampState}", null);
        }

        private void TextInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            string textValue = textInput.Text;
            SendToClients($"Son text degeri: {textValue}", null);
        }

        private void RandomButton_Click(object sender, RoutedEventArgs e)
        {
            Random random = new Random();
            int randomNumber = random.Next(1, 100);
            textInput.Text = randomNumber.ToString();
        }
    }
}