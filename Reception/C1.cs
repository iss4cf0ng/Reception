using System.Text;
using System.Text.RegularExpressions;

public class C1
{
    static string PayloadFolder_py = Path.Combine(Application.StartupPath, "Payload", "Python");

    static string[] image_exts =
    {
        "jpg",
        "png",
        "bmp",
        "ico",
    };

    //BASE64 BYTES DECODE TO STRING
    public static string B64ByteD2Str(byte[] data)
    {
        return Encoding.UTF8.GetString(Convert.FromBase64String(Encoding.UTF8.GetString(data)));
    }
    //STRING ENCODE TO BASE64 BYTES
    public static byte[] StrE2B64Byte(string data)
    {
        return Encoding.UTF8.GetBytes(Convert.ToBase64String(Encoding.UTF8.GetBytes(data)));
    }
    //STRING ENCODE TO BASE64 STRING
    public static string B64StrD2Str(string data)
    {
        return Encoding.UTF8.GetString(Convert.FromBase64String(data));
    }
    public static byte[] B64StrD2Byte(string data)
    {
        return Convert.FromBase64String(data);
    }
    //STRING ENCODE TO BASE64 STRING
    public static string StrE2B64Str(string data)
    {
        return Convert.ToBase64String(Encoding.UTF8.GetBytes(data));
    }
    //BASE64 STRING CONVERT TO IMAGE
    public static Image B64StrC2Img(string b64_str)
    {
        byte[] image_bytes = Convert.FromBase64String(b64_str);
        using (var ms = new MemoryStream(image_bytes, 0, image_bytes.Length))
        {
            Image image = Image.FromStream(ms, true);
            return image;
        }
    }

    //GET PAYLOAD
    public static string GetPayload(string name)
    {
        return File.ReadAllText(Path.Combine(PayloadFolder_py, $"{name}.py"));
    }

    //EXTRACT FILE ICON
    public static Image GetIcon(string ext)
    {
        if (C2.il.Images.ContainsKey(ext))
            return C2.il.Images[ext];

        string temp = Path.GetTempPath();
        string temp_file = $"{DateTime.Now.ToLongDateString()}.{ext}";
        temp_file = Path.Combine(temp, temp_file);
        File.Create(temp_file).Close();
        Icon icon = Icon.ExtractAssociatedIcon(temp_file);
        File.Delete(temp_file);

        int index = C2.il.Images.Count;
        C2.il.Images.Add(icon);
        C2.il.Images.SetKeyName(index, ext);
        return C2.il.Images[ext];
    }

    //CHECK IMAGE EXTENSION
    public static bool IsImage(string ext)
    {
        return image_exts.Contains(ext.Replace(".", string.Empty));
    }

    //ANCHOR STYLE
    public static AnchorStyles AnchorStyle_All { get { return AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right; } }

    //DATE TIME STRING - FILE
    public static string DateTimeName(string ext)
    {
        DateTime date = DateTime.Now;
        string date_str = $"{date.Year}{date.Month}{date.Day}{date.Hour}{date.Minute}{date.Second}{date.Millisecond}";
        date_str += "." + ext ?? "";
        return date_str;
    }

    //DATE TIME STRING - STATUS
    public static string DateTimeStrFormat()
    {
        return DateTime.Now.ToString("F");
    }

    public static string FileDialogGetFilter(FileFilter ff)
    {
        switch (ff)
        {
            case FileFilter.TXT:
                return "text file (*.txt)|*.txt";
            case FileFilter.CSV:
                return "csv file (*.csv)|*.csv";
        }

        return null;
    }
}