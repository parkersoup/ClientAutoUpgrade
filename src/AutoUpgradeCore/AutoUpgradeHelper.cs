using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace AutoUpgradeCore
{
    public static class AutoUpgradeHelper
    {
        public const string defApp = "App";
        public const string ConfigPath = "upgradeConfig.json";
        public const string VersionPath = "upgradeVersion.json";

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

        /// <summary>
        /// 按产品和标记获取全部可用版本
        /// </summary>
        /// <param name="app"></param>
        /// <param name="dir"></param>
        /// <returns></returns>
        public static List<UpgradeVersionInfo> GetAllVersion(string app, string dir)
        {
            if (string.IsNullOrEmpty(app)) app = defApp;
            const string versionFile = "version.txt";
            var list = new List<UpgradeVersionInfo>();
            var verfile = Path.Combine(dir, app, versionFile); 
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

        public static List<string> GetAllApp(string dir)
        {
            var list = new List<string>();

            if (!Directory.Exists(dir)) return list;

            var dirs = Directory.GetDirectories(dir);
            foreach (var item in dirs)
            {
                list.Add(Path.GetFileName(item));
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
            if (str!=null&&str.Contains('|'))
            {
                var arr = str.Split('|');
                m.Version = arr[0];
                m.Tag = arr[1];
                m.Date = DateTime.ParseExact(arr[2], "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
                m.Size = long.Parse(arr[3]);
                m.Log = arr[5];
            }
          
            return m;
        }

        public static UpgradeVersionInfo GetLastVersion(string app, string dir, string tag)
        {
            if (string.IsNullOrEmpty(app)) app = defApp;

            var all = GetAllVersion(app, dir).Where(x =>
            {
                if (!string.IsNullOrWhiteSpace(x.Tag))
                {
                    return x.Tag == tag;
                }
                return true;
            }).OrderBy(a => new VersionCompare()).ToList();
            return all.LastOrDefault();
        }



        public static void WriteVersion(string dir, UpgradeVersionInfo info)
        {
            const string versionFile = "version.txt";
            var verfile = Path.Combine(dir, versionFile);
            var str = info.ToVersionStr();
            using (var fs = new StreamWriter(verfile, true, Encoding.UTF8))
            {
                fs.WriteLine(str);
            }
        }



        public static void CopyDirectory(string sourceDir, string destDir, bool overwrite)
        {
            if (!Directory.Exists(sourceDir)) return;
            var dirs = Directory.GetDirectories(sourceDir);
            var files = Directory.GetFiles(sourceDir);
            foreach (string file in files)
            {
                string name = System.IO.Path.GetFileName(file);
                string dest = System.IO.Path.Combine(destDir, name);
                System.IO.File.Copy(file, dest, overwrite);
            }
            foreach (string dir in dirs)
            {
                string name = System.IO.Path.GetFileName(dir);
                string dest = System.IO.Path.Combine(destDir, name);
                CopyDirectory(dir, dest, overwrite);
            }
        }

        public static List<DriveInfo> GetUsbList()
        {
            var drivers = System.IO.DriveInfo.GetDrives().Where(x=>x.DriveType == DriveType.Removable&& x.IsReady && x.RootDirectory.Exists).ToList();
            return drivers;
        }
    }

    public class VersionCompare : IComparer<UpgradeVersionInfo>
    {
        public int Compare(UpgradeVersionInfo x, UpgradeVersionInfo y)
        {
            if (x.Version == y.Version)
            {
                return x.Date.CompareTo(y.Date);
            }
            else
            {
                return new Version(x.Version).CompareTo(y.Version);
            }
        }


    }
}
