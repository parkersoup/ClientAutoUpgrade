using System;

namespace AutoUpgradeCore
{
    /// <summary>
    /// 版本信息
    /// </summary>
    public class UpgradeVersionInfo
    {
        /// <summary>
        /// 当前版本
        /// </summary>
        public string Version { get; set; }
        /// <summary>
        /// 补丁
        /// </summary>
        public int Patch { get; set; }
        public string Tag { get; set; } 
        public DateTime Date { get; set; }
        /// <summary>
        /// 特殊标记
        /// </summary>
        public string Log { get; set; }

        public long  Size { get; set; }
        public string  SizeStr { get { return Size.ConvertFileSizeStr(); } }
        public string FileName { get; set; }
    }


    /// <summary>
    /// 升级配置
    /// </summary>
    public class UpgradeConfig
    {
        /// <summary>
        /// 产品名
        /// </summary>
        public string AppName { get; set; } = "App";
        /// <summary>
        /// 程序目录路径
        /// </summary>
        public string AppWorkDir { get; set; }
        /// <summary>
        /// 程序启动路径
        /// </summary>
        public string AppStartPath { get; set; }
        /// <summary>
        /// 程序进程名字
        /// </summary>
        public string AppPocessName { get; set; }
        
        /// <summary>
        /// 服务端地址，不指定则禁用Web更新
        /// </summary>
        public string Server { get; set; }
        /// <summary>
        /// Usb设备名，更新优先级更高，不指定则禁用Usb更新
        /// </summary>
       public string UsbName { get; set; }
   
        /// <summary>
        /// 自动升级，检查后跳过确认升级环节
        /// </summary>
       public bool Auto { get; set; } 
        /// <summary>
        /// 指定值可以额外获取特殊版本
        /// </summary>
        public string Tag { get; set;}
    }
}
