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
    public partial class frmFileFind : Form
    {
        public frmFileFind()
        {
            InitializeComponent();
        }

        void setup()
        {

        }

        void set_filter()
        {
            file_F filter = new file_F()
            {
                pattern = textBox1.Text,
                regex = checkBox1.Checked,
                ignore_case = checkBox2.Checked,

                file = checkBox4.Checked,
                folder = checkBox5.Checked,

                exts = checkBox6.Checked ? textBox2.Text : "",

                use_date = checkBox3.Checked,
                cd = checkBox7.Checked, //CREATION DATE
                md = checkBox8.Checked, //MODIFIED DATE
                ad = checkBox9.Checked, //LAST ACCESSED DATE

                date_initial = long.Parse(dateTimePicker1.Value.ToString("F")),
                date_final = long.Parse(dateTimePicker2.Value.ToString("F")),

                size_init = (int)numericUpDown1.Value,
                size_final = (int)numericUpDown2.Value,
            };

            C2.form1.file_Filter(filter);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            set_filter();
            DialogResult = DialogResult.OK;
            ActiveForm.Close();
        }

        private void frmFileFind_Load(object sender, EventArgs e)
        {
            setup();
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                set_filter();
                DialogResult = DialogResult.OK;
                ActiveForm.Close();
            }
        }
    }
}
