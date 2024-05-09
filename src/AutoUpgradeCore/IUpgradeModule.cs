using System;
using System.IO;

namespace AutoUpgradeCore
{
    public interface IUpgradeModule
    {
        void CheckVer(string tag);

        void  DownloadVer();

        void Upgrade();
        void Upgrade(string ver);
    }


    public class UpgradeModule : IUpgradeModule
    {
        public const string VerDir  = "Upgrade";
        private readonly string _dir;
        public UpgradeModule(UpgradeModel model,UpgradeConfig config)
        {
           _dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, VerDir);
        }
        public void CheckVer(string tag)
        {
          
        }

        public void DownloadVer()
        {
           
        }

        public void Upgrade()
        {
           
        }

        public void Upgrade(string ver)
        {
            
        }

        public void UseServer(string server)
        {
           
        }

        public void UseUsb(string usb, string dir)
        {
            
        }
    }
}
