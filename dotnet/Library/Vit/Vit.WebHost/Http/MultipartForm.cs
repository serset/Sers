﻿using System.Collections.Generic;
using System.IO;

using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;

using Vit.Extensions;

namespace Vit.WebHost.Http.FormFile
{


    /*
     * https://www.cnblogs.com/liuxiaoji/p/10266609.html
     * 
        <ItemGroup>
        <PackageReference Include = "Microsoft.AspNetCore.Http" Version="2.2.2" />
        <PackageReference Include = "Microsoft.Net.Http.Headers" Version="2.2.8" />
        </ItemGroup>
    */



    public class FormFile
    {
        /// <summary>
        /// 文件在form中的name（如 "files"）
        /// </summary>
        public string formKey;
        /// <summary>
        /// 上传文件时的文件名称（含后缀 如 "user.jpg"）
        /// </summary>
        public string fileName;
        /// <summary>
        /// 文件的二进制正文内容
        /// </summary>
        public byte[] content;
    }

    /// <summary>
    /// 
    /// </summary>
    public class MultipartForm
    {
        public Dictionary<string, string> form { get; private set; }
        public List<FormFile> files { get; private set; }


        public MultipartForm(Stream Body, string ContentType)
        {
            ReadMultipartForm(this, Body, ContentType);
        }


        public MultipartForm(byte[] Body, string ContentType)
        {
            using var stream = new MemoryStream(Body);
            ReadMultipartForm(this, stream, ContentType);
        }


        #region ReadMultipartForm
        private static MultipartForm ReadMultipartForm(MultipartForm formData, Stream Body, string ContentType)
        {
            var boundary = HeaderUtilities.RemoveQuotes(MediaTypeHeaderValue.Parse(ContentType).Boundary).Value;
            var reader = new MultipartReader(boundary, Body);


            formData.files = new List<FormFile>();
            formData.form = new Dictionary<string, string>();


            MultipartSection section;
            while ((section = reader.ReadNextSectionAsync().Result) != null)
            {
                ContentDispositionHeaderValue contentDisposition = section.GetContentDispositionHeader();
                if (contentDisposition == null) continue;

                if (contentDisposition.IsFileDisposition())
                {
                    var file = section.AsFileSection();
                    byte[] buff = section.Body.ToBytes();
                    //byte[] buff = await file.FileStream.ToBytesAsync();

                    formData.files.Add(new FormFile { formKey = file.Name, fileName = contentDisposition.FileName.ToString(), content = buff });
                }
                else if (contentDisposition.IsFormDisposition())
                {
                    var form = section.AsFormDataSection();
                    formData.form[form.Name] = form.GetValueAsync().Result;
                }
            }
            return formData;
        }
        #endregion
    }
}
