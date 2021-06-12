using MyLib;
using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;

namespace ServerForImage
{
    class Program
    {
        // адреса віддаленого хоста
        static IPAddress iPAddress = IPAddress.Parse("100.66.248.237");
        // порт віддаленого хоста
        static int port = 9393;

        static void Main(string[] args)
        {
            IPEndPoint localEndPoint = new IPEndPoint(iPAddress, port);
            IPEndPoint clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
            // створюємо екземпляр сервера вказуючи кінцеву точку для приєднання
            TcpListener serverToClient = new TcpListener(localEndPoint);
            // запускаємо прослуховування вказаної кінцевої точки
            serverToClient.Start(10);

            while (true)
            {
                Console.WriteLine("Waiting for picture...");
                // отримуємо зв'язок з клієнтом
                TcpClient client = serverToClient.AcceptTcpClient();

                // отримуємо дані від клієнта
                // та десеріалізуємо об'єкт
                BinaryFormatter serializer = new BinaryFormatter();

                NetworkStream stream = client.GetStream();
                var info = (FileTransferInfo)serializer.Deserialize(stream);
                Console.WriteLine("Picture taked");
            }
        }
    }
}
