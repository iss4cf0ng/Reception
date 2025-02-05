namespace Reception
{
    partial class frmProcessFind
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
            checkBox2 = new CheckBox();
            button1 = new Button();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 9);
            label1.Name = "label1";
            label1.Size = new Size(66, 19);
            label1.TabIndex = 0;
            label1.Text = "Pattern :";
            // 
            // textBox1
            // 
            textBox1.Location = new Point(84, 6);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(313, 27);
            textBox1.TabIndex = 1;
            // 
            // checkBox1
            // 
            checkBox1.AutoSize = true;
            checkBox1.Location = new Point(32, 52);
            checkBox1.Name = "checkBox1";
            checkBox1.Size = new Size(71, 23);
            checkBox1.TabIndex = 2;
            checkBox1.Text = "Regex";
            checkBox1.UseVisualStyleBackColor = true;
            // 
            // checkBox2
            // 
            checkBox2.AutoSize = true;
            checkBox2.Location = new Point(109, 52);
            checkBox2.Name = "checkBox2";
            checkBox2.Size = new Size(107, 23);
            checkBox2.TabIndex = 3;
            checkBox2.Text = "IgnoreCase";
            checkBox2.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            button1.Location = new Point(234, 39);
            button1.Name = "button1";
            button1.Size = new Size(163, 46);
            button1.TabIndex = 4;
            button1.Text = "Find";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // frmProcessFind
            // 
            AutoScaleDimensions = new SizeF(9F, 19F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(415, 98);
            Controls.Add(button1);
            Controls.Add(checkBox2);
            Controls.Add(checkBox1);
            Controls.Add(textBox1);
            Controls.Add(label1);
            Font = new Font("Microsoft JhengHei UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            Margin = new Padding(4);
            MaximizeBox = false;
            Name = "frmProcessFind";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "frmProcessFind";
            Load += frmProcessFind_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private TextBox textBox1;
        private CheckBox checkBox1;
        private CheckBox checkBox2;
        private Button button1;
    }
}