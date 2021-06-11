using MyLib;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

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
        private static int portToImage = 9393;


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
            //Task.Run(() => ListenImage());

            SendMessage("<connect>");

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Color color = ((SolidColorBrush)(sender as Button).Background).Color;
            this.inkCanvas.DefaultDrawingAttributes.Color = color;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.inkCanvas.DefaultDrawingAttributes.Height = (sender as Button).Height - 3;
            this.inkCanvas.DefaultDrawingAttributes.Width = (sender as Button).Width - 3;
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

        //private void ListenImage()
        //{
        //    while (true)
        //    {
        //        TcpClient client = new TcpClient(remoteIPAddress, portToImage);

        //        BinaryFormatter serializer = new BinaryFormatter();
        //        var info = (FileTransferInfo)serializer.Deserialize(client.GetStream());

        //        // byte to imagesource
        //        BitmapImage biImg = new BitmapImage();
        //        MemoryStream ms = new MemoryStream(info.Data);
        //        biImg.BeginInit();
        //        biImg.StreamSource = ms;
        //        biImg.EndInit();

        //        ImageSource imgSrc = biImg as ImageSource;
        //        inkImage.Source = imgSrc;
        //    }
        //}

        private void inkCanvas_StrokeCollected(object sender, InkCanvasStrokeCollectedEventArgs e)
        {
            //Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            TcpClient client = new TcpClient();
            IPEndPoint endpoint = new IPEndPoint(IPAddress.Parse(remoteIPAddress), portToImage);

            client.Connect(endpoint);
            RenderTargetBitmap rtb = new RenderTargetBitmap((int)inkCanvas.ActualWidth, (int)inkCanvas.ActualHeight-33, 96d, 96d, PixelFormats.Default);
            rtb.Render(inkCanvas);
            
            // створюємо клас, який містить інформацію про файл
            FileTransferInfo info = new FileTransferInfo();
            BmpBitmapEncoder encoder = new BmpBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(rtb));

            byte[] bitmapBytes;
            using (MemoryStream ms = new MemoryStream())
            {
                encoder.Save(ms);
                ms.Position = 0;
                bitmapBytes = ms.ToArray();
                info.Data = bitmapBytes;
            }

            BinaryFormatter serializer = new BinaryFormatter();
            using (NetworkStream stream = client.GetStream())
            {
                // серіалізуємо об'єкт класа
                // та відправляємо його на сервер
                serializer.Serialize(stream, info);
            }


            //try
            //{
            //    server.Connect(endpoint);
            //    server.SendTo(bitmapBytes, endpoint);
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //}
        }
    }
}
