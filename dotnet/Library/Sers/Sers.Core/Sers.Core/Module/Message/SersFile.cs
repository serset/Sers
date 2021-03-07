using System;
using System.Collections.Generic;
using System.Linq;
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


        public virtual Vit.Core.Util.Pipelines.ByteData Files { get; protected set; }



        #region 拆包 与 打包     
        public SersFile Unpack(ArraySegment<byte> oriData)
        {
            Files = UnpackOriData(oriData); 
            return this;
        }



        public Vit.Core.Util.Pipelines.ByteData Package()
        {
            return PackageArraySegmentByte(Files);
        }
        #endregion



        #region 文件读写
        public int FileCount => Files.Count();


        public ArraySegment<byte> GetFile(int FileIndex)
        {
            return Files[FileIndex];
        }




        public void AddFile(ArraySegment<byte>file)
        {
            Files.Add(file);
        }

        public SersFile SetFiles(Vit.Core.Util.Pipelines.ByteData files)
        {
            Files = files;
            return this;
        }
        public SersFile SetFiles(params ArraySegment<byte>[] files)
        {
            Files = files.ToList();
            return this;
        }

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
        static Vit.Core.Util.Pipelines.ByteData PackageArraySegmentByte(Vit.Core.Util.Pipelines.ByteData files)
        {
            var oriData = new Vit.Core.Util.Pipelines.ByteData();

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
        //static Vit.Core.Util.Pipelines.ByteData PackageByteData(params Vit.Core.Util.Pipelines.ByteData[] files)
        //{
        //    var byteData = new Vit.Core.Util.Pipelines.ByteData();

        //    foreach (var file in files)
        //    {
        //        byteData.Add((file?.ByteDataCount() ?? 0).Int32ToArraySegmentByte());
        //        if (null != file)
        //            byteData.AddRange(file);
        //    }
        //    return byteData;
        //}


        static Vit.Core.Util.Pipelines.ByteData UnpackOriData(ArraySegment<byte> oriData)
        {
            Vit.Core.Util.Pipelines.ByteData files = new Vit.Core.Util.Pipelines.ByteData();
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
