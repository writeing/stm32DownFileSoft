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
using System.Diagnostics;

namespace SerialPoart
{
    public partial class Form1 : Form
    {
        string[] PortNames = SerialPort.GetPortNames();

        public delegate void UpdateForm_dl();//声明委托
        DStringExtend d_StringExtend = new DStringExtend();//串口数据处理
        private programme_serial sp_programme = new programme_serial(); /// 调用串口处理类函数
        private ISPProgramerL071 isp = new ISPProgramerL071(); ///烧写处理函数

        public JsonSave Path = new JsonSave();

        private List<BackgroundWorker> workers = new List<BackgroundWorker>();

        public const int BAUDRATE = 115200;

        bool set_path = false;

        public bool g_DownFileMode = true;    //true usart  false stlink

        //********************************************************
        List<initListinfo> initLists = new List<initListinfo>();
        string initPath = "init.json";
        public int initIndex = 0;
        private initList g_tempInitlist = new initList();
        public hstoryCombox g_hcom;
        //********************************************************

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load_1(object sender, EventArgs e)
        {
            cbxCom_Click(this, null);
            cbxCom.SelectedIndex = 0;



            this.Text = "NBI Pro气象站下载器V1.0";            
        }



        int StartEraseCount = 0;
        public int _downfileCoun = 0;
        private void usartDownFile(object Object)
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            Button bt = btnStartWrite;
            List<FirmwareInfomation> firmwares = (List<FirmwareInfomation>)Object;
            try
            {
                if (!EraseCheckSerial(BAUDRATE)) ///擦除检验串口
                {
                    this.rtbSerial.AppendText("串口已经被打开\r\n");
                    bt.BackColor = Color.Red;
                    return;
                }
                else
                    this.rtbSerial.AppendText("开始烧写文件\r\n");

            }
            catch (Exception)
            {
                rtbSerial.Text = "串口打开失败\n";
                return;
            }
            bool status = false;
            BackgroundWorker worker = isp.WriteData(firmwares, ref status, true);
            if (status == true)
            {
                bt.BackColor = Color.Blue;
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
                StartEraseFlash_Click(btnStartWrite, null);
            }
            if (worker != null)
            {
                this.btnStartWrite.Text = "开始烧写";
                StartEraseCount = 0;
            }
        }
        private int[] converData(byte[] data)
        {
            int[] temp = new int[20];
            int j = 0;
            byte[] cc = new byte[100];
            Array.Clear(cc, 0, 100);

            Array.Copy(data, cc, data.Length);
            for (int i = 0; i < data.Length; i += 4)
            {

                temp[j++] = cc[i] << (8 * 3) | cc[i + 1] << (8 * 2) | cc[i + 2] << (8 * 1) | cc[i + 3];
            }
            return temp;
        }
        private void stLinkDownFile(object Object)
        {
            ProcessStartInfo si = new ProcessStartInfo(@"ST-LINK_CLI.exe");
            si.WindowStyle = ProcessWindowStyle.Hidden;
            si.CreateNoWindow = true;
            si.UseShellExecute = false;
            si.RedirectStandardOutput = true;

            //erase all flash
            si.Arguments = "-c -me";
            Process po = Process.Start(si);
            po.OutputDataReceived += new DataReceivedEventHandler(Po_OutputDataReceived);
            po.BeginOutputReadLine();
            po.WaitForExit();

            DeviceOperation device = new DeviceOperation();

            List<FirmwareInfomation> firmwares = isp.GetDataToWrite(ref device,g_curDeviceId,g_curDelayOut); ///读取文件
            string args = "";
            int probarValue = 100 / firmwares.Count;
            foreach (FirmwareInfomation ff in firmwares)
            {
                if (ff.Name == "data")
                {
                    //set data
                    int[] dd = converData(ff.Data);
                    for (int i = 0; dd[i] != 0; i++)
                    {
                        string aa = Convert.ToString(dd[i], 16);
                        args += " -w32 " + (ff.BaseAddress + i).ToString("X") + " " + aa;
                    }
                    si.Arguments = args;
                    po = Process.Start(si);
                    po.OutputDataReceived += new DataReceivedEventHandler(Po_OutputDataReceived);
                    po.BeginOutputReadLine();
                    po.WaitForExit();
                    args = "";
                }
            }
            args = "";
            foreach (FirmwareInfomation ff in firmwares)
            {
                //down file
                if (ff.Name != "data")
                {
                    args += string.Format(" -p {0} {1} ", ff.Name, ff.BaseAddress.ToString("X"));
                }
            }
            // SerialFlash.AppendText(args);
            si.Arguments = args + " -rst";
            po = Process.Start(si);
            po.OutputDataReceived += new DataReceivedEventHandler(Po_OutputDataReceived);
            po.BeginOutputReadLine();
            po.WaitForExit();
        }
        private void Po_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            if (e.Data != null && (e.Data.IndexOf("erased") > -1 || e.Data.IndexOf("Complete") > -1 || e.Data.IndexOf("Reset") > -1 || e.Data.IndexOf("Connected") > -1))
            {
                if (e.Data.IndexOf("Reset") > -1)
                {
                    btnStartWrite.BackColor = Color.Blue;
                    btnStartWrite.Text = "下载成功";
                }
            }
            if (e.Data != null && e.Data.IndexOf("No target connected") > -1)
            {
                btnStartWrite.BackColor = Color.Red;
                btnStartWrite.Text = "下载失败";
            }
        }
        string g_curDeviceId;
        int g_curDelayOut;
        private bool checkInputData()
        {
            try
            {
                g_curDeviceId = tbdeviceId.Text;
                g_curDelayOut = Convert.ToInt32(tbDelayout.Text);
                if (g_curDeviceId.Length != 16 || g_curDeviceId[1] != '7')
                {
                    return false;
                }
                if(g_curDelayOut < 0 || g_curDelayOut > 1800)
                {
                    return false;
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
            
               
        }
        private void StartEraseFlash_Click(object sender, EventArgs e)
        {
            Button bt = (Button)sender;
            bt.BackColor = Color.GreenYellow;
            bt.Text = "下载中";
            if (!checkInputData())
            {
                MessageBox.Show("输入数据格式不正确");
                return;
            }
            // send serial object and port name ,rich
            isp.UseISPProgramerL071(serialPort1, cbxCom.Text, rtbSerial); 

            if (g_DownFileMode == true)
            {              
                // uart down  
                DeviceOperation device = new DeviceOperation();
                // get write file 
                List<FirmwareInfomation> firmwares = isp.GetDataToWrite(ref device,g_curDeviceId,g_curDelayOut); ///读取文件
                Action<bool> runAction = new Action<bool>((re) =>
                {
                    this.btnStartWrite.Text = "取消";
                    bt.BackColor = Color.White;
                    StartEraseCount = 0;
                });

                if (firmwares == null)
                {
                    this.rtbSerial.AppendText("请选择烧写文件\r\n");
                    bt.BackColor = Color.Red;
                    return;
                }
                // begin start down file to device
                Thread oneThread = new Thread(new ParameterizedThreadStart(usartDownFile));
                oneThread.Start(firmwares); //启动线程                                   
            }
            else
            {
                Thread oneThread = new Thread(new ParameterizedThreadStart(stLinkDownFile));
                oneThread.Start(); //启动线程  
            }

               
        }

        private void ChipInfor_Click(object sender, EventArgs e)
        {
            if (!EraseCheckSerial(BAUDRATE))
            {
                this.rtbSerial.AppendText("串口初始化失败\r\n");
                return;
            }
        }

        private bool EraseCheckSerial(int baudRate)
        {
            int index = 3;
            var initType = (ISPProgramerL071.InitialType)index;
            string portName = this.cbxCom.SelectedItem as string;
            this.rtbSerial.AppendText("串口: " + portName + "\r\n");

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
            this.rtbSerial.AppendText("串口" + this.isp.PortName + "打开成功,波特率" + this.isp.PortBaudRate + "\r\n");
            this.rtbSerial.AppendText("芯片类型：" + device.ChipType + "\r\n");
            this.rtbSerial.AppendText("芯片内部bootloader版本号：" + device.BootloaderVer + "\r\n");
            this.rtbSerial.AppendText("芯片FLASH容量为：" + this.isp.ChipSize + "字节\r\n");
            this.rtbSerial.AppendText("设备唯一ID：" + device.UniqueID + "\r\n");
            this.rtbSerial.AppendText("选项字节：" + device.OptionByte + "\r\n");
        }
        private void ReadFlash_Click(object sender, EventArgs e)
        {
            int index = 3;
            var initType = (ISPProgramerL071.InitialType)index;
            string portName = this.cbxCom.SelectedItem as string;
            this.rtbSerial.AppendText("串口: " + portName + "打开\r\n");
          
            if (!this.isp.Init(portName, BAUDRATE, initType))
            {
                this.rtbSerial.AppendText("串口初始化失败\r\n");
                return;
            }
            SaveFileDialog sf = new SaveFileDialog();
            sf.OverwritePrompt = false;
            sf.CreatePrompt = false;
            sf.Filter = ("bin文件|*.bin");
            if (sf.ShowDialog() == DialogResult.OK)
            {

                this.rtbSerial.AppendText("开始读取文件\r\n");
                Action<byte[]> checkData = (data) =>
                {
                    this.isp.Close();
                    if (data != null)
                    {
                        bool result = FilesOperator.SaveBin(sf.FileName, data);
                        this.rtbSerial.AppendText("读取成功，保存" + sf.FileName + (result ? "成功\r\n" : "失败\r\n"));
                    }
                    else
                    {
                        this.rtbSerial.AppendText("读取失败\r\n");
                    }
                };
                isp.ReadData(ISPProgramer.FLASH_BASE_ADDR, this.isp.ChipSize, checkData);//.RunWorkerAsync();
            }
        }

        private void WipeFlash_Click(object sender, EventArgs e)
        {
            if (!EraseCheckSerial(BAUDRATE))
            {
                this.rtbSerial.AppendText("串口初始化失败\r\n");
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

                rtbSerial.AppendText("Reset\r\n");
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

                rtbSerial.AppendText("Reset\r\n");
            }
        }
        private void SerialFlash_TextChanged(object sender, EventArgs e)
        {
            rtbSerial.MouseDoubleClick += SerialFlash_MouseDoubleClick; ///鼠标双击删除事件

            /********************** 自动下拉到最后 **********************/
            rtbSerial.SelectionStart = rtbSerial.TextLength;

            // Scrolls the contents of the control to the current caret position.
            rtbSerial.ScrollToCaret();
        }

        private void SerialFlash_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            rtbSerial.Clear();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            EraseCheckSerial(BAUDRATE);
            if (isp.erase_allFash() == true)
            {
                rtbSerial.AppendText("擦除成功");
            }          
            else
            {
                rtbSerial.AppendText("擦除失败");
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

       
        private void open_initButton_Click(object sender, EventArgs e)
        {
            // JsonConvert.SerializeObject(one);
            //JsonConvert.DeserializeObject<Student>(str);
            DataConverObject(initLists[initIndex]);
        }
     
        private void initNameCombox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cb = (ComboBox)sender;
            initIndex = cb.SelectedIndex;
        }
       

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
        }

        private void 校验ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Crc crc = new Crc();
            crc.Show();
        }

        private void cbxCom_Click(object sender, EventArgs e)
        {
            string[] serNames = SerialPort.GetPortNames();
            cbxCom.Items.Clear();
            cbxCom.Items.AddRange(serNames);            
        }

        private void cbxCom_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        //read device flash data and show data
        private void 获取设备信息ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            isp.UseISPProgramerL071(serialPort1, cbxCom.Text, rtbSerial);

            if (!EraseCheckSerial(BAUDRATE)) ///擦除检验串口
            {
                this.rtbSerial.AppendText("串口已经被打开\r\n");
                btnStartWrite.BackColor = Color.Red;
                return;
            }
            else
                this.rtbSerial.AppendText("开始读取设备信息\r\n");
            // 获取flash的信息
            List<string> ll = isp.readDeviceinfo();
            foreach(string data in ll)
            {
                this.rtbSerial.AppendText(data);
                this.rtbSerial.AppendText("\r\n");
            }
            this.rtbSerial.AppendText("读取设备信息完成\r\n");
        }
    }

}

