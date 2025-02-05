using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Reception
{
    public partial class frmConnectedSession : Form
    {
        Listener conn_Listener;

        public frmConnectedSession()
        {
            InitializeComponent();
        }

        void ListConnected()
        {
            string sql = "SELECT * FROM ConnMethod";
            DataTable dt = C2.sql_conn.DataReader(sql);
            foreach (DataRow dr in dt.Rows)
            {
                string online_id = dr["online_id"].ToString();
                string method = dr["method"].ToString();

                sql = "SELECT host,os,first_online_date,last_online_date FROM Client";
                dt = C2.sql_conn.DataReader(sql);
                DataRow _dr = dt.Rows[0];

                string host = _dr["host"].ToString();
                string os = _dr["os"].ToString();
                string date_first = _dr["first_online_date"].ToString();
                string date_last = _dr["last_online_date"].ToString();

                if (method == "CONNECT")
                {
                    ListViewItem item = new ListViewItem(online_id);
                    item.SubItems.Add(host);
                    item.SubItems.Add(os);
                    item.SubItems.Add(date_first);
                    item.SubItems.Add(date_last);
                    item.Checked = true;
                    listView1.Items.Add(item);
                }
            }

            if (listView1.Items.Count == 0)
                Close();
        }

        void Connect()
        {
            foreach (ListViewItem item in listView1.CheckedItems)
            {
                string host = item.SubItems[1].Text;
                string ip = host.Split(':')[0];
                int port = int.Parse(host.Split(":")[1]);

                string sql_query = "SELECT password FROM ConnMethod";
                string password = C2.sql_conn.DataReader(sql_query).Rows[0]["password"].ToString();

                conn_Listener = new Listener();
                conn_Listener.Received += new Listener.ReceivedEventHandler(C2.form1.Received);
                conn_Listener.Disconnected += new Listener.DisconnectedEventHandler(C2.form1.Disconnect);
                conn_Listener.Connect(ip, port, password);
            }
        }
        void Remove()
        {
            foreach (ListViewItem item in listView1.CheckedItems)
                C2.sql_conn.DeleteClient(item.Text);

            Close();
        }

        void setup()
        {
            ListConnected();
        }

        private void frmConnectedSession_Load(object sender, EventArgs e)
        {
            setup();
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
