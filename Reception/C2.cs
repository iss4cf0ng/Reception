using Reception;

public class C2
{
    //CONFIG
    public static IniManager ini_manager = new IniManager(Path.Combine(new string[] { Application.StartupPath, "config.ini" }));

    //STORAGE
    public static Form1 form1;
    public static Dictionary<string, string[]> Victims = new Dictionary<string, string[]>();
    public static ImageList il = new ImageList();

    //STATUS
    public static int sent = 0;
    public static int received = 0;

    //DATABASE
    public static SQL_Conn sql_conn;

    //FILTER
    public static Dictionary<Victim, Dictionary<Function, List<file_Info>>> filter_storage = new Dictionary<Victim, Dictionary<Function, List<file_Info>>>();
}