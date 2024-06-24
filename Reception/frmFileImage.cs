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
    public partial class frmFileImage : Form
    {
        public ImageList il = new ImageList();

        public frmFileImage()
        {
            InitializeComponent();
        }

        private void ViewImage(bool all = false)
        {
            List<ListViewItem> items = new List<ListViewItem>();
            if (all) foreach (ListViewItem item in listView1.Items) items.Add(item);
            else foreach (ListViewItem item in listView1.SelectedItems) items.Add(item);


            foreach (ListViewItem item in items)
            {
                TabPage page = new TabPage();
                PictureBox pb = new PictureBox();
                object[] objs = (object[])item.Tag;

                tabControl1.TabPages.Add(page);
                page.Controls.Add(pb);

                page.Text = Path.GetFileName((string)objs[0]);

                pb.Dock = DockStyle.Fill;
                pb.Image = C1.B64StrC2Img((string)objs[1]);

                //CHOOSE PICTURE SIZE MODE
                switch (toolStripComboBox1.SelectedIndex)
                {
                    case 0:
                        pb.SizeMode = PictureBoxSizeMode.Normal;
                        break;
                    case 1:
                        pb.SizeMode = PictureBoxSizeMode.StretchImage;
                        break;
                    case 2:
                        pb.SizeMode = PictureBoxSizeMode.AutoSize;
                        break;
                    case 3:
                        pb.SizeMode = PictureBoxSizeMode.CenterImage;
                        break;
                    case 4:
                        pb.SizeMode = PictureBoxSizeMode.Zoom;
                        break;

                }

                tabControl1.SelectedTab = page;
            }
        }

        public void NewImage(string filename, Image img, string b64_str)
        {
            listView1.Invoke(new Action(() =>
            {
                if (il.Images.Keys.Contains(filename))
                    return;

                il.Images.Add(filename, img);
                ListViewItem item = new ListViewItem(Path.GetFileName(filename));
                item.Tag = new object[] { filename, b64_str };
                item.ImageKey = filename;
                listView1.Items.Add(item);
            }));
        }

        void setup()
        {
            il.ImageSize = new Size(256, 256);
            listView1.LargeImageList = il;
            toolStripComboBox1.SelectedIndex = 4;
        }

        private void frmFileImage_Load(object sender, EventArgs e)
        {
            setup();
        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            ViewImage();
        }

        private void toolStripComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            foreach (TabPage page in tabControl1.TabPages)
            {
                if (tabControl1.TabPages[0] != page)
                {
                    //GET PICTURE BOX
                    PictureBox pb = (PictureBox)page.Controls[0];

                    //CHOOSE PICTURE SIZE MODE
                    switch (toolStripComboBox1.SelectedIndex)
                    {
                        case 0:
                            pb.SizeMode = PictureBoxSizeMode.Normal;
                            break;
                        case 1:
                            pb.SizeMode = PictureBoxSizeMode.StretchImage;
                            break;
                        case 2:
                            pb.SizeMode = PictureBoxSizeMode.AutoSize;
                            break;
                        case 3:
                            pb.SizeMode = PictureBoxSizeMode.CenterImage;
                            break;
                        case 4:
                            pb.SizeMode = PictureBoxSizeMode.Zoom;
                            break;

                    }
                }
            }
        }

        private void tabControl1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Modifiers == Keys.Control)
            {
                TabPage page = tabControl1.SelectedTab;
                if (e.KeyCode == Keys.W)
                {
                    if (page != tabControl1.TabPages[0])
                    {
                        tabControl1.TabPages.Remove(page);
                    }
                }
                else if (e.KeyCode == Keys.A)
                {
                    if (page == tabControl1.TabPages[0])
                    {
                        foreach (ListViewItem item in listView1.Items)
                            item.Selected = true;
                    }
                }
            }
            else if (e.KeyCode == Keys.Enter)
            {
                ViewImage();
            }
        }
    }
}
