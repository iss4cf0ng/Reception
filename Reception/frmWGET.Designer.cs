namespace Reception
{
    partial class frmWGET
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
            textEditorControl1 = new ICSharpCode.TextEditor.TextEditorControl();
            button1 = new Button();
            checkBox1 = new CheckBox();
            textBox1 = new TextBox();
            label1 = new Label();
            tabControl1 = new TabControl();
            tabPage1 = new TabPage();
            tabControl2 = new TabControl();
            tabPage4 = new TabPage();
            tabPage5 = new TabPage();
            listView2 = new ListView();
            columnHeader3 = new ColumnHeader();
            columnHeader5 = new ColumnHeader();
            columnHeader4 = new ColumnHeader();
            tabPage3 = new TabPage();
            listView1 = new ListView();
            columnHeader1 = new ColumnHeader();
            columnHeader2 = new ColumnHeader();
            tabPage2 = new TabPage();
            richTextBox1 = new RichTextBox();
            button2 = new Button();
            tabControl1.SuspendLayout();
            tabPage1.SuspendLayout();
            tabControl2.SuspendLayout();
            tabPage4.SuspendLayout();
            tabPage5.SuspendLayout();
            tabPage3.SuspendLayout();
            tabPage2.SuspendLayout();
            SuspendLayout();
            // 
            // textEditorControl1
            // 
            textEditorControl1.Dock = DockStyle.Fill;
            textEditorControl1.Highlighting = null;
            textEditorControl1.Location = new Point(3, 3);
            textEditorControl1.Name = "textEditorControl1";
            textEditorControl1.Size = new Size(442, 288);
            textEditorControl1.TabIndex = 0;
            // 
            // button1
            // 
            button1.Location = new Point(330, 397);
            button1.Name = "button1";
            button1.Size = new Size(131, 39);
            button1.TabIndex = 1;
            button1.Text = "Download";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // checkBox1
            // 
            checkBox1.AutoSize = true;
            checkBox1.Location = new Point(12, 407);
            checkBox1.Name = "checkBox1";
            checkBox1.Size = new Size(169, 21);
            checkBox1.TabIndex = 2;
            checkBox1.Text = "Create folder if not exist";
            checkBox1.UseVisualStyleBackColor = true;
            // 
            // textBox1
            // 
            textBox1.Location = new Point(78, 367);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(383, 24);
            textBox1.TabIndex = 3;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 370);
            label1.Name = "label1";
            label1.Size = new Size(60, 17);
            label1.TabIndex = 4;
            label1.Text = "Save To :";
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Controls.Add(tabPage3);
            tabControl1.Controls.Add(tabPage2);
            tabControl1.Location = new Point(1, 1);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(470, 360);
            tabControl1.TabIndex = 5;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(tabControl2);
            tabPage1.Location = new Point(4, 26);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(462, 330);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "Target URL";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabControl2
            // 
            tabControl2.Controls.Add(tabPage4);
            tabControl2.Controls.Add(tabPage5);
            tabControl2.Dock = DockStyle.Fill;
            tabControl2.Location = new Point(3, 3);
            tabControl2.Name = "tabControl2";
            tabControl2.SelectedIndex = 0;
            tabControl2.Size = new Size(456, 324);
            tabControl2.TabIndex = 1;
            // 
            // tabPage4
            // 
            tabPage4.Controls.Add(textEditorControl1);
            tabPage4.Location = new Point(4, 26);
            tabPage4.Name = "tabPage4";
            tabPage4.Padding = new Padding(3);
            tabPage4.Size = new Size(448, 294);
            tabPage4.TabIndex = 0;
            tabPage4.Text = "Target URL";
            tabPage4.UseVisualStyleBackColor = true;
            // 
            // tabPage5
            // 
            tabPage5.Controls.Add(listView2);
            tabPage5.Location = new Point(4, 26);
            tabPage5.Name = "tabPage5";
            tabPage5.Padding = new Padding(3);
            tabPage5.Size = new Size(448, 294);
            tabPage5.TabIndex = 1;
            tabPage5.Text = "Result";
            tabPage5.UseVisualStyleBackColor = true;
            // 
            // listView2
            // 
            listView2.Columns.AddRange(new ColumnHeader[] { columnHeader3, columnHeader5, columnHeader4 });
            listView2.Dock = DockStyle.Fill;
            listView2.Location = new Point(3, 3);
            listView2.Name = "listView2";
            listView2.Size = new Size(442, 288);
            listView2.TabIndex = 0;
            listView2.UseCompatibleStateImageBehavior = false;
            listView2.View = View.Details;
            // 
            // columnHeader3
            // 
            columnHeader3.Text = "URL";
            columnHeader3.Width = 300;
            // 
            // columnHeader5
            // 
            columnHeader5.Text = "File";
            columnHeader5.Width = 300;
            // 
            // columnHeader4
            // 
            columnHeader4.Text = "Status";
            columnHeader4.Width = 150;
            // 
            // tabPage3
            // 
            tabPage3.Controls.Add(listView1);
            tabPage3.Location = new Point(4, 24);
            tabPage3.Name = "tabPage3";
            tabPage3.Size = new Size(462, 332);
            tabPage3.TabIndex = 2;
            tabPage3.Text = "Alive Test";
            tabPage3.UseVisualStyleBackColor = true;
            // 
            // listView1
            // 
            listView1.Columns.AddRange(new ColumnHeader[] { columnHeader1, columnHeader2 });
            listView1.Dock = DockStyle.Fill;
            listView1.Location = new Point(0, 0);
            listView1.Name = "listView1";
            listView1.Size = new Size(462, 332);
            listView1.TabIndex = 0;
            listView1.UseCompatibleStateImageBehavior = false;
            listView1.View = View.Details;
            // 
            // columnHeader1
            // 
            columnHeader1.Text = "URL";
            columnHeader1.Width = 350;
            // 
            // columnHeader2
            // 
            columnHeader2.Text = "Status Code";
            columnHeader2.Width = 100;
            // 
            // tabPage2
            // 
            tabPage2.Controls.Add(richTextBox1);
            tabPage2.Location = new Point(4, 24);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(462, 332);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "Request Logs";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // richTextBox1
            // 
            richTextBox1.Dock = DockStyle.Fill;
            richTextBox1.Location = new Point(3, 3);
            richTextBox1.Name = "richTextBox1";
            richTextBox1.Size = new Size(456, 326);
            richTextBox1.TabIndex = 6;
            richTextBox1.Text = "";
            // 
            // button2
            // 
            button2.Location = new Point(193, 397);
            button2.Name = "button2";
            button2.Size = new Size(131, 39);
            button2.TabIndex = 6;
            button2.Text = "Test Url";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // frmWGET
            // 
            AutoScaleDimensions = new SizeF(8F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(471, 445);
            Controls.Add(button2);
            Controls.Add(tabControl1);
            Controls.Add(label1);
            Controls.Add(textBox1);
            Controls.Add(checkBox1);
            Controls.Add(button1);
            Font = new Font("Microsoft JhengHei UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            Name = "frmWGET";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "frmWGET";
            Load += frmWGET_Load;
            tabControl1.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            tabControl2.ResumeLayout(false);
            tabPage4.ResumeLayout(false);
            tabPage5.ResumeLayout(false);
            tabPage3.ResumeLayout(false);
            tabPage2.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ICSharpCode.TextEditor.TextEditorControl textEditorControl1;
        private Button button1;
        private CheckBox checkBox1;
        private TextBox textBox1;
        private Label label1;
        private TabControl tabControl1;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private RichTextBox richTextBox1;
        private TabPage tabPage3;
        private Button button2;
        private ListView listView1;
        private ColumnHeader columnHeader1;
        private ColumnHeader columnHeader2;
        private TabControl tabControl2;
        private TabPage tabPage4;
        private TabPage tabPage5;
        private ListView listView2;
        private ColumnHeader columnHeader3;
        private ColumnHeader columnHeader4;
        private ColumnHeader columnHeader5;
    }
}