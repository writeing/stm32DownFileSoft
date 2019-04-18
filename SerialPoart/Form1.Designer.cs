namespace SerialPoart
{
    partial class Form1
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.serialPort1 = new System.IO.Ports.SerialPort(this.components);
            this.cbxCom = new System.Windows.Forms.ComboBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.btnStartWrite = new System.Windows.Forms.Button();
            this.rtbSerial = new System.Windows.Forms.TextBox();
            this.serialPort2 = new System.IO.Ports.SerialPort(this.components);
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.optionOToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.校验ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.退出ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tbdeviceId = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tbDelayout = new System.Windows.Forms.TextBox();
            this.获取设备信息ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // cbxCom
            // 
            this.cbxCom.Font = new System.Drawing.Font("宋体", 13F);
            this.cbxCom.FormattingEnabled = true;
            this.cbxCom.Location = new System.Drawing.Point(5, 28);
            this.cbxCom.Name = "cbxCom";
            this.cbxCom.Size = new System.Drawing.Size(74, 25);
            this.cbxCom.TabIndex = 24;
            this.cbxCom.SelectedIndexChanged += new System.EventHandler(this.cbxCom_SelectedIndexChanged);
            this.cbxCom.Click += new System.EventHandler(this.cbxCom_Click);
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 10;
            // 
            // btnStartWrite
            // 
            this.btnStartWrite.BackColor = System.Drawing.SystemColors.Control;
            this.btnStartWrite.Font = new System.Drawing.Font("宋体", 16F);
            this.btnStartWrite.Location = new System.Drawing.Point(5, 70);
            this.btnStartWrite.Name = "btnStartWrite";
            this.btnStartWrite.Size = new System.Drawing.Size(497, 72);
            this.btnStartWrite.TabIndex = 38;
            this.btnStartWrite.Text = "开始烧写";
            this.btnStartWrite.UseVisualStyleBackColor = false;
            this.btnStartWrite.Click += new System.EventHandler(this.StartEraseFlash_Click);
            // 
            // rtbSerial
            // 
            this.rtbSerial.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.rtbSerial.Font = new System.Drawing.Font("宋体", 9F);
            this.rtbSerial.Location = new System.Drawing.Point(2, 148);
            this.rtbSerial.Multiline = true;
            this.rtbSerial.Name = "rtbSerial";
            this.rtbSerial.ReadOnly = true;
            this.rtbSerial.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.rtbSerial.Size = new System.Drawing.Size(500, 221);
            this.rtbSerial.TabIndex = 4;
            this.rtbSerial.TextChanged += new System.EventHandler(this.SerialFlash_TextChanged);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.optionOToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(507, 25);
            this.menuStrip1.TabIndex = 68;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // optionOToolStripMenuItem
            // 
            this.optionOToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.校验ToolStripMenuItem,
            this.获取设备信息ToolStripMenuItem,
            this.退出ToolStripMenuItem});
            this.optionOToolStripMenuItem.Name = "optionOToolStripMenuItem";
            this.optionOToolStripMenuItem.Size = new System.Drawing.Size(58, 21);
            this.optionOToolStripMenuItem.Text = "option";
            // 
            // 校验ToolStripMenuItem
            // 
            this.校验ToolStripMenuItem.Name = "校验ToolStripMenuItem";
            this.校验ToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.校验ToolStripMenuItem.Text = "校验";
            this.校验ToolStripMenuItem.Click += new System.EventHandler(this.校验ToolStripMenuItem_Click);
            // 
            // 退出ToolStripMenuItem
            // 
            this.退出ToolStripMenuItem.Name = "退出ToolStripMenuItem";
            this.退出ToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.退出ToolStripMenuItem.Text = "退出";
            // 
            // tbdeviceId
            // 
            this.tbdeviceId.Font = new System.Drawing.Font("宋体", 13F);
            this.tbdeviceId.Location = new System.Drawing.Point(158, 28);
            this.tbdeviceId.Name = "tbdeviceId";
            this.tbdeviceId.Size = new System.Drawing.Size(160, 27);
            this.tbdeviceId.TabIndex = 69;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 13F);
            this.label1.Location = new System.Drawing.Point(82, 31);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 18);
            this.label1.TabIndex = 70;
            this.label1.Text = "设备号:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("宋体", 13F);
            this.label2.Location = new System.Drawing.Point(324, 31);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(89, 18);
            this.label2.TabIndex = 72;
            this.label2.Text = "采样时间:";
            // 
            // tbDelayout
            // 
            this.tbDelayout.Font = new System.Drawing.Font("宋体", 13F);
            this.tbDelayout.Location = new System.Drawing.Point(419, 28);
            this.tbDelayout.Name = "tbDelayout";
            this.tbDelayout.Size = new System.Drawing.Size(73, 27);
            this.tbDelayout.TabIndex = 71;
            this.tbDelayout.Text = "60";
            // 
            // 获取设备信息ToolStripMenuItem
            // 
            this.获取设备信息ToolStripMenuItem.Name = "获取设备信息ToolStripMenuItem";
            this.获取设备信息ToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.获取设备信息ToolStripMenuItem.Text = "获取设备信息";
            this.获取设备信息ToolStripMenuItem.Click += new System.EventHandler(this.获取设备信息ToolStripMenuItem_Click);
            // 
            // Form1
            // 
            this.AutoValidate = System.Windows.Forms.AutoValidate.EnableAllowFocusChange;
            this.BackColor = System.Drawing.SystemColors.ControlLight;
            this.ClientSize = new System.Drawing.Size(507, 381);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tbDelayout);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbdeviceId);
            this.Controls.Add(this.rtbSerial);
            this.Controls.Add(this.btnStartWrite);
            this.Controls.Add(this.cbxCom);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Location = new System.Drawing.Point(800, 500);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load_1);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.IO.Ports.SerialPort serialPort1;
        private System.Windows.Forms.Timer timer1;
        public System.Windows.Forms.ComboBox cbxCom;
        private System.Windows.Forms.TextBox rtbSerial;
        private System.IO.Ports.SerialPort serialPort2;
        private System.Windows.Forms.Button btnStartWrite;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem optionOToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 校验ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 退出ToolStripMenuItem;
        private System.Windows.Forms.TextBox tbdeviceId;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbDelayout;
        private System.Windows.Forms.ToolStripMenuItem 获取设备信息ToolStripMenuItem;
    }
}

