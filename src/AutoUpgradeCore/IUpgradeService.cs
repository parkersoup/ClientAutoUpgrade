using System;
using System.Collections.Generic;
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
    public interface IUpgradeService
    {
        UpgradeVersionInfo GetLastVersion(string tag);
        bool DownloadFile(string verName, string path);
    }


    public class HttpUpgradeService : IUpgradeService
    {
        private string _url;
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
                    client.DownloadFile(string.Concat(_url, "/Version/Download", verName), path);
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }

        }

        public UpgradeVersionInfo GetLastVersion(string tag)
        {
            try
            {
                using (WebClient client = new WebClient())
                {
                    var str = HttpUtility.UrlEncode(tag);
                    var url = $"{_url}/GetLastVer?tag={str}";
                    var stream = client.OpenRead(url);
                    StreamReader _read = new StreamReader(stream, Encoding.UTF8);
                    string result = _read.ReadToEnd();
                    var v= result.ParseVersion();
                    return v;
                }
             
            }
            catch (Exception ex)
            {

            }
            return new UpgradeVersionInfo();
        }
    }

    public class UsbUpgradeService : IUpgradeService
    {
        private string _dir;
        public UsbUpgradeService(string dir)
        {
            _dir = dir;
        }
        public bool DownloadFile(string verName, string path)
        {
            var zip = Path.Combine(_dir, string.Concat(verName,".zip"));
            if (!File.Exists(zip))return false;
            File.Copy(zip, path, true);
            return true;
        }

        public UpgradeVersionInfo GetLastVersion(string tag)
        {
            return AutoUpgradeHelper.GetLastVersion(_dir, tag);
        }
        
    }
}
