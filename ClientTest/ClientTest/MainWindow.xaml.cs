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
using System.Windows;
namespace ClientTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string ServerIP = "127.0.0.1";
        private const int ServerPort = 12345;
        private UdpClient client;
        private bool isLampOn;
        private string textValue;

        public MainWindow()
        {
            InitializeComponent();

            client = new UdpClient();

            //sunucu ile haberleşip ilk değerleri oluşturuyor
            GetInitialValues();

            //lamba renkleri ayarla
            SetLampButtonColor();

            //buton textini günceller
            UpdateTextButtonContent();
        }

        private void GetInitialValues()
        {
            // Server'dan başlangıç değerlerini al
            string lampState = SendMessageToServer("GET_LAMP_STATE");
            isLampOn = lampState == "Açık";

            //text areanın başlangıç değeri
            string initialTextValue = SendMessageToServer("GET_TEXT_VALUE");
            textValue = initialTextValue;
        }

        private string SendMessageToServer(string message)
        {
            byte[] sendData = Encoding.ASCII.GetBytes(message);
            client.Send(sendData, sendData.Length, ServerIP, ServerPort);

            IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Parse(ServerIP), ServerPort);
            byte[] receivedData = client.Receive(ref serverEndPoint);
            string receivedMessage = Encoding.ASCII.GetString(receivedData);

            return receivedMessage;
        }

        private void SetLampButtonColor()
        {
          if (isLampOn == true)
                lampButton.Background = Brushes.LightGreen;
            else
                lampButton.Background = Brushes.Gray;
        }

        private void UpdateTextButtonContent()
        {
            textButton.Content = textValue;
        }

        private void LampButton_Click(object sender, RoutedEventArgs e)
        {
            //açıksa kapalı kapalıysa açık yapıyor
            string message = isLampOn ? "TURN_OFF_LAMP" : "TURN_ON_LAMP";
            SendMessageToServer(message);

            isLampOn = !isLampOn;
            SetLampButtonColor();
        }

        private void TextButton_Click(object sender, RoutedEventArgs e)
        {
            textValue = SendMessageToServer("GENERATE_RANDOM_NUMBER");
            UpdateTextButtonContent();
        }
    }
}
