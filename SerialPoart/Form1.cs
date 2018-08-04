using DCOM_V1._0;
using Serial_second;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using Newtonsoft.Json;


namespace SerialPoart
{
    public partial class Form1 : Form
    {
        string[] PortNames = SerialPort.GetPortNames();
            
        public delegate void UpdateForm_dl( );//声明委托
        DStringExtend d_StringExtend = new DStringExtend();//串口数据处理
        private programme_serial sp_programme = new programme_serial(); /// 调用串口处理类函数
        private ISPProgramerL071 isp = new ISPProgramerL071(); ///烧写处理函数

        public JsonSave Path = new JsonSave();

        private List<BackgroundWorker> workers = new List<BackgroundWorker>();

        public const int BAUDRATE = 115200;

        bool set_path = false;


        //********************************************************
        List<initListinfo> initLists = new List<initListinfo>();
        string initPath = "init.json";
        public int initIndex = 0;
        private initList g_tempInitlist = new initList();
        public hstoryCombox g_hcom;
        //********************************************************
        ///调用类
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load_1(object sender, EventArgs e)
        {           
            /****************************烧写串口初始化*************************/
            sp_programme.MySerial_SetSerialPort(serialPort2, cbcom, cbproperty, checkfile1, checkfile2, checkfile3, cfilepath1, cfilepath2,
             cfilepath3, download1, download2, download3, StartEraseFlash, SerialFlash, Path); ///类传参数
            sp_programme.MySerialPoart();
            sp_programme.ProgrammeSet();

            //eraseserialPort.DataReceived += new SerialDataReceivedEventHandler(EraseserialPort_DataReceived); //订阅委托 ; 读取数据会慢不采用该模式


            //if (File.Exists(System.Windows.Forms.Application.StartupPath + "\\info.json"))
            //{
            //    JsonSave fp = null;

            //    foreach (string i in File.ReadLines("info.json"))
            //    {
            //        fp = JsonConvert.DeserializeObject<JsonSave>(i);  // 尖括号<>中填入对象的类名    

            //        cfilepath1.Text = fp.fpath[0];
            //        cfilepath2.Text = fp.fpath[1];
            //        cfilepath3.Text = fp.fpath[2];

            //        Path.fpath[0] = fp.fpath[0];
            //        Path.fpath[1] = fp.fpath[1];
            //        Path.fpath[2] = fp.fpath[2];
            //        set_path = true;
            //    }
            //}
            //Thread thr_save = new Thread(new ThreadStart(save_file_process));//创建线程  
            //thr_save.Start();//启动线程  
            initDownFile();
            initconfig();
            initReadJsonFile();
            initComboxHistory();

            this.Width -= 280;
        }
        private void initComboxHistory()
        {
            string downFilePath = "historyCombox.json";
            if (File.Exists(downFilePath) == false)
            {
                MessageBox.Show("还没有存储过数据,已经重新生成");
                FileStream file = File.Open(downFilePath, FileMode.Create);
                g_hcom = new hstoryCombox();
                file.Close();
                return;
            }
            foreach (string line in File.ReadLines(downFilePath))
            {
                try
                {
                    g_hcom = JsonConvert.DeserializeObject<hstoryCombox>(line);
                    foreach (string path in g_hcom.file1)
                    {
                        cfilepath1.Items.Add(path);
                    }
                    foreach (string path in g_hcom.file2)
                    {
                        cfilepath2.Items.Add(path);
                    }
                    foreach (string path in g_hcom.file3)
                    {
                        cfilepath3.Items.Add(path);
                    }
                }
                catch (Exception)
                {
                }
            }
        }
        private void initDownFile()
        {
            string downFilePath = "downFile.json";
            Downinfo downInfo = null;            
            if (File.Exists(downFilePath) == false)
            {
                MessageBox.Show("还没有存储过数据,已经重新生成");
                FileStream file = File.Open(downFilePath, FileMode.Create);
                file.Close();
                return;
            }
            foreach (string line in File.ReadLines(downFilePath))
            {
                try
                {
                    downInfo = JsonConvert.DeserializeObject<Downinfo>(line);
                    checkfile1.Checked = downInfo.checkbox[0];
                    checkfile2.Checked = downInfo.checkbox[1];
                    checkfile3.Checked = downInfo.checkbox[2];

                    cfilepath1.Text = downInfo.filePath[0];
                    cfilepath2.Text = downInfo.filePath[1];
                    cfilepath3.Text = downInfo.filePath[2];

                    initIndex = downInfo.initIndex;
                }
                catch (Exception)
                {
                }
            }
        }
        public void initconfig()
        {
            int x = 30;
            int y = 38;
            for(int i =0; i < 12; i ++)
            {
                CheckBox checkBox3 = new CheckBox();
                checkBox3.Location = new Point(x, y);
                checkBox3.Text = "";
                checkBox3.Size = new Size(17, 17);
                g_tempInitlist.select.Add(checkBox3);

                CheckBox checkBox2 = new CheckBox();
                checkBox2.Location = new Point(x + 58, y);
                checkBox2.Text = "";
                checkBox2.Size = new Size(17, 17);
                g_tempInitlist.hex.Add(checkBox2);

                TextBox textBox3 = new TextBox();
                textBox3.Location = new Point(x + 110, y);
                textBox3.Size = new Size(100, 25);
                g_tempInitlist.address.Add(textBox3);

                TextBox textBox2 = new TextBox();
                textBox2.Location = new Point(x + 220, y);
                textBox2.Size = new Size(133, 25);
                g_tempInitlist.data.Add(textBox2);

                TextBox textBox1 = new TextBox();
                textBox1.Location = new Point(x + 370, y);
                textBox1.Size = new Size(100, 25);
                g_tempInitlist.info.Add(textBox1);
                y += 30;

                panel1.Controls.Add(checkBox3);
                panel1.Controls.Add(checkBox2);
                panel1.Controls.Add(textBox3);
                panel1.Controls.Add(textBox2);
                panel1.Controls.Add(textBox1);

                g_tempInitlist.index.Add(i);
            }
        }

