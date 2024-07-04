using System;

using Newtonsoft.Json.Linq;

using Vit.Core.Module.Log;
using Vit.Extensions.Newtonsoft_Extensions;
using Vit.Extensions.Serialize_Extensions;

namespace Sers.Core.Util.StreamSecurity.Security
{
    public unsafe class SampleSecurity : ISecurity
    {

        public void Init(JObject config)
        {
            secretKey = config["secret"].ConvertToString();

            Logger.Info("[CL.SecurityManager] loaded security : SampleSecurity");
        }



        /*
         
             data0 secret temp0  
             加密            data1 = data0  ^ secret ^ temp0 
                             temp1 = data0  ^ temp0             


             data1 secret temp0  
             解密            data0 = data1  ^ secret ^ temp0 
                             temp1 = data0  ^ temp0
             */


        string _secretKey;
        public string secretKey
        {
            get => _secretKey;
            set
            {
                _secretKey = value;

                if (string.IsNullOrEmpty(value) || value.Length < 20)
                {
                    value += "YouMakeTheWorldBetter";
                }
                secretBytes = value.StringToBytes();
                secretLength = secretBytes.Length;

                //均匀化秘钥
                //防止 在秘钥仅有部分匹配时，却能解析部分正确的原文
                byte temp = 0;
                for (var t = 0; t < secretLength; t++) temp ^= secretBytes[t];
                for (var t = 0; t < secretLength; t++) secretBytes[t] ^= temp;


            }
        }


        byte[] secretBytes;
        int secretLength;


        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public void Decryption(ArraySegment<byte> data)
        {

            unsafe
            {
                fixed (byte* p = data.Array)
                {
                    byte* bytes = p + data.Offset;

                    byte data0;
                    byte temp = 0;
                    int curSecretIndex = secretLength - 1;

                    for (int curIndex = 0; curIndex < data.Count; curIndex++)
                    {
                        //bytes[curIndex] ^= 10;

                        data0 = bytes[curIndex] = (byte)(bytes[curIndex] ^ secretBytes[curSecretIndex] ^ temp);

                        temp ^= data0;

                        if ((--curSecretIndex) < 0) curSecretIndex = secretLength - 1;
                    }
                }
            }
        }




        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public void Encryption(ArraySegment<byte> data)
        {

            unsafe
            {
                fixed (byte* p = data.Array)
                {
                    byte* bytes = p + data.Offset;

                    byte data0;

                    byte temp = 0;
                    int curSecretIndex = secretLength - 1;

                    for (int curIndex = 0; curIndex < data.Count; curIndex++)
                    {
                        //bytes[curIndex] ^= 10;

                        data0 = bytes[curIndex];
                        bytes[curIndex] = (byte)(data0 ^ secretBytes[curSecretIndex] ^ temp);

                        temp ^= data0;

                        if ((--curSecretIndex) < 0) curSecretIndex = secretLength - 1;
                    }
                }
            }
        }


    }
}
