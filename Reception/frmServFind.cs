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
    public partial class frmServFind : Form
    {
        public Form1 f1;
        public Victim v;

        public frmServFind()
        {
            InitializeComponent();
        }

        void SetFilter()
        {
            serv_F f = new serv_F()
            {
                pattern = textBox1.Text,
                regex = checkBox1.Checked,
                ignore_case = checkBox2.Checked,
            };

            f1.serv_Filter(f, v);
        }

        void setup()
        {

        }

        private void frmServFind_Load(object sender, EventArgs e)
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
