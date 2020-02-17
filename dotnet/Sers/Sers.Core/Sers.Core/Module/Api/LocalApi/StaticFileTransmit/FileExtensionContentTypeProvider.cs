#region 程序集 Microsoft.AspNetCore.StaticFiles, Version=2.2.0.0, Culture=neutral, PublicKeyToken=adb9793829ddae60
// C:\Users\help\.nuget\packages\microsoft.aspnetcore.staticfiles\2.2.0\lib\netstandard2.0\Microsoft.AspNetCore.StaticFiles.dll
// Decompiled with ICSharpCode.Decompiler 3.1.0.3652
#endregion
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;


namespace Sers.Core.Module.Api.LocalApi.StaticFileTransmit
{
    /// <summary>
    /// Provides a mapping between file extensions and MIME types.
    /// </summary>
    public class FileExtensionContentTypeProvider 
    {
        /// <summary>
        /// The cross reference table of file extensions and content-types.
        /// </summary>
        public IDictionary<string, string> Mappings
        {
        
            get;
         
            private set;
        } 

        /// <summary>
        /// Creates a new provider with a set of default mappings.
        /// </summary>
        public FileExtensionContentTypeProvider()
        {
            //IL_0001: Unknown result type (might be due to invalid IL or missing references)
            //IL_0006: Expected O, but got Unknown
            Dictionary<string, string> obj = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            #region add map           
            obj.Add(".323", "text/h323");
            obj.Add(".3g2", "video/3gpp2");
            obj.Add(".3gp2", "video/3gpp2");
            obj.Add(".3gp", "video/3gpp");
            obj.Add(".3gpp", "video/3gpp");
            obj.Add(".aac", "audio/aac");
            obj.Add(".aaf", "application/octet-stream");
            obj.Add(".aca", "application/octet-stream");
            obj.Add(".accdb", "application/msaccess");
            obj.Add(".accde", "application/msaccess");
            obj.Add(".accdt", "application/msaccess");
            obj.Add(".acx", "application/internet-property-stream");
            obj.Add(".adt", "audio/vnd.dlna.adts");
            obj.Add(".adts", "audio/vnd.dlna.adts");
            obj.Add(".afm", "application/octet-stream");
            obj.Add(".ai", "application/postscript");
            obj.Add(".aif", "audio/x-aiff");
            obj.Add(".aifc", "audio/aiff");
            obj.Add(".aiff", "audio/aiff");
            obj.Add(".appcache", "text/cache-manifest");
            obj.Add(".application", "application/x-ms-application");
            obj.Add(".art", "image/x-jg");
            obj.Add(".asd", "application/octet-stream");
            obj.Add(".asf", "video/x-ms-asf");
            obj.Add(".asi", "application/octet-stream");
            obj.Add(".asm", "text/plain");
            obj.Add(".asr", "video/x-ms-asf");
            obj.Add(".asx", "video/x-ms-asf");
            obj.Add(".atom", "application/atom+xml");
            obj.Add(".au", "audio/basic");
            obj.Add(".avi", "video/x-msvideo");
            obj.Add(".axs", "application/olescript");
            obj.Add(".bas", "text/plain");
            obj.Add(".bcpio", "application/x-bcpio");
            obj.Add(".bin", "application/octet-stream");
            obj.Add(".bmp", "image/bmp");
            obj.Add(".c", "text/plain");
            obj.Add(".cab", "application/vnd.ms-cab-compressed");
            obj.Add(".calx", "application/vnd.ms-office.calx");
            obj.Add(".cat", "application/vnd.ms-pki.seccat");
            obj.Add(".cdf", "application/x-cdf");
            obj.Add(".chm", "application/octet-stream");
            obj.Add(".class", "application/x-java-applet");
            obj.Add(".clp", "application/x-msclip");
            obj.Add(".cmx", "image/x-cmx");
            obj.Add(".cnf", "text/plain");
            obj.Add(".cod", "image/cis-cod");
            obj.Add(".cpio", "application/x-cpio");
            obj.Add(".cpp", "text/plain");
            obj.Add(".crd", "application/x-mscardfile");
            obj.Add(".crl", "application/pkix-crl");
            obj.Add(".crt", "application/x-x509-ca-cert");
            obj.Add(".csh", "application/x-csh");
            obj.Add(".css", "text/css");
            obj.Add(".csv", "application/octet-stream");
            obj.Add(".cur", "application/octet-stream");
            obj.Add(".dcr", "application/x-director");
            obj.Add(".deploy", "application/octet-stream");
            obj.Add(".der", "application/x-x509-ca-cert");
            obj.Add(".dib", "image/bmp");
            obj.Add(".dir", "application/x-director");
            obj.Add(".disco", "text/xml");
            obj.Add(".dlm", "text/dlm");
            obj.Add(".doc", "application/msword");
            obj.Add(".docm", "application/vnd.ms-word.document.macroEnabled.12");
            obj.Add(".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document");
            obj.Add(".dot", "application/msword");
            obj.Add(".dotm", "application/vnd.ms-word.template.macroEnabled.12");
            obj.Add(".dotx", "application/vnd.openxmlformats-officedocument.wordprocessingml.template");
            obj.Add(".dsp", "application/octet-stream");
            obj.Add(".dtd", "text/xml");
            obj.Add(".dvi", "application/x-dvi");
            obj.Add(".dvr-ms", "video/x-ms-dvr");
            obj.Add(".dwf", "drawing/x-dwf");
            obj.Add(".dwp", "application/octet-stream");
            obj.Add(".dxr", "application/x-director");
            obj.Add(".eml", "message/rfc822");
            obj.Add(".emz", "application/octet-stream");
            obj.Add(".eot", "application/vnd.ms-fontobject");
            obj.Add(".eps", "application/postscript");
            obj.Add(".etx", "text/x-setext");
            obj.Add(".evy", "application/envoy");
            obj.Add(".fdf", "application/vnd.fdf");
            obj.Add(".fif", "application/fractals");
            obj.Add(".fla", "application/octet-stream");
            obj.Add(".flr", "x-world/x-vrml");
            obj.Add(".flv", "video/x-flv");
            obj.Add(".gif", "image/gif");
            obj.Add(".gtar", "application/x-gtar");
            obj.Add(".gz", "application/x-gzip");
            obj.Add(".h", "text/plain");
            obj.Add(".hdf", "application/x-hdf");
            obj.Add(".hdml", "text/x-hdml");
            obj.Add(".hhc", "application/x-oleobject");
            obj.Add(".hhk", "application/octet-stream");
            obj.Add(".hhp", "application/octet-stream");
            obj.Add(".hlp", "application/winhlp");
            obj.Add(".hqx", "application/mac-binhex40");
            obj.Add(".hta", "application/hta");
            obj.Add(".htc", "text/x-component");
            obj.Add(".htm", "text/html");
            obj.Add(".html", "text/html");
            obj.Add(".htt", "text/webviewhtml");
            obj.Add(".hxt", "text/html");
            obj.Add(".ical", "text/calendar");
            obj.Add(".icalendar", "text/calendar");
            obj.Add(".ico", "image/x-icon");
            obj.Add(".ics", "text/calendar");
            obj.Add(".ief", "image/ief");
            obj.Add(".ifb", "text/calendar");
            obj.Add(".iii", "application/x-iphone");
            obj.Add(".inf", "application/octet-stream");
            obj.Add(".ins", "application/x-internet-signup");
            obj.Add(".isp", "application/x-internet-signup");
            obj.Add(".IVF", "video/x-ivf");
            obj.Add(".jar", "application/java-archive");
            obj.Add(".java", "application/octet-stream");
            obj.Add(".jck", "application/liquidmotion");
            obj.Add(".jcz", "application/liquidmotion");
            obj.Add(".jfif", "image/pjpeg");
            obj.Add(".jpb", "application/octet-stream");
            obj.Add(".jpe", "image/jpeg");
            obj.Add(".jpeg", "image/jpeg");
            obj.Add(".jpg", "image/jpeg");
            obj.Add(".js", "application/javascript");
            obj.Add(".json", "application/json");
            obj.Add(".jsx", "text/jscript");
            obj.Add(".latex", "application/x-latex");
            obj.Add(".lit", "application/x-ms-reader");
            obj.Add(".lpk", "application/octet-stream");
            obj.Add(".lsf", "video/x-la-asf");
            obj.Add(".lsx", "video/x-la-asf");
            obj.Add(".lzh", "application/octet-stream");
            obj.Add(".m13", "application/x-msmediaview");
            obj.Add(".m14", "application/x-msmediaview");
            obj.Add(".m1v", "video/mpeg");
            obj.Add(".m2ts", "video/vnd.dlna.mpeg-tts");
            obj.Add(".m3u", "audio/x-mpegurl");
            obj.Add(".m4a", "audio/mp4");
            obj.Add(".m4v", "video/mp4");
            obj.Add(".man", "application/x-troff-man");
            obj.Add(".manifest", "application/x-ms-manifest");
            obj.Add(".map", "text/plain");
            obj.Add(".markdown", "text/markdown");
            obj.Add(".md", "text/markdown");
            obj.Add(".mdb", "application/x-msaccess");
            obj.Add(".mdp", "application/octet-stream");
            obj.Add(".me", "application/x-troff-me");
            obj.Add(".mht", "message/rfc822");
            obj.Add(".mhtml", "message/rfc822");
            obj.Add(".mid", "audio/mid");
            obj.Add(".midi", "audio/mid");
            obj.Add(".mix", "application/octet-stream");
            obj.Add(".mmf", "application/x-smaf");
            obj.Add(".mno", "text/xml");
            obj.Add(".mny", "application/x-msmoney");
            obj.Add(".mov", "video/quicktime");
            obj.Add(".movie", "video/x-sgi-movie");
            obj.Add(".mp2", "video/mpeg");
            obj.Add(".mp3", "audio/mpeg");
            obj.Add(".mp4", "video/mp4");
            obj.Add(".mp4v", "video/mp4");
            obj.Add(".mpa", "video/mpeg");
            obj.Add(".mpe", "video/mpeg");
            obj.Add(".mpeg", "video/mpeg");
            obj.Add(".mpg", "video/mpeg");
            obj.Add(".mpp", "application/vnd.ms-project");
            obj.Add(".mpv2", "video/mpeg");
            obj.Add(".ms", "application/x-troff-ms");
            obj.Add(".msi", "application/octet-stream");
            obj.Add(".mso", "application/octet-stream");
            obj.Add(".mvb", "application/x-msmediaview");
            obj.Add(".mvc", "application/x-miva-compiled");
            obj.Add(".nc", "application/x-netcdf");
            obj.Add(".nsc", "video/x-ms-asf");
            obj.Add(".nws", "message/rfc822");
            obj.Add(".ocx", "application/octet-stream");
            obj.Add(".oda", "application/oda");
            obj.Add(".odc", "text/x-ms-odc");
            obj.Add(".ods", "application/oleobject");
            obj.Add(".oga", "audio/ogg");
            obj.Add(".ogg", "video/ogg");
            obj.Add(".ogv", "video/ogg");
            obj.Add(".ogx", "application/ogg");
            obj.Add(".one", "application/onenote");
            obj.Add(".onea", "application/onenote");
            obj.Add(".onetoc", "application/onenote");
            obj.Add(".onetoc2", "application/onenote");
            obj.Add(".onetmp", "application/onenote");
            obj.Add(".onepkg", "application/onenote");
            obj.Add(".osdx", "application/opensearchdescription+xml");
            obj.Add(".otf", "font/otf");
            obj.Add(".p10", "application/pkcs10");
            obj.Add(".p12", "application/x-pkcs12");
            obj.Add(".p7b", "application/x-pkcs7-certificates");
            obj.Add(".p7c", "application/pkcs7-mime");
            obj.Add(".p7m", "application/pkcs7-mime");
            obj.Add(".p7r", "application/x-pkcs7-certreqresp");
            obj.Add(".p7s", "application/pkcs7-signature");
            obj.Add(".pbm", "image/x-portable-bitmap");
            obj.Add(".pcx", "application/octet-stream");
            obj.Add(".pcz", "application/octet-stream");
            obj.Add(".pdf", "application/pdf");
            obj.Add(".pfb", "application/octet-stream");
            obj.Add(".pfm", "application/octet-stream");
            obj.Add(".pfx", "application/x-pkcs12");
            obj.Add(".pgm", "image/x-portable-graymap");
            obj.Add(".pko", "application/vnd.ms-pki.pko");
            obj.Add(".pma", "application/x-perfmon");
            obj.Add(".pmc", "application/x-perfmon");
            obj.Add(".pml", "application/x-perfmon");
            obj.Add(".pmr", "application/x-perfmon");
            obj.Add(".pmw", "application/x-perfmon");
            obj.Add(".png", "image/png");
            obj.Add(".pnm", "image/x-portable-anymap");
            obj.Add(".pnz", "image/png");
            obj.Add(".pot", "application/vnd.ms-powerpoint");
            obj.Add(".potm", "application/vnd.ms-powerpoint.template.macroEnabled.12");
            obj.Add(".potx", "application/vnd.openxmlformats-officedocument.presentationml.template");
            obj.Add(".ppam", "application/vnd.ms-powerpoint.addin.macroEnabled.12");
            obj.Add(".ppm", "image/x-portable-pixmap");
            obj.Add(".pps", "application/vnd.ms-powerpoint");
            obj.Add(".ppsm", "application/vnd.ms-powerpoint.slideshow.macroEnabled.12");
            obj.Add(".ppsx", "application/vnd.openxmlformats-officedocument.presentationml.slideshow");
            obj.Add(".ppt", "application/vnd.ms-powerpoint");
            obj.Add(".pptm", "application/vnd.ms-powerpoint.presentation.macroEnabled.12");
            obj.Add(".pptx", "application/vnd.openxmlformats-officedocument.presentationml.presentation");
            obj.Add(".prf", "application/pics-rules");
            obj.Add(".prm", "application/octet-stream");
            obj.Add(".prx", "application/octet-stream");
            obj.Add(".ps", "application/postscript");
            obj.Add(".psd", "application/octet-stream");
            obj.Add(".psm", "application/octet-stream");
            obj.Add(".psp", "application/octet-stream");
            obj.Add(".pub", "application/x-mspublisher");
            obj.Add(".qt", "video/quicktime");
            obj.Add(".qtl", "application/x-quicktimeplayer");
            obj.Add(".qxd", "application/octet-stream");
            obj.Add(".ra", "audio/x-pn-realaudio");
            obj.Add(".ram", "audio/x-pn-realaudio");
            obj.Add(".rar", "application/octet-stream");
            obj.Add(".ras", "image/x-cmu-raster");
            obj.Add(".rf", "image/vnd.rn-realflash");
            obj.Add(".rgb", "image/x-rgb");
            obj.Add(".rm", "application/vnd.rn-realmedia");
            obj.Add(".rmi", "audio/mid");
            obj.Add(".roff", "application/x-troff");
            obj.Add(".rpm", "audio/x-pn-realaudio-plugin");
            obj.Add(".rtf", "application/rtf");
            obj.Add(".rtx", "text/richtext");
            obj.Add(".scd", "application/x-msschedule");
            obj.Add(".sct", "text/scriptlet");
            obj.Add(".sea", "application/octet-stream");
            obj.Add(".setpay", "application/set-payment-initiation");
            obj.Add(".setreg", "application/set-registration-initiation");
            obj.Add(".sgml", "text/sgml");
            obj.Add(".sh", "application/x-sh");
            obj.Add(".shar", "application/x-shar");
            obj.Add(".sit", "application/x-stuffit");
            obj.Add(".sldm", "application/vnd.ms-powerpoint.slide.macroEnabled.12");
            obj.Add(".sldx", "application/vnd.openxmlformats-officedocument.presentationml.slide");
            obj.Add(".smd", "audio/x-smd");
            obj.Add(".smi", "application/octet-stream");
            obj.Add(".smx", "audio/x-smd");
            obj.Add(".smz", "audio/x-smd");
            obj.Add(".snd", "audio/basic");
            obj.Add(".snp", "application/octet-stream");
            obj.Add(".spc", "application/x-pkcs7-certificates");
            obj.Add(".spl", "application/futuresplash");
            obj.Add(".spx", "audio/ogg");
            obj.Add(".src", "application/x-wais-source");
            obj.Add(".ssm", "application/streamingmedia");
            obj.Add(".sst", "application/vnd.ms-pki.certstore");
            obj.Add(".stl", "application/vnd.ms-pki.stl");
            obj.Add(".sv4cpio", "application/x-sv4cpio");
            obj.Add(".sv4crc", "application/x-sv4crc");
            obj.Add(".svg", "image/svg+xml");
            obj.Add(".svgz", "image/svg+xml");
            obj.Add(".swf", "application/x-shockwave-flash");
            obj.Add(".t", "application/x-troff");
            obj.Add(".tar", "application/x-tar");
            obj.Add(".tcl", "application/x-tcl");
            obj.Add(".tex", "application/x-tex");
            obj.Add(".texi", "application/x-texinfo");
            obj.Add(".texinfo", "application/x-texinfo");
            obj.Add(".tgz", "application/x-compressed");
            obj.Add(".thmx", "application/vnd.ms-officetheme");
            obj.Add(".thn", "application/octet-stream");
            obj.Add(".tif", "image/tiff");
            obj.Add(".tiff", "image/tiff");
            obj.Add(".toc", "application/octet-stream");
            obj.Add(".tr", "application/x-troff");
            obj.Add(".trm", "application/x-msterminal");
            obj.Add(".ts", "video/vnd.dlna.mpeg-tts");
            obj.Add(".tsv", "text/tab-separated-values");
            obj.Add(".ttc", "application/x-font-ttf");
            obj.Add(".ttf", "application/x-font-ttf");
            obj.Add(".tts", "video/vnd.dlna.mpeg-tts");
            obj.Add(".txt", "text/plain");
            obj.Add(".u32", "application/octet-stream");
            obj.Add(".uls", "text/iuls");
            obj.Add(".ustar", "application/x-ustar");
            obj.Add(".vbs", "text/vbscript");
            obj.Add(".vcf", "text/x-vcard");
            obj.Add(".vcs", "text/plain");
            obj.Add(".vdx", "application/vnd.ms-visio.viewer");
            obj.Add(".vml", "text/xml");
            obj.Add(".vsd", "application/vnd.visio");
            obj.Add(".vss", "application/vnd.visio");
            obj.Add(".vst", "application/vnd.visio");
            obj.Add(".vsto", "application/x-ms-vsto");
            obj.Add(".vsw", "application/vnd.visio");
            obj.Add(".vsx", "application/vnd.visio");
            obj.Add(".vtx", "application/vnd.visio");
            obj.Add(".wasm", "application/wasm");
            obj.Add(".wav", "audio/wav");
            obj.Add(".wax", "audio/x-ms-wax");
            obj.Add(".wbmp", "image/vnd.wap.wbmp");
            obj.Add(".wcm", "application/vnd.ms-works");
            obj.Add(".wdb", "application/vnd.ms-works");
            obj.Add(".webm", "video/webm");
            obj.Add(".webp", "image/webp");
            obj.Add(".wks", "application/vnd.ms-works");
            obj.Add(".wm", "video/x-ms-wm");
            obj.Add(".wma", "audio/x-ms-wma");
            obj.Add(".wmd", "application/x-ms-wmd");
            obj.Add(".wmf", "application/x-msmetafile");
            obj.Add(".wml", "text/vnd.wap.wml");
            obj.Add(".wmlc", "application/vnd.wap.wmlc");
            obj.Add(".wmls", "text/vnd.wap.wmlscript");
            obj.Add(".wmlsc", "application/vnd.wap.wmlscriptc");
            obj.Add(".wmp", "video/x-ms-wmp");
            obj.Add(".wmv", "video/x-ms-wmv");
            obj.Add(".wmx", "video/x-ms-wmx");
            obj.Add(".wmz", "application/x-ms-wmz");
            obj.Add(".woff", "application/font-woff");
            obj.Add(".woff2", "font/woff2");
            obj.Add(".wps", "application/vnd.ms-works");
            obj.Add(".wri", "application/x-mswrite");
            obj.Add(".wrl", "x-world/x-vrml");
            obj.Add(".wrz", "x-world/x-vrml");
            obj.Add(".wsdl", "text/xml");
            obj.Add(".wtv", "video/x-ms-wtv");
            obj.Add(".wvx", "video/x-ms-wvx");
            obj.Add(".x", "application/directx");
            obj.Add(".xaf", "x-world/x-vrml");
            obj.Add(".xaml", "application/xaml+xml");
            obj.Add(".xap", "application/x-silverlight-app");
            obj.Add(".xbap", "application/x-ms-xbap");
            obj.Add(".xbm", "image/x-xbitmap");
            obj.Add(".xdr", "text/plain");
            obj.Add(".xht", "application/xhtml+xml");
            obj.Add(".xhtml", "application/xhtml+xml");
            obj.Add(".xla", "application/vnd.ms-excel");
            obj.Add(".xlam", "application/vnd.ms-excel.addin.macroEnabled.12");
            obj.Add(".xlc", "application/vnd.ms-excel");
            obj.Add(".xlm", "application/vnd.ms-excel");
            obj.Add(".xls", "application/vnd.ms-excel");
            obj.Add(".xlsb", "application/vnd.ms-excel.sheet.binary.macroEnabled.12");
            obj.Add(".xlsm", "application/vnd.ms-excel.sheet.macroEnabled.12");
            obj.Add(".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            obj.Add(".xlt", "application/vnd.ms-excel");
            obj.Add(".xltm", "application/vnd.ms-excel.template.macroEnabled.12");
            obj.Add(".xltx", "application/vnd.openxmlformats-officedocument.spreadsheetml.template");
            obj.Add(".xlw", "application/vnd.ms-excel");
            obj.Add(".xml", "text/xml");
            obj.Add(".xof", "x-world/x-vrml");
            obj.Add(".xpm", "image/x-xpixmap");
            obj.Add(".xps", "application/vnd.ms-xpsdocument");
            obj.Add(".xsd", "text/xml");
            obj.Add(".xsf", "text/xml");
            obj.Add(".xsl", "text/xml");
            obj.Add(".xslt", "text/xml");
            obj.Add(".xsn", "application/octet-stream");
            obj.Add(".xtp", "application/octet-stream");
            obj.Add(".xwd", "image/x-xwindowdump");
            obj.Add(".z", "application/x-compress");
            obj.Add(".zip", "application/x-zip-compressed");
            #endregion

            Mappings = obj;
        }

        /// <summary>
        /// Creates a lookup engine using the provided mapping.
        /// It is recommended that the IDictionary instance use StringComparer.OrdinalIgnoreCase.
        /// </summary>
        /// <param name="mapping"></param>
        public FileExtensionContentTypeProvider(IDictionary<string, string> mapping)
        {
            //IL_000e: Unknown result type (might be due to invalid IL or missing references)
            if (mapping == null)
            {
                throw new ArgumentNullException("mapping");
            }
            Mappings = mapping;
        }

        /// <summary>
        /// Given a file path, determine the MIME type
        /// </summary>
        /// <param name="subpath">A file path</param>
        /// <param name="contentType">The resulting MIME type</param>
        /// <returns>True if MIME type could be determined</returns>
        public bool TryGetContentType(string subpath, out string contentType)
        {
            string extension = GetExtension(subpath);
            if (extension == null)
            {
                contentType = null;
                return false;
            }
            return Mappings.TryGetValue(extension, out contentType);
        }

        private static string GetExtension(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return null;
            }
            int num = path.LastIndexOf('.');
            if (num < 0)
            {
                return null;
            }
            return path.Substring(num);
        }
    }
}
