using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _7._12_debug_assistant.Service
{
    public  class FormatConert
    {
        /// <summary>
        /// 字符串转16进制字符
        /// </summary>
        /// <param name="_str">字符串</param>
        /// <param name="encode">编码格式</param>
        /// <returns></returns>
        public  static string StringToHexString(string _str, Encoding encode)
        {
            //去掉空格,Default
            _str = _str.Replace(" ", "");
            //将字符串转换成字节数组，8位二进制。10进制
            byte[] buffer = encode.GetBytes(_str);
            //定义一个string类型的变量，用于存储转换后的值。
            string result = string.Empty;
            int[] num = new int[buffer.Length];
            for (int i = 0; i < buffer.Length; i++)
            {
                //将每一个字节数组转换成16进制的字符串，以空格相隔开。
                result += Convert.ToString(buffer[i], 16) + " ";
                //num[i] = int.Parse(result1[i], System.Globalization.NumberStyles.HexNumber);
            }
            //  string result = string.Join("", num);
            return result;
        }

        /// <summary>
        /// 16进制字符转字符串
        /// </summary>
        /// <param name="hex">16进制字符</param>
        /// <param name="encode">编码格式</param>
        /// <returns></returns>
        public  static string HexStringToString(string hex, Encoding encode)
        {
            hex = hex.Replace(" ", "");
            var s = hex.Length;
            byte[] buffer = new byte[hex.Length / 2];

            string result = string.Empty;
            for (int i = 0; i < hex.Length / 2; i++)
            {
                result = hex.Substring(i * 2, 2);
                buffer[i] = Convert.ToByte(result, 16);
            }
            //返回指定编码格式的字符串
            return encode.GetString(buffer);
        }

    }
}
