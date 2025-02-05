//Reception Protocol(RP)

//DISABLE WARNING
#pragma warning disable CS8600

public class RP
{
    //HEADER
    public const int HEAD_LENGTH = 6; //Buffer header, 6 bytes
    public byte _Command = 0; //Command, 1 bytes
    public byte Command { get => _Command; }
    private byte _Param = 0; //Parameter, 1 bytes
    public byte Param { get => _Param; }
    private int _DataLength = 0; //Data length, 4 bytes
    public int DataLength { get => _DataLength; }

    //DATA
    private byte[] _MessageData = new byte[0];
    public byte[] MessageData = new byte[0];
    private byte[] _MoreData = new byte[0];
    public byte[] MoreData { get => _MoreData; }

    //CONSTRUCTOR-1 : NO IDEA ABOUT BUFFER.
    public RP(byte[] buffer) 
    {
        if (buffer == null || buffer.Length < HEAD_LENGTH)
            return;

        //BUFFER INFORMATION
        MemoryStream ms = new MemoryStream(buffer);
        BinaryReader br = new BinaryReader(ms);
        try
        {
            //HEADER
            _Command = br.ReadByte(); // 1 BYTE
            _Param = br.ReadByte(); // 1 BYTE
            _DataLength = br.ReadInt32(); // READ 4 BYTES AND CONVERT TO INTERGER, INT32 MEAN A INTEGER STORE IN 32 BITS, WHICH IS 4 BYTES

            if (buffer.Length - HEAD_LENGTH >= _DataLength)
                _MessageData = br.ReadBytes(_DataLength);
            if (buffer.Length - HEAD_LENGTH - DataLength > 0)
                _MoreData = br.ReadBytes(buffer.Length - HEAD_LENGTH - _DataLength);
        }
        catch (Exception)
        {

        }
        br.Close();
        ms.Close();
    }
    //CONSTRUCTOR-2 : KNEW SOMETHING ABOUT THE BUFFER.
    public RP(byte command, byte para, byte[] msg_data)
    {
        _Command = command;
        _Param = para;
        _MessageData = msg_data;
        _DataLength = msg_data.Length;
    }

    //RETURN RECEIVED BUFFER, BUT WE NEED TO REASSEMBLE IT.
    public byte[] GetBytes()
    {
        try
        {
            byte[] bytes = null;
            MemoryStream ms = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(ms);
            bw.Write(_Command);
            bw.Write(_Param);
            bw.Write(_DataLength);
            bw.Write(_MessageData);
            bytes = ms.ToArray();
            bw.Close();
            ms.Close();
            return bytes;
        }
        catch
        {
            return new byte[0];
        }
    }

    //GET MESSAGE
    public (byte command, byte para, int data_length, byte[] msg_data) GetMessage()
    {
        (byte Command, byte Param, int DataLength, byte[] msg_data) retValue = (
            _Command,
            _Param,
            _MessageData.Length,
            _MessageData
            );
        return retValue;
    }

    public static (byte Command, byte Param, int DataLength) GetHeadInfo(byte[] buffer)
    {
        (byte Command, byte Param, int DataLength) retValue = (0, 0, 0);
        if (buffer == null || buffer.Length < HEAD_LENGTH)
        {
            return retValue;
        }

        retValue.Command = buffer[0];
        retValue.Param = buffer[1];
        byte[] len_field = new byte[4];
        retValue.DataLength = BitConverter.ToInt32(buffer, 2);
        return retValue;
    }
} 