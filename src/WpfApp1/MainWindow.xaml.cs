using AutoUpgradeClient;
using AutoUpgradeCore;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            LoadTestData();
            CheckVersion();
        }

        private void LoadTestData()
        {
            var str = File.ReadAllText(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData", "测试数据.txt"));
            rich.AppendText(str);
        }

        private async void CheckVersion()
        {
            AutoUpgradeCore.UpgradeClient upgrade = new AutoUpgradeCore.UpgradeClient(new AutoUpgradeCore.UpgradeConfig()
            {
                Server = "http://localhost:9090/"
            });
           var b= upgrade.GetNewVersion(out UpgradeVersionInfo info);
            if (b)
            {
                dialog.Children.Add(new UpdateInfo(info, UpdateAction, "测试项目"));
                dialog.Visibility = Visibility.Visible;
            }
            
        }

        private void UpdateAction(bool b)
        {
            if (b)
            {
                Environment.Exit(0);
            }
            else
            {
                dialog.Children.Clear();
                dialog.Visibility = Visibility.Hidden;
            }
        }
    }
}