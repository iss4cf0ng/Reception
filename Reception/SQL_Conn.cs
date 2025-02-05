using System.Data;
using System.Data.SQLite;

public class SQL_Conn
{
    public SQLiteConnection Connection { get; set; }
    public string ConnectionString { get; set; }
    public string Filename { get; set; }

    //DB STRUCTURE
    Dictionary<string, string[]> db_struct = new Dictionary<string, string[]>()
    {
        {
            "Client",
            new string[]
            {
                "online_id",
                "folder",
                "first_online_date",
                "last_online_date",
            }
        },
        {
            "Backdoor",
            new string[]
            {
                "guid",
                "knock_msg",
                "ack_msg",
                "crypto",
                "AES_size",
                "RSA_size",
            }
        },
        {
            "Plugins",
            new string[]
            {
                "online_id",
                "tmp_folder",
                "plugin_name",
                "install_date",
                "uninstall_date",
            }
        },
        {
            "Key", //KEY INFORMATION, SHOULD NOT STORE PLAIN TEXT OF CRYPTO KEY.
            new string[]
            {
                "Create date",
            }
        },
        {
            "Logs",
            new string[]
            {
                "online_id",
                "type",
                "date",
                "message",
            }
        }
    };

    //CONSTRUCTOR
    public SQL_Conn(string file = null)
    {
        try
        {
            string db_filename = file ?? "db.sqlite";
            bool new_db = !File.Exists(db_filename);

            ConnectionString = $"Data Source={db_filename};";
            Connection = new SQLiteConnection(this.ConnectionString);
            Connection.Open();

            if (new_db) DB_init();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "DB ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            DB_Close();
        }
    }

    private bool DB_init()
    {
        foreach (string table in db_struct.Keys)
        {
            string[] columns = db_struct[table];
            CreateTable(table, columns);
        }
        return false;
    }

    private void CreateTable(string table, string[] columns)
    {
        if (Connection.State == System.Data.ConnectionState.Open)
        {
            string[] empty_values = Enumerable.Repeat("\'\'", columns.Length).ToArray();
            string[] _columns = columns.Select(x => x + " string").ToArray();
            string sql_query = $"CREATE TABLE {table} ({string.Join(",", _columns)});";
            ExecQuery(sql_query);
        }
    }

    public void SetValue(string table, string online_id, string column, string value)
    {
        if (Connection.State == System.Data.ConnectionState.Open)
        {
            string sql_query = $"UPDATE {table} SET {column} = \"{value}\" WHERE online_id = {online_id};";
            ExecQuery(sql_query);
        }
    }

    public void NewClient(string online_id, string folder)
    {
        if (Connection.State == System.Data.ConnectionState.Open)
        {
            string table = "Client";
            string date = C1.DateTimeStrFormat();
            string sql_query = $"INSERT INTO {table} VALUES ({online_id},{folder},{date},{date});";
            ExecQuery(sql_query);
        }
    }

    public void NewPlugins(string online_id)
    {
        if (Connection.State == System.Data.ConnectionState.Open)
        {

        }
    }

    public void NewLogs(string online_id, log_Type type, string msg)
    {
        if (Connection.State == System.Data.ConnectionState.Open)
        {
            string table = "Logs";
            string type_str = Enum.GetName(typeof(log_Type), type);
            string date = C1.DateTimeStrFormat();
            string sql_query = $"INSERT INTO {table} VALUES ({online_id},{type_str},{msg})";
            ExecQuery(sql_query);
        }
    }

    public void ExecQuery(string sql_query)
    {
        if (Connection.State == System.Data.ConnectionState.Open)
        {
            SQLiteCommand sql_cmd = new SQLiteCommand(sql_query, Connection);
            try
            {
                sql_cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "SQL ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    public DataTable DataReader(string sql_query)
    {
        if (Connection.State == ConnectionState.Open)
        {
            try
            {
                DataTable dt = new DataTable();
                DataSet ds = new DataSet();
                ds.Clear();

                using (var data_adapter = new SQLiteDataAdapter(sql_query, Connection))
                {
                    data_adapter.Fill(ds);
                    dt = ds.Tables[0];
                }

                return dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "SQL DATA READER ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        return null;
    }

    public void DeleteClient(string online_id)
    {

    }

    private bool DB_Close()
    {
        if (Connection.State == System.Data.ConnectionState.Open)
        {
            Connection.Close();
            return true;
        }
        return false;
    }
}