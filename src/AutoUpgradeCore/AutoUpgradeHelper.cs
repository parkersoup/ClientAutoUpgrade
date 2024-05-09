using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace AutoUpgradeCore
{
    public static class AutoUpgradeHelper
    {
        public static string ConvertFileSizeStr(this long size)
        {
            var str = string.Empty;
            if (size < 1024)
            {
                str = $"{size} B";
            }
            else if (size < 1024 * 1024)
            {
                str = $"{size / 1024} KB";
            }
            else if (size < 1024 * 1024 * 1024)
            {
                str = $"{size / (1024 * 1024)} MB";
            }
            else
            {
                str = $"{size / (1024 * 1024 * 1024)} GB";
            }
            return str;
        }

        public static List<UpgradeVersionInfo> GetAllVersion(string dir)
        {
            const string versionFile = "version.txt";
            var list = new List<UpgradeVersionInfo>();
            var verfile = Path.Combine(dir, versionFile);
            if (!File.Exists(verfile)) return list;
            var vers = File.ReadAllLines(verfile, Encoding.UTF8);
            for (int i = vers.Length - 1; i >= 0; i--)
            {
                var str = vers[i];
                if (string.IsNullOrWhiteSpace(str) || !str.Contains("|")) continue;
                var info = str.ParseVersion();
                list.Add(info);
            }
            return list;
        }

        public static string ToVersionStr(this UpgradeVersionInfo v)
        {
            var str = $"{v.Version}|{v.Tag}|{v.Date.ToString("yyyyMMddHHmmss")}|{v.Size}|{v.SizeStr}|{v.Log}";
            return str;
        }

        public static UpgradeVersionInfo ParseVersion(this string str)
        {
            var m = new UpgradeVersionInfo();
            var arr = str.Split('|');
            m.Version = arr[0];
            m.Tag = arr[1];
            m.Date = DateTime.ParseExact(arr[2], "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
            m.Size = long.Parse(arr[3]);
            m.Log = arr[5];
            return m;
        }

        public static UpgradeVersionInfo GetLastVersion(string dir, string tag)
        {
            var all = GetAllVersion(dir).Where(x =>
            {
                if (!string.IsNullOrWhiteSpace(x.Tag))
                {
                    return x.Tag == tag;
                }
                return true;
            }).ToList();
          
            return all.LastOrDefault();
        }

        public static void WriteVersion(string  dir,UpgradeVersionInfo info)
        {
            const string versionFile = "version.txt";
            var verfile = Path.Combine(dir, versionFile);
            var str = info.ToVersionStr();
            using (var fs = new StreamWriter(verfile, true, Encoding.UTF8))
            {
                fs.WriteLine(str);
            }
        }
    }


}
