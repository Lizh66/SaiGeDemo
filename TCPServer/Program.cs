using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TCPServer
{
    class Program
    {
        static void Main(string[] args)
        {
            int port = 8888;

            // 创建负责监听的套接字，注意其中的参数；
            Socket ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            IPAddress iPAddress = IPAddress.Parse("127.0.0.1");

            // 创建包含ip和端口号的网络节点对象；
            IPEndPoint endPoint = new IPEndPoint(iPAddress, port);
            try
            {
                // 将负责监听的套接字绑定到唯一的ip和端口上；
                ServerSocket.Bind(endPoint);
            }
            catch
            {
                
            }
            // 设置监听队列的长度；
            ServerSocket.Listen(100);

            while (true)
            {
                Socket sokConnection = ServerSocket.Accept();
                while (true)
                {
                    try
                    {
                        byte[] data = new byte[1024];
                        int length = sokConnection.Receive(data);
                        string message = Encoding.UTF8.GetString(data, 0, length);
                        Console.WriteLine(message);
                        sokConnection.Send(Encoding.UTF8.GetBytes($"已接收内容：{message}"));
                    }
                    catch
                    {
                        Console.WriteLine("断开连接");
                        break;
                    }
                }
            }
        }
    }
}
