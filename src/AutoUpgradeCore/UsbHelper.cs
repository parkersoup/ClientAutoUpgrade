using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace AutoUpgradeCore
{
    public class UsbHelper
    {

        //定义常量  
        public const int WM_DEVICECHANGE = 0x219;
        public const int DBT_DEVICEARRIVAL = 0x8000;
        public const int DBT_CONFIGCHANGECANCELED = 0x0019;
        public const int DBT_CONFIGCHANGED = 0x0018;
        public const int DBT_CUSTOMEVENT = 0x8006;
        public const int DBT_DEVICEQUERYREMOVE = 0x8001;
        public const int DBT_DEVICEQUERYREMOVEFAILED = 0x8002;
        public const int DBT_DEVICEREMOVECOMPLETE = 0x8004;
        public const int DBT_DEVICEREMOVEPENDING = 0x8003;
        public const int DBT_DEVICETYPESPECIFIC = 0x8005;
        public const int DBT_DEVNODES_CHANGED = 0x0007;
        public const int DBT_QUERYCHANGECONFIG = 0x0017;
        public const int DBT_USERDEFINED = 0xFFFF;


        public static void WndProcEx(int msg, IntPtr wParam, IntPtr lParam,Action usbAdded)
        {
            if ((msg == WM_DEVICECHANGE) && (lParam != IntPtr.Zero))
            {
                switch (wParam.ToInt32())
                {
                    case DBT_DEVICEARRIVAL:
                        usbAdded?.Invoke();
                        break;

                    case DBT_DEVICEQUERYREMOVE:
                        // can intercept
                        break;

                    case DBT_DEVICEREMOVECOMPLETE:
                        //   Debug.WriteLine("拔出Usb");
                        break;
                }
            }
        }

        public static IntPtr WndProc(int msg, IntPtr wParam, IntPtr lParam, ref bool handled, Action usbAdded)
        {
            WndProcEx(msg, wParam, lParam, usbAdded);
            handled = false;
            return IntPtr.Zero;
        }

    }

   

}
