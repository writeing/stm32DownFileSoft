using Serial_second;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SerialPoart
{
    public class FilesOperator
    {
        public static bool SaveLog(string path, List<Exception> e)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(path, true, Encoding.Unicode))
                {
                    writer.NewLine = "\r\n";
                    writer.WriteLine("-------------------------------------------------------------------------------");
                    foreach (Exception item in e)
                    {
                        writer.WriteLine(DateTime.Now.ToString("[yyyy-MM-dd:hh:mm:ss]") + item.HelpLink + item.Message + item.Source + item.StackTrace + item.TargetSite);
                        writer.WriteLine();
                    }
                    writer.WriteLine("-------------------------------------------------------------------------------");
                    writer.Flush();
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool SaveResult(string path, DeviceOperation result)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(path, true, Encoding.Unicode))
                {
                    writer.NewLine = "\r\n";
                    writer.WriteLine(result.Time + ";" + result.UniqueID + ";" + result.OptionByte + ";" + result.ChipType + ";" + result.BootloaderVer + ";" + result.File1 + ";" + result.File2 + ";" + result.FirmwareVer + ";" + result.SerialID);
                    writer.Flush();
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool SaveResult(string path, string result)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(path, true, Encoding.Unicode))
                {
                    writer.Write(result);
                    writer.Flush();
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool SaveBin(string name, byte[] data)
        {
            try
            {
                using (FileStream fs = File.Open(name, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    long length = data.LongLength;
                    int len = 0;
                    int offset = 0;
                    do
                    {
                        len = length < Int32.MaxValue ? (int)length : Int32.MaxValue;
                        fs.Write(data, 0, len);
                        offset += len;
                        length -= len;
                    } while (length > 0);

                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        public static byte[] ReadBin(string name)
        {
            try
            {
                using (FileStream fs = File.Open(name, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    long length = fs.Length;
                    int len = 0;
                    byte[] result = new byte[length];
                    do
                    {
                        len = length < Int32.MaxValue ? (int)length : Int32.MaxValue;
                        fs.Read(result, 0, len);
                        length -= len;
                    } while (length > 0);

                    return result;
                }
            }
            catch
            {
                return null;
            }
        }

        public static FirmwareInfomation ReadHex(string name)
        {
            FirmwareInfomation firmware = new FirmwareInfomation();
            firmware.Name = name;
            try
            {
                using (StreamReader sr = new StreamReader(name))
                {
                    byte[] data = new byte[1024 * 128];
                    for (long i = 0; i < data.LongLength; i++)
                    {
                        data[i] = 0xff;
                    }
                    bool hasBaseAddr = false;
                    UInt32 baseOffset = 0;
                    UInt16 offset = 0;
                    int line = 0;
                    while (!sr.EndOfStream)
                    {
                        string s = sr.ReadLine();
                        line++;
                        if (s.StartsWith(":") && s.Length >= 11)
                        {
                            byte orr = 0;
                            byte len = Convert.ToByte(s.Substring(1, 2), 16);
                            orr += len;
                            offset = Convert.ToUInt16(s.Substring(3, 4), 16);
                            orr += Convert.ToByte(s.Substring(3, 2), 16);
                            orr += Convert.ToByte(s.Substring(5, 2), 16);
                            byte type = Convert.ToByte(s.Substring(7, 2), 16);
                            orr += type;

                            if (s.Length != len * 2 + 11)
                            {
                                firmware.Error = new Exception("文件不符合hexfrmt标准");
                                return firmware;
                            }

                            byte[] load = new byte[len];
                            for (int i = 0; i < load.Length; i++)
                            {
                                load[i] = Convert.ToByte(s.Substring(i * 2 + 9, 2), 16);
                                orr += load[i];
                            }

                            orr += Convert.ToByte(s.Substring(s.Length - 2, 2), 16);

                            if (orr != 0)
                            {
                                firmware.Error = new Exception("文件不符合hexfrmt标准，" + line + "行校验和失败");
                                return firmware;
                            }

                            switch (type)
                            {
                                case 0:
                                    if (!hasBaseAddr && firmware.BaseAddress == 0)
                                    {
                                        firmware.Error = new Exception("文件不符合hexfrmt标准，" + line + "行数据记录出现在扩展线性地址前");
                                        return firmware;
                                    }
                                    else if (!hasBaseAddr)
                                    {
                                        //第一行数据记录，保存基地址偏移
                                        firmware.BaseAddress += offset;
                                        hasBaseAddr = true;
                                    }
                                    Array.Copy(load, 0, data, 0x8000000 + baseOffset + offset - firmware.BaseAddress, len);
                                    break;
                                case 1:
                                    break;
                                case 2:
                                    break;
                                case 3:
                                    break;
                                case 4:
                                    if (len == 2 && offset == 0)
                                    {
                                        if (!hasBaseAddr)
                                        {
                                            //第一行记录，保存基地址，偏移量0
                                            firmware.BaseAddress = (UInt32)(((load[0] << 8) + load[1]) << 16);
                                            baseOffset = 0;
                                        }
                                        else
                                        {
                                            //第二次出现数据记录
                                            baseOffset = (UInt32)((((load[0] << 8) + load[1]) << 16) - 0x8000000);
                                        }
                                    }
                                    break;
                                case 5:
                                    break;
                                default:
                                    break;
                            }
                        }
                        else if (!s.Equals(""))
                        {
                            firmware.Error = new Exception("文件不符合hexfrmt标准，" + line + "行非法");
                        }
                    }
                    if (hasBaseAddr)
                    {
                        firmware.Data = data.GetEffectiveData();
                    }
                    else
                    {
                        firmware.Error = new Exception("文件不包含基地址");
                    }
                    return firmware;
                }

            }
            catch (Exception e)
            {
                firmware.Error = e;
                return firmware;
            }
        }

        public static bool SaveLog(string p, Exception exception)
        {
            List<Exception> ls = new List<Exception>();
            ls.Add(exception);
            return SaveLog(p, ls);
        }
    }
    public struct DeviceOperation
    {
        public DateTime Time { get; set; }
        public string UniqueID { get; set; }
        public string OptionByte { get; set; }
        public ISPProgramerL071.DensityType ChipType { get; set; }
        public float BootloaderVer { get; set; }
        public string File1 { get; set; }
        public string File2 { get; set; }
        public string File3 { get; set; }
        public string FirmwareVer { get; set; }
        public string SerialID { get; set; }
    }

    public struct FirmwareInfomation
    {
        public string Name { get; set; }
        public UInt32 BaseAddress { get; set; }
        public byte[] Data { get; set; }
        public Exception Error { set; get; }
    }
}
