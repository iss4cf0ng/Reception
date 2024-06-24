using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Reception
{
    public partial class frmListen : Form
    {
        public static string ip;
        public static int port;

        public frmListen()
        {
            InitializeComponent();
        }

        void setup()
        {
            string ip = C2.ini_manager.ReadIniFile("SERVER", "ip", "127.0.0.1");
            string port = C2.ini_manager.ReadIniFile("SERVER", "port", "4444");

            textBox1.Text = ip;
            textBox2.Text = port;
        }

        void Listen()
        {
            try { int.Parse(textBox2.Text); } catch (Exception ex) { MessageBox.Show(ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
            try { IPAddress.Parse(textBox1.Text); } catch (Exception ex) { MessageBox.Show(ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
            ip = textBox1.Text;
            port = int.Parse(textBox2.Text);
            DialogResult = DialogResult.OK;
            ActiveForm.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Listen();
        }

        private void frmListen_Load(object sender, EventArgs e)
        {
            setup();
        }
    }
}
