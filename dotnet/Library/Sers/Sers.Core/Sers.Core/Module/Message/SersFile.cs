using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Vit.Core.Util.Pipelines;
using Vit.Extensions;

namespace Sers.Core.Module.Message
{
    public class SersFile
    {
        public SersFile() { }

        public SersFile(ArraySegment<byte> oriData)
        {
            Unpack(oriData);
        }


        public virtual List<ArraySegment<byte>> Files { get; protected set; }



        #region 拆包 与 打包     
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SersFile Unpack(ArraySegment<byte> oriData)
        {
            Files = UnpackOriData(oriData); 
            return this;
        }



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ByteData Package()
        {
            return PackageArraySegmentByte(Files);
        }

         
        #endregion



        #region 文件读写 


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ArraySegment<byte> GetFile(int FileIndex)
        {
            return Files[FileIndex];
        }




        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddFile(ArraySegment<byte>file)
        {
            Files.Add(file);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SersFile SetFiles(List<ArraySegment<byte>> files)
        {
            Files = files;
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SersFile SetFiles(params ArraySegment<byte>[] files)
        {
            Files = files.ToList();
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SersFile AddFiles(params ArraySegment<byte>[] extFiles)
        {
            Files.AddRange(extFiles);
            return this;
        }
        #endregion



        #region static  Package Unpack

        /// <summary>
        /// 每个文件为 ArraySegmentByte 类型
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static ByteData PackageArraySegmentByte(List<ArraySegment<byte>> files)
        {
            var oriData = new ByteData();

            foreach (var file in files)
            {
                oriData.Add((null == file ? 0 : file.Count).Int32ToArraySegmentByte());
                if (null != file) oriData.Add(file);
            }
            return oriData;
        }

        ///// <summary>
        ///// 每个文件为ByteData类型
        ///// </summary>
        ///// <param name="files"></param>
        ///// <returns></returns>
        //static List<ArraySegment<byte>> PackageByteData(params List<ArraySegment<byte>>[] files)
        //{
        //    var byteData = new List<ArraySegment<byte>>();

        //    foreach (var file in files)
        //    {
        //        byteData.Add((file?.ByteDataCount() ?? 0).Int32ToArraySegmentByte());
        //        if (null != file)
        //            byteData.AddRange(file);
        //    }
        //    return byteData;
        //}


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static List<ArraySegment<byte>> UnpackOriData(ArraySegment<byte> oriData)
        {
            List<ArraySegment<byte>> files = new List<ArraySegment<byte>>();
            int index = 0;
            int fileLen;
           
            while (index < oriData.Count)
            {
                fileLen = oriData.ArraySegmentByteToInt32(index);
                index += 4;

                files.Add(oriData.Slice(index, fileLen));
                index += fileLen;
            }
            return files;
        }

        #endregion
    }
}
