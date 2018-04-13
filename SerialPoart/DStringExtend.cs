using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DCOM_V1._0
{
    class DStringExtend
    {
        //将字符变为16进制显示
        public string StringToHexString(string s)
        {
            string str = null;
            string str_hex = null;
            string str_buf = null;

            str = s;//取得字符
                    //转为16进制
            for (int i = 0; i < str.Length; i++)
            {
                str_buf = Convert.ToString((byte)str[i], 16).ToUpper();
                str_hex += (" " + (str_buf.Length == 1 ? "0" + str_buf : str_buf));
            }

            return str_hex;
        }

        //将16进制显示变为字符显示
        public string HexStringToString(string s)
        {
            Int32 i;
            string str = null;
            string str_hex = null;

            //将输入数据去掉空格符
            for (i = 0; i < s.Length; i++)
            {
                if (s[i] != ' ' && IsHexadecimal(s[i]))
                {
                    str_hex += s[i];
                }
            }

            if (str_hex == null)
                return null;

            Int32 len = str_hex.Length / 2;
            string c = null;
            byte[] data = new byte[len];
            for (i = 0; i < len; i++)
            {
                c += str_hex[2 * i];
                c += str_hex[2 * i + 1];
                data[i] = (byte)Convert.ToInt32(c, 16);
                c = null;
            }
            str = Encoding.Unicode.GetString(data);
            return str;
        }

        //判断这串数据是否为16进制格式
        public bool IsHexaStringdecimal(string str)
        {
            Int32 i;
            const string PATTERN = @"[A-Fa-f0-9]+$";

            string str_hex = null;

            if (str == null)
                return true;

            //将输入数据去掉空格符
            for (i = 0; i < str.Length; i++)
            {
                if (str[i] != ' ')
                {
                    str_hex += str[i];
                }
            }

            if (str_hex == null)
                return true;

            return System.Text.RegularExpressions.Regex.IsMatch(str_hex, PATTERN);
        }

        //判断这个字节是否为16进制
        public bool IsHexadecimal(char str)
        {
            string str_check = null;
            str_check += str;
            const string PATTERN = @"[A-Fa-f0-9]+$";
            return System.Text.RegularExpressions.Regex.IsMatch(str_check, PATTERN);
        }

        public string RemoveKG(string s)
        {
            Int32 i;
            string str = null;

            //将输入数据去掉空格符
            for (i = 0; i < s.Length; i++)
            {
                if (s[i] != ' ' && IsHexadecimal(s[i]))
                {
                    str += s[i];
                }
            }

            if (str == null)
                return null;

            return str;
        }


        public string strTextTostrBin(string strText)
        {
            byte[] bytearr = null;
            string str_buf = null;

            string stringtobin = "";
            System.Text.Encoding encoding = System.Text.Encoding.Unicode;
            bytearr = encoding.GetBytes(strText);

            for (int i = 0; i < bytearr.Length; i++)
            {
                //stringtobin += "," + bytearr[i].ToString();
                str_buf = Convert.ToString((byte)bytearr[i], 16).ToUpper();
                stringtobin += (" " + (str_buf.Length == 1 ? "0" + str_buf : str_buf));
            }
            return stringtobin.Substring(1);

        }

        public string strBinTostrText(string strBin)
        {
            string[] bintostr = strBin.Split(',');
            Array binArray = Array.CreateInstance(Type.GetType("System.Byte"), bintostr.Length);
            for (int i = binArray.GetLowerBound(0); i <= binArray.GetUpperBound(0); i++)
            {
                binArray.SetValue(byte.Parse(bintostr[i] + ""), i);
            }

            byte[] strtobin = new byte[bintostr.Length];
            for (int i = binArray.GetLowerBound(0); i <= binArray.GetUpperBound(0); i++)
            {
                strtobin[i] = (byte)binArray.GetValue(i);
            }
            System.Text.Encoding encoding = System.Text.Encoding.UTF8;
            return encoding.GetString(strtobin);
        }


        public string HexToASCII(string Msg)
        {
            byte[] buff = new byte[Msg.Length / 2];
            string Message = "";
            for (int i = 0; i < buff.Length; i++)
            {
                buff[i] = byte.Parse(Msg.Substring(i * 2, 2),
                   System.Globalization.NumberStyles.HexNumber);
            }

            System.Text.Encoding chs = System.Text.Encoding.ASCII;
            Message = chs.GetString(buff);
            return Message;
        }

        public string HexToStr(string Msg)
        {
            // byte[] buff = new byte[Msg.Length / 2];
            string Message = "";

            string str_hex = null;
            Int32 i;

            if (Msg != null)
            {
                //将输入数据去掉空格符
                for (i = 0; i < Msg.Length; i++)
                {
                    if (Msg[i] != ' ' && IsHexadecimal(Msg[i]))
                    {
                        str_hex += Msg[i];
                    }
                }

                byte[] buff = new byte[str_hex.Length / 2];

                for (i = 0; i < buff.Length; i++)
                {
                    buff[i] = byte.Parse(str_hex.Substring(i * 2, 2),
                       System.Globalization.NumberStyles.HexNumber);
                }

                System.Text.Encoding chs = System.Text.Encoding.GetEncoding("gb2312");
                Message = chs.GetString(buff);
            }          
            return Message;
        }

        public string StrToHex(string Msg)
        {
            byte[] bytes = System.Text.Encoding.Default.GetBytes(Msg);
            string str = "";
            string str2 = null;
            for (int i = 0; i < bytes.Length; i++)
            {
                str2 = string.Format("{0:X}", bytes[i]);
                str += (" " + (str2.Length == 1 ? "0" + str2 : str2));
            }
            return str;
        }


    }





}
