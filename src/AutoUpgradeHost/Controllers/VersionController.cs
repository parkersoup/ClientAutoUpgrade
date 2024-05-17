using AutoUpgradeCore;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc;
using System.Web;

namespace AutoUpgradeHost.Controllers
{
    public class VersionController : Controller
    {
        private string _versionDir;
        public VersionController(IWebHostEnvironment hostEnvironment, IConfiguration config)
        {
            var dir = config.GetValue<string>("FilePath");
            _versionDir = Path.Combine(hostEnvironment.WebRootPath, dir);
            if (!Directory.Exists(_versionDir))
            {
                Directory.CreateDirectory(_versionDir);
            }

        }
        public IActionResult Index()
        {
            var Apps = AutoUpgradeHelper.GetAllApp(_versionDir);
            var all = new Dictionary<string,List<UpgradeVersionInfo>>();
            foreach (var app in Apps)
            {
                all.Add(app,AutoUpgradeHelper.GetAllVersion(app, _versionDir));
            }
            ViewBag.Data = all;
            return View();
        }

        public IActionResult GetLastVer(string app, string tag)
        {
            var v = AutoUpgradeHelper.GetLastVersion(app, _versionDir, tag);
            return Content(v.ToVersionStr());
        }

        [HttpPost]
        public IActionResult Upload(IFormFile file,string app,bool patch, string log)
        {
            if (string.IsNullOrEmpty(app)) app = AutoUpgradeHelper.defApp;
            var dir = Path.Combine(_versionDir, app);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            var en = HttpUtility.UrlDecode(log) ?? string.Empty;
            var m = new UpgradeVersionInfo();
            m.Date = DateTime.Now;
            m.Size = file.Length;
            m.Log = en;
            var name = Path.GetFileNameWithoutExtension(file.FileName);
            m.Version = name;
            m.Tag = string.Empty;
            if (name.Contains('-'))
            {
                var arr = name.Split('-');
                m.Version = arr[0];
                m.Tag = arr[1];
            }
            using (var stream = new FileStream($"{dir}/{file.FileName}", FileMode.Create))
            {
                file.CopyTo(stream);
                AutoUpgradeHelper.WriteVersion(dir, m);
            }
            return RedirectToAction("Index", "Version");
        }

        [HttpGet]
        public FileContentResult DownLoad(string path)
        {
            var decode = HttpUtility.UrlDecode(path);
            var absPath = Path.Combine(_versionDir, decode);
            if (System.IO.File.Exists(absPath))
            {
                FileStream myStream = new FileStream(absPath, FileMode.Open, FileAccess.Read);
                MemoryStream ms = new MemoryStream();
                myStream.CopyTo(ms);
                var data = ms.ToArray();
                myStream.Close();
                ms.Close();
                ms.Flush();
                return new FileContentResult(data, "application/octet-stream") { FileDownloadName = decode };
            }
            else
            {
                return new FileContentResult(new byte[0], "application/plain");
            }
        }

    }
}
