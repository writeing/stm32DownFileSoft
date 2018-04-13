using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SerialPoart
{
    public class ISPProgramer 
    {
        public const UInt32 FLASH_BASE_ADDR = 0x08000000;
        private const UInt32 UNIQUE_ID_ADDR = 0x1FFFF7E8;
        private const UInt32 OPTION_ADDR = 0x1FFFF800;
        private const UInt32 CHIP_SIZE_ADDR = 0x1FFFF7E0;

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

        private const int TIMEOUT_50MS = 50;
        private const int TIMEOUT_200MS = 200;
        private const int TIMEOUT_500MS = 500;
        private const int TIMEOUT_1000MS = 1000;
        private const int TIMEOUT_2000MS = 2000;
       
        private TextBox _TextBox = null;

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
            LOW_DENSITY = 0x12,
            MEDIUM_DENSITY = 0x10,
            HIGH_DENSITY = 0x14,
            XL_DENSITY = 0x30,
            CONNECTIVITY_DENSITY = 0x18,
            UNKNOW_DENSITY = NACK_REPLY
        };

        public string UniqueID { get; private set; }
        public string OptionByte { get; private set; }
        public float BootloaderVer { get; private set; }
        public DensityType ChipType { get; private set; }
        public UInt32 ChipSize { get; private set; }
        public UInt32 PageSize { get; private set; }
        public string PortName { get { return this.port.PortName; } }
        public int PortBaudRate { get { return this.port.BaudRate; } }
        public System.IO.Ports.SerialPort port = null;
        public event ProgressChangedEventHandler ProgressChanged = null;
        private bool cancelPending = false;
       

        private void ReportProgress(int pec, object o)
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
        public bool Init(string portName, int baudRate)
        {
            return this.Init(portName, baudRate, InitialType.DTR_HIGH_REBOOT_RTS_HIGH_ENTERBOOTLOADER);
        }

        public bool Init(string portName, int baudRate, InitialType index)
        {
            int type = (int)index - 1;

            int rebootCount = 0;
            while (rebootCount++ < 2)
            {
                this.port.WriteTimeout = TIMEOUT_50MS;
                this.port.ReadTimeout = TIMEOUT_50MS;
                try
                {
                    if (this.port.IsOpen)
                    {
                        this.port.Close();
                    }
                    this.port.PortName = portName;
                    this.port.BaudRate = baudRate;
                    this.port.Open();

                }
                catch (Exception)
                {
                    _TextBox.AppendText("Exception");
                    return false;
                }
                if (!this.port.IsOpen)
                {
                    _TextBox.AppendText("!this.port.IsOpen");
                    return false;
                }
                if (type >= 0)
                {
                    bool rtsReboot = (type / 6) > 0;
                    int enterBootloader = type % 3;
                    bool rebootLevel = ((type % 6) / 3) > 0;
                    if (rtsReboot)
                    {
                        this.port.RtsEnable = rebootLevel;
                    }
                    else
                    {
                        this.port.DtrEnable = rebootLevel;
                    }
                    if (enterBootloader > 0)
                    {
                        if (rtsReboot)
                        {
                            //this.port.DtrEnable = enterBootloader == 1;
                            this.port.DtrEnable = enterBootloader > 1;
                        }
                        else
                        {
                            //this.port.RtsEnable = enterBootloader == 1;
                            this.port.RtsEnable = enterBootloader > 1;
                        }
                    }
                    if (rtsReboot)
                    {
                        this.port.RtsEnable = !rebootLevel;
                    }
                    else
                    {
                        this.port.DtrEnable = !rebootLevel;
                    }
                }

                int successCount = 0;
                for (int i = 0, interval = 1; i < 8 && successCount < 2; i++, interval++)
                {
                    port.Write(new byte[] { INIT_COMMAND }, 0, 1);
                    try
                    {
                        byte reply = (byte)this.port.ReadByte();
                        if (reply == ACK_REPLY)
                        {
                            successCount = 1;
                            interval = 0;
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
                if (successCount == 2)
                {
                    this.UniqueID = this.ReadUinqueID();
                    this.OptionByte = this.ReadOptionByte();
                    this.ChipType = this.ReadDensity();
                    this.BootloaderVer = this.ReadBootloaderVer();
                    this.ChipSize = this.ReadChipSize() * (UInt32)1024;
                    return true;
                    //todo:完善读写保护时的初始化逻辑
                }

            }
            this.Close();
            return false;
        }

        public void Close()
        {
            if (this.port.IsOpen)
            {
                this.port.Close();
            }
        }
        #endregion

        #region 串口命令
        private bool SendCommand(byte[] data, int timeout)
        {
            byte bcc = 0;
            for (int i = 0; i < data.Length; i++)
            {
                port.Write(data, i, 1);
                bcc ^= data[i];
            }
            port.Write(new byte[] { bcc }, 0, 1);
            port.ReadTimeout = timeout;
            try
            {
                byte a = (byte)this.port.ReadByte();
                return (a == ACK_REPLY);
            }
            catch (Exception)
            {
                return false;
            }
        }

        private bool SendCommand(byte data)
        {
            port.Write(new byte[] { data }, 0, 1);
            port.Write(new byte[] { (byte)(data ^ 0xff) }, 0, 1);
            port.ReadTimeout = 200;
            try
            {
                byte a = (byte)this.port.ReadByte();
                return (a == ACK_REPLY);
            }
            catch (Exception)
            {
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
                case DensityType.LOW_DENSITY:
                case DensityType.MEDIUM_DENSITY:
                    this.PageSize = 1024;
                    break;
                case DensityType.HIGH_DENSITY:
                case DensityType.XL_DENSITY:
                case DensityType.CONNECTIVITY_DENSITY:
                    this.PageSize = 2048;
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
            byte[] s = this.Read(OPTION_ADDR.ToAddressArray(), 15);
            if (s != null)
            {
                return BitConverter.ToString(s).Replace("-", "");
            }
            return null;
        }
        #endregion

        #region flash操作
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

                this.ReportProgress((int)(offset / length));
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

                this.ReportProgress((int)((offset * 100) / (length + offset)));
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

            this.ReportProgress(0, "检查flash...\r\n");

            if (!this.EraseFlash(address, length))
            {
                this.ReportProgress(0, "flash擦除失败\r\n");
                return false;
            }

            this.ReportProgress(0, "开始写入...\r\n");

            UInt32 offset = 0;
            while (length > 0)
            {
                UInt32 inPageAddr = address - FLASH_BASE_ADDR + offset;
                byte page = (byte)((inPageAddr) / PageSize);
                byte len = length > 0x100 ? (byte)0xff : (byte)(length - 1);
                byte[] wData = new byte[len + 1];
                Array.Copy(data, offset, wData, 0, len + 1);
                if (!this.Write((address + offset).ToAddressArray(), wData))
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

        /// <summary>擦除指定范围的flash数据</summary>
        /// <param name="address">起始地址，必须为4的倍数</param>
        /// <param name="length">擦除长度，必须为4的倍数对齐，如果不为4的倍数将擦除到4的倍数对齐</param>
        /// <returns>擦除成功返回true，擦除失败返回false</returns>
        public bool EraseFlash(UInt32 address, UInt32 length)
        {
            if (length % 4 != 0)
            {
                length += (4 - length % 4);
            }

            UInt32 startOffset = address - FLASH_BASE_ADDR;
            UInt32 startRemain = startOffset % PageSize;
            byte[] startRemainData = null;
            byte startPage = (byte)(startOffset / PageSize);
            UInt32 taleOffset = startOffset + length;
            UInt32 taleRemain = taleOffset % PageSize;
            byte talePage = (byte)((startOffset + length) / PageSize);
            byte[] taleRemainData = null;

            List<byte> pagesToErase = new List<byte>();
            byte page = startPage;
            {
                if (startRemain != 0)
                {
                    byte[] pageData = new byte[PageSize];
                    for (UInt32 offset = 0; offset < PageSize; offset += 0x100)
                    {
                        UInt32 add = FLASH_BASE_ADDR + page * PageSize + offset;
                        byte[] data = this.Read(add.ToAddressArray(), 0xff);
                        Array.Copy(data, 0, pageData, offset, data.Length);
                    }
                    if (startPage != talePage)
                    {
                        if (!pageData.IsEmpty(startRemain, PageSize - startRemain))
                        {
                            pagesToErase.Add(startPage);
                            if (!pageData.IsEmpty(0, startRemain))
                            {
                                startRemainData = new byte[startRemain];
                                Array.Copy(pageData, startRemainData, startRemain);
                            }
                        }
                    }
                    else
                    {
                        if (!pageData.IsEmpty(startRemain, length))
                        {
                            pagesToErase.Add(startPage);
                            if (!pageData.IsEmpty(0, startRemain))
                            {
                                startRemainData = new byte[startRemain];
                                Array.Copy(pageData, startRemainData, startRemain);
                            }
                        }
                    }

                    page++;
                }
                for (; page < talePage; page++)
                {
                    for (UInt32 offset = 0; offset < PageSize; offset += 0x100)
                    {
                        UInt32 add = FLASH_BASE_ADDR + page * PageSize + offset;
                        byte[] data = this.Read(add.ToAddressArray(), 0xff);
                        if (!data.IsEmpty())
                        {
                            pagesToErase.Add(page);
                            break;
                        }

                        this.ReportProgress((100 * (page - startPage)) / (talePage - startPage));
                    }
                }
                if (taleRemain != 0)
                {
                    if (startPage == talePage)
                    {
                        if (pagesToErase.Contains(talePage))
                        {
                            taleRemainData = new byte[PageSize - taleRemain];
                            UInt32 offset = taleRemain;
                            while (offset < PageSize)
                            {
                                UInt32 add = FLASH_BASE_ADDR + talePage * PageSize + offset;
                                UInt32 len = Math.Min(0x100, (PageSize - offset));
                                byte[] data = this.Read(add.ToAddressArray(), (byte)(len - 1));
                                Array.Copy(data, 0, taleRemainData, offset - taleRemain, data.Length);
                                offset += len;
                            }
                        }
                    }
                    else
                    {
                        byte[] pageData = new byte[PageSize];
                        for (UInt32 offset = 0; offset < PageSize; offset += 0x100)
                        {
                            UInt32 add = FLASH_BASE_ADDR + talePage * PageSize + offset;
                            byte[] data = this.Read(add.ToAddressArray(), 0xff);
                            Array.Copy(data, 0, pageData, offset, data.Length);
                        }
                        if (!pageData.IsEmpty(0, taleRemain))
                        {
                            pagesToErase.Add(talePage);
                            if (!pageData.IsEmpty(taleRemain, PageSize - taleRemain))
                            {
                                taleRemainData = new byte[PageSize - taleRemain];
                                Array.Copy(pageData, taleRemain, taleRemainData, 0, PageSize - taleRemain);
                            }
                        }
                    }

                }
            }

            if (pagesToErase.Count == 0)
            {
                return true;
            }

            if (this.BootloaderVer < (float)3)
            {
                if (!this.Erase(pagesToErase.ToArray()))
                {
                    return false;
                }
            }
            else
            {
                List<ushort> pagesToErase16 = new List<ushort>();
                foreach (var item in pagesToErase)
                {
                    pagesToErase16.Add(item);
                }
                if (!this.ExternErase(pagesToErase16.ToArray()))
                {
                    return false;
                }
            }

            if (startRemainData != null)
            {
                UInt32 offset = 0;
                UInt32 addr = FLASH_BASE_ADDR + startPage * PageSize;
                while (offset < startRemain)
                {
                    UInt32 len = Math.Min(0x100, (startRemain - offset));
                    byte[] data = new byte[len];
                    Array.Copy(startRemainData, offset, data, 0, len);
                    if (!this.Write(addr.ToAddressArray(), data))
                        return false;
                    offset += len;
                    addr += len;
                }
            }
            if (taleRemainData != null)
            {
                UInt32 offset = 0;
                UInt32 addr = FLASH_BASE_ADDR + talePage * PageSize + taleRemain;
                UInt32 remain = PageSize - taleRemain;
                while (offset < remain)
                {
                    UInt32 len = Math.Min(0x100, (remain - offset));
                    byte[] data = new byte[len];
                    Array.Copy(taleRemainData, offset, data, 0, len);
                    if (!this.Write(addr.ToAddressArray(), data))
                        return false;
                    offset += len;
                    addr += len;
                }
            }

            return true;
        }

        /// <summary>擦除整个芯片</summary>
        /// <returns>擦除结果</returns>
        public bool EraseChip()
        {
            if (this.BootloaderVer > (float)2.2)
            {
                return this.ExternErase(0xffff);    //f030xC
            }
            else
            {
                return this.Erase(0xff);            //f103xx
            }
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
                byte len = length > 0x100 ? (byte)0xff : (byte)(length - 1);
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
                return this.SendCommand(data, TIMEOUT_500MS * (pages.Length / 16 + 1));       //每增加16k擦除空间增加500ms等待超时时间
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
                    return this.SendCommand(new byte[] { 0, index }, TIMEOUT_1000MS);
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
                return this.SendCommand(new byte[] { (byte)(command >> 8), (byte)(command & 0xff) }, TIMEOUT_500MS);
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
                if (this.SendCommand(new byte[] { (byte)(pages.Length >> 8), (byte)(pages.Length & 0xff) }, TIMEOUT_200MS))
                {
                    byte[] data = new byte[pages.Length * 2];
                    for (int i = 0; i < pages.Length; i++)
                    {
                        data[i * 2] = (byte)(pages[i] >> 8);
                        data[i * 2 + 1] = (byte)(pages[i] & 0xff);
                    }
                    return (this.SendCommand(data, TIMEOUT_500MS * (pages.Length / 16 + 1)));   //每增加16k擦除空间增加500ms等待超时时间
                }
            }
            return false;
        }

        /// <summary>
        /// 向地址读取写入指定数据
        /// </summary>
        /// <param name="addr">4字节地址，msb在前，必须为4的倍数对齐，否则写入失败</param>
        /// <param name="data">data长度应不大于0x100否则会写入失败，且必须为4的倍数对齐，否则虽然不会报错但是末尾会被未知数据填充</param>
        /// <returns></returns>
        private bool Write(byte[] addr, byte[] data)
        {
            if (this.SendCommand(WRITE_MEM_COMMAND))
            {
                if (this.SendCommand(addr, TIMEOUT_200MS))
                {
                    byte[] wData = new byte[data.Length + 1];
                    wData[0] = (byte)(data.Length - 1);
                    Array.Copy(data, 0, wData, 1, data.Length);
                    return this.SendCommand(wData, TIMEOUT_200MS);
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
            if (this.SendCommand(READ_MEM_COMMAND))
            {
                if (this.SendCommand(addr, TIMEOUT_200MS))
                {
                    int retryCount = 0;
                    if (this.SendCommand(length))
                    {
                        byte[] data = new byte[length + 1];
                        for (UInt32 i = 0; i <= length && retryCount < 0xfff; i++)
                        {
                            if (port.BytesToRead == 0)
                            {
                                i--;
                                retryCount++;
                                continue;
                            }
                            data[i] = (byte)this.port.ReadByte();
                        }
                        if (port.BytesToRead != 0)
                        {
                            port.ReadExisting();
                            return null;
                        }
                        if (retryCount == 0xfff)
                        {
                            return null;
                        }
                        return data;
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
                byte low, high, count;
                int retryCount = 0;
                while (this.port.BytesToRead < 4 && retryCount++ < 0xfff) ;
                count = (byte)this.port.ReadByte();
                high = (byte)this.port.ReadByte();
                low = (byte)this.port.ReadByte();
                if (count == 0x1 && this.port.ReadByte() == ACK_REPLY && high == 0x04 && this.port.BytesToRead == 0)
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
                byte op1, op2, ver;
                int retryCount = 0;
                while (this.port.BytesToRead < 4 && retryCount++ < 0xfff) ;
                ver = (byte)this.port.ReadByte();
                op2 = (byte)this.port.ReadByte();
                op1 = (byte)this.port.ReadByte();
                if (this.port.ReadByte() == ACK_REPLY && op1 == 0x0 && op2 == 0x0 && this.port.BytesToRead == 0)
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
                return this.SendCommand(addr, TIMEOUT_200MS);
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
                    return this.SendCommand(data, TIMEOUT_200MS);
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
                while (this.port.BytesToRead < 1 && retryCount++ < 0xfff) ;
                return (this.port.ReadByte() == ACK_REPLY);
            }
            return false;
        }

        /// <summary>
        /// 对芯片执行读保护操作，操作成功之后芯片会重启
        /// </summary>
        /// <returns>操作成功返回true，否则返回false</returns>
        public bool ReadOutProtect()
        {
            if (this.SendCommand(READOUT_PROTECT_COMMAND))
            {
                int retryCount = 0;
                while (this.port.BytesToRead < 1 && retryCount++ < 0xfff) ;
                return (this.port.ReadByte() == ACK_REPLY);
            }
            return false;
        }

        /// <summary>
        /// 取消芯片读保护功能.！！注意取消读保护会擦除整个芯片
        /// </summary>
        /// <returns>操作成功返回true，否则返回false</returns>
        public bool ReadOutUnProtect()
        {
            if (this.SendCommand(READOUT_UNPROTECT_COMMAND))
            {
                int retryCount = 0;
                while (this.port.BytesToRead < 1 && retryCount++ < 0xfff) ;
                return (this.port.ReadByte() == ACK_REPLY);
            }
            return false;
        }
        #endregion
    }

    public static class DataHelper
    {
        public static bool IsEmpty(this byte[] data)
        {
            if (data == null)
            {
                return true;
            }
            foreach (byte b in data)
            {
                if (b != 0xff)
                {
                    return false;
                }
            }
            return true;
        }

        public static bool IsEmpty(this byte[] data, long startOffset, long length)
        {
            if (data == null)
            {
                return true;
            }
            if (startOffset + length > data.LongLength)
            {
                throw new IndexOutOfRangeException();
            }
            for (long i = startOffset; i < startOffset + length; i++)
            {
                if (data[i] != 0xff)
                {
                    return false;
                }
            }
            return true;
        }

        public static void FillEmpty(this byte[] data)
        {
            for (long i = 0; i < data.LongLength; i++)
            {
                data[i] = 0xff;
            }
        }

        public static byte[] GetEffectiveData(this byte[] data)
        {
            if (data == null)
            {
                return null;
            }
            long length;
            for (length = data.LongLength - 1; length > 0 && data[length] == 0xff; length--) ;
            if (length == 0)
            {
                return null;
            }
            else if (length == data.LongLength - 1)
            {
                return data;
            }
            else
            {
                byte[] result = new byte[length + 1];
                Array.Copy(data, 0, result, 0, length + 1);
                return result;
            }
        }

        /// <summary>
        /// 反转数组里面的元素
        /// </summary>
        /// <param name="data">待转换数组，转换完成该数组本身会被反转</param>
        /// <returns>返回反转后的数组也就是原始数据本身</returns>
        public static TResult[] Reverse<TResult>(this TResult[] data)
        {
            Array.Reverse(data);
            return data;
        }

        public static byte[] ToByteArray(this string hexString)
        {
            hexString = hexString.Replace(" ", "");
            if ((hexString.Length % 2) != 0)
                hexString = "0" + hexString;
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            return returnBytes;
        }

        public static byte[] ToAddressArray(this UInt32 addr)
        {
            byte[] address = BitConverter.GetBytes(addr);
            Array.Reverse(address); ///MSB转LSB：大-小端
            return address;
        }
    }
}
