using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server
{
    public class User
    {
        public string Nickname { get; set; }
        public UdpClient Client { get; set; }
        public IPEndPoint ip { get; set; }

    }
}
