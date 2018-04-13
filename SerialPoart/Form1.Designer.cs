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
            this.button2 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.serialPort1 = new System.IO.Ports.SerialPort(this.components);
            this.cbproperty = new System.Windows.Forms.ComboBox();
            this.cbcom = new System.Windows.Forms.ComboBox();
            this.cfilepath1 = new System.Windows.Forms.ComboBox();
            this.checkfile1 = new System.Windows.Forms.CheckBox();
            this.checkfile2 = new System.Windows.Forms.CheckBox();
            this.cfilepath2 = new System.Windows.Forms.ComboBox();
            this.checkfile3 = new System.Windows.Forms.CheckBox();
            this.cfilepath3 = new System.Windows.Forms.ComboBox();
            this.download2 = new System.Windows.Forms.Button();
            this.download1 = new System.Windows.Forms.Button();
            this.download3 = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.StartEraseFlash = new System.Windows.Forms.Button();
            this.SerialFlash = new System.Windows.Forms.TextBox();
            this.ChipInfor = new System.Windows.Forms.Button();
            this.WipeFlash = new System.Windows.Forms.Button();
            this.ReadFlash = new System.Windows.Forms.Button();
            this.Reset = new System.Windows.Forms.Button();
            this.serialPort2 = new System.IO.Ports.SerialPort(this.components);
            this.panel1 = new System.Windows.Forms.Panel();
            this.save_initButton = new System.Windows.Forms.Button();
            this.clean_initButton = new System.Windows.Forms.Button();
            this.open_initButton = new System.Windows.Forms.Button();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.checkBox3 = new System.Windows.Forms.CheckBox();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(0, 0);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 58;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 23);
            this.label1.TabIndex = 56;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(0, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(100, 23);
            this.label2.TabIndex = 55;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(0, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(100, 23);
            this.label3.TabIndex = 54;
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(0, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(100, 23);
            this.label4.TabIndex = 52;
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(0, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(100, 23);
            this.label5.TabIndex = 50;
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(0, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(100, 23);
            this.label6.TabIndex = 48;
            // 
            // cbproperty
            // 
            this.cbproperty.FormattingEnabled = true;
            this.cbproperty.Location = new System.Drawing.Point(82, 12);
            this.cbproperty.Name = "cbproperty";
            this.cbproperty.Size = new System.Drawing.Size(420, 23);
            this.cbproperty.TabIndex = 23;
            // 
            // cbcom
            // 
            this.cbcom.FormattingEnabled = true;
            this.cbcom.Location = new System.Drawing.Point(2, 12);
            this.cbcom.Name = "cbcom";
            this.cbcom.Size = new System.Drawing.Size(74, 23);
            this.cbcom.TabIndex = 24;
            this.cbcom.DropDown += new System.EventHandler(this.cbcom_DropDown);
            // 
            // cfilepath1
            // 
            this.cfilepath1.FormattingEnabled = true;
            this.cfilepath1.Location = new System.Drawing.Point(26, 52);
            this.cfilepath1.Name = "cfilepath1";
            this.cfilepath1.Size = new System.Drawing.Size(413, 23);
            this.cfilepath1.TabIndex = 26;
            // 
            // checkfile1
            // 
            this.checkfile1.AllowDrop = true;
            this.checkfile1.AutoSize = true;
            this.checkfile1.Location = new System.Drawing.Point(2, 58);
            this.checkfile1.Name = "checkfile1";
            this.checkfile1.Size = new System.Drawing.Size(18, 17);
            this.checkfile1.TabIndex = 27;
            this.checkfile1.UseVisualStyleBackColor = true;
            // 
            // checkfile2
            // 
            this.checkfile2.AllowDrop = true;
            this.checkfile2.AutoSize = true;
            this.checkfile2.Location = new System.Drawing.Point(2, 96);
            this.checkfile2.Name = "checkfile2";
            this.checkfile2.Size = new System.Drawing.Size(18, 17);
            this.checkfile2.TabIndex = 29;
            this.checkfile2.UseVisualStyleBackColor = true;
            // 
            // cfilepath2
            // 
            this.cfilepath2.FormattingEnabled = true;
            this.cfilepath2.Location = new System.Drawing.Point(26, 90);
            this.cfilepath2.Name = "cfilepath2";
            this.cfilepath2.Size = new System.Drawing.Size(413, 23);
            this.cfilepath2.TabIndex = 28;
            // 
            // checkfile3
            // 
            this.checkfile3.AllowDrop = true;
            this.checkfile3.AutoSize = true;
            this.checkfile3.Location = new System.Drawing.Point(2, 136);
            this.checkfile3.Name = "checkfile3";
            this.checkfile3.Size = new System.Drawing.Size(18, 17);
            this.checkfile3.TabIndex = 31;
            this.checkfile3.UseVisualStyleBackColor = true;
            // 
            // cfilepath3
            // 
            this.cfilepath3.FormattingEnabled = true;
            this.cfilepath3.Location = new System.Drawing.Point(26, 130);
            this.cfilepath3.Name = "cfilepath3";
            this.cfilepath3.Size = new System.Drawing.Size(413, 23);
            this.cfilepath3.TabIndex = 30;
            // 
            // download2
            // 
            this.download2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.download2.Location = new System.Drawing.Point(445, 87);
            this.download2.Name = "download2";
            this.download2.Size = new System.Drawing.Size(57, 26);
            this.download2.TabIndex = 33;
            this.download2.Text = "...";
            this.download2.UseVisualStyleBackColor = true;
            // 
            // download1
            // 
            this.download1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.download1.Location = new System.Drawing.Point(445, 49);
            this.download1.Name = "download1";
            this.download1.Size = new System.Drawing.Size(57, 26);
            this.download1.TabIndex = 34;
            this.download1.Text = "...";
            this.download1.UseVisualStyleBackColor = true;
            // 
            // download3
            // 
            this.download3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.download3.Location = new System.Drawing.Point(445, 127);
            this.download3.Name = "download3";
            this.download3.Size = new System.Drawing.Size(57, 26);
            this.download3.TabIndex = 35;
            this.download3.Text = "...";
            this.download3.UseVisualStyleBackColor = true;
            this.download3.Click += new System.EventHandler(this.download3_Click);
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 10;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // StartEraseFlash
            // 
            this.StartEraseFlash.Location = new System.Drawing.Point(3, 162);
            this.StartEraseFlash.Name = "StartEraseFlash";
            this.StartEraseFlash.Size = new System.Drawing.Size(239, 72);
            this.StartEraseFlash.TabIndex = 38;
            this.StartEraseFlash.Text = "开始烧写";
            this.StartEraseFlash.UseVisualStyleBackColor = true;
            this.StartEraseFlash.Click += new System.EventHandler(this.StartEraseFlash_Click);
            // 
            // SerialFlash
            // 
            this.SerialFlash.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.SerialFlash.Location = new System.Drawing.Point(3, 267);
            this.SerialFlash.Multiline = true;
            this.SerialFlash.Name = "SerialFlash";
            this.SerialFlash.ReadOnly = true;
            this.SerialFlash.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.SerialFlash.Size = new System.Drawing.Size(500, 196);
            this.SerialFlash.TabIndex = 4;
            this.SerialFlash.TextChanged += new System.EventHandler(this.SerialFlash_TextChanged);
            // 
            // ChipInfor
            // 
            this.ChipInfor.Location = new System.Drawing.Point(248, 162);
            this.ChipInfor.Name = "ChipInfor";
            this.ChipInfor.Size = new System.Drawing.Size(125, 33);
            this.ChipInfor.TabIndex = 40;
            this.ChipInfor.Text = "芯片信息";
            this.ChipInfor.UseVisualStyleBackColor = true;
            this.ChipInfor.Click += new System.EventHandler(this.ChipInfor_Click);
            // 
            // WipeFlash
            // 
            this.WipeFlash.Location = new System.Drawing.Point(248, 199);
            this.WipeFlash.Name = "WipeFlash";
            this.WipeFlash.Size = new System.Drawing.Size(125, 33);
            this.WipeFlash.TabIndex = 42;
            this.WipeFlash.Text = "擦除芯片";
            this.WipeFlash.UseVisualStyleBackColor = true;
            this.WipeFlash.Click += new System.EventHandler(this.WipeFlash_Click);
            // 
            // ReadFlash
            // 
            this.ReadFlash.Location = new System.Drawing.Point(383, 162);
            this.ReadFlash.Name = "ReadFlash";
            this.ReadFlash.Size = new System.Drawing.Size(120, 33);
            this.ReadFlash.TabIndex = 43;
            this.ReadFlash.Text = "读取flash";
            this.ReadFlash.UseVisualStyleBackColor = true;
            this.ReadFlash.Click += new System.EventHandler(this.ReadFlash_Click);
            // 
            // Reset
            // 
            this.Reset.Location = new System.Drawing.Point(383, 199);
            this.Reset.Name = "Reset";
            this.Reset.Size = new System.Drawing.Size(120, 33);
            this.Reset.TabIndex = 44;
            this.Reset.Text = "复位";
            this.Reset.UseVisualStyleBackColor = true;
            this.Reset.Click += new System.EventHandler(this.Reset_Click);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.Control;
            this.panel1.Controls.Add(this.save_initButton);
            this.panel1.Controls.Add(this.clean_initButton);
            this.panel1.Controls.Add(this.open_initButton);
            this.panel1.Controls.Add(this.textBox3);
            this.panel1.Controls.Add(this.textBox2);
            this.panel1.Controls.Add(this.textBox1);
            this.panel1.Controls.Add(this.checkBox3);
            this.panel1.Controls.Add(this.checkBox2);
            this.panel1.Controls.Add(this.label11);
            this.panel1.Controls.Add(this.label10);
            this.panel1.Controls.Add(this.label9);
            this.panel1.Controls.Add(this.label8);
            this.panel1.Controls.Add(this.label7);
            this.panel1.Location = new System.Drawing.Point(509, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(522, 451);
            this.panel1.TabIndex = 59;
            // 
            // save_initButton
            // 
            this.save_initButton.Location = new System.Drawing.Point(405, 405);
            this.save_initButton.Name = "save_initButton";
            this.save_initButton.Size = new System.Drawing.Size(100, 34);
            this.save_initButton.TabIndex = 68;
            this.save_initButton.Text = "保存";
            this.save_initButton.UseVisualStyleBackColor = true;
            this.save_initButton.Click += new System.EventHandler(this.save_initButton_Click);
            // 
            // clean_initButton
            // 
            this.clean_initButton.Location = new System.Drawing.Point(262, 404);
            this.clean_initButton.Name = "clean_initButton";
            this.clean_initButton.Size = new System.Drawing.Size(112, 34);
            this.clean_initButton.TabIndex = 67;
            this.clean_initButton.Text = "清空";
            this.clean_initButton.UseVisualStyleBackColor = true;
            this.clean_initButton.Click += new System.EventHandler(this.clean_initButton_Click);
            // 
            // open_initButton
            // 
            this.open_initButton.Location = new System.Drawing.Point(139, 403);
            this.open_initButton.Name = "open_initButton";
            this.open_initButton.Size = new System.Drawing.Size(100, 36);
            this.open_initButton.TabIndex = 66;
            this.open_initButton.Text = "打开";
            this.open_initButton.UseVisualStyleBackColor = true;
            this.open_initButton.Click += new System.EventHandler(this.open_initButton_Click);
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(139, 35);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(100, 25);
            this.textBox3.TabIndex = 10;
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(254, 35);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(133, 25);
            this.textBox2.TabIndex = 9;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(405, 35);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(100, 25);
            this.textBox1.TabIndex = 8;
            // 
            // checkBox3
            // 
            this.checkBox3.AutoSize = true;
            this.checkBox3.Location = new System.Drawing.Point(29, 38);
            this.checkBox3.Name = "checkBox3";
            this.checkBox3.Size = new System.Drawing.Size(18, 17);
            this.checkBox3.TabIndex = 7;
            this.checkBox3.UseVisualStyleBackColor = true;
            // 
            // checkBox2
            // 
            this.checkBox2.AutoSize = true;
            this.checkBox2.Location = new System.Drawing.Point(88, 38);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(18, 17);
            this.checkBox2.TabIndex = 6;
            this.checkBox2.UseVisualStyleBackColor = true;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(163, 17);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(37, 15);
            this.label11.TabIndex = 4;
            this.label11.Text = "地址";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(19, 17);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(37, 15);
            this.label10.TabIndex = 3;
            this.label10.Text = "选中";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(436, 8);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(37, 15);
            this.label9.TabIndex = 2;
            this.label9.Text = "说明";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(259, 17);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(52, 15);
            this.label8.TabIndex = 1;
            this.label8.Text = "字符串";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(85, 17);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(31, 15);
            this.label7.TabIndex = 0;
            this.label7.Text = "hex";
            // 
            // Form1
            // 
            this.AutoValidate = System.Windows.Forms.AutoValidate.EnableAllowFocusChange;
            this.BackColor = System.Drawing.SystemColors.ControlLight;
            this.ClientSize = new System.Drawing.Size(1040, 475);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.Reset);
            this.Controls.Add(this.ReadFlash);
            this.Controls.Add(this.WipeFlash);
            this.Controls.Add(this.ChipInfor);
            this.Controls.Add(this.SerialFlash);
            this.Controls.Add(this.StartEraseFlash);
            this.Controls.Add(this.download3);
            this.Controls.Add(this.download1);
            this.Controls.Add(this.download2);
            this.Controls.Add(this.checkfile3);
            this.Controls.Add(this.cfilepath3);
            this.Controls.Add(this.checkfile2);
            this.Controls.Add(this.cfilepath2);
            this.Controls.Add(this.checkfile1);
            this.Controls.Add(this.cfilepath1);
            this.Controls.Add(this.cbcom);
            this.Controls.Add(this.cbproperty);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button2);
            this.Name = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load_1);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.IO.Ports.SerialPort serialPort1;
        private System.Windows.Forms.Timer timer1;
        public System.Windows.Forms.ComboBox cbproperty;
        public System.Windows.Forms.ComboBox cbcom;
        public System.Windows.Forms.CheckBox checkfile1;
        public System.Windows.Forms.ComboBox cfilepath1;
        public System.Windows.Forms.CheckBox checkfile2;
        public System.Windows.Forms.ComboBox cfilepath2;
        public System.Windows.Forms.CheckBox checkfile3;
        public System.Windows.Forms.ComboBox cfilepath3;
        public System.Windows.Forms.Button download2;
        public System.Windows.Forms.Button download1;
        public System.Windows.Forms.Button download3;
        private System.Windows.Forms.TextBox SerialFlash;
        private System.Windows.Forms.Button ChipInfor;
        private System.Windows.Forms.Button WipeFlash;
        private System.Windows.Forms.Button ReadFlash;
        private System.Windows.Forms.Button Reset;
        private System.IO.Ports.SerialPort serialPort2;
        private System.Windows.Forms.Button StartEraseFlash;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.CheckBox checkBox3;
        private System.Windows.Forms.CheckBox checkBox2;
        private System.Windows.Forms.Button save_initButton;
        private System.Windows.Forms.Button clean_initButton;
        private System.Windows.Forms.Button open_initButton;
    }
}