        int StartEraseCount = 0;
        public int _downfileCoun = 0;
        private void StartEraseFlash_Click(object sender, EventArgs e)
        {
            Button bt = (Button)sender;
            bt.BackColor = Color.Blue;
            bt.Text = "下载中";
            isp.UseISPProgramerL071(isp, workers, dataConver(g_tempInitlist), serialPort1, serialPort2, cbcom, cbproperty, SerialFlash, checkfile1, checkfile2, checkfile3, cfilepath1, cfilepath2,
            cfilepath3,richTextBox1);///类传参数
            if (StartEraseCount % 2 == 0)
            {
                //StartEraseCount++;
                DeviceOperation device = new DeviceOperation();

                List<FirmwareInfomation> firmwares = isp.GetDataToWrite(ref device); ///读取文件
                Action<bool> runAction = new Action<bool>((re) =>
                {
                    this.StartEraseFlash.Text = "取消";
                    bt.BackColor = Color.White;
                    StartEraseCount = 0;
                });

                if (firmwares == null)
                {
                    this.SerialFlash.AppendText("请选择烧写文件\r\n");
                    bt.BackColor = Color.Red;
                    return;
                }
                try
                {
                    if (!EraseCheckSerial(BAUDRATE)) ///擦除检验串口
                    {
                        this.SerialFlash.AppendText("串口已经被打开\r\n");
                        bt.BackColor = Color.Red;
                        return;
                    }
                    else
                        this.SerialFlash.AppendText("开始烧写文件\r\n");

                }
                catch (Exception)
                {
                    
                }                
                bool status = false;
                BackgroundWorker worker = isp.WriteData(firmwares, runAction,ref status);                
                if (status == true)
                {
                    bt.BackColor = Color.White;
                    bt.Text = "下载成功";
                    StartEraseCount = 0;
                }
                else
                {
                    bt.Text = "开始下载";
                    bt.BackColor = Color.Red;
                }
                if (_downfileCoun-- > 0)
                {
                    textBox3.Text = _downfileCoun.ToString();
                    StartEraseFlash_Click(StartEraseFlash, null);
                }
                if (worker != null)
                {
                    this.StartEraseFlash.Text = "开始烧写";
                    StartEraseCount = 0;                    
                }
            }
            else if (StartEraseCount % 2 == 1)
            {
                StartEraseCount = 0;
                foreach (var item in workers)
                {
                    if (item.IsBusy)
                    {
                        item.CancelAsync();
                        item.Dispose();
                    }
                }
                this.isp.CancelWrok();
                this.StartEraseFlash.Text = "开始烧写";
            }
        }

