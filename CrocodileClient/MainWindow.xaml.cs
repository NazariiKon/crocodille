using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
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

namespace CrocodileClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        User client;
        // адреса віддаленого хоста
        private static string remoteIPAddress = "192.168.137.1";
        // порт віддаленого хоста
        private static int remotePort = 8080;

        public MainWindow()
        {
            InitializeComponent();
        }

        public MainWindow(User user)
        {
            InitializeComponent();
            client = user;
            client.Client = new UdpClient(0);
            Task.Run(() => Listen());
            SendMessage("<connect>");

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Color color = ((SolidColorBrush)(sender as Button).Background).Color;
            this.inkCanvas.DefaultDrawingAttributes.Color = color;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.inkCanvas.DefaultDrawingAttributes.Height = (sender as Button).Height-3;
            this.inkCanvas.DefaultDrawingAttributes.Width = (sender as Button).Width-3;
        }

        private void sendMessageBtn_Click(object sender, RoutedEventArgs e)
        {
            SendMessage(messageTextBox.Text);
            messageTextBox.Clear();
        }

        private void SendMessage(string msg)
        {
            if (string.IsNullOrWhiteSpace(msg)) return;

            IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Parse(remoteIPAddress), remotePort);
            byte[] data;
            if (msg == "<connect>")
                data = Encoding.UTF8.GetBytes($"{msg}");
            else
                data = Encoding.UTF8.GetBytes($"{client.Nickname}: {msg}");

            client.Client.Send(data, data.Length, iPEndPoint);
        }

        private void Listen()
        {
            IPEndPoint iPEndPoint = null;
            while (true)
            {
                try
                {
                    byte[] data = client.Client.Receive(ref iPEndPoint);

                    string msg = Encoding.UTF8.GetString(data);

                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        messagesTextBox.AppendText(msg + "\r\n");
                    }));
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
    }
}
