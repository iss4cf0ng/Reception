namespace Reception
{
    partial class frmPCControl
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
            numericUpDown1 = new NumericUpDown();
            label1 = new Label();
            button3 = new Button();
            button2 = new Button();
            button1 = new Button();
            groupBox2 = new GroupBox();
            button4 = new Button();
            button5 = new Button();
            groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numericUpDown1).BeginInit();
            groupBox2.SuspendLayout();
            SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(numericUpDown1);
            groupBox1.Controls.Add(label1);
            groupBox1.Controls.Add(button3);
            groupBox1.Controls.Add(button2);
            groupBox1.Controls.Add(button1);
            groupBox1.Location = new Point(13, 13);
            groupBox1.Margin = new Padding(4);
            groupBox1.Name = "groupBox1";
            groupBox1.Padding = new Padding(4);
            groupBox1.Size = new Size(332, 135);
            groupBox1.TabIndex = 0;
            groupBox1.TabStop = false;
            groupBox1.Text = "Power";
            // 
            // numericUpDown1
            // 
            numericUpDown1.Location = new Point(247, 88);
            numericUpDown1.Margin = new Padding(4);
            numericUpDown1.Name = "numericUpDown1";
            numericUpDown1.Size = new Size(77, 27);
            numericUpDown1.TabIndex = 4;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(180, 90);
            label1.Name = "label1";
            label1.Size = new Size(68, 19);
            label1.TabIndex = 3;
            label1.Text = "Second :";
            // 
            // button3
            // 
            button3.Location = new Point(16, 77);
            button3.Name = "button3";
            button3.Size = new Size(151, 44);
            button3.TabIndex = 2;
            button3.Text = "Shutdown";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            // 
            // button2
            // 
            button2.Location = new Point(173, 27);
            button2.Name = "button2";
            button2.Size = new Size(151, 44);
            button2.TabIndex = 1;
            button2.Text = "Logout";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // button1
            // 
            button1.Location = new Point(16, 27);
            button1.Name = "button1";
            button1.Size = new Size(151, 44);
            button1.TabIndex = 0;
            button1.Text = "Restart";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(button4);
            groupBox2.Controls.Add(button5);
            groupBox2.Location = new Point(13, 155);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(332, 72);
            groupBox2.TabIndex = 1;
            groupBox2.TabStop = false;
            groupBox2.Text = "CD drive";
            // 
            // button4
            // 
            button4.Location = new Point(16, 19);
            button4.Name = "button4";
            button4.Size = new Size(151, 43);
            button4.TabIndex = 0;
            button4.Text = "Open CD";
            button4.UseVisualStyleBackColor = true;
            // 
            // button5
            // 
            button5.Location = new Point(173, 19);
            button5.Name = "button5";
            button5.Size = new Size(151, 43);
            button5.TabIndex = 2;
            button5.Text = "Close CD";
            button5.UseVisualStyleBackColor = true;
            // 
            // frmPCControl
            // 
            AutoScaleDimensions = new SizeF(9F, 19F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(357, 240);
            Controls.Add(groupBox2);
            Controls.Add(groupBox1);
            Font = new Font("Microsoft JhengHei UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            Margin = new Padding(4);
            Name = "frmPCControl";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "frmPCControl";
            Load += frmPCControl_Load;
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numericUpDown1).EndInit();
            groupBox2.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private GroupBox groupBox1;
        private NumericUpDown numericUpDown1;
        private Label label1;
        private Button button3;
        private Button button2;
        private Button button1;
        private GroupBox groupBox2;
        private Button button4;
        private Button button5;
    }
}