        private void ChipInfor_Click(object sender, EventArgs e)
        {
            if (!EraseCheckSerial(BAUDRATE))
            {
                this.SerialFlash.AppendText("串口初始化失败\r\n");
                return;
            }
        }

        private bool EraseCheckSerial(int baudRate)
        {
            var initType = (ISPProgramerL071.InitialType)this.cbproperty.SelectedIndex;
            string portName = this.cbcom.SelectedItem as string;
            this.SerialFlash.AppendText("串口: " + portName + "\r\n");

            bool IspInitState = this.isp.Init(portName, baudRate, initType);

            DeviceOperation temp = new DeviceOperation(); ///打印信息
            this.ShowDeviceInfo(ref temp);
            return IspInitState;
        }

        private void ShowDeviceInfo(ref DeviceOperation device)//显示芯片信息
        {
            device.BootloaderVer = this.isp.BootloaderVer;
            device.ChipType = this.isp.ChipType;
            device.OptionByte = this.isp.OptionByte;
            device.UniqueID = this.isp.UniqueID;
            this.SerialFlash.AppendText("串口" + this.isp.PortName + "打开成功,波特率" + this.isp.PortBaudRate + "\r\n");
            this.SerialFlash.AppendText("芯片类型：" + device.ChipType + "\r\n");
            this.SerialFlash.AppendText("芯片内部bootloader版本号：" + device.BootloaderVer + "\r\n");
            this.SerialFlash.AppendText("芯片FLASH容量为：" + this.isp.ChipSize + "字节\r\n");
            this.SerialFlash.AppendText("设备唯一ID：" + device.UniqueID + "\r\n");
            this.SerialFlash.AppendText("选项字节：" + device.OptionByte + "\r\n");
        }
        private void ReadFlash_Click(object sender, EventArgs e)
        {      
            var initType = (ISPProgramerL071.InitialType)this.cbproperty.SelectedIndex;
            string portName = this.cbcom.SelectedItem as string;
            this.SerialFlash.AppendText("串口: " + portName + "打开\r\n");
          
            if (!this.isp.Init(portName, BAUDRATE, initType))
            {
                this.SerialFlash.AppendText("串口初始化失败\r\n");
                return;
            }
            SaveFileDialog sf = new SaveFileDialog();
            sf.OverwritePrompt = false;
            sf.CreatePrompt = false;
            sf.Filter = ("bin文件|*.bin");
            if (sf.ShowDialog() == DialogResult.OK)
            {

                this.SerialFlash.AppendText("开始读取文件\r\n");
                Action<byte[]> checkData = (data) =>
                {
                    this.isp.Close();
                    if (data != null)
                    {
                        bool result = FilesOperator.SaveBin(sf.FileName, data);
                        this.SerialFlash.AppendText("读取成功，保存" + sf.FileName + (result ? "成功\r\n" : "失败\r\n"));
                    }
                    else
                    {
                        this.SerialFlash.AppendText("读取失败\r\n");
                    }
                };
                isp.ReadData(ISPProgramer.FLASH_BASE_ADDR, this.isp.ChipSize, checkData);//.RunWorkerAsync();
            }
        }

        private void WipeFlash_Click(object sender, EventArgs e)
        {
            if (!EraseCheckSerial(BAUDRATE))
            {
                this.SerialFlash.AppendText("串口初始化失败\r\n");
                return;
            }
            else
                isp.erase_allFash();
        }

        private void Reset_Click(object sender, EventArgs e)
        {
            if (serialPort2.IsOpen)
            {
                serialPort2.DtrEnable = false;
                serialPort2.RtsEnable = true;
                Thread.Sleep(500);

                serialPort2.DtrEnable = true;
                serialPort2.RtsEnable = false;

                SerialFlash.AppendText("Reset\r\n");
            }
            else if (serialPort1.IsOpen)
            {
                serialPort1.DtrEnable = false;
                serialPort1.RtsEnable = true;
                Thread.Sleep(500);

                serialPort1.DtrEnable = true;
                serialPort1.RtsEnable = false;

                
            }
            else
            {
                sp_programme.UseStartFlash();
                serialPort2.DtrEnable = false;
                serialPort2.RtsEnable = true;
                Thread.Sleep(500);

                serialPort2.DtrEnable = true;
                serialPort2.RtsEnable = false;

                SerialFlash.AppendText("Reset\r\n");
            }
        }
        private void SerialFlash_TextChanged(object sender, EventArgs e)
        {
            SerialFlash.MouseDoubleClick += SerialFlash_MouseDoubleClick; ///鼠标双击删除事件

            /********************** 自动下拉到最后 **********************/
            SerialFlash.SelectionStart = SerialFlash.TextLength;

            // Scrolls the contents of the control to the current caret position.
            SerialFlash.ScrollToCaret();
        }

