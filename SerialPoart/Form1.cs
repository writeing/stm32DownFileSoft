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
            isp.UseISPProgramerL071(isp, workers, dataGridView1, serialPort1, serialPort2, cbcom, cbproperty, SerialFlash, checkfile1, checkfile2, checkfile3, cfilepath1, cfilepath2,
            cfilepath3);///类传参数

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


            initconfig();
        }
        public void initconfig()
        {
            for(int i =0; i < 12; i ++)
            {
                CheckBox checkBox3 = new CheckBox();
                g_tempInitlist.select.Add(checkBox3);
                g_tempInitlist.hex.Add(checkBox2);
                g_tempInitlist.address.Add(textBox3);
                g_tempInitlist.data.Add(textBox2);
                g_tempInitlist.info.Add(textBox1);
            }
        }
        public void save_file_process()
        {
            //while(true)
            //{
            //    Thread.Sleep(4000);
            //    if (set_path || sp_programme.get_path)
            //    {
            //        File.WriteAllText("info.json", JsonConvert.SerializeObject(Path));
            //        set_path = false;
            //        sp_programme.get_path = false;
            //    }         
            //}           
        }

        /*串口应用*/      
        public void cbcom_DropDown(object sender, EventArgs e)  //更新串口列表
        {
            //string[] ports = System.IO.Ports.SerialPort.GetPortNames();

            //cbcom.Items.Clear();
            //for (int i = 0; i < ports.Length; i++)
            //{
            //    string name = ports[i];
            //    cbcom.Items.Add(name);
            //}
            //if (ports.Length > 0)
            //    cbcom.Text = ports[0];
            //else
            //    cbcom.Text = null;
        }

        public void download3_Click(object sender, EventArgs e)
        {
            sp_programme.UseDownLoad3();
        }

        int StartEraseCount = 0;
        private void StartEraseFlash_Click(object sender, EventArgs e)
        {
            if (StartEraseCount % 2 == 0)
            {
                StartEraseCount++;
                DeviceOperation device = new DeviceOperation();

                List<FirmwareInfomation> firmwares = isp.GetDataToWrite(ref device); ///读取文件

                if (firmwares == null)
                {
                    this.SerialFlash.AppendText("请选择烧写文件\r\n");
                    return;
                }

                if (!EraseCheckSerial(BAUDRATE)) ///擦除检验串口
                {
                    this.SerialFlash.AppendText("串口初始化失败\r\n");
                }
                else
                    this.SerialFlash.AppendText("开始烧写文件\r\n");

                Action<bool> runAction = new Action<bool>((re) =>
                {
                    this.StartEraseFlash.Text = "取消";
                    StartEraseCount = 0;
                });

                BackgroundWorker worker = isp.WriteData(firmwares, runAction);
                if (worker != null)
                {
                    this.StartEraseFlash.Text = "开始烧写";
                    StartEraseCount = 0;
                    //worker.RunWorkerAsync();
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

        }
        List<initList> initLists = new List<initList>();
        string initPath = "init.json";
        public int initIndex = 0;
        private initList g_tempInitlist;
        private void open_initButton_Click(object sender, EventArgs e)
        {
            // JsonConvert.SerializeObject(one);
            //JsonConvert.DeserializeObject<Student>(str);
            initList init;            
            List<string> initNames = new List<string>();            
            foreach (string line in File.ReadLines(initPath))
            {
                init = JsonConvert.DeserializeObject<initList>(line);
                initNames.Add(init.name);
                initLists.Add(init);
                SerialFlash.AppendText(init.name + "\r\n");
            }
            g_tempInitlist = initLists[initIndex];
        }

        private void save_initButton_Click(object sender, EventArgs e)
        {
            FileStream file = File.Open(initPath, FileMode.Create);
            initLists[initIndex] = g_tempInitlist;
            foreach (initList init in initLists)
            {
                string strinit = JsonConvert.SerializeObject(file);
                byte[] byteArray = System.Text.Encoding.Default.GetBytes(strinit);
                file.Write(byteArray,0,byteArray.Length);
            }
            SerialFlash.AppendText("name:"+ initLists[initIndex].name + "had save success"+ "\r\n");
            file.Close();
        }
        private void clean_initButton_Click(object sender, EventArgs e)
        {            
            //g_tempInitlist.hex.Clear();
            //g_tempInitlist.info.Clear();
            //g_tempInitlist.select.Clear();
            //g_tempInitlist.address.Clear();
            //g_tempInitlist.index.Clear();

        }
    }
    
}
