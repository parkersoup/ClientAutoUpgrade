using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoUpgradeCore
{
    public class UpgradeClient
    {
        /// <summary>
        /// 更新程序所在文件夹名称
        /// </summary>
        public string UpgradeDir = "Upgrade";

        private UpgradeConfig _config;
        private IUpgradeService _upgradeService;
        private string _workDir = string.Empty;
        private string _workAppDir = string.Empty;
        /// <summary>
        /// 新版本通知
        /// </summary>
        public Action<UpgradeVersionInfo> NewVersionNotice;

        public UpgradeClient(UpgradeConfig config)
        {
            _config = config;
            _workDir = Path.Combine(config.AppWorkDir, UpgradeDir);
            _workAppDir = Path.Combine(_workDir, config.AppName);
        }




        public virtual bool GetNewVersion(out UpgradeVersionInfo v)
        {
            v = GetCurVerison();
            var v2 = GetLastVersion();
            if (v2 == null)
            {
                return false;
            }
            var b = new VersionCompare().Compare(v2, v) > 0;
            if (b)
            {
                v = v2;
            }
            return b;
        }

        public virtual async Task<Tuple<bool, string>> UpdateProcessAsync(Action<int, string> progressCallback = null)
        {
            return await Task.Run(() =>
             {
                 var err = string.Empty;
                 progressCallback?.Invoke(10, "获取最新版本");
                 var b = GetNewVersion(out UpgradeVersionInfo v);
                 if (!b)
                 {
                     var has = CheckProcess();
                     if (!has)
                         ExecStart();
                     return Tuple.Create(true, "");
                 }
                 if (string.IsNullOrEmpty(err)) return Tuple.Create(false, "获取最新版本失败");
                 progressCallback?.Invoke(30, "正在下载文件");
                 err = DownloadProcess(v, out string zipPath, out string downDir, progressCallback);
                 if (string.IsNullOrEmpty(err)) return Tuple.Create(false, err);
                 progressCallback?.Invoke(50, "正在解压文件");
                 UnZipFileToDir(zipPath, out string unZipDir);
                 progressCallback?.Invoke(60, "正在检查进程");
                 CheckKillProcess();
                 progressCallback?.Invoke(70, "正在升级");
                 err = UpgradeFile(unZipDir, downDir);
                 if (string.IsNullOrEmpty(err)) return Tuple.Create(false, err);
                 progressCallback?.Invoke(90, "正在启动");
                 ExecStart();
                 progressCallback?.Invoke(100, "升级完成");
                 return Tuple.Create(true, "");
             });
        }

        protected virtual UpgradeVersionInfo GetCurVerison()
        {
            var str = string.Empty;
            var fileName = "version.txt";
            var fullFileName = Path.Combine(_workAppDir, fileName);
            if (File.Exists(fullFileName))
            {
                str = File.ReadAllText(fullFileName, Encoding.UTF8);
            }
            return str.ParseVersion();
        }

        protected virtual UpgradeVersionInfo GetLastVersion()
        {
            _upgradeService = new HttpUpgradeService(_config.Server);
            if (!string.IsNullOrEmpty(_config.UsbName))
            {
                var drives = AutoUpgradeHelper.GetUsbList();
                var usb = drives.FirstOrDefault(x => x.VolumeLabel == _config.UsbName);
                if (usb != null)
                {
                    var usbDir = Path.Combine(usb.RootDirectory.FullName, UpgradeDir);
                    if (Directory.Exists(usbDir))
                    {
                        _upgradeService = new UsbUpgradeService(usbDir);
                    }
                }
            }
            return _upgradeService.GetLastVersion(_config.AppName, _config.Tag);
        }

        protected virtual string StopProcess()
        {
            if (!string.IsNullOrWhiteSpace(_config.AppPocessName))
            {
                var process = Process.GetProcessesByName(_config.AppPocessName);
                foreach (var item in process)
                {
                    item.Kill();
                }
            }
            return string.Empty;
        }


        protected virtual string DownloadProcess(UpgradeVersionInfo v, out string zippath, out string downDir, Action<int, string> progressCallback = null)
        {
            downDir = Path.Combine(_workAppDir, "DownLoad");
            if (Directory.Exists(downDir))
                Directory.Delete(downDir, true);
            Directory.CreateDirectory(downDir);
            var filename = $"{v.Version}-{v.Tag}.zip";
            zippath = Path.Combine(downDir, filename);
            _upgradeService.DownloadProgressChanged = (cur, total, progress) =>
            {
                progressCallback?.Invoke(-1, $"文件下载进度[{cur.ConvertFileSizeStr()}/{total.ConvertFileSizeStr()}] {progress}%");
            };
            var res = _upgradeService.DownloadFile(filename, zippath);
            return res ? string.Empty : "文件下载失败";
        }

        protected virtual void UnZipFileToDir(string zipPath, out string unZipDir)
        {
            unZipDir = Path.Combine(Path.GetDirectoryName(zipPath), "unzip");
            ZipFile.ExtractToDirectory(zipPath, unZipDir);
        }


        protected virtual bool CheckProcess()
        {
            if (string.IsNullOrEmpty(_config.AppPocessName)) return true;
            var ps = Process.GetProcessesByName(_config.AppPocessName);
            return ps?.Length > 0;
        }
        protected virtual void CheckKillProcess()
        {
            if (string.IsNullOrEmpty(_config.AppPocessName)) return;
            var ps = Process.GetProcessesByName(_config.AppPocessName);
            if (ps != null && ps.Length > 0)
            {
                foreach (var p in ps)
                {
                    p.Kill();
                }
            }
        }

        protected virtual string UpgradeFile(string unZipDir, string delDir)
        {
            try
            {
                AutoUpgradeHelper.CopyDirectory(unZipDir, _config.AppWorkDir, true);
                Directory.Delete(delDir, true);
                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }

        protected virtual void ExecStart()
        {
            if (string.IsNullOrEmpty(_config.AppStartPath)) return;

            Process.Start(new ProcessStartInfo()
            {
                WorkingDirectory = _workDir,
                CreateNoWindow = true,
                FileName = _config.AppStartPath
            });
        }
    }
}
