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
    public partial class frmFileTransferStatus : Form
    {
        public Dictionary<string, int> status = new Dictionary<string, int>();

        public frmFileTransferStatus()
        {
            InitializeComponent();
        }

        private void AddlvItem(string filename, string status, int max_value)
        {
            //LISTVIEW INIT
            ListViewItem item = new ListViewItem(filename);

            item.SubItems.Add(status);
            item.SubItems.Add("0 %");
            item.Tag = new int[] { 0, max_value };
            listView1.Items.Add(item);
        }

        public void UpdateProgress(string filename)
        {
            listView1.Invoke(new Action(() =>
            {
                ListViewItem item = listView1.Items.Cast<ListViewItem>().FirstOrDefault(i => i.SubItems[0].Text == filename);
                if (item != null)
                {
                    int[] progress = (int[])item.Tag;
                    int done = progress[0];
                    int max = progress[1];
                    done++;
                    decimal percentage = Decimal.Divide(done + 1, max) * 100;
                    item.SubItems[2].Text = $"{percentage} %";
                    item.Tag = new int[] { done, max };
                }
            }));
        }

        private void setup()
        {
            foreach (string filename in status.Keys)
            {
                int max_value = status[filename];
                AddlvItem(filename, "Running", max_value);
            }
        }

        private void frmFileTransferStatus_Load(object sender, EventArgs e)
        {
            setup();
        }
    }
}
