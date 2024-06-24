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
    public partial class frmProcessFind : Form
    {
        public frmProcessFind()
        {
            InitializeComponent();
        }

        void set_filter()
        {
            Proc_F filter = new Proc_F()
            {

            };

            C2.form1.proc_Filter(filter);
        }

        void setup()
        {

        }

        private void frmProcessFind_Load(object sender, EventArgs e)
        {

        }
    }
}
