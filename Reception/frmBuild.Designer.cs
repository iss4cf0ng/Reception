namespace Reception
{
    partial class frmBuild
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
            groupBox1 = new GroupBox();
            label10 = new Label();
            numericUpDown3 = new NumericUpDown();
            label9 = new Label();
            checkBox4 = new CheckBox();
            numericUpDown2 = new NumericUpDown();
            label5 = new Label();
            radioButton2 = new RadioButton();
            radioButton1 = new RadioButton();
            numericUpDown1 = new NumericUpDown();
            label2 = new Label();
            textBox1 = new TextBox();
            label1 = new Label();
            button1 = new Button();
            groupBox2 = new GroupBox();
            numericUpDown4 = new NumericUpDown();
            label8 = new Label();
            comboBox2 = new ComboBox();
            label7 = new Label();
            label6 = new Label();
            comboBox1 = new ComboBox();
            textBox3 = new TextBox();
            textBox2 = new TextBox();
            label4 = new Label();
            label3 = new Label();
            groupBox3 = new GroupBox();
            checkBox3 = new CheckBox();
            comboBox3 = new ComboBox();
            button2 = new Button();
            checkBox2 = new CheckBox();
            checkBox6 = new CheckBox();
            checkBox5 = new CheckBox();
            checkBox1 = new CheckBox();
            groupBox5 = new GroupBox();
            listView1 = new ListView();
            columnHeader1 = new ColumnHeader();
            columnHeader2 = new ColumnHeader();
            label11 = new Label();
            textBox4 = new TextBox();
            button3 = new Button();
            groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numericUpDown3).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown1).BeginInit();
            groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numericUpDown4).BeginInit();
            groupBox3.SuspendLayout();
            groupBox5.SuspendLayout();
            SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(label10);
            groupBox1.Controls.Add(numericUpDown3);
            groupBox1.Controls.Add(label9);
            groupBox1.Controls.Add(checkBox4);
            groupBox1.Controls.Add(numericUpDown2);
            groupBox1.Controls.Add(label5);
            groupBox1.Controls.Add(radioButton2);
            groupBox1.Controls.Add(radioButton1);
            groupBox1.Controls.Add(numericUpDown1);
            groupBox1.Controls.Add(label2);
            groupBox1.Controls.Add(textBox1);
            groupBox1.Controls.Add(label1);
            groupBox1.Location = new Point(12, 97);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(276, 193);
            groupBox1.TabIndex = 0;
            groupBox1.TabStop = false;
            groupBox1.Text = "Server";
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Location = new Point(237, 88);
            label10.Name = "label10";
            label10.Size = new Size(33, 19);
            label10.TabIndex = 14;
            label10.Text = "Sec";
            // 
            // numericUpDown3
            // 
            numericUpDown3.Location = new Point(70, 86);
            numericUpDown3.Maximum = new decimal(new int[] { 65535, 0, 0, 0 });
            numericUpDown3.Name = "numericUpDown3";
            numericUpDown3.Size = new Size(161, 27);
            numericUpDown3.TabIndex = 13;
            numericUpDown3.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new Point(7, 88);
            label9.Name = "label9";
            label9.Size = new Size(54, 19);
            label9.TabIndex = 12;
            label9.Text = "Sleep :";
            // 
            // checkBox4
            // 
            checkBox4.AutoSize = true;
            checkBox4.Location = new Point(210, 24);
            checkBox4.Name = "checkBox4";
            checkBox4.Size = new Size(60, 23);
            checkBox4.TabIndex = 8;
            checkBox4.Text = "DNS";
            checkBox4.UseVisualStyleBackColor = true;
            // 
            // numericUpDown2
            // 
            numericUpDown2.Location = new Point(82, 152);
            numericUpDown2.Maximum = new decimal(new int[] { 65535, 0, 0, 0 });
            numericUpDown2.Name = "numericUpDown2";
            numericUpDown2.Size = new Size(165, 27);
            numericUpDown2.TabIndex = 11;
            numericUpDown2.Value = new decimal(new int[] { 4444, 0, 0, 0 });
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(19, 154);
            label5.Name = "label5";
            label5.Size = new Size(57, 19);
            label5.TabIndex = 10;
            label5.Text = "Listen :";
            // 
            // radioButton2
            // 
            radioButton2.AutoSize = true;
            radioButton2.Location = new Point(146, 119);
            radioButton2.Name = "radioButton2";
            radioButton2.Size = new Size(101, 23);
            radioButton2.TabIndex = 9;
            radioButton2.Text = "Listen Port";
            radioButton2.UseVisualStyleBackColor = true;
            // 
            // radioButton1
            // 
            radioButton1.AutoSize = true;
            radioButton1.Checked = true;
            radioButton1.Location = new Point(19, 119);
            radioButton1.Name = "radioButton1";
            radioButton1.Size = new Size(82, 23);
            radioButton1.TabIndex = 8;
            radioButton1.TabStop = true;
            radioButton1.Text = "Reverse";
            radioButton1.UseVisualStyleBackColor = true;
            radioButton1.CheckedChanged += radioButton1_CheckedChanged;
            // 
            // numericUpDown1
            // 
            numericUpDown1.Location = new Point(70, 55);
            numericUpDown1.Maximum = new decimal(new int[] { 65535, 0, 0, 0 });
            numericUpDown1.Name = "numericUpDown1";
            numericUpDown1.Size = new Size(200, 27);
            numericUpDown1.TabIndex = 3;
            numericUpDown1.Value = new decimal(new int[] { 4444, 0, 0, 0 });
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(19, 57);
            label2.Name = "label2";
            label2.Size = new Size(45, 19);
            label2.TabIndex = 2;
            label2.Text = "Port :";
            // 
            // textBox1
            // 
            textBox1.Location = new Point(70, 22);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(134, 27);
            textBox1.TabIndex = 1;
            textBox1.Text = "127.0.0.1";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(35, 25);
            label1.Name = "label1";
            label1.Size = new Size(29, 19);
            label1.TabIndex = 0;
            label1.Text = "IP :";
            // 
            // button1
            // 
            button1.Location = new Point(294, 348);
            button1.Name = "button1";
            button1.Size = new Size(316, 39);
            button1.TabIndex = 1;
            button1.Text = "Build !";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(numericUpDown4);
            groupBox2.Controls.Add(label8);
            groupBox2.Controls.Add(comboBox2);
            groupBox2.Controls.Add(label7);
            groupBox2.Controls.Add(label6);
            groupBox2.Controls.Add(comboBox1);
            groupBox2.Controls.Add(textBox3);
            groupBox2.Controls.Add(textBox2);
            groupBox2.Controls.Add(label4);
            groupBox2.Controls.Add(label3);
            groupBox2.Location = new Point(294, 12);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(316, 193);
            groupBox2.TabIndex = 2;
            groupBox2.TabStop = false;
            groupBox2.Text = "Key Exchange";
            // 
            // numericUpDown4
            // 
            numericUpDown4.Location = new Point(112, 154);
            numericUpDown4.Maximum = new decimal(new int[] { 65535, 0, 0, 0 });
            numericUpDown4.Name = "numericUpDown4";
            numericUpDown4.Size = new Size(198, 27);
            numericUpDown4.TabIndex = 12;
            numericUpDown4.Value = new decimal(new int[] { 2048, 0, 0, 0 });
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new Point(6, 156);
            label8.Name = "label8";
            label8.Size = new Size(102, 19);
            label8.TabIndex = 10;
            label8.Text = "RSA KeySize :";
            // 
            // comboBox2
            // 
            comboBox2.FormattingEnabled = true;
            comboBox2.Items.AddRange(new object[] { "128", "192", "256" });
            comboBox2.Location = new Point(112, 118);
            comboBox2.Name = "comboBox2";
            comboBox2.Size = new Size(198, 27);
            comboBox2.TabIndex = 9;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(6, 121);
            label7.Name = "label7";
            label7.Size = new Size(100, 19);
            label7.TabIndex = 8;
            label7.Text = "AES KeySize :";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(6, 88);
            label6.Name = "label6";
            label6.Size = new Size(63, 19);
            label6.TabIndex = 7;
            label6.Text = "Crypto :";
            // 
            // comboBox1
            // 
            comboBox1.FormattingEnabled = true;
            comboBox1.Items.AddRange(new object[] { "AES-CBC" });
            comboBox1.Location = new Point(75, 85);
            comboBox1.Name = "comboBox1";
            comboBox1.Size = new Size(235, 27);
            comboBox1.TabIndex = 6;
            // 
            // textBox3
            // 
            textBox3.Location = new Point(75, 22);
            textBox3.Name = "textBox3";
            textBox3.Size = new Size(235, 27);
            textBox3.TabIndex = 5;
            textBox3.Text = "WTF";
            // 
            // textBox2
            // 
            textBox2.Location = new Point(159, 55);
            textBox2.Name = "textBox2";
            textBox2.Size = new Size(151, 27);
            textBox2.TabIndex = 3;
            textBox2.Text = "HelloWorld!";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(6, 25);
            label4.Name = "label4";
            label4.Size = new Size(58, 19);
            label4.TabIndex = 4;
            label4.Text = "Knock :";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(6, 58);
            label3.Name = "label3";
            label3.Size = new Size(147, 19);
            label3.TabIndex = 2;
            label3.Text = "Acknowledgement :";
            // 
            // groupBox3
            // 
            groupBox3.Controls.Add(checkBox3);
            groupBox3.Controls.Add(comboBox3);
            groupBox3.Controls.Add(button2);
            groupBox3.Controls.Add(checkBox2);
            groupBox3.Controls.Add(checkBox6);
            groupBox3.Controls.Add(checkBox5);
            groupBox3.Controls.Add(checkBox1);
            groupBox3.Location = new Point(12, 299);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new Size(276, 88);
            groupBox3.TabIndex = 5;
            groupBox3.TabStop = false;
            groupBox3.Text = "Protection";
            groupBox3.Enter += groupBox3_Enter;
            // 
            // checkBox3
            // 
            checkBox3.AutoSize = true;
            checkBox3.Location = new Point(208, 26);
            checkBox3.Name = "checkBox3";
            checkBox3.Size = new Size(50, 23);
            checkBox3.TabIndex = 7;
            checkBox3.Text = "Bin";
            checkBox3.UseVisualStyleBackColor = true;
            // 
            // comboBox3
            // 
            comboBox3.FormattingEnabled = true;
            comboBox3.Location = new Point(68, 55);
            comboBox3.Name = "comboBox3";
            comboBox3.Size = new Size(132, 27);
            comboBox3.TabIndex = 6;
            // 
            // button2
            // 
            button2.Location = new Point(206, 55);
            button2.Name = "button2";
            button2.Size = new Size(54, 27);
            button2.TabIndex = 6;
            button2.Text = "?";
            button2.UseVisualStyleBackColor = true;
            // 
            // checkBox2
            // 
            checkBox2.AutoSize = true;
            checkBox2.Location = new Point(7, 57);
            checkBox2.Name = "checkBox2";
            checkBox2.Size = new Size(57, 23);
            checkBox2.TabIndex = 3;
            checkBox2.Text = "RSA";
            checkBox2.UseVisualStyleBackColor = true;
            // 
            // checkBox6
            // 
            checkBox6.AutoSize = true;
            checkBox6.Location = new Point(151, 26);
            checkBox6.Name = "checkBox6";
            checkBox6.Size = new Size(53, 23);
            checkBox6.TabIndex = 2;
            checkBox6.Text = "Chr";
            checkBox6.UseVisualStyleBackColor = true;
            // 
            // checkBox5
            // 
            checkBox5.AutoSize = true;
            checkBox5.Location = new Point(91, 26);
            checkBox5.Name = "checkBox5";
            checkBox5.Size = new Size(54, 23);
            checkBox5.TabIndex = 1;
            checkBox5.Text = "Hex";
            checkBox5.UseVisualStyleBackColor = true;
            // 
            // checkBox1
            // 
            checkBox1.AutoSize = true;
            checkBox1.Location = new Point(7, 26);
            checkBox1.Name = "checkBox1";
            checkBox1.Size = new Size(78, 23);
            checkBox1.TabIndex = 0;
            checkBox1.Text = "Base64";
            checkBox1.UseVisualStyleBackColor = true;
            // 
            // groupBox5
            // 
            groupBox5.Controls.Add(listView1);
            groupBox5.Location = new Point(294, 211);
            groupBox5.Name = "groupBox5";
            groupBox5.Size = new Size(316, 131);
            groupBox5.TabIndex = 6;
            groupBox5.TabStop = false;
            groupBox5.Text = "Payload";
            // 
            // listView1
            // 
            listView1.Columns.AddRange(new ColumnHeader[] { columnHeader1, columnHeader2 });
            listView1.Location = new Point(6, 26);
            listView1.Name = "listView1";
            listView1.Size = new Size(304, 99);
            listView1.TabIndex = 0;
            listView1.UseCompatibleStateImageBehavior = false;
            listView1.View = View.Details;
            // 
            // columnHeader1
            // 
            columnHeader1.Text = "Name";
            columnHeader1.Width = 120;
            // 
            // columnHeader2
            // 
            columnHeader2.Text = "Version";
            columnHeader2.Width = 150;
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Location = new Point(12, 13);
            label11.Name = "label11";
            label11.Size = new Size(123, 19);
            label11.TabIndex = 7;
            label11.Text = "Backdoor UUID :";
            // 
            // textBox4
            // 
            textBox4.Location = new Point(12, 41);
            textBox4.Multiline = true;
            textBox4.Name = "textBox4";
            textBox4.Size = new Size(276, 48);
            textBox4.TabIndex = 8;
            textBox4.TextChanged += textBox4_TextChanged;
            // 
            // button3
            // 
            button3.Location = new Point(141, 9);
            button3.Name = "button3";
            button3.Size = new Size(147, 26);
            button3.TabIndex = 13;
            button3.Text = "Refresh";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            // 
            // frmBuild
            // 
            AutoScaleDimensions = new SizeF(9F, 19F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(622, 399);
            Controls.Add(button3);
            Controls.Add(textBox4);
            Controls.Add(label11);
            Controls.Add(groupBox5);
            Controls.Add(groupBox3);
            Controls.Add(groupBox2);
            Controls.Add(button1);
            Controls.Add(groupBox1);
            Font = new Font("Microsoft JhengHei UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            MaximizeBox = false;
            Name = "frmBuild";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Builder";
            Load += frmBuild_Load;
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numericUpDown3).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown2).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown1).EndInit();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numericUpDown4).EndInit();
            groupBox3.ResumeLayout(false);
            groupBox3.PerformLayout();
            groupBox5.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private GroupBox groupBox1;
        private TextBox textBox1;
        private Label label1;
        private NumericUpDown numericUpDown1;
        private Label label2;
        private Button button1;
        private GroupBox groupBox2;
        private TextBox textBox2;
        private Label label3;
        private Label label4;
        private GroupBox groupBox3;
        private CheckBox checkBox1;
        private CheckBox checkBox2;
        private CheckBox checkBox3;
        private NumericUpDown numericUpDown2;
        private Label label5;
        private RadioButton radioButton2;
        private RadioButton radioButton1;
        private TextBox textBox3;
        private Label label7;
        private Label label6;
        private ComboBox comboBox1;
        private ComboBox comboBox2;
        private Label label8;
        private CheckBox checkBox4;
        private Label label10;
        private NumericUpDown numericUpDown3;
        private Label label9;
        private NumericUpDown numericUpDown4;
        private CheckBox checkBox5;
        private CheckBox checkBox6;
        private Button button2;
        private ComboBox comboBox3;
        private GroupBox groupBox5;
        private ListView listView1;
        private ColumnHeader columnHeader1;
        private ColumnHeader columnHeader2;
        private Label label11;
        private TextBox textBox4;
        private Button button3;
    }
}