using AutoUpgradeCore;
using System;
using System.Collections.Generic;
using System.Linq;
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

namespace AutoUpgradeClient
{
    /// <summary>
    /// Info.xaml 的交互逻辑
    /// </summary>
    public partial class UpdateInfo : Page
    {
        public Action<bool>? _callBack;
        private string TitleText { get; set; } = string.Empty;
        public  UpgradeVersionInfo InfoData{ get; set; }=new UpgradeVersionInfo();
        public UpdateInfo()
        {
            InitializeComponent();
            this.Loaded += UpdateInfo_Loaded;
        }

        private void UpdateInfo_Loaded(object sender, RoutedEventArgs e)
        {
            txt_title.Text = TitleText;
            txt_version.Text = InfoData.Version;
            txt_tag.Text = InfoData.Tag;
            txt_time.Text = InfoData.Date.ToString("yyyy年MM月dd日");
            foreach (var item in InfoData.Log.Split(';'))
            {
                rich.AppendText(item);
            };
        }

        public UpdateInfo(UpgradeVersionInfo info, Action<bool> callBack,string title)
        {
            InitializeComponent();
            TitleText = title;
            InfoData=info;
            _callBack = callBack;
        }
       

        private void NoClick(object sender, RoutedEventArgs e)
        {
            _callBack?.Invoke(false);
        }

        private void OkClick(object sender, RoutedEventArgs e)
        {
            _callBack?.Invoke(true);

        }
    }
}
