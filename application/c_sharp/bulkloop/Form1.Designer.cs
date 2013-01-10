namespace BulkLoop
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;


        /* Summary
         The function is used to dispose or clean up all the resources allocated, after the use.
         <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        */
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.StartBtn = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.EptPair2Btn = new System.Windows.Forms.RadioButton();
            this.EptPair1Btn = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.StartValBox = new System.Windows.Forms.TextBox();
            this.IncrWordBtn = new System.Windows.Forms.RadioButton();
            this.IncrByteBtn = new System.Windows.Forms.RadioButton();
            this.RandomByteBtn = new System.Windows.Forms.RadioButton();
            this.ConstByteBtn = new System.Windows.Forms.RadioButton();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.BytesOutLabel = new System.Windows.Forms.Label();
            this.BytesInLabel = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // StartBtn
            // 
            this.StartBtn.BackColor = System.Drawing.Color.Aquamarine;
            this.StartBtn.Location = new System.Drawing.Point(168, 354);
            this.StartBtn.Name = "StartBtn";
            this.StartBtn.Size = new System.Drawing.Size(75, 23);
            this.StartBtn.TabIndex = 0;
            this.StartBtn.Text = "Start";
            this.StartBtn.UseVisualStyleBackColor = false;
            this.StartBtn.Click += new System.EventHandler(this.StartBtn_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.EptPair2Btn);
            this.groupBox1.Controls.Add(this.EptPair1Btn);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(258, 67);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = " Endpoint Pair (Out / In) ";
            // 
            // EptPair2Btn
            // 
            this.EptPair2Btn.AutoSize = true;
            this.EptPair2Btn.Enabled = false;
            this.EptPair2Btn.Location = new System.Drawing.Point(149, 29);
            this.EptPair2Btn.Name = "EptPair2Btn";
            this.EptPair2Btn.Size = new System.Drawing.Size(82, 17);
            this.EptPair2Btn.TabIndex = 1;
            this.EptPair2Btn.Text = "0x04 / 0x88";
            this.EptPair2Btn.UseVisualStyleBackColor = true;
            this.EptPair2Btn.Click += new System.EventHandler(this.EptPair1Btn_Click);
            // 
            // EptPair1Btn
            // 
            this.EptPair1Btn.AutoSize = true;
            this.EptPair1Btn.Checked = true;
            this.EptPair1Btn.Location = new System.Drawing.Point(25, 29);
            this.EptPair1Btn.Name = "EptPair1Btn";
            this.EptPair1Btn.Size = new System.Drawing.Size(82, 17);
            this.EptPair1Btn.TabIndex = 0;
            this.EptPair1Btn.TabStop = true;
            this.EptPair1Btn.Text = "0x01 / 0x81";
            this.EptPair1Btn.UseVisualStyleBackColor = true;
            this.EptPair1Btn.Click += new System.EventHandler(this.EptPair1Btn_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.StartValBox);
            this.groupBox2.Controls.Add(this.IncrWordBtn);
            this.groupBox2.Controls.Add(this.IncrByteBtn);
            this.groupBox2.Controls.Add(this.RandomByteBtn);
            this.groupBox2.Controls.Add(this.ConstByteBtn);
            this.groupBox2.Location = new System.Drawing.Point(12, 96);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(258, 157);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = " Data Pattern ";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(114, 130);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Start Value";
            // 
            // StartValBox
            // 
            this.StartValBox.Location = new System.Drawing.Point(179, 127);
            this.StartValBox.Name = "StartValBox";
            this.StartValBox.Size = new System.Drawing.Size(52, 20);
            this.StartValBox.TabIndex = 4;
            this.StartValBox.Text = "10";
            this.StartValBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.StartValBox.TextChanged += new System.EventHandler(this.StartValBox_TextChanged);
            // 
            // IncrWordBtn
            // 
            this.IncrWordBtn.AutoSize = true;
            this.IncrWordBtn.Location = new System.Drawing.Point(24, 92);
            this.IncrWordBtn.Name = "IncrWordBtn";
            this.IncrWordBtn.Size = new System.Drawing.Size(113, 17);
            this.IncrWordBtn.TabIndex = 3;
            this.IncrWordBtn.Text = "Incrementing Int32";
            this.IncrWordBtn.UseVisualStyleBackColor = true;
            // 
            // IncrByteBtn
            // 
            this.IncrByteBtn.AutoSize = true;
            this.IncrByteBtn.Location = new System.Drawing.Point(24, 69);
            this.IncrByteBtn.Name = "IncrByteBtn";
            this.IncrByteBtn.Size = new System.Drawing.Size(110, 17);
            this.IncrByteBtn.TabIndex = 2;
            this.IncrByteBtn.Text = "Incrementing Byte";
            this.IncrByteBtn.UseVisualStyleBackColor = true;
            // 
            // RandomByteBtn
            // 
            this.RandomByteBtn.AutoSize = true;
            this.RandomByteBtn.Location = new System.Drawing.Point(24, 46);
            this.RandomByteBtn.Name = "RandomByteBtn";
            this.RandomByteBtn.Size = new System.Drawing.Size(88, 17);
            this.RandomByteBtn.TabIndex = 1;
            this.RandomByteBtn.Text = "Random byte";
            this.RandomByteBtn.UseVisualStyleBackColor = true;
            // 
            // ConstByteBtn
            // 
            this.ConstByteBtn.AutoSize = true;
            this.ConstByteBtn.Checked = true;
            this.ConstByteBtn.Location = new System.Drawing.Point(24, 23);
            this.ConstByteBtn.Name = "ConstByteBtn";
            this.ConstByteBtn.Size = new System.Drawing.Size(90, 17);
            this.ConstByteBtn.TabIndex = 0;
            this.ConstByteBtn.TabStop = true;
            this.ConstByteBtn.Text = "Constant byte";
            this.ConstByteBtn.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(24, 276);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(112, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Bytes transferred OUT";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(36, 305);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(100, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Bytes transferred IN";
            // 
            // BytesOutLabel
            // 
            this.BytesOutLabel.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.BytesOutLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.BytesOutLabel.Location = new System.Drawing.Point(161, 270);
            this.BytesOutLabel.Name = "BytesOutLabel";
            this.BytesOutLabel.Size = new System.Drawing.Size(82, 19);
            this.BytesOutLabel.TabIndex = 5;
            this.BytesOutLabel.Text = "0";
            this.BytesOutLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // BytesInLabel
            // 
            this.BytesInLabel.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.BytesInLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.BytesInLabel.Location = new System.Drawing.Point(161, 299);
            this.BytesInLabel.Name = "BytesInLabel";
            this.BytesInLabel.Size = new System.Drawing.Size(82, 19);
            this.BytesInLabel.TabIndex = 6;
            this.BytesInLabel.Text = "0";
            this.BytesInLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(288, 397);
            this.Controls.Add(this.BytesInLabel);
            this.Controls.Add(this.BytesOutLabel);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.StartBtn);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "C# BulkLoop";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button StartBtn;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton EptPair2Btn;
        private System.Windows.Forms.RadioButton EptPair1Btn;
        private System.Windows.Forms.RadioButton IncrWordBtn;
        private System.Windows.Forms.RadioButton IncrByteBtn;
        private System.Windows.Forms.RadioButton RandomByteBtn;
        private System.Windows.Forms.RadioButton ConstByteBtn;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox StartValBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label BytesOutLabel;
        private System.Windows.Forms.Label BytesInLabel;
    }
}

