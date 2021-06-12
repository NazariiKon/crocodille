using MyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class Program
    {
        static IPAddress iPAddress = IPAddress.Parse("100.66.248.237");
        // порт для прослуховування
        private const int port = 8080;
        // список учасників чату
        private static List<IPEndPoint> members = new List<IPEndPoint>(); // всі клієнти
        private static List<string> userNicknames = new List<string>(); // всі клієнти


        static void Main(string[] args)
        {
            // створення об'єкту UdpClient та встановлюємо порт для прослуховування
            UdpClient server = new UdpClient(port);
            // створюємо об'єкт для збреження адреси віддаленого хоста
            IPEndPoint groupEP = null;

            try
            {
                while (true)
                {
                    byte[] bytes = server.Receive(ref groupEP);

                    // конвертуємо масив байтів в рядок
                    string msg = Encoding.UTF8.GetString(bytes, 0, bytes.Length);

                    if (msg.IndexOf("<connect>") > -1)
                    {
                        string[] messageSplit = msg.Split(':');
                        userNicknames.Add(messageSplit[1]);

                        string nicknames = "<nicknames>";
                        for (int i = 0; i < userNicknames.Count; i++)
                        {
                            nicknames += ':' + userNicknames[i];
                        }

                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"Request to connect from {groupEP} at {DateTime.Now.ToShortTimeString()}\n");
                        AddMember(groupEP);
                       
                        byte[] data = Encoding.UTF8.GetBytes(nicknames);
                        foreach (var m in members)
                        {
                            try
                            {
                                server.Send(data, data.Length, m);
                            }
                            catch (Exception ex)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine($"Error with {m}: {ex.Message}\n");
                            }
                        }
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine($"Message from {groupEP} at {DateTime.Now.ToShortTimeString()}: {msg}\n");
                        foreach (var m in members)
                        {
                            try
                            {
                                server.Send(bytes, bytes.Length, m);
                            }
                            catch (Exception ex)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine($"Error with {m}: {ex.Message}\n");
                            }
                        }
                    }
                    Console.ResetColor();
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                // закриття з'єднання
                server.Close();
            }

        }

        static bool AddMember(IPEndPoint endPoint)
        {
            var member = members.FirstOrDefault(m => m.ToString() == endPoint.ToString());
            if (member == null)
            {
                members.Add(endPoint);
                return true;
            }
            return false;
        }
    }
}
   
       
