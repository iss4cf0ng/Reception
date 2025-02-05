public sealed class MyProtocol
{
    public const int HEAD_LENGTH = 6;
    private byte _Command = 0;
    public byte Command { get => _Command; }
    private byte _Param = 0;
    public byte Param { get => _Param; }
    private int _DataLength = 0;
    public int DataLength { get => _DataLength; }
    private byte[] _MessageData = new byte[0];
    public byte[] MessageData { get => _MessageData; }
    private byte[] _MoreData = new byte[0];
    public byte[] MoreData { get => _MoreData; }

    public MyProtocol(byte[] buffer)
    {
        if (buffer == null || buffer.Length < HEAD_LENGTH)
            return;

        MemoryStream ms = new MemoryStream(buffer);
        BinaryReader br = new BinaryReader(ms);
        try
        {
            _Command = br.ReadByte();
            _Param = br.ReadByte();
            _DataLength = br.ReadInt32();

            if (buffer.Length - HEAD_LENGTH >= _DataLength)
                _MessageData = br.ReadBytes(_DataLength);
            if (buffer.Length - HEAD_LENGTH - DataLength > 0)
                _MoreData = br.ReadBytes(buffer.Length - HEAD_LENGTH - _DataLength);
            Console.WriteLine("=>" + _DataLength.ToString());
        }
        catch (Exception)
        {
            
        }
        br.Close();
        ms.Close();
    }

    public MyProtocol(byte command, byte para, byte[] msg_data)
    {
        _Command = command;
        _Param = para;
        _MessageData = msg_data;
        _DataLength = msg_data.Length;
    }

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
        catch (Exception)
        {
            return new byte[0];
        }
    }

    public (byte Command, byte Param, int DataLength, byte[] msg_data) GetMessage()
    {
        (byte Command, byte Param, int DataLength, byte[] MessageData) returnValue = (
            _Command,
            _Param,
            _MessageData.Length,
            _MessageData
            );
        return returnValue;
    }

    public static (byte Command, byte Param, int DataLength) GetHeadInfo(byte[] buffer)
    {
        (byte Command, byte Param, int DataLength) returnValue = (0, 0, 0);
        if (buffer == null || buffer.Length < HEAD_LENGTH)
            return returnValue;

        returnValue.Command = buffer[0];
        returnValue.Param = buffer[1];
        byte[] len_field = new byte[4];
        returnValue.DataLength = BitConverter.ToInt32(buffer, 2);
        return returnValue;
    }
}