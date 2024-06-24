using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Server
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            server.Bind(new IPEndPoint(IPAddress.Any, 4444));
            server.Listen(10);
            Task.Run(() => AcceptClient(server)).Wait(10);
            Console.WriteLine("Server started, IP: {0}", server.LocalEndPoint.ToString());
            Console.ReadLine();
            server.Close();
            Console.WriteLine("Server stopped");

            return;
        }

        public static void AcceptClient(Socket server)
        {
            Socket client = null;
            do
            {
                try
                {
                    client = server.Accept();
                    if (client != null)
                    {
                        Console.WriteLine("Client from : {0}", client.RemoteEndPoint.ToString());
                        Task.Run(() => ReceiveData(client));
                    }
                }
                catch (Exception e)
                {
                    client = null;
                }
            } while (client != null);
        }

        public static byte[] CombineBytes(byte[] firstBytes, int firstIndex, int firstLength, byte[] secondBytes, int secondIndex, int secondLength)
        {
            byte[] bytes = null;
            MemoryStream ms = new MemoryStream();
            ms.Write(firstBytes, firstIndex, firstLength);
            ms.Write(secondBytes, secondIndex, secondLength);
            bytes = ms.ToArray();
            ms.Close();
            return bytes;
        }

        public static void Response(Socket client, (byte Command, byte Param, int DataLength, byte[] MessageData) message)
        {
            string text = Encoding.UTF8.GetString(message.MessageData);
            Console.WriteLine("Client({0}) : {1}", client.RemoteEndPoint.ToString(), text);
            text = "OK!";
            client.Send(new MyProtocol(1, 1, Encoding.UTF8.GetBytes(text)).GetBytes());
        }

        public static void ReceiveData(Socket socket)
        {
            try
            {
                MyProtocol mp = null;
                int receive_length = 0;
                byte[] static_receiveBuffer = new byte[65536];
                byte[] dynamic_receiveBuffer = new byte[] { };
                do
                {
                    receive_length = socket.Receive(static_receiveBuffer);
                    dynamic_receiveBuffer = CombineBytes(dynamic_receiveBuffer, 0, dynamic_receiveBuffer.Length, static_receiveBuffer, 0, receive_length);
                    //Console.WriteLine(dynamic_receiveBuffer.Length.ToString());
                    if (receive_length <= 0)
                    {
                        Console.WriteLine("Receive 0 byte data");
                        break;
                    }
                    else if (dynamic_receiveBuffer.Length < MyProtocol.HEAD_LENGTH)
                    {
                        continue;
                    }
                    else
                    {
                        var head_info = MyProtocol.GetHeadInfo(dynamic_receiveBuffer);
                        while (dynamic_receiveBuffer.Length - MyProtocol.HEAD_LENGTH >= head_info.DataLength)
                        {
                            mp = new MyProtocol(dynamic_receiveBuffer);
                            dynamic_receiveBuffer = mp.MoreData;
                            head_info = MyProtocol.GetHeadInfo(dynamic_receiveBuffer);
                            if (mp.Command == 0 && mp.Param == 0)
                            {
                                socket.Send(new MyProtocol(0, 0, Encoding.UTF8.GetBytes("Byebye")).GetBytes());
                                return;
                            }
                            //Console.WriteLine("Command:{0}, Param:{1},Data:{2}", mp.Command, mp.Param, Encoding.UTF8.GetString(mp.MessageData));
                            Response(socket, mp.GetMessage());
                        }
                    }
                } while (receive_length > 0);
            }
            finally
            {
                Console.WriteLine("Client {0} disconnected", socket.RemoteEndPoint.ToString());
                socket.Close();
            }
        }
    }
}