using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web;

namespace AutoUpgradeCore
{
    internal interface IUpgradeService
    {
        Action<long, long, int> DownloadProgressChanged { get; set; }
        UpgradeVersionInfo GetLastVersion(string app,string tag);
        bool DownloadFile(string verName, string path);
    }


    internal class HttpUpgradeService : IUpgradeService
    {
        private string _url;
        public Action<long, long, int> DownloadProgressChanged { get; set; }
        public HttpUpgradeService(string url)
        {
            _url = url;
        }
        public bool DownloadFile(string verName, string path)
        {
            try
            {
                using (WebClient client = new WebClient())
                {
                    client.DownloadProgressChanged += client_DownloadProgressChanged;
                    client.DownloadFile(string.Concat(_url, "/Version/Download", verName), path);
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }

        }

        void client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            DownloadProgressChanged?.Invoke(e.BytesReceived, e.TotalBytesToReceive,e.ProgressPercentage);
        }

        public UpgradeVersionInfo GetLastVersion(string app,string tag)
        {
            try
            {
                using (WebClient client = new WebClient())
                {
                    var str = HttpUtility.UrlEncode(tag);
                    var url = $"{_url}/GetLastVer?app={app}&tag={str}";
                    var stream = client.OpenRead(url);
                    StreamReader _read = new StreamReader(stream, Encoding.UTF8);
                    string result = _read.ReadToEnd();
                    var v= result.ParseVersion();
                    return v;
                }
             
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }

    internal class UsbUpgradeService : IUpgradeService
    {
        private string _dir;
        public Action<long, long, int> DownloadProgressChanged { get; set; }
        public UsbUpgradeService(string dir)
        {
            _dir = dir;
        }
        public bool DownloadFile(string verName, string path)
        {
            if (!File.Exists(verName))return false;

            FileInfo fileInfo = new FileInfo(verName);
            long totalBytes = fileInfo.Length; // 获取源文件的总字节数
            long soFar = 0;

            using (FileStream sourceStream = File.OpenRead(verName))
            {
                using (FileStream destinationStream = File.Create(path))
                {
                    byte[] buffer = new byte[4096];
                    int read;

                    while ((read = sourceStream.Read(buffer, 0, buffer.Length)) != 0)
                    {
                        destinationStream.Write(buffer, 0, read);
                        soFar += read;
                        int progress = (int)(soFar * 100 / totalBytes) ;
                        DownloadProgressChanged?.Invoke(soFar, totalBytes, progress);
                    }
                }
            }
         
            return true;
        }

        public UpgradeVersionInfo GetLastVersion(string app, string tag)
        {
            return AutoUpgradeHelper.GetLastVersion(app, _dir, tag);
        }
        
    }
}
