using System;

namespace AutoUpgradeCore
{
    /// <summary>
    /// 版本信息
    /// </summary>
    public struct UpgradeVersionInfo
    {
        /// <summary>
        /// 当前版本
        /// </summary>
        public string Version { get; set; }
        public string Tag { get; set; } 
        public DateTime Date { get; set; }
        /// <summary>
        /// 特殊标记
        /// </summary>
        public string Log { get; set; }

        public long  Size { get; set; }
        public string  SizeStr { get { return Size.ConvertFileSizeStr(); } }

    }

    public struct UpgradeModel
    {
        /// <summary>
        /// 程序路径
        /// </summary>
        public string ProgramPath { get; set; }
        /// <summary>
        /// 当前版本
        /// </summary>
        public string Version { get; set; }
        /// <summary>
        /// 特殊标记
        /// </summary>
        public string Tag { get; set; }
    }


    /// <summary>
    /// 升级配置
    /// </summary>
    public struct UpgradeConfig
    { 
        /// <summary>
        /// 服务端地址
        /// </summary>
       public string Server { get; set; }
        /// <summary>
        /// Usb设备名
        /// </summary>
       public string UsbName { get; set; }
        /// <summary>
        /// Usb指定文件夹
        /// </summary>
       public string UsbDir { get; set; }
        /// <summary>
        /// 自动升级最近
        /// </summary>
       public bool Auto { get; set; } 
    }
}