        private void SerialFlash_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            SerialFlash.Clear();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            EraseCheckSerial(BAUDRATE);
            if (isp.erase_allFash() == true)
            {
                SerialFlash.AppendText("擦除成功");
            }          
            else
            {
                SerialFlash.AppendText("擦除失败");
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            string[] ports = System.IO.Ports.SerialPort.GetPortNames();
            if(cbcom.Items.Count != ports.Length)
            {
                try
                {
                    cbcom.Items.Clear();
                    cbcom.Text = "";
                    cbcom.Items.AddRange(ports);
                    cbcom.SelectedIndex = 0;
                }
                catch (Exception)
                {                    
                }

            }
        }

        private initListinfo dataConver(initList init)
        {
            initListinfo info = new initListinfo();
            info.name = init.name;
            for(int i = 0; i < init.hex.Count; i ++)
            {
                info.hex.Add(init.hex[i].Checked);
                info.index.Add(init.index[i]);
                info.select.Add(init.select[i].Checked);
                info.address.Add(init.address[i].Text);
                info.data.Add(init.data[i].Text);
                info.info.Add(init.info[i].Text);
            }
            return info;
        }
        private void DataConverObject(initListinfo info)
        {
            g_tempInitlist.name = info.name;
            System.Text.ASCIIEncoding asciiEncoding = new System.Text.ASCIIEncoding();
            for (int i = 0; i < info.hex.Count; i++)
            {
                
                g_tempInitlist.hex[i].Checked = info.hex[i];
                g_tempInitlist.index[i] = info.index[i];
                g_tempInitlist.select[i].Checked = info.select[i];
                g_tempInitlist.address[i].Text = info.address[i];
                g_tempInitlist.data[i].Text = info.data[i];                
                g_tempInitlist.info[i].Text = info.info[i];
                //g_tempInitlist.info[i].Text = info.info[i];
            }            
        }

        private void initReadJsonFile()
        {
            initListinfo info = null;
            List<string> initNames = new List<string>();
            if (File.Exists(initPath) == false)
            {
                MessageBox.Show("还没有存储过数据,已经重新生成");
                FileStream file = File.Open(initPath, FileMode.Create);
                file.Close();
                return;
            }            
            foreach (string line in File.ReadLines(initPath))
            {
                try
                {
                    info = JsonConvert.DeserializeObject<initListinfo>(line);
                    initNames.Add(info.name);

                    initLists.Add(info);
                    SerialFlash.AppendText(info.name + "\r\n");
                    initNameCombox.Items.Add(info.name);
                }
                catch (Exception)
                {
                    //SerialFlash.AppendText("读取错误了一个" + info.name + "\r\n");
                    initLists.Remove(info);
                }
            }
            try
            {
                initNameCombox.SelectedIndex = initIndex;
                DataConverObject(initLists[initIndex]);
            }
            catch (Exception)
            {
            }         
        }
        private void open_initButton_Click(object sender, EventArgs e)
        {
            // JsonConvert.SerializeObject(one);
            //JsonConvert.DeserializeObject<Student>(str);
            DataConverObject(initLists[initIndex]);
        }
        private void save_initButton_Click(object sender, EventArgs e)
        {
            FileStream file = File.Open(initPath, FileMode.Create);            
            if (initNameCombox.Text == "")
            {
                MessageBox.Show("请填写保存的名字");
                return;
            }
            g_tempInitlist.name = initNameCombox.Text;
            if (initLists.Count == 0)
            {
                initLists.Add(dataConver(g_tempInitlist));
                initIndex = 0;
            }
            else
            {
                initLists[initIndex] = dataConver(g_tempInitlist);
            }            
            foreach (initListinfo info in initLists)
            {
                string strinit = JsonConvert.SerializeObject(info);
                byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(strinit + "\r\n");
                //byte[] byteArray = System.Text.Encoding.Default.GetBytes(strinit+"\r\n");
                file.Write(byteArray,0,byteArray.Length);
            }
            SerialFlash.AppendText("name:"+ initLists[initIndex].name + "had save success"+ "\r\n");
            file.Close();
        }
        private void clean_initButton_Click(object sender, EventArgs e)
        {            
            if(MessageBox.Show("你确定需要删除这条数据?","提示",MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                try
                {
                    initLists.RemoveAt(initIndex);
                    FileStream file = File.Open(initPath, FileMode.Create);
                    foreach (initListinfo info in initLists)
                    {
                        string strinit = JsonConvert.SerializeObject(info);
                        byte[] byteArray = System.Text.Encoding.Default.GetBytes(strinit);
                        file.Write(byteArray, 0, byteArray.Length);
                    }
                    initNameCombox.Items.RemoveAt(initIndex);
                }
                catch (Exception)
                {
                    MessageBox.Show("删除失败");
                    return;
                }
                MessageBox.Show("删除成功");
            }
        }
        private void initNameCombox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cb = (ComboBox)sender;
            initIndex = cb.SelectedIndex;
        }
        public void download3_Click(object sender, EventArgs e)
        {
            sp_programme.UseDownLoad(3);
            g_hcom.file3.Add(cfilepath3.Text);
        }
        private void download2_Click(object sender, EventArgs e)
        {
            sp_programme.UseDownLoad(2);
            g_hcom.file2.Add(cfilepath2.Text);
        }

