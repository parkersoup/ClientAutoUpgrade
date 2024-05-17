using AutoUpgradeCore;
using System.ComponentModel;
using System.Configuration;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Interop;

namespace AutoUpgradeClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        //protected override void OnSourceInitialized(EventArgs e)
        //{
        //    base.OnSourceInitialized(e);
        //    HwndSource source = PresentationSource.FromVisual(this) as HwndSource;
        //    source?.AddHook(new HwndSourceHook(WndProc));

        //}

        //private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        //{
        //    return UsbHelper.WndProc( msg, wParam, lParam, ref handled,null);
        //}

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
        }
    }

    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string Title { get; set; }
        public bool IsShowProcess { get; set; }
        public bool ProcessValue { get; set; }
        public bool ProcessText { get; set; }
        public UpgradeVersionInfo VersionInfo { get; set; } = new UpgradeVersionInfo();
    }
}