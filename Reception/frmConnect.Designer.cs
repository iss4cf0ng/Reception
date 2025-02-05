namespace Reception
{
    partial class frmConnect
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
            numericUpDown1 = new NumericUpDown();
            label2 = new Label();
            textBox2 = new TextBox();
            label3 = new Label();
            button1 = new Button();
            checkBox2 = new CheckBox();
            ((System.ComponentModel.ISupportInitialize)numericUpDown1).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(67, 9);
            label1.Name = "label1";
            label1.Size = new Size(29, 19);
            label1.TabIndex = 0;
            label1.Text = "IP :";
            // 
            // textBox1
            // 
            textBox1.Location = new Point(102, 6);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(173, 27);
            textBox1.TabIndex = 1;
            // 
            // checkBox1
            // 
            checkBox1.AutoSize = true;
            checkBox1.Location = new Point(281, 8);
            checkBox1.Name = "checkBox1";
            checkBox1.Size = new Size(60, 23);
            checkBox1.TabIndex = 2;
            checkBox1.Text = "DNS";
            checkBox1.UseVisualStyleBackColor = true;
            // 
            // numericUpDown1
            // 
            numericUpDown1.Location = new Point(416, 7);
            numericUpDown1.Margin = new Padding(4);
            numericUpDown1.Maximum = new decimal(new int[] { 65535, 0, 0, 0 });
            numericUpDown1.Name = "numericUpDown1";
            numericUpDown1.Size = new Size(100, 27);
            numericUpDown1.TabIndex = 3;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(364, 9);
            label2.Name = "label2";
            label2.Size = new Size(45, 19);
            label2.TabIndex = 4;
            label2.Text = "Port :";
            // 
            // textBox2
            // 
            textBox2.Location = new Point(102, 39);
            textBox2.Name = "textBox2";
            textBox2.Size = new Size(173, 27);
            textBox2.TabIndex = 5;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(12, 42);
            label3.Name = "label3";
            label3.Size = new Size(84, 19);
            label3.TabIndex = 6;
            label3.Text = "Password :";
            // 
            // button1
            // 
            button1.Location = new Point(354, 38);
            button1.Name = "button1";
            button1.Size = new Size(162, 28);
            button1.TabIndex = 7;
            button1.Text = "Run into Priviledge";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // checkBox2
            // 
            checkBox2.AutoSize = true;
            checkBox2.Location = new Point(281, 41);
            checkBox2.Name = "checkBox2";
            checkBox2.Size = new Size(67, 23);
            checkBox2.TabIndex = 8;
            checkBox2.Text = "Show";
            checkBox2.UseVisualStyleBackColor = true;
            // 
            // frmConnect
            // 
            AutoScaleDimensions = new SizeF(9F, 19F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(526, 73);
            Controls.Add(checkBox2);
            Controls.Add(button1);
            Controls.Add(label3);
            Controls.Add(textBox2);
            Controls.Add(label2);
            Controls.Add(numericUpDown1);
            Controls.Add(checkBox1);
            Controls.Add(textBox1);
            Controls.Add(label1);
            Font = new Font("Microsoft JhengHei UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            Margin = new Padding(4);
            MaximizeBox = false;
            Name = "frmConnect";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "New Connection";
            Load += frmConnect_Load;
            ((System.ComponentModel.ISupportInitialize)numericUpDown1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private TextBox textBox1;
        private CheckBox checkBox1;
        private NumericUpDown numericUpDown1;
        private Label label2;
        private TextBox textBox2;
        private Label label3;
        private Button button1;
        private CheckBox checkBox2;
    }
}