using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SerialPoart
{
    class programme_serial
    {
        /// <summary>
        /// 串口属性控制
        /// </summary>
        private System.IO.Ports.SerialPort _serial_port = null;
        private ComboBox _ComNum_cmb = null;   //串口号
        private ComboBox _Property = null;     //烧写属性

        /// <summary>
        /// 下载文件选择
        /// </summary>

        private List<CheckBox> CheckFileList = new List<CheckBox>();
        private CheckBox _CheckFile1 = null;
        private CheckBox _CheckFile2 = null;
        private CheckBox _CheckFile3 = null;

        /// <summary>
        /// 文件路径
        /// </summary>
        private List<ComboBox> FilePathList = new List<ComboBox>();
        private ComboBox _FilePath1 = null;
        private ComboBox _FilePath2 = null;
        private ComboBox _FilePath3 = null;

        /// <summary>
        /// 下载
        /// </summary>
        private List<Button> _DownLoadList = new List<Button>();
        private Button _DownLoad1 = null;
        private Button _DownLoad2 = null;
        private Button _DownLoad3 = null;

        /// <summary> 
        /// 烧写
        /// </summary>
        private Button _StartFlash = null;
        private TextBox _ShowMessage = null;
        private DataGridView customDataGridView;

        public SerialDataReceivedEventHandler DataReceived { get; internal set; }

        private JsonSave _Path = new JsonSave();

        public  bool get_path = false;
        public programme_serial()
        {

        }

        #region 烧写参数

        #endregion

        public void MySerial_SetSerialPort(System.IO.Ports.SerialPort serial_port, ComboBox ComNum, ComboBox Property, CheckBox File1,
            CheckBox File2, CheckBox File3, ComboBox FilePath1, ComboBox FilePath2, ComboBox FilePath3, Button DownLoad1,
            Button DownLoad2, Button DownLoad3, Button StartFlashs, TextBox SerialText, JsonSave Path)
        {
            _serial_port = serial_port;
            _ComNum_cmb = ComNum;
            _Property = Property;

            _CheckFile1 = File1;
            _CheckFile2 = File2;
            _CheckFile3 = File3;

            _FilePath1 = FilePath1;
            _FilePath2 = FilePath2;
            _FilePath3 = FilePath3;

            _DownLoad1 = DownLoad1;
            _DownLoad2 = DownLoad2;
            _DownLoad3 = DownLoad3;

            _StartFlash = StartFlashs;
            _ShowMessage = SerialText;

            _Path = Path;

            CheckFileList.Add(_CheckFile1);
            CheckFileList.Add(_CheckFile2);
            CheckFileList.Add(_CheckFile3);

            FilePathList.Add(_FilePath1);
            FilePathList.Add(_FilePath2);
            FilePathList.Add(_FilePath3);

            _DownLoadList.Add(_DownLoad1);
            _DownLoadList.Add(_DownLoad2);
            _DownLoadList.Add(_DownLoad3);
        }

        public void MySerialPoart()
        {
            string[] PortNamesecond = SerialPort.GetPortNames();

            if (_serial_port == null)
            {
                MessageBox.Show("本机没有串口！");
            }

            foreach (string s in System.IO.Ports.SerialPort.GetPortNames())   //添加串口
            {
                _ComNum_cmb.Items.Add(s);          //获取有多少个COM口，添加到控件里
            }
            if (PortNamesecond.Length >= 1)
            {
                _ComNum_cmb.Text = PortNamesecond[0];
            }
            _serial_port.ReadTimeout = 1;

            _serial_port.BaudRate = 115200;//波特率 
            _serial_port.DataBits = 8; //数据位
            _serial_port.StopBits = StopBits.One; //停止位：1
            _serial_port.Parity = Parity.None;  ///校验位：无    
        }

        public void SerialPortDataReceived() ///串口接收函数
        {

        }

        public void ProgrammeSet()
        {
            _Property.Items.AddRange(new object[] {
            "不使用RTS和DTR",
            "DTR低电平复位，不使用RTS",
            "DTR低电平复位，RTS低电平进bootloader",
            "DTR低电平复位，RTS高电平进bootloader",
            "DTR高电平复位，不使用RTS",
            "DTR高电平复位，RTS低电平进bootloader",
            "DTR高电平复位，RTS高电平进bootloader",
            "RTS低电平复位，不使用DTR",
            "RTS低电平复位，DTR低电平进bootloader",
            "RTS低电平复位，DTR高电平进bootloader",
            "RTS高电平复位，不使用DTR",
            "RTS高电平复位，DTR低电平进bootloader",
            "RTS高电平复位，DTR高电平进bootloader"});
            _Property.SelectedIndex = 3; //设置默认选择
        }

        public void UseCheckFile1() ///文件选择控件1
        {
            if (_CheckFile1.Checked)
            {
                _DownLoad1.BackColor = System.Drawing.Color.Gray;
                _DownLoad1.Enabled = false;

                //_Path = JsonConvert.DeserializeObject<JsonSave>(File.ReadAllText(_Path.fpath1));  // 尖括号<>中填入对象的类名   

                   
            }
            else
            {
                _DownLoad1.BackColor = System.Drawing.Color.LightGray;
                _DownLoad1.Enabled = true;
            }
        }

        public void UseCheckFile2() ///文件选择控件2
        {
            if (_CheckFile2.Checked)
            {
                _DownLoad2.BackColor = System.Drawing.Color.Gray;
                _DownLoad2.Enabled = false;
            }
            else
            {
                _DownLoad2.BackColor = System.Drawing.Color.LightGray;
                _DownLoad2.Enabled = true;
            }
        }

        public void UseCheckFile3() ///文件选择控件3
        {
            if (_CheckFile3.Checked)
            {
                _DownLoad3.BackColor = System.Drawing.Color.Gray;
                _DownLoad3.Enabled = false;
            }
            else
            {
                _DownLoad3.BackColor = System.Drawing.Color.LightGray;
                _DownLoad3.Enabled = true;
            }
        }

        public void UseFilePath1() ///烧写路径选择1
        {
        }

        public void UseFilePath2() ///烧写路径选择2
        {
        }

        public void UseFilePath3() ///烧写路径选择3
        {
        }

        //选择保存路径
        private string ShowSaveFileDialog()
        {
            string localFilePath = "";
            //string localFilePath, fileNameExt, newFileName, FilePath; 
            SaveFileDialog sfd = new SaveFileDialog();
            //设置文件类型 
            sfd.Filter = "hex文件|*.hex|bin文件|*.bin";

            //设置默认文件类型显示顺序 
            sfd.FilterIndex = 1;

            //保存对话框是否记忆上次打开的目录 
            sfd.RestoreDirectory = true;

            //点了保存按钮进入 
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                localFilePath = sfd.FileName.ToString(); //获得文件路径 
                string fileNameExt = localFilePath.Substring(localFilePath.LastIndexOf("\\") + 1); //获取文件名，不带路径

                //获取文件路径，不带文件名 
                //FilePath = localFilePath.Substring(0, localFilePath.LastIndexOf("\\")); 

                //给文件名前加上时间 
                //newFileName = DateTime.Now.ToString("yyyyMMdd") + fileNameExt; 

                //在文件名里加字符 
                //saveFileDialog1.FileName.Insert(1,"dameng"); 
                //System.IO.FileStream fs = (System.IO.FileStream)sfd.OpenFile();//输出文件 

                ////fs输出带文字或图片的文件，就看需求了 
            }

            return localFilePath;
        }

        public void UseDownLoad1() ///下载1
        {
            OpenFileDialog fd = new OpenFileDialog();

            fd.Filter = "hex文件|*.hex|bin文件|*.bin";

            if (fd.ShowDialog() == DialogResult.OK)
            {
                if (_FilePath1.FindString(fd.FileName) < 0)
                {
                    _FilePath1.Items.Add(fd.FileName);
                    _FilePath1.SelectedIndex = _FilePath1.FindString(fd.FileName);                
                }
                _FilePath1.SelectedIndex = _FilePath1.FindString(fd.FileName);
                // 获取当前程序所在路径，并将要创建的文件命名为info.json
                _Path.fpath[0] = fd.FileName;
                get_path = true;
                //_Path.fpath.Add(fd.FileName);
                //_Path.fpath3 = System.Windows.Forms.Application.StartupPath + "\\info.json";
                //File.WriteAllText("info.json", JsonConvert.SerializeObject(_Path));
                //File.AppendAllText("info.json", JsonConvert.SerializeObject(_Path.fpath[0]));
            }
        }

        public void UseDownLoad2() ///下载2
        {
            OpenFileDialog fd = new OpenFileDialog();
            fd.Filter = "hex文件|*.hex|bin文件|*.bin";
            if (fd.ShowDialog() == DialogResult.OK)
            {
                if (_FilePath2.FindString(fd.FileName) < 0)
                {
                    _FilePath2.Items.Add(fd.FileName);
                }
                _FilePath2.SelectedIndex = _FilePath2.FindString(fd.FileName);
                //_Path.fpath.Add(fd.FileName);
                _Path.fpath[1] = fd.FileName;
                get_path = true;
                //_Path.fpath3 = System.Windows.Forms.Application.StartupPath + "\\info.json";
                //File.WriteAllText("info.json", JsonConvert.SerializeObject(_Path));
            }
        }

        public void UseDownLoad3() ///下载3
        {
            OpenFileDialog fd = new OpenFileDialog();
            fd.Filter = "hex文件|*.hex|bin文件|*.bin";
            if (fd.ShowDialog() == DialogResult.OK)
            {
                if (_FilePath3.FindString(fd.FileName) < 0)
                {
                    _FilePath3.Items.Add(fd.FileName);
                }
                _FilePath3.SelectedIndex = _FilePath3.FindString(fd.FileName);
                //_Path.fpath.Add(fd.FileName);
                _Path.fpath[2] = fd.FileName;
                get_path = true;
                //File.WriteAllText("info.json", JsonConvert.SerializeObject(_Path));
            }
        }
        public void UseDownLoad(int index) ///
        {
            index = index - 1;
            OpenFileDialog fd = new OpenFileDialog();
            fd.Filter = "hex文件|*.hex|bin文件|*.bin";
            if (fd.ShowDialog() == DialogResult.OK)
            {                
                if (FilePathList[index].FindString(fd.FileName) < 0)
                {
                    FilePathList[index].Items.Add(fd.FileName);
                }
                FilePathList[index].SelectedIndex = FilePathList[index].FindString(fd.FileName);
                //_Path.fpath.Add(fd.FileName);
                CheckFileList[index].Checked = true;
                _Path.fpath[2] = fd.FileName;
                get_path = true;
                //File.WriteAllText("info.json", JsonConvert.SerializeObject(_Path));
            }
        }
        public void UseStartFlash()
        {
            if (!_serial_port.IsOpen)
            {
                string serialName = null;
                try ///异常机制
                {
                    serialName = _ComNum_cmb.SelectedItem.ToString();//设置串口号,获取外部选择串口
                    _serial_port.PortName = serialName;

                    if (_serial_port.IsOpen == true)//如果打开状态，则先关闭一下 
                    {
                        _serial_port.Close();
                    }
                    _serial_port.Open(); //打开串口 
                }
                catch (System.Exception ex)
                {
                    if (serialName == null)
                        MessageBox.Show("请选择串口");
                    else
                        MessageBox.Show("Error:" + ex.Message, "Error");
                    return;
                }
            }
            else
            {
                _serial_port.Close(); //关闭串口 
            }
        }       
    }
}
