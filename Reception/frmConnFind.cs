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
    public partial class frmConnFind : Form
    {
        public Victim v;
        public Form1 f1;

        public frmConnFind()
        {
            InitializeComponent();
        }

        void SetFilter()
        {
            conn_F f = new conn_F()
            {
                pattern = textBox1.Text,
                regex = checkBox1.Checked,
                ignore_case = checkBox2.Checked,
            };

            f1.conn_Filter(f, v);
        }

        void setup()
        {

        }

        private void frmConnFind_Load(object sender, EventArgs e)
        {
            setup();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SetFilter();
            DialogResult = DialogResult.OK;
            ActiveForm.Close();
        }
    }
}
