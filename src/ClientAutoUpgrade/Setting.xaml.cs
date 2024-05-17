using AutoUpgradeCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UserControl = System.Windows.Controls.UserControl;

namespace AutoUpgradeClient
{
    /// <summary>
    /// Setting.xaml 的交互逻辑
    /// </summary>
    public partial class Setting : UserControl
    {
        public Setting()
        {
            InitializeComponent();
            this.DataContext = new SettingViewModel();
        }
       
    }

    public class SettingViewModel : NotifyModel
    {
        public string AppName
        {
            get => _conf.AppName;
            set
            {
                if (_conf.AppName != value)
                {
                    _conf.AppName = value;
                    OnPropertyChanged();
                }
            }
        }

        public string AppWorkDir
        {
            get => _conf.AppWorkDir;
            set
            {
                if (_conf.AppWorkDir != value)
                {
                    _conf.AppWorkDir = value;
                    OnPropertyChanged();
                }
            }
        }

        public string AppStartPath
        {
            get => _conf.AppStartPath;
            set
            {
                if (_conf.AppStartPath != value)
                {
                    _conf.AppStartPath = value;
                    OnPropertyChanged();
                }
            }
        }

        public string AppPocessName
        {
            get => _conf.AppPocessName;
            set
            {
                if (_conf.AppPocessName != value)
                {
                    _conf.AppPocessName = value;
                    OnPropertyChanged();
                }
            }
        }
        public string Server
        {
            get => _conf.Server;
            set
            {
                if (_conf.Server != value)
                {
                    _conf.Server = value;
                    OnPropertyChanged();
                }
            }
        }
        public string UsbName
        {
            get => _conf.UsbName;
            set
            {
                if (_conf.UsbName != value)
                {
                    _conf.UsbName = value;
                    OnPropertyChanged();
                }
            }
        }
        public bool Auto
        {
            get => _conf.Auto;
            set
            {
                if (_conf.Auto != value)
                {
                    _conf.Auto = value;
                    OnPropertyChanged();
                }
            }
        }
        public string Tag
        {
            get => _conf.Tag;
            set
            {
                if (_conf.Tag != value)
                {
                    _conf.Tag = value;
                    OnPropertyChanged();
                }
            }
        }
        private UpgradeConfig _conf = new UpgradeConfig();

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get;  }

        public SettingViewModel()
        {
            ReadConfig();
            SaveCommand = new RelayCommand(SaveConfig);
            CancelCommand = new RelayCommand(ExitSetting);

        }

        private void ReadConfig()
        {
            var path = AutoUpgradeHelper.ConfigPath;
            if (File.Exists(path))
            {
                var str = File.ReadAllText(path, Encoding.UTF8);
                try
                {
                    var obj = JsonConvert.DeserializeObject<UpgradeConfig>(str);
                    _conf = obj;
                }
                catch { }
            }
        }

        private void SaveConfig(object obj)
        {
            var str = JsonConvert.SerializeObject(_conf, Formatting.Indented);
            File.WriteAllText(AutoUpgradeHelper.ConfigPath, str);
            ExitSetting(obj);
        }

        private void ExitSetting(object obj)
        { 
          Environment.Exit(0);
        }

        
    }
}
