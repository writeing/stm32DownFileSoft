using DCOM_V1._0;
using SerialPoart;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Serial_second
{
    public class ISPProgramerL071
    {
        //可在相关芯片的stm32f030xc.h头文件中可找到 第500行左右
        //public const UInt32 FLASH_BASE_ADDR = 0x08000000;
        //private const UInt32 UNIQUE_ID_ADDR = 0x1FFFF7AC;
        //private const UInt32 OPTION_ADDR = 0x1FFFF800;
        //private const UInt32 CHIP_SIZE_ADDR = 0x1FFFF7CC;

        ////可在相关芯片的stm32l071xx.h头文件中可找到 第590行左右
        //public const UInt32 FLASH_BASE_ADDR = 0x08000000;
        //private const UInt32 UNIQUE_ID_ADDR = 0x1FF80050;
        //private const UInt32 OPTION_ADDR = 0x1FF80000;
        //private const UInt32 CHIP_SIZE_ADDR = 0x1FF8007C;

        //可在相关芯片的stm32l072xx.h头文件中可找到 第690行左右
        public const UInt32 FLASH_BASE_ADDR = 0x08000000;
        private const UInt32 UNIQUE_ID_ADDR = 0x1FF80050;
        private const UInt32 OPTION_ADDR = 0x1FF80000;
        private const UInt32 CHIP_SIZE_ADDR = 0x1FF8007C;
        
        private const byte ACK_REPLY = 0x79;
        private const byte NACK_REPLY = 0x1f;
        private const byte INIT_COMMAND = 0x7f;
        private const byte INDENT_COMMAND = 0x00;
        private const byte GET_VER_COMMAND = 0x01;
        private const byte GET_ID_COMMAND = 0x02;
        private const byte READ_MEM_COMMAND = 0x11;
        private const byte GO_COMMAND = 0x21;
        private const byte WRITE_MEM_COMMAND = 0x31;
        private const byte ERASE_MEM_COMMAND = 0x43;
        private const byte EXTERN_ERASE_MEM_COMMAND = 0x44;
        private const byte WRITE_PROTECT_COMMAND = 0x63;
        private const byte WRITE_UNPROTECT_COMMAND = 0x73;
        private const byte READOUT_PROTECT_COMMAND = 0x82;
        private const byte READOUT_UNPROTECT_COMMAND = 0x92;

        private const int TIMEOUT_10MS = 10;
        private const int TIMEOUT_50MS = 50;
        private const int TIMEOUT_200MS = 200;
        private const int TIMEOUT_500MS = 500;
        private const int TIMEOUT_1000MS = 1000;
        private const int TIMEOUT_2000MS = 2000;
        
        private ISPProgramer isp_71 = new ISPProgramer();
        DStringExtend StringExtend = new DStringExtend();//串口数据处理

        /// <summary>
        /// 下载文件选择
        /// </summary>
        private CheckBox _check_file1 = null;
        private CheckBox _check_file2 = null;
        private CheckBox _check_file3 = null;

        /// <summary>
        /// 文件路径
        /// </summary>
        private ComboBox _file_path1 = null;
        private ComboBox _file_path2 = null;
        private ComboBox _file_path3 = null;

        private List<BackgroundWorker> _work = null;

        private DataGridView dt;

        public enum InitialType
        {
            NOT_USE_RTS_DRT = 0,
            DTR_LOW_REBOOT = 1,
            DTR_LOW_REBOOT_RTS_LOW_ENTERBOOTLOADER = 2,
            DTR_LOW_REBOOT_RTS_HIGH_ENTERBOOTLOADER = 3,
            DTR_HIGH_REBOOT = 4,
            DTR_HIGH_REBOOT_RTS_LOW_ENTERBOOTLOADER = 5,
            DTR_HIGH_REBOOT_RTS_HIGH_ENTERBOOTLOADER = 6,
            RTS_LOW_REBOOT = 7,
            RTS_LOW_REBOOT_DTR_LOW_ENTERBOOTLOADER = 8,
            RTS_LOW_REBOOT_DTR_HIGH_ENTERBOOTLOADER = 9,
            RTS_HIGH_REBOOT = 10,
            RTS_HIGH_REBOOT_DTR_LOW_ENTERBOOTLOADER = 11,
            RTS_HIGH_REBOOT_DTR_HIGH_ENTERBOOTLOADER = 12
        };
        public enum DensityType
        {
            F103_LOW_DENSITY = 0x12,
            F103_MEDIUM_DENSITY = 0x10,
            F103_HIGH_DENSITY = 0x14,
            F103_XL_DENSITY = 0x30,
            F103_CONNECTIVITY_DENSITY = 0x18,
            F030XC_DENSITY = 0x42,
            L07X_DENSITY = 0x47,
            UNKNOW_DENSITY = NACK_REPLY
        };

        public string UniqueID { get; private set; }
        public string OptionByte { get; private set; }
        public float BootloaderVer { get; private set; }
        public DensityType ChipType { get; private set; }
        public UInt32 ChipSize { get; private set; }
        public UInt32 PageSize { get; private set; }
        public string PortName { get { return this.port2.PortName; } }
        public int PortBaudRate { get { return this.port2.BaudRate; } }
       
        public System.IO.Ports.SerialPort port2 = null;//烧写串口号
        public System.IO.Ports.SerialPort port1 = null; //打印串口号
        private ComboBox _ComNum_cmb2 = null;   //烧写串口号
        private ComboBox _ComNum_cmb1 = null;   //打印串口号
        private ComboBox _cbproperty = null;   //串口DTR、RTS选择
        public event ProgressChangedEventHandler ProgressChanged = null;
        private bool cancelPending = false;

        private TextBox _TextBox = null;

        ISPProgramerL071 _isp;
        public DataGridView customDataGridView;

        public ISPProgramerL071() { }

        public void UseISPProgramerL071(ISPProgramerL071 sp, List<BackgroundWorker> work, DataGridView customDataGridView, System.IO.Ports.SerialPort srt1, System.IO.Ports.SerialPort srt2, ComboBox ComNum1, ComboBox property,TextBox SerialText, CheckBox Checkfile1, CheckBox Checkfile2, CheckBox Checkfile3,
            ComboBox Filepath1, ComboBox Filepath2, ComboBox Filepath3)
        {
            _isp = sp;

            _work = work;

            //串口
            port1 = srt1;
            port2 = srt2;
            _ComNum_cmb2 = ComNum1;
            _ComNum_cmb1 = ComNum1; 
            _cbproperty = property; ///烧写串口属性

            dt = customDataGridView;

            _TextBox = SerialText;

            _check_file1 = Checkfile1;
            _check_file2 = Checkfile2;
            _check_file3 = Checkfile3;

            _file_path1 = Filepath1;
            _file_path2 = Filepath2;
            _file_path3 = Filepath3;           
        }

        private void ReportProgress(int pec, object o) ///进度条
        {
            if (this.cancelPending == true)
            {
                this.cancelPending = false;
                throw new Exception("操作被用户取消\r\n");
            }
            if (ProgressChanged != null)
            {
                ProgressChanged(this, new ProgressChangedEventArgs(pec, o));
            }
        }

        private void ReportProgress(int pec)
        {
            this.ReportProgress(pec, null);
        }

        public void CancelWrok()
        {
            this.cancelPending = true;
        }

        #region 初始化
        public bool Init(String portName, int baudRate)
        {
            return this.Init(portName, baudRate, InitialType.DTR_HIGH_REBOOT_RTS_HIGH_ENTERBOOTLOADER);
        }

        public bool Init(String portName, int baudRate, InitialType index)
        {
           int type = (int)index - 1;

           int rebootCount = 0;
           while (rebootCount++ < 2)
           {
              this.port2.WriteTimeout = TIMEOUT_200MS;
              this.port2.ReadTimeout = TIMEOUT_200MS;
              try
              {
                  if (this.port2.IsOpen)
                  {
                     this.port2.Close();
                  }
                  this.port2.PortName = portName;
                  this.port2.BaudRate = baudRate;                               
                  this.port2.Open();
              }
              catch (Exception)
              {
                  return false;
              }

              if (!this.port2.IsOpen)
              {
                  _TextBox.Text += "Exception" + "!IsOpen";
                  return false;
               }

           ////    F030XXC 此代码与烧录器硬件有关
           ////复位一次，重新进入bootloader
           ////port.DtrEnable = false;
           ////    port.RtsEnable = true;
           ////    port.DtrEnable = false;
           ////    port.RtsEnable = false;
           ////    Thread.Sleep(100);

               if (type >= 0)
               {
                    bool rtsReboot = (type / 6) > 0;
                    int enterBootloader = type % 3;
                    bool rebootLevel = ((type % 6) / 3) > 0;
                    if (rtsReboot)
                    {
                        this.port2.RtsEnable = rebootLevel;
                    }
                    else
                    {
                        this.port2.DtrEnable = rebootLevel;
                    }

                   if (enterBootloader > 0)
                   {
                       if (rtsReboot)
                       {
                          this.port2.DtrEnable = enterBootloader > 1;
                       }
                       else
                       {
                          this.port2.RtsEnable = enterBootloader > 1;
                       }
                   }
                  Thread.Sleep(200);
                  if (rtsReboot)
                  {
                      this.port2.RtsEnable = !rebootLevel;
                  }
                  else
                  {
                      this.port2.DtrEnable = !rebootLevel;
                  }
               }

               int successCount = 0;
               for (int i = 0, interval = 1; i < 8 && successCount < 2; i++, interval++)
               {
                    port2.Write(new byte[] { INIT_COMMAND }, 0, 1); ///进bootloader

                   Thread.Sleep(50);
                    try
                   {                                    
                       byte reply = 0;
                       if (this.port2.BytesToRead > 0)
                          reply = (byte)this.port2.ReadByte();

                        _TextBox.Text += "0x" + reply.ToString("X2") + " ";
                        if (reply == ACK_REPLY)
                       {
                           successCount = 1;
                           interval = 0;
                           break;
                       }
                       else if (reply == NACK_REPLY)
                       {
                           if (interval == 2)
                           {
                               successCount++;
                           }
                           else
                           {
                               successCount = 0;
                           }
                               interval = 0;
                       }
                   }
                   catch (TimeoutException) { }
               }

               if (successCount == 1)
               {
                   //this.UniqueID = this.ReadUinqueID();
                   this.OptionByte = this.ReadOptionByte();
                    //this.ChipType = this.ReadDensity();
                    //this.BootloaderVer = this.ReadBootloaderVer();
                    //this.ChipSize = this.ReadChipSize() * (UInt32)1024;
                    this.UniqueID = "L07X芯片无法用Bootloader读取UID";
                    ////this.OptionByte = "l071芯片暂时无法读取";
                    this.ChipType = this.ReadDensity();
                    this.BootloaderVer = this.ReadBootloaderVer();
                    this.ChipSize = this.PageSize * 1024;//目前使用的芯片Flash为128K
                    if (this.ChipType == (DensityType)0x79 || this.ChipSize <= 0)
                        return false;
                    return true;
                    //todo:完善读写保护时的初始化逻辑
               }
           }
          this.Close();
 
          return false;
        }

        public void L071SerialPortDataReceived() ///串口接收函数：中断委托处理
        {
            byte reply = 0;
            if (this.port2.BytesToRead == 1)
            {
                reply = (byte)this.port2.ReadByte();
            }
            else
            {
                Byte[] ReceivedData = new Byte[port2.BytesToRead];
                port2.Read(ReceivedData, 0, ReceivedData.Length);

                string str = System.Text.Encoding.ASCII.GetString(ReceivedData, 0, ReceivedData.Length);
                _TextBox.Text += "--- reply ---" + str;
            }               

            if (reply == ACK_REPLY)
            {
                _TextBox.Text += "--- reply ---" + reply.ToString("X2");
            }                            
        }

        public void Close()
        {
            if (this.port2.IsOpen)
            {
                this.port2.Close();
            }
        }
        #endregion

        #region 串口命令
        private bool SendCommands(byte[] data, int timeout)
        {
            byte bcc = 0;

            /********************设置串口发送超时机制*****************/
            port2.ReadTimeout = timeout;
            port2.WriteTimeout = timeout;
            int time = 40000;///至少40000
            for (int i = 0; i < data.Length; i++)
            {
                port2.Write(data, i, 1);
                time = 40000;///至少40000
                while (--time > 1) ;//增加延时要解决ACK失败问题，不采用sleep延时太大
                bcc ^= data[i]; //数据XOR：异或处理
            }
            Thread.Sleep(1); //增加延时要解决读取失败问题
            port2.Write(new byte[] { bcc }, 0, 1);

            try
            {
                byte a = (byte)this.port2.ReadByte();
                return (a == ACK_REPLY);
            }
            catch (Exception)
            {
                return false;
            }
        }

        private void SendData(byte[] data)  //发送数据，并且最后发送校验和
        {
            byte bcc = 0;
            for (int i = 0; i < data.Length; i++)
            {
                port2.Write(data, i, 1);
                bcc ^= data[i];
            }
            port2.Write(new byte[] { bcc }, 0, 1);
        }
                     
        private bool SendCommand(byte data)
        {
            /********************设置串口发送超时机制*****************/
            port2.WriteTimeout = 500;
            port2.ReadTimeout = 500;

            port2.Write(new byte[] { data }, 0, 1);
            Thread.Sleep(1); //增加延时要解决读取失败问题
            port2.Write(new byte[] { (byte)(data ^ 0xff) }, 0, 1);
            try
           {
                byte a = (byte)this.port2.ReadByte();
                //_TextBox.Text += "retryCountok: " + " a: 0x" + a.ToString("X2") + "\r\n";
                return (a == ACK_REPLY);
           }
           catch (Exception)
           {
                //_TextBox.Text += "retryCounterror: " + "\r\n";
                return false;
           }

        }
        #endregion

        #region 功能函数
        public bool StartProgram(UInt32 address)
        {
            return Go(address.ToAddressArray());
        }

        public DensityType ReadDensity()
        {
            DensityType pid = (DensityType)this.GetPID(); ;
            switch (pid)
            {
                case DensityType.F103_LOW_DENSITY:
                case DensityType.F103_MEDIUM_DENSITY:
                    this.PageSize = 1024;
                    break;
                case DensityType.F103_HIGH_DENSITY:
                case DensityType.F103_XL_DENSITY:
                case DensityType.F103_CONNECTIVITY_DENSITY:
                    this.PageSize = 2048;
                    break;
                case DensityType.F030XC_DENSITY:    //F030XC页大小为2048 0x800
                    this.PageSize = 2048;
                    break;
                case DensityType.L07X_DENSITY:    //L071页大小为128 128
                    this.PageSize = 128;
                    break;
                default:
                    this.PageSize = 1024;
                    break;
            }
            return pid;
        }

        public float ReadBootloaderVer()
        {
            byte readVer = this.GetBootLoaderVer();
            if (readVer != NACK_REPLY)
            {
                return ((readVer & 0xf0) >> 4) + (float)0.1 * (readVer & 0xf);
            }
            return NACK_REPLY;
        }

        public UInt16 ReadChipSize()
        {
            byte[] data = this.Read(CHIP_SIZE_ADDR.ToAddressArray(), 1);
            if (data != null)
            {
                UInt16 size = BitConverter.ToUInt16(data, 0);
                this.ChipSize = (UInt32)size * 1024;
                return size;
            }
            return 0;
        }

        public string ReadUinqueID()
        {
            byte[] s = this.Read(UNIQUE_ID_ADDR.ToAddressArray(), 11);
            if (s != null)
            {
                return BitConverter.ToString(s).Replace("-", "");
            }
            return null;
        }

        public string ReadOptionByte()
        {
            //byte[] s = this.Read(OPTION_ADDR.ToAddressArray(), 15);//F103、F030的选项字节有16位
            byte[] s = this.Read(OPTION_ADDR.ToAddressArray(), 19);//L071、L072的选项字节有20位
            if (s != null)
            {
                return BitConverter.ToString(s).Replace("-", "");
            }
            return null;
        }
        #endregion

        #region flash操作
        /// <summary>
        /// 运用读写保护的替换，清除所有flash
        /// </summary>
        /// <returns></returns>
        public bool erase_allFash()
        {
            ReadOutProtect();
            {
                Thread.Sleep(500);
                _TextBox.AppendText("设置读保护成功，开始初始化");
                //进入读保护成功
                //芯片复位，重新开始连接初始化
                Reset_Serial_Port(); ///进bootloader

                Thread.Sleep(50);
                //开始去除芯片读保护
                if (ReadOutUnProtect() == true)
                {
                    Reset_Serial_Port();
                }
                else
                {
                    return false;
                }
            }
                
            return true;
        }


        /// <summary>指定地址写入数据，写入位置如果不为空则擦除整一页，可能擦除不在写入区域的额外数据，但是效率高</summary>
        /// <param name="address">写入起始地址，应为4的倍数且不小于FLASH_BASE_ADDR但小于FLASH_BASE_ADDR+ChipSize</param>
        /// <param name="startRemainData">写入数据，长度应为4的倍数且加上address应小于FLASH_BASE_ADDR+ChipSize</param>
        /// <returns>写入成功返回true，写入失败返回false</returns>
        public bool WriteErasePage(UInt32 address, byte[] data)
        {
            UInt32 length = (UInt32)data.Length;
            if (address < FLASH_BASE_ADDR || address + length > FLASH_BASE_ADDR + ChipSize || address % 4 != 0 || length % 4 != 0)
            {
                return false;
            }

            UInt32 offset = 0;
            while (offset < length)
            {
                UInt32 inPageAddr = address - FLASH_BASE_ADDR + offset;
                byte page = (byte)((inPageAddr) / PageSize);
                byte len = (byte)(Math.Min((PageSize - inPageAddr % PageSize), Math.Min(length - offset, 0x100)) - 1);
                byte[] addr = BitConverter.GetBytes(address + offset);
                Array.Reverse(addr);
                if (!this.Read(addr, len).IsEmpty())
                {
                    if (!this.Erase(page))
                        return false;
                }
                byte[] wData = new byte[len + 1];
                Array.Copy(data, offset, wData, 0, len + 1);
                if (!this.Write(addr, wData))
                {
                    return false;
                }
                offset += (UInt32)(1 + len);

                //this.ReportProgress((int)(offset / length));
            }
            return true;
        }

        /// <summary>从指定地址读取指定长度数据</summary>
        /// <param name="addr">起始读取地址，应不小于FLASH_BASE_ADDR且小于FLASH_BASE_ADDR+ChipSize</param>
        /// <param name="length">读取长度，addr + length应不大于FLASH_BASE_ADDR + ChipSize</param>
        /// <returns>读取成功返回读取到的数据，否则返回null</returns>
        public byte[] ReadFlash(UInt32 addr, UInt32 length)
        {
            if (addr + length > FLASH_BASE_ADDR + ChipSize)
            {
                return null;
            }
            byte[] data = new byte[length];
            UInt32 offset = 0;
            while (length > 0)
            {
                UInt32 len = Math.Min(0x100, length); 
                byte[] d = this.Read(addr.ToAddressArray(), (byte)(len - 1));
                if (d == null)
                {
                    return null;
                }
                Array.Copy(d, 0, data, offset, len);
                offset += len;
                length -= len;
                addr += len;

                //this.ReportProgress((int)((offset * 100) / (length + offset)));
            }
            return data;
        }

        /// <summary>在指定flash地址写入数据，仅覆盖写入地址范围内的数据</summary>
        /// <param name="address">写入起始地址，应为4的倍数且不小于FLASH_BASE_ADDR但小于FLASH_BASE_ADDR+ChipSize</param>
        /// <param name="startRemainData">写入数据，长度加上address应小于FLASH_BASE_ADDR+ChipSize</param>
        /// <returns>写入成功返回true，写入错误返回false</returns>
        public bool WriteFlash(UInt32 address, byte[] data)
        {
            UInt32 length = (UInt32)data.Length;
            if (address < FLASH_BASE_ADDR || address + length > FLASH_BASE_ADDR + ChipSize || address % 4 != 0)
            {
                throw new ArgumentOutOfRangeException("地址或数据长度非法\r\n");
            }

            _TextBox.AppendText("length" + length + "\r\n");
            if (length % 4 != 0)
            {
                List<byte> temp = new List<byte>();
                temp.AddRange(data);
                byte[] tale = this.Read((address + length).ToAddressArray(), (byte)(3 - length % 4));
                if (tale != null)
                {
                    temp.AddRange(tale);
                    data = temp.ToArray();
                    length = (UInt32)data.Length;                  
                }
            }
            
            UInt32 startOffset = address - FLASH_BASE_ADDR;         //数据的首地址
            UInt16 startPage = (UInt16)(startOffset / PageSize);    //数据的开始页

            UInt32 taleOffset = startOffset + length;                       //数据的尾部地址
            UInt16 talePage = (UInt16)((startOffset + length) / PageSize);  //数据的尾部地址所在的页，判断数据是否跨页
            UInt32 taleRemain = taleOffset % PageSize;                      //尾部页偏移

            List<UInt16> pagesToErase = new List<UInt16>();         //要擦的页
            UInt16 page = startPage;

            //_TextBox.AppendText("page" + page + "\r\n");

            if (length > PageSize)//写入的是烧录文件
            {
                if (erase_allFash() == true)
                {
                    _TextBox.AppendText("擦除成功");
                }
                else
                {
                    _TextBox.AppendText("擦除失败");
                }
                //for (page = startPage; page <= talePage; page++)        //先擦除数据
                //{
                //    pagesToErase.Add(page); ///将要擦除数据分页保存到数组中
                //}
                //if (pagesToErase.Count != 0)
                //{
                //    if (this.BootloaderVer < (float)3)//L07X 系列都不会小于3
                //        return false;
                //    else
                //    {
                //        List<ushort> pagesToErase16 = new List<ushort>();
                //        foreach (var item in pagesToErase)
                //        {
                //            pagesToErase16.Add(item);
                //        }
                //        this.ReportProgress(0, "擦除flash\r\n");
                //        _TextBox.AppendText("擦除flash\r\n");
                //        for (int i = 0; i < pagesToErase.Count; i++)
                //        {
                //            if (!this.ExternErase(pagesToErase16[i]))   //擦除所有页 pagesToErase16[i])：擦除页：每页128B
                //            {
                //                this.ReportProgress(100, "flash擦除失败\r\n");
                //                _TextBox.AppendText("flash擦除失败\r\n");
                //                return false;
                //            }
                //            else
                //            {
                //                //this.ReportProgress((int)((i * 100) / pagesToErase.Count)); ///进度条显示
                //            }
                //        }                     
                //    }
                //}
                //this.ReportProgress(0, "开始写入文件数据\r\n");
                _TextBox.AppendText("开始写入文件数据" + length + "\r\n");
                UInt32 offset = 0;
                while (length > 0)
                {
                    //_TextBox.AppendText("开始烧写" + length + "\r\n");
                    UInt32 inPageAddr = address - FLASH_BASE_ADDR + offset;
                    byte len = length > 0x80 ? (byte)0x7f : (byte)(length - 1);
                    byte[] wData = new byte[len + 1];
                    Array.Copy(data, offset, wData, 0, len + 1);
                    if (!this.Write((address + offset).ToAddressArray(), wData)) ///0x8000000+offset
                    {
                        _TextBox.AppendText("写入false\r\n");
                        return false;
                    }
                    offset += (UInt32)(1 + len);
                    length -= (UInt32)(1 + len);

                    //this.ReportProgress((int)((offset * 100) / (length + offset))); ///进度条
                }
                //this.ReportProgress(100, "写入完成\r\n");
                _TextBox.AppendText("写入完成\r\n");
            }
           
            else//写入的是自定义数据，自定义数据不能大于1页大小
            {
                this.ReportProgress(0, "开始写入自定义数据\r\n");
                _TextBox.AppendText("开始写入自定义数据\r\n");
                //this.ReportProgress(100, "暂时不支持\r\n");
                if (startPage != talePage)//数据跨页
                {
                    this.ReportProgress(0, "自定义数据长度较长，最大为" + PageSize.ToString() + "字节\r\n");
                    return false;
                }
                UInt32 add = FLASH_BASE_ADDR + page * PageSize;         //读一页
                byte[] FlashData = this.Read(add.ToAddressArray(), 0x80 - 1);
                //修改数据
                for (int i = 0; i < 0x80; i++)
                {
                    if (address == add + i)
                    {
                        Array.Copy(data, 0, FlashData, i, data.Length);
                        break;
                    }
                }
                if (this.BootloaderVer < (float)3)//L07X 系列都不会小于3
                    return false;
                else
                {
                    List<ushort> pagesToErase16 = new List<ushort>();
                    pagesToErase16.Add(page);
                    this.ReportProgress(0, "擦除flash\r\n");
                    _TextBox.AppendText("擦除flash\r\n");
                    if (!this.ExternErase(pagesToErase16.ToArray()))    //擦除所有页
                    {
                        this.ReportProgress(0, "flash擦除失败\r\n");
                        _TextBox.AppendText("flash擦除失败\r\n");
                        return false;
                    }
                }
                if (!this.Write(add.ToAddressArray(), FlashData))
                {
                    return false;
                }
                this.ReportProgress(100, "写入完成\r\n");
                _TextBox.AppendText("写入完成\r\n");
                return true;
            }
            
            return true;
        }

        /// <summary>
        /// 重新配置串口
        /// </summary>
        public void Reset_Serial_Port()
        {
                if (port1.IsOpen)
                    port1.Close();
                _ComNum_cmb1.Text = "打开串口";
         
            var initType = (ISPProgramerL071.InitialType)this._cbproperty.SelectedIndex;
            string portName = this._ComNum_cmb2.SelectedItem as string;
            this._TextBox.AppendText("串口: " + portName + "\r\n");

            bool IspInitState = this.Init(portName, 115200, initType);
        }

        /// <summary>擦除整个芯片</summary>：先解除读保护----设置读保护----解除读保护
        /// <returns>擦除结果</returns>
        public bool EraseChip()
        {
            if (this.BootloaderVer > (float)2.2)
            {
                _TextBox.AppendText("开始擦除芯片\r\n");
                if (ReadOutUnProtect())
                {
                    Reset_Serial_Port();  ///读写操作必须重置串口,否则失败

                    if (ReadOutProtect()) 
                    {
                        Reset_Serial_Port();

                        if (ReadOutUnProtect())
                        {
                            _TextBox.AppendText("擦除芯片成功\r\n");
                            return true;
                        }
                        else
                        {
                            _TextBox.AppendText("擦除芯片失败\r\n");
                            return false;
                        }
                    }
                    else
                    {
                        _TextBox.AppendText("擦除芯片失败\r\n");
                        return false;
                    }
                }
                 
                else
                {
                    _TextBox.AppendText("擦除芯片失败\r\n");
                    return false;
                }
            }
            else
                return false;
        }

        /// <summary>从指定地址开始擦除指定长度空间，页擦除后会写回不在擦除范围的数据，和WriteMem存在嵌套调用</summary>
        /// <param name="address">起始地址，必须为4的倍数</param>
        /// <param name="length">擦除长度，必须为4的倍数</param>
        /// <returns>擦除成功返回true，擦除失败返回false</returns>
        public bool EraseMem(UInt32 address, UInt32 length)
        {
            UInt32 offset = 0;
            if (length % 4 != 0)
            {
                length += (4 - length % 4);
            }
            ///161125 修正偏移和结束条件计算错误导致数据溢出无限擦除情况
            ///<exception>计算错误可能导致无限擦除</exception>
            while (offset < length)
            {
                UInt32 offsetAddr = address - FLASH_BASE_ADDR + offset;
                byte page = (byte)(offsetAddr / PageSize);
                UInt32 inPageOffset = offsetAddr % PageSize;
                UInt32 inPageRemain = PageSize - inPageOffset;
                UInt32 remain = length - offset;
                byte[] frontData = null;
                byte[] behindData = null;

                if (inPageOffset > 0)
                {
                    frontData = ReadFlash(FLASH_BASE_ADDR + page * PageSize, inPageOffset);
                    if (frontData.IsEmpty())
                    {
                        frontData = null;
                    }
                }
                if (remain < inPageRemain)
                {
                    behindData = ReadFlash(address + length, inPageRemain - remain);
                    if (behindData.IsEmpty())
                    {
                        behindData = null;
                    }
                    offset += remain;
                }
                else
                {
                    offset += inPageRemain;
                }
                if (!this.Erase(page))
                {
                    return false;
                }
                if (frontData != null)
                {
                    this.WriteMem(FLASH_BASE_ADDR + page * PageSize, frontData);
                }
                if (behindData != null)
                {
                    this.WriteMem(address + length, behindData);
                }
            }

            return true;
        }

        /// <summary>在指定flash地址写入数据，仅覆盖写入地址范围内的数据，和EraseMem存在嵌套调用</summary>
        /// <param name="address">写入起始地址，应为4的倍数且不小于FLASH_BASE_ADDR但小于FLASH_BASE_ADDR+ChipSize</param>
        /// <param name="startRemainData">写入数据，长度加上address应小于FLASH_BASE_ADDR+ChipSize</param>
        /// <returns>写入成功返回true，写入错误返回false</returns>
        public bool WriteMem(UInt32 address, byte[] data)
        {
            UInt32 length = (UInt32)data.Length;
            if (address < FLASH_BASE_ADDR || address + length > FLASH_BASE_ADDR + ChipSize || address % 4 != 0)
            {
                return false;
            }

            if (length % 4 != 0)
            {
                byte[] temp = new byte[length + 4 - length % 4];
                byte[] tale = this.ReadFlash((UInt32)(address + length), 4 - length % 4);
                if (tale != null)
                {
                    Array.Copy(data, temp, length);
                    Array.Copy(tale, 0, temp, length, tale.Length);
                    data = temp;
                    length = (UInt32)data.Length;
                }
            }

            this.ReportProgress(0, "检查flash\r\n");

            if (!this.ReadFlash(address, length).IsEmpty())
            {
                this.ReportProgress(0, "擦除flash\r\n");
                if (!this.EraseMem(address, length))
                {
                    this.ReportProgress(100, "擦除失败\r\n");
                    return false;
                }
                else
                {
                    this.ReportProgress(100, "擦除成功\r\n");
                }
            }

            UInt32 offset = 0;

            this.ReportProgress(0, "开始写入...\r\n");

            while (length > 0)
            {
                UInt32 inPageAddr = address - FLASH_BASE_ADDR + offset;
                byte page = (byte)((inPageAddr) / PageSize);
                byte len = length > 0x80 ? (byte)0x80 : (byte)(length - 1);
                byte[] addr = BitConverter.GetBytes(address + offset);
                Array.Reverse(addr);
                byte[] wData = new byte[len + 1];
                Array.Copy(data, offset, wData, 0, len + 1);
                if (!this.Write(addr, wData))
                {
                    return false;
                }
                offset += (UInt32)(1 + len);
                length -= (UInt32)(1 + len);

                this.ReportProgress((int)((offset * 100) / (length + offset)));
            }
            this.ReportProgress(100, "写入完成\r\n");
            return true;
        }
        #endregion

        #region 基础指令操作
        /// <summary>
        /// 擦除指定的页面，用于2.2版本以及以下的Bootloader
        /// </summary>
        /// <param name="pages">待擦除的多个页面，数量应不大于0xff</param>
        /// <returns>擦除成功返回true，擦除失败返回false。具有写保护的扇区擦除不会报错</returns>
        private bool Erase(byte[] pages)
        {
            if (this.SendCommand(ERASE_MEM_COMMAND))
            {
                byte[] data = new byte[pages.Length + 1];
                data[0] = (byte)(pages.Length - 1);
                Array.Copy(pages, 0, data, 1, pages.Length);
                return this.SendCommands(data, TIMEOUT_500MS * (pages.Length / 16 + 1));       //每增加16k擦除空间增加500ms等待超时时间
            }
            return false;
        }

        /// <summary>
        /// 擦除特定页面，用于2.2版本以及以下的Bootloader
        /// </summary>
        /// <param name="index">
        /// 0xFF:全部擦除
        /// other:擦除指定一个页面
        /// </param>
        /// <returns>擦除成功返回true，擦除失败返回false。具有写保护的扇区擦除不会报错</returns>
        private bool Erase(byte index)
        {
            if (this.SendCommand(ERASE_MEM_COMMAND))
            {
                if (index == 0xFF)
                {
                    return this.SendCommand(index);
                }
                else
                {
                    return this.SendCommands(new byte[] { 0, index }, TIMEOUT_1000MS);
                }
            }
            return false;
        }

        /// <summary>
        /// 扩展特殊擦除指令，仅支持v3.0及以上 bootloader，用来代替Erase的全部擦除功能，未测试
        /// </summary>
        /// <param name="command">
        /// 双字节命令，MSB在前，调用者需自行保障数据正确
        /// 0xFFFF:全部擦除；
        /// 0xFFFE:存储区1批量擦除
        /// 0xFFFD:存储区2批量擦除
        /// </param>
        /// <returns>擦除成功返回true，擦除失败返回false。具有写保护的扇区擦除不会报错</returns>
        public bool ExternErase(UInt16 command)
        {
            if (this.SendCommand(EXTERN_ERASE_MEM_COMMAND)) 
            {
                byte bcc = 0;
                byte[] data = new byte[1 * 2 + 2];
                data[0] = (byte)((1 - 1) >> 8);
                data[1] = (byte)((1 - 1) & 0xff);

                port2.WriteTimeout = 500;
                port2.ReadTimeout = 500;
                for (int i = 1; i < 2; i++)
                {
                    data[i * 2] = (byte)(command >> 8);
                    data[i * 2 + 1] = (byte)(command & 0xff);
                }

                int time = 50000;
                for (int i = 0; i < data.Length; i++)
                {
                    port2.Write(data, i, 1);
                    time = 50000;///至少40000
                    while (--time > 1) ;//增加延时要解决接收ACKs失败问题，不采用sleep延时太大
                    bcc ^= data[i];
                }
                Thread.Sleep(1);
                port2.Write(new byte[] { bcc }, 0, 1);
                try
                {                   
                    byte a = (byte)this.port2.ReadByte();
                    //_TextBox.AppendText("a: 0x" + a.ToString("X2") + "\r\n");
                    return (a == ACK_REPLY);
                }
                catch (Exception)
                {
                    _TextBox.AppendText("break" + "a" + "\r\n");                 
                    return false;
                }
            }
            return false;
        }

        /// <summary>
        /// 扩展擦除指令，仅支持v3.0及以上bootloader，用来代替Erase，未测试
        /// </summary>
        /// <param name="pagesToErase">待擦除起始页面，双字节，msb在前</param>
        /// <returns>擦除成功返回true，擦除失败返回false。具有写保护的扇区擦除不会报错</returns>
        public bool ExternErase(UInt16[] pages)
        {
            if (this.SendCommand(EXTERN_ERASE_MEM_COMMAND))
            {
                byte bcc = 0;
                byte[] data = new byte[pages.Length * 2 + 2];
                data[0] = (byte)((pages.Length - 1) >> 8);
                data[1] = (byte)((pages.Length - 1) & 0xff);
                //data[0] = (byte)(pages.Length >> 8);
                //data[1] = (byte)(pages.Length & 0xff);

                for (int i = 1; i < pages.Length + 1; i++)
                {
                    data[i * 2] = (byte)(pages[i - 1] >> 8);
                    data[i * 2 + 1] = (byte)(pages[i - 1] & 0xff);
                }

                for (int i = 0; i < data.Length; i++)
                {
                    port2.Write(data, i, 1);
                    Thread.Sleep(1);
                    bcc ^= data[i];
                }
                Thread.Sleep(1);
                port2.Write(new byte[] { bcc }, 0, 1);

                port2.ReadTimeout = Math.Min(2000, Math.Max(pages.Length * 5, 100));
                Thread.Sleep(Math.Min(2000, Math.Max(pages.Length * 5, 100)));//100ms太短，读不到数据
                if (port2.BytesToRead > 0)
                {
                    try
                    {
                        byte a = (byte)port2.ReadByte();
                        return (a == ACK_REPLY);                           
                    }                 
                     catch (Exception)
                    {                     
                        return false;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 向地址读取写入指定数据
        /// </summary>
        /// <param name="addr">4字节地址，msb在前，必须为4的倍数对齐，否则写入失败</param>
        /// <param name="data">data长度应不大于0x80否则会写入失败，且必须为4的倍数对齐，否则虽然不会报错但是末尾会被未知数据填充</param>
        /// <returns></returns>
        private bool Write(byte[] addr, byte[] data)
        {
            if (this.SendCommand(WRITE_MEM_COMMAND))
            {
                if (this.SendCommands(addr, TIMEOUT_10MS))
                {
                    byte[] wData = new byte[data.Length + 1];
                    wData[0] = (byte)(data.Length - 1);
                    Array.Copy(data, 0, wData, 1, data.Length);
                    return this.SendCommands(wData, TIMEOUT_10MS);
                }
            }
            return false;
        }

        /// <summary>
        /// 从指定地址读取指定长度数据
        /// </summary>
        /// <param name="addr">4字节地址，msb在前</param>
        /// <param name="length">读取长度为length+1</param>
        /// <returns></returns>
        public byte[] Read(byte[] addr, byte length)
        {
            //_TextBox.Text += "READ_MEM_COMMAND: ";
            if (this.SendCommand(READ_MEM_COMMAND)) //发送读指令
            {
                //_TextBox.Text += "addr: ";
                
                if (this.SendCommands(addr, TIMEOUT_500MS))  //发送读地址
                {
                    //_TextBox.Text += "length: ";
                    if (this.SendCommand(length))   //发送读长度
                    {
                        port2.ReadTimeout = 500;

                        for (int i = 0; i < length; i++)
                        {
                            int time = 60000;///至少60000
                            while (--time > 1) ;//增加延时要解决读取失败问题，不采用sleep延时太大
                        }
                        try
                        {
                            byte[] data = new byte[length + 1];
                            port2.Read(data, 0, data.Length);
                            //_TextBox.Text += "data: " + "\r\n";
                            return data;
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// 获得双字节芯片PID
        /// </summary>
        /// <returns>如读取成功返回PID低字节数据，高字节固定为0x04，否则返回NACK_REPLY</returns>
        public byte GetPID()
        {
            if (this.SendCommand(GET_ID_COMMAND))
            {
                //F030会返回：79 01 04 42 79 
                //第一个79在SendCommand()里面被接收
                //01表示字节数为 01+1=2字节 
                //数据1：04 
                //数据2：42 
                //ACK回复 79
                //将数据1、2拼接起来就是PID：0x0442,PID可以在AN2606"器件相关的自举程序参数(Device-dependent bootloader parameters)"章节参看
                //L071会返回：79 01 04 47 79 
                //第一个79在SendCommand()里面被接收
                //01表示字节数为 01+1=2字节 
                //数据1：04 
                //数据2：47 
                //ACK回复 79
                //将数据1、2拼接起来就是PID：0x0442,PID可以在AN2606"器件相关的自举程序参数(Device-dependent bootloader parameters)"章节参看
                byte low, high, count;
                int retryCount = 0;

                port2.ReadTimeout = 500;
                while (this.port2.BytesToRead < 4 && retryCount++ < 0xfff) ;
                count = (byte)this.port2.ReadByte();
                high = (byte)this.port2.ReadByte();
                low = (byte)this.port2.ReadByte();
                if (count == 0x1 && this.port2.ReadByte() == ACK_REPLY && high == 0x04 && this.port2.BytesToRead == 0)
                {
                    return low;
                }
            }
            return NACK_REPLY;
        }

        /// <summary>
        /// 获得芯片内部自举Bootloader程序版本号
        /// </summary>
        /// <returns>读取成功返回一个字节版本号，高4bit为大版本号，低4bit为小版本号，否则返回NACK_REPLY</returns>
        public byte GetBootLoaderVer()
        {
            if (this.SendCommand(GET_VER_COMMAND))
            {
                //F030会返回：79 31 00 00 79
                //第一个79在SendCommand()里面被接收
                //31表示版本号为3.1 
                //选项字节1：00 
                //选项字节2：00 
                //ACK回复 79

                //L071会返回：79 31 00 00 79
                //第一个79在SendCommand()里面被接收
                //31表示版本号为3.1 
                //选项字节1：00 
                //选项字节2：00 
                //ACK回复 79

                byte op1, op2, ver;
                int retryCount = 0;
                while (this.port2.BytesToRead < 4 && retryCount++ < 0xfff) ;
                ver = (byte)this.port2.ReadByte();
                op1 = (byte)this.port2.ReadByte();
                op2 = (byte)this.port2.ReadByte();
                if (this.port2.ReadByte() == ACK_REPLY && op1 == 0x0 && op2 == 0x0 && this.port2.BytesToRead == 0)
                {
                    return ver;
                }
            }
            return NACK_REPLY;
        }

        /// <summary>
        /// 从指定地址运行程序
        /// </summary>
        /// <param name="addr">4字节程序地址，msb在前</param>
        /// <returns>成功返回true，否则返回false</returns>
        private bool Go(byte[] addr)
        {
            if (this.SendCommand(GO_COMMAND))
            {
                return this.SendCommands(addr, TIMEOUT_200MS);
            }
            return false;
        }

        //todo:读写保护功能待测试
        /// <summary>
        /// 对指定页面执行写保护操作
        /// </summary>
        /// <param name="pageCount">待保护页面，数量应小于0xff</param>
        /// <returns>执行成功返回true，否则返回false，页面无效不会返回错误</returns>  
        public bool WriteProtect(byte[] pages)
        {
            if (this.SendCommand(WRITE_PROTECT_COMMAND))
            {
                if (this.SendCommand((byte)pages.Length))
                {
                    byte[] data = new byte[pages.Length + 1];
                    data[0] = (byte)(pages.Length - 1);
                    Array.Copy(pages, 0, data, 1, pages.Length);
                    return this.SendCommands(data, TIMEOUT_200MS);
                }
            }
            return false;
        }

        /// <summary>
        /// 取消芯片写保护，操作成功之后芯片会重启
        /// </summary>
        /// <returns>取消成功返回true，否则返回false</returns>
        public bool WriteUnProtect()
        {
            if (this.SendCommand(WRITE_UNPROTECT_COMMAND))
            {
                int retryCount = 0;
                while (this.port2.BytesToRead < 1 && retryCount++ < 0xfff) ;
                return (this.port2.ReadByte() == ACK_REPLY);
            }
            return false;
        }

        /// <summary>
        /// 对芯片执行读保护操作，操作成功之后芯片会重启
        /// </summary>
        /// <returns>操作成功返回true，否则返回false</returns>
        public bool ReadOutProtect()
        {
            port2.WriteTimeout = 500;
            port2.ReadTimeout = 500;

            port2.Write(new byte[] { READOUT_PROTECT_COMMAND }, 0, 1);
            Thread.Sleep(10);
            port2.Write(new byte[] { (byte)(READOUT_PROTECT_COMMAND ^ 0xff) }, 0, 1);
            Thread.Sleep(500);
            try
            {
                 byte a = (byte)this.port2.ReadByte();
                 _TextBox.Text += "ReadOutProtect: " + " a: 0x" + a.ToString("X2") + "\r\n";
                 return (a == ACK_REPLY);
             }
             catch (Exception)
            {
                _TextBox.Text += "RReadOutProtecterror: " + "\r\n";
                 return false;
             }
   
        }

        /// <summary>
        /// 取消芯片读保护功能.！！注意取消读保护会擦除整个芯片
        /// </summary>
        /// <returns>操作成功返回true，否则返回false</returns>
        public bool ReadOutUnProtect()
        {
            port2.WriteTimeout = 500;
            port2.ReadTimeout = 500;

            port2.Write(new byte[] { READOUT_UNPROTECT_COMMAND }, 0, 1);
            Thread.Sleep(10);
            port2.Write(new byte[] { (byte)(READOUT_UNPROTECT_COMMAND ^ 0xff) }, 0, 1);
            try
            {
                byte a;
                int count = 0;
                while (true)
                {
                    if(this.port2.BytesToRead > 0)
                    {
                        a = (byte)this.port2.ReadByte();
                        _TextBox.Text += "ReadOutUnProtect: " + " a: 0x" + a.ToString("X2") + "\r\n";
                        if (a == ACK_REPLY)
                            break;
                    }
                    count++;
                    //if(count > 500)
                    //{
                    //    return false;
                    //}
                    //_TextBox.Text += count.ToString();
                    Thread.Sleep(1);
                }
                Thread.Sleep(2000);
                return (a == ACK_REPLY);
            }
            catch (Exception)
            {
               _TextBox.Text += "ReadOutUnProtecterror: " + "\r\n";
               return false;
            }
        }
        #endregion

        #region 烧录功能

        public List<FirmwareInfomation> GetDataToWrite(ref DeviceOperation result)
        {
            List<FirmwareInfomation> firmwares = new List<FirmwareInfomation>();

            if (this._check_file1.Checked)
            {
                FirmwareInfomation firmware = this.ReadFile(this._file_path1.Text); ///读取烧写文件
                if (firmware.Error != null)
                {
                    _TextBox.AppendText(firmware.Error.Message + "\r\n");
                    return null;
                }
                result.File1 = this._file_path1.Text;
                firmwares.Add(firmware);
            }
            else if (this._check_file2.Checked)
            {
                FirmwareInfomation firmware = this.ReadFile(this._file_path2.Text);
                if (firmware.Error != null)
                {
                    _TextBox.AppendText(firmware.Error.Message + "\r\n");
                    return null;
                }
                result.File2 = this._file_path2.Text;
                firmwares.Add(firmware);
            }
            else if (this._check_file3.Checked)
            {
                FirmwareInfomation firmware = this.ReadFile(this._file_path3.Text);
                if (firmware.Error != null)
                {
                    _TextBox.AppendText(firmware.Error.Message + "\r\n");
                    return null;
                }
                result.File3 = this._file_path3.Text;
                firmwares.Add(firmware);
            }

            //写入自定义数据
            if(dt.Rows.Count>0)
            for (int i = 0; i < dt.Rows.Count - 1; i++)
            {
                if (dt.Rows[i].Cells[0].Value.Equals(true))
                {
                    UInt32 addr;
                    if (!UInt32.TryParse(dt.Rows[i].Cells[1].Value.ToString(), System.Globalization.NumberStyles.HexNumber, null, out addr)
                        || addr <= ISPProgramer.FLASH_BASE_ADDR || addr % 4 != 0)
                    {
                        _TextBox.AppendText("自定义数据" + (i + 1) + "行地址" + dt.Rows[i].Cells[1].Value.ToString() + "非法\r\n");
                        return null;
                    }
                    FirmwareInfomation customRow = new FirmwareInfomation();
                    customRow.BaseAddress = addr;

                    if (dt.Rows[i].Cells[2].Value != null && dt.Rows[i].Cells[2].Value.Equals(true)) ///选择HEX格式
                    {
                        try
                        {
                            customRow.Data = dt.Rows[i].Cells[3].Value.ToString().ToByteArray();
                        }
                        catch (Exception)
                        {
                            _TextBox.AppendText("16进制数据格式错误");
                            return null;
                        }
                    }
                    else
                    {
                        customRow.Data = Encoding.ASCII.GetBytes(dt.Rows[i].Cells[3].Value.ToString() + "\0");
                    }
                    if (dt.Rows[i].Cells[4].Value != null)
                        customRow.Name = dt.Rows[i].Cells[4].Value.ToString();
                    else
                        customRow.Name = "";
                    firmwares.Add(customRow);
                }
            }

            return firmwares;
        }

        public FirmwareInfomation ReadFile(string name)
        {
            FirmwareInfomation result = new FirmwareInfomation();
            byte[] data = null;
            if (name.EndsWith(".bin"))
            {
                data = FilesOperator.ReadBin(name);
                if (data != null)
                {
                    result.Name = name;
                    result.Data = data;
                    result.BaseAddress = ISPProgramer.FLASH_BASE_ADDR;
                }
                else
                {
                    result.Error = new Exception("读取文件" + name + "失败");
                }
            }
            else if (name.EndsWith(".hex"))
            {
                result = FilesOperator.ReadHex(name);            
                _TextBox.AppendText(result.BaseAddress.ToString("X8"));
            }
            else
            {
                this._TextBox.AppendText("读取文件" + name + "失败\r\n");
            }
            return result;
        }

        public BackgroundWorker WriteData(List<FirmwareInfomation> list, Action<bool> NextAction)
        {
            BackgroundWorker writeWork = new BackgroundWorker();
            writeWork.WorkerReportsProgress = true;
            writeWork.WorkerSupportsCancellation = true;
            writeWork.ProgressChanged += WriteWork_ProgressChanged; ///添加烧写进度条
            //writeWork.DoWork += new DoWorkEventHandler(delegate (Object o, DoWorkEventArgs e)
            //{
                //BackgroundWorker worker = (BackgroundWorker)o;
                bool ret = true;
                //if (e.Argument == null || (bool)e.Argument)
                {
                    //lock (_isp)
                    {
                    //this._work.Add(worker);
                        foreach (FirmwareInfomation item in list)
                        {
                            //if (worker.CancellationPending == true)
                            //{
                            //    e.Cancel = true;
                            //    break;
                            //}
                            //worker.ReportProgress(0, "开始烧录" + item.Name + "\r\n");
                            _TextBox.AppendText("读取文件"+ item.BaseAddress  + "\r\n");
                            ret = this.WriteFlash(item.BaseAddress, item.Data);
                            if (!ret)
                            {
                                //worker.ReportProgress(0, "写入出错\r\n");
                                _TextBox.AppendText("写入出错\r\n");
                                break;
                            }
                            else
                            {
                            //worker.ReportProgress(0, "开始校验\r\n");
                               _TextBox.AppendText("开始校验\r\n");
                               byte[] d = this._isp.ReadFlash(item.BaseAddress, (UInt32)item.Data.Length);
                               ret = (d != null && BitConverter.ToString(d).Equals(BitConverter.ToString(item.Data)));
                                if (!ret)
                                {
                                    //worker.ReportProgress(0, "数据校验失败\r\n");
                                    _TextBox.AppendText("数据校验失败\r\n");
                                    break;
                                }
                                else
                                {
                                //worker.ReportProgress(0, Convert.ToString(item.BaseAddress, 16) + "写入" + item.Data.Length + "字节并校验成功\r\n");
                                    _TextBox.AppendText(Convert.ToString(item.BaseAddress, 16) + "写入" + item.Data.Length + "字节并校验成功\r\n");
                                }
                            }
                        }
                        this._isp.Close();
                        //e.Result = ret;
                    }
                }
                //else
                //{
                //    e.Result = false;
                //    e.Cancel = true;
                //}

            //});
            //writeWork.RunWorkerCompleted += new RunWorkerCompletedEventHandler(delegate (Object o, RunWorkerCompletedEventArgs e)
            //{
            //    this._work.Remove((BackgroundWorker)o);
            //    bool ret = false;
            //    if (e.Cancelled == true)
            //    {
            //        this._TextBox.AppendText("写入取消\r\n");
            //    }
            //    else if (e.Error != null)
            //    {
            //        this._TextBox.AppendText("写入中断:" + e.Error.Message + "\r\n");
            //        FilesOperator.SaveLog("error.log", e.Error);
            //    }
            //    else if (e.Result != null)
            //    {
            //        ret = (bool)e.Result;
            //    }
            //    if (NextAction != null)
            //    {
            //        NextAction(ret);
            //    }

            //});
            return writeWork;
        }

        /// <summary>
        /// 进度条
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WriteWork_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //if (this.progressBar.InvokeRequired)
            //{
            //    this.progressBar.BeginInvoke(new Action(() => this.progressBar.Value = e.ProgressPercentage));
            //}
            //else
            //{
            //    this.progressBar.Value = e.ProgressPercentage;
            //}

            //if (e.UserState != null && e.UserState is string)
            //{
            //    string str = e.UserState as string;
            //    this.ShowMessage(str);
            //}
        }

        public BackgroundWorker ReadData(UInt32 startAddr, UInt32 length, Action<byte[]> NextAction)
        {
            BackgroundWorker readWork = new BackgroundWorker();
            readWork.WorkerReportsProgress = true;
            readWork.WorkerSupportsCancellation = true;
            readWork.ProgressChanged += this.ProgressChanged;
            readWork.DoWork += new DoWorkEventHandler(delegate (Object readWorkObject, DoWorkEventArgs readWorkEventArgs)
            {
                if (readWorkEventArgs.Argument == null || (bool)readWorkEventArgs.Argument)
                {
                    //lock (isp)
                    //{
                    //    this.workers.Add((BackgroundWorker)readWorkObject);
                    //    readWorkEventArgs.Result = this.isp.ReadFlash(startAddr, length);
                    //}
                }
                else
                {
                    readWorkEventArgs.Result = null;
                    readWorkEventArgs.Cancel = true;
                }
            });
            readWork.RunWorkerCompleted += new RunWorkerCompletedEventHandler(delegate (Object readWorkObject, RunWorkerCompletedEventArgs readCompletedEventArgs)
            {
                //this.workers.Remove((BackgroundWorker)readWorkObject);
                //this.button_Write.SetOnFreeMode();
                if (readCompletedEventArgs.Cancelled == true)
                {
                    this._TextBox.AppendText("数据读取取消\r\n");
                }
                else if (readCompletedEventArgs.Error != null)
                {
                    this._TextBox.AppendText("读取出错:" + readCompletedEventArgs.Error.Message);
                    FilesOperator.SaveLog("error.log", readCompletedEventArgs.Error);
                }
                if (NextAction != null)
                {
                    NextAction(readCompletedEventArgs.Result as byte[]);
                }
            });
            return readWork;
        }

        #endregion
    }

}
