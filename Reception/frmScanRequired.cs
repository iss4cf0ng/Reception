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
    public partial class frmScanRequired : Form
    {
        public List<string> plugin_installed;
        IniManager ini = C2.ini_manager;

        public frmScanRequired()
        {
            InitializeComponent();
        }

        void Scan()
        {
            foreach (ListViewItem item in listView1.Items)
            {
                string name = item.SubItems[0].Text;
                bool existed = plugin_installed.Contains(name);
                item.SubItems[1].Text = existed ? "Y" : "N";
                item.BackColor = existed ? Color.LightGreen : Color.LightCoral;
            }
        }

        void Export(FileFilter ff)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = C1.FileDialogGetFilter(ff);
            switch (ff)
            {
                case FileFilter.TXT:
                    clsExportSave.lvExport2TXT(listView1, sfd.FileName);
                    break;
                case FileFilter.CSV:
                    clsExportSave.lvExport2CSV(listView1, sfd.FileName);
                    break;
            }
        }

        List<string> GetRequired()
        {
            return null;
        }

        void LoadFromFile()
        {
            List<ListViewItem> items = new List<ListViewItem>();
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = true;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                foreach (string file in ofd.FileNames)
                {
                    List<string> names = File.ReadAllLines(file).Select(x => x.Split("=")[0]).ToList();
                    foreach (string name in names)
                    {
                        ListViewItem item = new ListViewItem(name);
                        item.SubItems.Add("?");
                        items.Add(item);
                    }
                }

                //ADD ITEMS
                Invoke(new Action(() => listView1.Items.AddRange(items.ToArray())));
            }
        }

        void setup()
        {
            string reqTXT_path = ini.Read("OTHERS", "req_txt");
            if (!File.Exists(reqTXT_path))
            {
                MessageBox.Show("File not found: " + reqTXT_path, "Scan Requirements - setup", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            foreach (string req in File.ReadAllLines(reqTXT_path))
            {
                string _req = req;
                if (_req.Contains("="))
                    _req = req.Split("=")[0];

                ListViewItem item = new ListViewItem(_req);
                item.SubItems.Add("?");
                Invoke(new Action(() => listView1.Items.Add(item)));
            }
        }

        private void frmScanRequired_Load(object sender, EventArgs e)
        {
            setup();
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Scan();
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            
        }

        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            new Thread(LoadFromFile).Start();
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            Export(FileFilter.TXT);
        }

        private void toolStripMenuItem5_Click(object sender, EventArgs e)
        {
            Export(FileFilter.CSV);
        }
    }
}
