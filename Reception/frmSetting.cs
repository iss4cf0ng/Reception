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
    public partial class frmSetting : Form
    {
        IniManager ini = C2.ini_manager;

        public frmSetting()
        {
            InitializeComponent();
        }

        void SaveChange()
        {

        }

        void setup()
        {
            MessageBox.Show(ini.ReadIniFile("DEFAULT", "db", "aaa"));
            //SERVER
            textBox1.Text = ini.ReadIniFile("SERVER", "ip", "127.0.0.1");
            numericUpDown5.Value = int.Parse(ini.ReadIniFile("SERVER", "port", "4444"));
            numericUpDown1.Value = int.Parse(ini.ReadIniFile("SERVER", "sock_limit", "10000"));
            numericUpDown2.Value = int.Parse(ini.ReadIniFile("SERVER", "sendtimeout", "10000"));
            numericUpDown4.Value = int.Parse(ini.ReadIniFile("SERVER", "recvtimeout", "10000"));

            //MANAGER
            numericUpDown3.Value = int.Parse(ini.ReadIniFile("FILE", "thread", "20"));
            checkBox1.Checked = ini.ReadIniFile("PROCESS", "child", "0") == "1";
            textBox3.Text = ini.ReadIniFile("PROCESS", "AVjson", "");

            //SHELL
            textBox4.Text = ini.ReadIniFile("SHELL", "linux_exec", "");
            textBox5.Text = ini.ReadIniFile("SHELL", "linux_init", "");

            textBox7.Text = ini.ReadIniFile("SHELL", "win_exec", "");
            textBox6.Text = ini.ReadIniFile("SHELL", "win_init", "");

            textBox8.Text = ini.ReadIniFile("WMIC", "init", "");
        }

        private void frmSetting_Load(object sender, EventArgs e)
        {
            setup();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            SaveChange();
        }
    }
}