        private void download1_Click(object sender, EventArgs e)
        {
            sp_programme.UseDownLoad(1);
            g_hcom.file1.Add(cfilepath1.Text);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Downinfo downInfo = new Downinfo();
            downInfo.initIndex = initIndex;
            downInfo.checkbox[0] = checkfile1.Checked;
            downInfo.checkbox[1] = checkfile2.Checked;
            downInfo.checkbox[2] = checkfile3.Checked;

            downInfo.filePath[0] = cfilepath1.Text;
            downInfo.filePath[1] = cfilepath2.Text;
            downInfo.filePath[2] = cfilepath3.Text;

            initPath = "downFile.json";
            FileStream file = File.Open(initPath, FileMode.Create);            
            string strinit = JsonConvert.SerializeObject(downInfo);
            byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(strinit);
            file.Write(byteArray, 0, byteArray.Length);
            file.Close();


            initPath = "historyCombox.json";
            file = File.Open(initPath, FileMode.Create);
            strinit = JsonConvert.SerializeObject(g_hcom);
            byteArray = System.Text.Encoding.UTF8.GetBytes(strinit);
            file.Write(byteArray, 0, byteArray.Length);
            file.Close();

        }
        
        private void newBtn_Click(object sender, EventArgs e)
        {            
            try
            {
                initListinfo temp = new initListinfo();
                initLists.Add(temp);
                initNameCombox.Items.Add("");
                initNameCombox.Text = "";
                initIndex = initNameCombox.Items.Count - 1;
            }
            catch (Exception)
            {                
            }
            
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            string cmd = this.textBox1.Text;
            cmd = cmd + CRC.ToModbusCRC16(cmd);
            this.textBox2.Text = cmd;            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            _downfileCoun = Convert.ToInt32(textBox3.Text);

        }

        private void button4_Click(object sender, EventArgs e)
        {
            int index = 30;
            while (true)
            {
                isp.UseISPProgramerL071(isp, workers, dataConver(g_tempInitlist), serialPort1, serialPort2, cbcom, cbproperty, SerialFlash, checkfile1, checkfile2, checkfile3, cfilepath1, cfilepath2,
                cfilepath3, richTextBox1);///类传参数
                EraseCheckSerial(115200);
                isp.erase_allFash();
                byte[] byteArray = new byte[1024 * 40];
                for (int i = 0; i < byteArray.Length; i++)
                    byteArray[i] = 0x11;
                for (int i = 0; i < 1; i++)
                {
                    if (isp.WriteFlash(0x08000000, byteArray) == true)
                    {
                        richTextBox1.AppendText("write success\r\n");
                    }
                    else
                        richTextBox1.AppendText("write false\r\n");
                }
                if (index-- == 0)
                {
                    break;
                }
                textBox3.Text = index.ToString();
                this.Update();


            }
        }
    }

}

