namespace SCTPClient
{
    partial class Mainform
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.mainMsgBox = new System.Windows.Forms.GroupBox();
            this.recvBox = new System.Windows.Forms.RichTextBox();
            this.sendBox = new System.Windows.Forms.GroupBox();
            this.sendInput = new System.Windows.Forms.TextBox();
            this.mainMsgBox.SuspendLayout();
            this.sendBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainMsgBox
            // 
            this.mainMsgBox.Controls.Add(this.recvBox);
            this.mainMsgBox.Font = new System.Drawing.Font("Cascadia Mono", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mainMsgBox.ForeColor = System.Drawing.Color.White;
            this.mainMsgBox.Location = new System.Drawing.Point(12, 4);
            this.mainMsgBox.Name = "mainMsgBox";
            this.mainMsgBox.Size = new System.Drawing.Size(860, 487);
            this.mainMsgBox.TabIndex = 1;
            this.mainMsgBox.TabStop = false;
            this.mainMsgBox.Text = "Received";
            // 
            // recvBox
            // 
            this.recvBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(16)))), ((int)(((byte)(16)))));
            this.recvBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.recvBox.DetectUrls = false;
            this.recvBox.Font = new System.Drawing.Font("Cascadia Mono", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.recvBox.ForeColor = System.Drawing.Color.White;
            this.recvBox.Location = new System.Drawing.Point(12, 20);
            this.recvBox.Name = "recvBox";
            this.recvBox.ReadOnly = true;
            this.recvBox.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.recvBox.Size = new System.Drawing.Size(847, 465);
            this.recvBox.TabIndex = 0;
            this.recvBox.Text = "";
            this.recvBox.StyleChanged += new System.EventHandler(this.Rezoom);
            // 
            // sendBox
            // 
            this.sendBox.Controls.Add(this.sendInput);
            this.sendBox.Font = new System.Drawing.Font("Cascadia Mono", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.sendBox.ForeColor = System.Drawing.Color.White;
            this.sendBox.Location = new System.Drawing.Point(12, 497);
            this.sendBox.Name = "sendBox";
            this.sendBox.Size = new System.Drawing.Size(860, 52);
            this.sendBox.TabIndex = 2;
            this.sendBox.TabStop = false;
            this.sendBox.Text = "Send";
            // 
            // sendInput
            // 
            this.sendInput.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(16)))), ((int)(((byte)(16)))));
            this.sendInput.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.sendInput.Font = new System.Drawing.Font("Cascadia Mono", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.sendInput.ForeColor = System.Drawing.Color.White;
            this.sendInput.Location = new System.Drawing.Point(12, 24);
            this.sendInput.Multiline = true;
            this.sendInput.Name = "sendInput";
            this.sendInput.Size = new System.Drawing.Size(836, 21);
            this.sendInput.TabIndex = 0;
            this.sendInput.TextChanged += new System.EventHandler(this.InputTextChange);
            this.sendInput.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.OnKeyDown);
            // 
            // Mainform
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(15)))), ((int)(((byte)(15)))));
            this.ClientSize = new System.Drawing.Size(884, 561);
            this.Controls.Add(this.sendBox);
            this.Controls.Add(this.mainMsgBox);
            this.Font = new System.Drawing.Font("微软雅黑", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "Mainform";
            this.Text = "ESCTP Client";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormClose);
            this.Load += new System.EventHandler(this.MainformLoad);
            this.ResizeEnd += new System.EventHandler(this.Resize);
            this.SizeChanged += new System.EventHandler(this.Resize);
            this.mainMsgBox.ResumeLayout(false);
            this.sendBox.ResumeLayout(false);
            this.sendBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        public System.Windows.Forms.RichTextBox recvBox;
        public System.Windows.Forms.TextBox sendInput;
        public System.Windows.Forms.GroupBox mainMsgBox;
        public System.Windows.Forms.GroupBox sendBox;
    }
}

