using System;
using System.Collections.Generic;
using System.Text;

namespace TcpTestClient.linux
{
    public class NetworkConfig
    {

        /// <summary>
        /// 获取子掩码
        /// </summary>
        /// <param name="mark"></param>
        /// <returns></returns>
        public static int GetSubnetMarkValue(int mark)
        {
            int value = 0;
            uint temp_value = 0x80000000;
            for (int i = 0; i < 32; i++)
            {
                if ((mark & temp_value) != 0)
                {
                    value++;
                }
                temp_value >>= 1;
            }
            return value;
        }

        /// <summary>
        /// 获取子掩码
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetSubnetMarkString(int value)
        {
            int temp_value = 0;
            int temp = unchecked((int)0x80000000);
            for (int i = 0; i < value; i++)
            {
                temp_value |= temp;
                temp >>= 1;
            }
            return SubnetMarkString(temp_value);
        }

        /// <summary>
        /// 子掩码数值
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static int SubnetMarkValue(string code)
        {
            var array_value = code.Split('.');
            byte[] byte_array = new byte[4];
            for (int i = 0; i < byte_array.Length; i++)
            {
                byte_array[3 - i] = Convert.ToByte(array_value[i]);
            }
            return BitConverter.ToInt32(byte_array, 0);
        }

        /// <summary>
        /// 子掩码
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        private static string SubnetMarkString(int code)
        {
            var byte_value = BitConverter.GetBytes(code);
            string[] string_value = new string[4];
            for (int i = 0; i < byte_value.Length; i++)
            {
                string_value[i] = byte_value[3 - i].ToString();
            }
            return string.Join(".", string_value);
        }
    }

}
