namespace Reception
{
    partial class frmFileFind
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
            label1 = new Label();
            textBox1 = new TextBox();
            checkBox1 = new CheckBox();
            button1 = new Button();
            dateTimePicker1 = new DateTimePicker();
            checkBox2 = new CheckBox();
            dateTimePicker2 = new DateTimePicker();
            label2 = new Label();
            numericUpDown1 = new NumericUpDown();
            numericUpDown2 = new NumericUpDown();
            label3 = new Label();
            checkBox3 = new CheckBox();
            checkBox4 = new CheckBox();
            checkBox5 = new CheckBox();
            checkBox6 = new CheckBox();
            textBox2 = new TextBox();
            checkBox7 = new CheckBox();
            checkBox8 = new CheckBox();
            checkBox9 = new CheckBox();
            checkBox10 = new CheckBox();
            comboBox1 = new ComboBox();
            ((System.ComponentModel.ISupportInitialize)numericUpDown1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown2).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(10, 17);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(82, 19);
            label1.TabIndex = 0;
            label1.Text = "FileName :";
            // 
            // textBox1
            // 
            textBox1.Location = new Point(99, 14);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(156, 27);
            textBox1.TabIndex = 1;
            textBox1.KeyDown += textBox1_KeyDown;
            // 
            // checkBox1
            // 
            checkBox1.AutoSize = true;
            checkBox1.Location = new Point(261, 16);
            checkBox1.Name = "checkBox1";
            checkBox1.Size = new Size(71, 23);
            checkBox1.TabIndex = 2;
            checkBox1.Text = "Regex";
            checkBox1.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            button1.Location = new Point(10, 175);
            button1.Name = "button1";
            button1.Size = new Size(426, 50);
            button1.TabIndex = 3;
            button1.Text = "Find";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // dateTimePicker1
            // 
            dateTimePicker1.Location = new Point(99, 77);
            dateTimePicker1.Name = "dateTimePicker1";
            dateTimePicker1.Size = new Size(156, 27);
            dateTimePicker1.TabIndex = 4;
            // 
            // checkBox2
            // 
            checkBox2.AutoSize = true;
            checkBox2.Location = new Point(25, 81);
            checkBox2.Name = "checkBox2";
            checkBox2.Size = new Size(67, 23);
            checkBox2.TabIndex = 5;
            checkBox2.Text = "Date :";
            checkBox2.UseVisualStyleBackColor = true;
            // 
            // dateTimePicker2
            // 
            dateTimePicker2.Location = new Point(292, 77);
            dateTimePicker2.Name = "dateTimePicker2";
            dateTimePicker2.Size = new Size(144, 27);
            dateTimePicker2.TabIndex = 6;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(262, 81);
            label2.Margin = new Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new Size(20, 19);
            label2.TabIndex = 7;
            label2.Text = "~";
            // 
            // numericUpDown1
            // 
            numericUpDown1.Location = new Point(99, 142);
            numericUpDown1.Name = "numericUpDown1";
            numericUpDown1.Size = new Size(93, 27);
            numericUpDown1.TabIndex = 8;
            // 
            // numericUpDown2
            // 
            numericUpDown2.Location = new Point(224, 142);
            numericUpDown2.Name = "numericUpDown2";
            numericUpDown2.Size = new Size(93, 27);
            numericUpDown2.TabIndex = 9;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(197, 144);
            label3.Margin = new Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.Size = new Size(20, 19);
            label3.TabIndex = 10;
            label3.Text = "~";
            // 
            // checkBox3
            // 
            checkBox3.AutoSize = true;
            checkBox3.Location = new Point(338, 16);
            checkBox3.Name = "checkBox3";
            checkBox3.Size = new Size(107, 23);
            checkBox3.TabIndex = 12;
            checkBox3.Text = "IgnoreCase";
            checkBox3.UseVisualStyleBackColor = true;
            // 
            // checkBox4
            // 
            checkBox4.AutoSize = true;
            checkBox4.Location = new Point(99, 48);
            checkBox4.Name = "checkBox4";
            checkBox4.Size = new Size(52, 23);
            checkBox4.TabIndex = 13;
            checkBox4.Text = "File";
            checkBox4.UseVisualStyleBackColor = true;
            // 
            // checkBox5
            // 
            checkBox5.AutoSize = true;
            checkBox5.Location = new Point(157, 48);
            checkBox5.Name = "checkBox5";
            checkBox5.Size = new Size(73, 23);
            checkBox5.TabIndex = 14;
            checkBox5.Text = "Folder";
            checkBox5.UseVisualStyleBackColor = true;
            // 
            // checkBox6
            // 
            checkBox6.AutoSize = true;
            checkBox6.Location = new Point(236, 48);
            checkBox6.Name = "checkBox6";
            checkBox6.Size = new Size(101, 23);
            checkBox6.TabIndex = 15;
            checkBox6.Text = "Extension :";
            checkBox6.UseVisualStyleBackColor = true;
            // 
            // textBox2
            // 
            textBox2.Location = new Point(343, 46);
            textBox2.Name = "textBox2";
            textBox2.Size = new Size(93, 27);
            textBox2.TabIndex = 16;
            // 
            // checkBox7
            // 
            checkBox7.AutoSize = true;
            checkBox7.Location = new Point(99, 113);
            checkBox7.Name = "checkBox7";
            checkBox7.Size = new Size(87, 23);
            checkBox7.TabIndex = 17;
            checkBox7.Text = "Creation";
            checkBox7.UseVisualStyleBackColor = true;
            // 
            // checkBox8
            // 
            checkBox8.AutoSize = true;
            checkBox8.Location = new Point(192, 113);
            checkBox8.Name = "checkBox8";
            checkBox8.Size = new Size(92, 23);
            checkBox8.TabIndex = 18;
            checkBox8.Text = "Modified";
            checkBox8.UseVisualStyleBackColor = true;
            // 
            // checkBox9
            // 
            checkBox9.AutoSize = true;
            checkBox9.Location = new Point(292, 113);
            checkBox9.Name = "checkBox9";
            checkBox9.Size = new Size(124, 23);
            checkBox9.TabIndex = 19;
            checkBox9.Text = "Last Accessed";
            checkBox9.UseVisualStyleBackColor = true;
            // 
            // checkBox10
            // 
            checkBox10.AutoSize = true;
            checkBox10.Location = new Point(12, 143);
            checkBox10.Name = "checkBox10";
            checkBox10.Size = new Size(84, 23);
            checkBox10.TabIndex = 20;
            checkBox10.Text = "Length :";
            checkBox10.UseVisualStyleBackColor = true;
            // 
            // comboBox1
            // 
            comboBox1.FormattingEnabled = true;
            comboBox1.Items.AddRange(new object[] { "BYTE", "KB", "MB", "GB" });
            comboBox1.Location = new Point(323, 142);
            comboBox1.Name = "comboBox1";
            comboBox1.Size = new Size(113, 27);
            comboBox1.TabIndex = 21;
            // 
            // frmFileFind
            // 
            AutoScaleDimensions = new SizeF(9F, 19F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(448, 237);
            Controls.Add(comboBox1);
            Controls.Add(checkBox10);
            Controls.Add(checkBox9);
            Controls.Add(checkBox8);
            Controls.Add(checkBox7);
            Controls.Add(textBox2);
            Controls.Add(checkBox6);
            Controls.Add(checkBox5);
            Controls.Add(checkBox4);
            Controls.Add(checkBox3);
            Controls.Add(label3);
            Controls.Add(numericUpDown2);
            Controls.Add(numericUpDown1);
            Controls.Add(label2);
            Controls.Add(dateTimePicker2);
            Controls.Add(checkBox2);
            Controls.Add(dateTimePicker1);
            Controls.Add(button1);
            Controls.Add(checkBox1);
            Controls.Add(textBox1);
            Controls.Add(label1);
            Font = new Font("Microsoft JhengHei UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            Margin = new Padding(4);
            Name = "frmFileFind";
            StartPosition = FormStartPosition.CenterParent;
            Text = "File Filter";
            Load += frmFileFind_Load;
            ((System.ComponentModel.ISupportInitialize)numericUpDown1).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown2).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private TextBox textBox1;
        private CheckBox checkBox1;
        private Button button1;
        private DateTimePicker dateTimePicker1;
        private CheckBox checkBox2;
        private DateTimePicker dateTimePicker2;
        private Label label2;
        private NumericUpDown numericUpDown1;
        private NumericUpDown numericUpDown2;
        private Label label3;
        private CheckBox checkBox3;
        private CheckBox checkBox4;
        private CheckBox checkBox5;
        private CheckBox checkBox6;
        private TextBox textBox2;
        private CheckBox checkBox7;
        private CheckBox checkBox8;
        private CheckBox checkBox9;
        private CheckBox checkBox10;
        private ComboBox comboBox1;
    }
}