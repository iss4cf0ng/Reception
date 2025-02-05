namespace Reception
{
    partial class frmRPluginUpload
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmRPluginUpload));
            listView1 = new ListView();
            columnHeader1 = new ColumnHeader();
            columnHeader2 = new ColumnHeader();
            columnHeader4 = new ColumnHeader();
            columnHeader3 = new ColumnHeader();
            imageList1 = new ImageList(components);
            progressBar1 = new ProgressBar();
            statusStrip1 = new StatusStrip();
            toolStripStatusLabel1 = new ToolStripStatusLabel();
            richTextBox1 = new RichTextBox();
            statusStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // listView1
            // 
            listView1.Columns.AddRange(new ColumnHeader[] { columnHeader1, columnHeader2, columnHeader4, columnHeader3 });
            listView1.FullRowSelect = true;
            listView1.Location = new Point(0, -1);
            listView1.Name = "listView1";
            listView1.Size = new Size(468, 230);
            listView1.SmallImageList = imageList1;
            listView1.TabIndex = 0;
            listView1.UseCompatibleStateImageBehavior = false;
            listView1.View = View.Details;
            // 
            // columnHeader1
            // 
            columnHeader1.Text = "Name";
            columnHeader1.Width = 100;
            // 
            // columnHeader2
            // 
            columnHeader2.Text = "Temp Folder";
            columnHeader2.Width = 120;
            // 
            // columnHeader4
            // 
            columnHeader4.Text = "OK?";
            // 
            // columnHeader3
            // 
            columnHeader3.Text = "Path";
            columnHeader3.Width = 300;
            // 
            // imageList1
            // 
            imageList1.ColorDepth = ColorDepth.Depth32Bit;
            imageList1.ImageStream = (ImageListStreamer)resources.GetObject("imageList1.ImageStream");
            imageList1.TransparentColor = Color.Transparent;
            imageList1.Images.SetKeyName(0, "4954479.png");
            imageList1.Images.SetKeyName(1, "warning-sign-icon-transparent-background-free-png.png");
            // 
            // progressBar1
            // 
            progressBar1.Location = new Point(0, 377);
            progressBar1.Name = "progressBar1";
            progressBar1.Size = new Size(468, 23);
            progressBar1.TabIndex = 1;
            // 
            // statusStrip1
            // 
            statusStrip1.Items.AddRange(new ToolStripItem[] { toolStripStatusLabel1 });
            statusStrip1.Location = new Point(0, 403);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(468, 22);
            statusStrip1.TabIndex = 2;
            statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            toolStripStatusLabel1.Size = new Size(128, 17);
            toolStripStatusLabel1.Text = "toolStripStatusLabel1";
            // 
            // richTextBox1
            // 
            richTextBox1.Location = new Point(0, 235);
            richTextBox1.Name = "richTextBox1";
            richTextBox1.Size = new Size(468, 136);
            richTextBox1.TabIndex = 3;
            richTextBox1.Text = "";
            // 
            // frmRPluginUpload
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(468, 425);
            Controls.Add(richTextBox1);
            Controls.Add(statusStrip1);
            Controls.Add(progressBar1);
            Controls.Add(listView1);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            MaximizeBox = false;
            Name = "frmRPluginUpload";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "frmRPluginUpload";
            Load += frmRPluginUpload_Load;
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ListView listView1;
        private ColumnHeader columnHeader1;
        private ColumnHeader columnHeader2;
        private ColumnHeader columnHeader4;
        private ColumnHeader columnHeader3;
        private ProgressBar progressBar1;
        private StatusStrip statusStrip1;
        private ToolStripStatusLabel toolStripStatusLabel1;
        private RichTextBox richTextBox1;
        private ImageList imageList1;
    }
}