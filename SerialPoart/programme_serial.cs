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

        public void MySerial_SetSerialPort(SerialPort serial_port, ComboBox ComNum)
        {
            _serial_port = serial_port;
            _ComNum_cmb = ComNum;
        }

        public void MySerialPoart()
        { 
            if (_serial_port == null)
            {
                MessageBox.Show("本机没有串口！");
                return;
            }            
            _serial_port.ReadTimeout = 10;
            _serial_port.BaudRate = 115200;//波特率 
            _serial_port.DataBits = 8; //数据位
            _serial_port.StopBits = StopBits.One; //停止位：1
            _serial_port.Parity = Parity.None;  ///校验位：无    
        }

        public void UseStartFlash()
        {
            if (!_serial_port.IsOpen)
            {
                string serialName = null;
                try ///异常机制
                {
                    serialName = _ComNum_cmb.Text;//设置串口号,获取外部选择串口
                    _serial_port.PortName = serialName;

                    if (_serial_port.IsOpen == true)//如果打开状态，则先关闭一下 
                    {
                        _serial_port.Close();
                    }
                    _serial_port.Open(); //打开串口 
                }
                catch (System.Exception ex)
                {
                }
            }
            else
            {
                _serial_port.Close(); //关闭串口 
            }
        }       
    }
}
