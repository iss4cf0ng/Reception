using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reception
{
    public class clsExportSave
    {
        /// <summary>
        /// Export all listview items to specified file.
        /// </summary>
        /// <param name="lv"></param>
        /// <param name="filename"></param>
        public static void lvExport2CSV(ListView lv, string filename)
        {
            using (FileStream fs = new FileStream(filename, FileMode.Create, FileAccess.Write))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.WriteLine(string.Join(",", lv.Columns.Cast<ColumnHeader>().Select(column => column.Text).ToArray()));
                    foreach (ListViewItem item in lv.Items)
                    {
                        sw.WriteLine(string.Join(",", item.SubItems.Cast<ListViewItem.ListViewSubItem>().Select(x => x.Text).ToArray()));
                    }
                }
            }
            MessageBox.Show("Export file successfully: " + filename, "OK", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        public static void lvExport2TXT(ListView lv, string filename)
        {
            using (FileStream fs = new FileStream(filename, FileMode.Create, FileAccess.Write))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    string line_columns = string.Join("|", lv.Columns.Cast<ColumnHeader>().Select(column => column.Text).ToArray());
                    sw.WriteLine(line_columns);
                    sw.WriteLine(new string('-', line_columns.Length));
                    foreach (ListViewItem item in lv.Items)
                    {
                        sw.WriteLine(string.Join("|", item.SubItems.Cast<ListViewItem.ListViewSubItem>().Select(x => x.Text).ToArray()));
                    }
                }
            }
        }

        /// <summary>
        /// Set Clipboard text from one selected listview item
        /// </summary>
        /// <param name="lv"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static string lvCopy(ListView lv, int[] index) //SPECIFIED
        {
            string text = null;
            string[] arr = lv.SelectedItems[0].SubItems.Cast<ListViewItem.ListViewSubItem>().Select(x => x.Text).ToArray();
            text = string.Join("|", arr.Where((x, i) => index.Contains(i)));
            return text;
        }
        public static string lvCopy(ListView lv, int[][] index, string s1, string s2)
        {
            string text = null;
            string[] arr = lv.SelectedItems[0].SubItems.Cast<ListViewItem.ListViewSubItem>().Select(x => x.Text).ToArray();
            List<string> list = new List<string>();
            foreach (int[] i in index)
            {
                list.Add(string.Join(s1, arr.Where((x, j) => i.Contains(j))));
            }

            text = string.Join(s2, list.ToArray());

            return text;
        }
        public static string lvCopy(ListView lv) //ALL
        {
            string text = null;
            text = string.Join("|", lv.SelectedItems[0].SubItems.Cast<ListViewItem.ListViewSubItem>().Select(x => x.Text).ToArray());
            return text;
        }
    }
}
