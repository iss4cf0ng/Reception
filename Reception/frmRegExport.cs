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
    public partial class frmRegExport : Form
    {
        public Victim v;
        public string path;

        public frmRegExport()
        {
            InitializeComponent();
        }

        void setup()
        {
            textBox1.Text = C1.DateTimeName("reg");
            radioButton1.Checked = true;
        }

        private void frmRegExport_Load(object sender, EventArgs e)
        {
            setup();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox1.Text = C1.DateTimeName("reg");
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }
    }
}
